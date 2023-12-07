using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;
using System.Linq;
using System.Text;

public static class NetManager
{
    //定义套接字
    static Socket socket;

    //接收缓冲区
    static ByteArray readBuff;

    //写入队列
    static Queue<ByteArray> writeQueue;

    //是否正在连接
    static bool isConnecting ;

    //是否正在关闭
    static bool isClosing ;

    //消息列表
    static List<MsgBase> msgList = new();

    //消息列表长度 减少对msgList.count的调用次数
    static int msgCount = 0;

    //每一次Update处理的消息量
    static readonly  int MAX_MESSAGE_FIRE = 10;

    #region Ping

    //是否启用心跳
    public static bool isUsePing = true;

    //心跳间隔时间
    public static int pingInterval = 30;

    //上一次发送PING的时间
    private static float lastPingTime;

    //上一次收到PONG的时间
    static float lastPongTime;

    #endregion


    //事件
    public enum NetEvent
    {
        ConnectSucc = 1,
        ConnectFail,
        Close
    }

    //事件委托类型
    public delegate void EventListener(String err);

    //事件监听列表
    private static  Dictionary<NetEvent, EventListener> eventListeners = new();

    //添加事件监听
    public static void AddEventListener(NetEvent netEvent, EventListener listener)
    {
        //添加事件
        if (eventListeners.ContainsKey(netEvent))
        {
            eventListeners[netEvent] += listener;
        }
        //新增事件
        else
        {
            eventListeners[netEvent] = listener;
        }
    }

    //删除事件监听
    public static void RemoveEventListener(NetEvent netEvent, EventListener listener)
    {
        if (eventListeners.ContainsKey(netEvent))
        {
            eventListeners[netEvent] -= listener;
        }
    }

    //分发事件
    private static void FireEvent(NetEvent netEvent, String err)
    {
        if (eventListeners.TryGetValue(netEvent, out EventListener eventListener))
        {
            eventListener(err);
        }
    }


    //消息委托类型
    public delegate void MsgListener(MsgBase msgBase);

    //消息监听列表
    private static Dictionary<string, MsgListener> msgListeners = new();

    //添加消息监听
    public static void AddMsgListener(string msgName, MsgListener listener)
    {
        //添加
        // if (msgListeners.ContainsKey(msgName))
        // 注意：这里的msgListener是一个局部变量，而不是msgListeners[msgName]的引用，因此对msgListener的修改不会影响到msgListeners[msgName]
        if (msgListeners.TryGetValue(msgName, out MsgListener msgListener))
        {
            msgListener += listener;
            msgListeners[msgName] = msgListener;
            // msgListeners[msgName] += listener;
        }
        //新增
        else
        {
            msgListeners[msgName] = listener;
        }
    }

    //删除消息监听
    public static void RemoveMsgListener(string msgName, MsgListener listener)
    {
        if (msgListeners.ContainsKey(msgName))
        {
            msgListeners[msgName] -= listener;
        }
    }

    //分发消息
    private static void FireMsg(string msgName, MsgBase msgBase)
    {
        // 这里用TryGetValue没有问题，因为直接调用了委托方法
        if (msgListeners.TryGetValue(msgName, out MsgListener msgListener))
        {
            msgListener(msgBase);
        }
    }

    #region Connect

    //连接
    public static void Connect(string ip, int port)
    {
        //状态判断
        if (socket != null && socket.Connected)
        {
            Debug.Log("Connect fail, already connected!");
            return;
        }

        // isConnecting: 仅在Connect回调ConnectCallback时为true
        // 如果在回调ConnectCallback过程中调用Connect，isConnecting为true，此时不应该再次调用Connect
        if (isConnecting)
        {
            Debug.Log("Connect fail, isConnecting");
            return;
        }

        //初始化成员
        InitState();
        //参数设置
        socket.NoDelay = true;
        //Connect
        isConnecting = true;
        socket.BeginConnect(ip, port, ConnectCallback, socket);
    }

    //初始化状态
    private static void InitState()
    {
        //Socket
        socket = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream, ProtocolType.Tcp);
        //接收缓冲区
        readBuff = new ByteArray();
        //写入队列
        writeQueue = new Queue<ByteArray>();
        //是否正在连接
        isConnecting = false;
        //是否正在关闭
        isClosing = false;
        //消息列表
        msgList = new List<MsgBase>();
        //消息列表长度
        msgCount = 0;
        //上一次发送PING的时间
        lastPingTime = Time.time;
        //上一次收到PONG的时间
        lastPongTime = Time.time;
        //监听PONG协议
        if (!msgListeners.ContainsKey("MsgPong"))
        {
            AddMsgListener("MsgPong", OnMsgPong);
        }
    }

    //Connect回调
    private static void ConnectCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            socket.EndConnect(ar);
            Debug.Log("Socket Connect Succ ");
            FireEvent(NetEvent.ConnectSucc, "");
            isConnecting = false;
            //开始接收
            // 从写位置writeIdx开始写入，写入剩余空间长度，接收回调
            socket.BeginReceive(readBuff.bytes, readBuff.writeIdx,
                readBuff.remain, 0, ReceiveCallback, socket);
        }
        catch (SocketException ex)
        {
            Debug.Log("Socket Connect fail " + ex.ToString());
            FireEvent(NetEvent.ConnectFail, ex.ToString());
            isConnecting = false;
        }
    }

    #endregion

    #region Close

    //关闭连接
    public static void Close()
    {
        //状态判断 必须是连接状态才能断开
        if (socket == null || !socket.Connected)
        {
            return;
        }

        if (isConnecting)
        {
            return;
        }

        //还有数据在发送
        if (writeQueue.Count > 0)
        {
            isClosing = true;
        }
        //没有数据在发送
        else
        {
            socket.Close();
            FireEvent(NetEvent.Close, "");
        }
    }

    #endregion

    #region Send

    //发送数据（组装协议名和协议体，然后写入缓冲区和队列）
    public static void Send(MsgBase msg)
    {
        //状态判断
        if (socket == null || !socket.Connected)
        {
            return;
        }

        if (isConnecting)
        {
            return;
        }

        if (isClosing)
        {
            return;
        }
        //
        Debug.Log("Send Msg " + msg.protoName);
        
        //数据编码
        byte[] nameBytes = MsgBase.EncodeName(msg);
        byte[] bodyBytes = MsgBase.Encode(msg);
        int len = nameBytes.Length + bodyBytes.Length;

        byte[] sendBytes = new byte[2 + len];
        //组装长度
        sendBytes[0] = (byte)(len % 256);
        sendBytes[1] = (byte)(len / 256);
        //组装名字
        Array.Copy(nameBytes, 0, sendBytes, 2, nameBytes.Length);
        //组装消息体
        Array.Copy(bodyBytes, 0, sendBytes, 2 + nameBytes.Length, bodyBytes.Length);

        //写入队列
        ByteArray ba = new ByteArray(sendBytes);


        int count; //writeQueue的长度
        lock (writeQueue)
        {
            writeQueue.Enqueue(ba);
            count = writeQueue.Count;
        }

        //send
        if (count == 1)
        {
            socket.BeginSend(sendBytes, 0, sendBytes.Length,
                0, SendCallback, socket);
        }
    }

    //Send回调
    private static void SendCallback(IAsyncResult ar)
    {
        //获取state、EndSend的处理
        Socket socket = (Socket)ar.AsyncState;
        //状态判断
        // if (socket == null || !socket.Connected)
        // {
        //     return;
        // }

        // 模式匹配
        if (socket is { Connected: false } or null)
        {
            return;
        }

        //EndSend
        int count = socket.EndSend(ar);
        //获取写入队列第一条数据            
        ByteArray ba;
        lock (writeQueue)
        {
            ba = writeQueue.First();
        }

        //完整发送
        ba.readIdx += count;
        if (ba.length == 0)
        {
            lock (writeQueue)
            {
                writeQueue.Dequeue();
                ba = writeQueue.First();
            }
        }

        //继续发送
        if (ba != null)
        {
            socket.BeginSend(ba.bytes, ba.readIdx, ba.length,
                0, SendCallback, socket);
        }
        //正在关闭
        else if (isClosing)
        {
            socket.Close();
        }
    }

    #endregion

    #region Receive

    //Receive回调
    private static void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            Socket socketReceive = (Socket)ar.AsyncState;
            //获取接收数据长度
            int count = socketReceive.EndReceive(ar);

            readBuff.writeIdx += count;
            //处理二进制消息
            HandleReceiveData();
            //继续接收数据
            if (readBuff.remain < 8)
            {
                readBuff.MoveBytes();
                readBuff.ReSize(readBuff.length * 2);
            }

            socketReceive.BeginReceive(readBuff.bytes, readBuff.writeIdx,
                readBuff.remain, 0, ReceiveCallback, socketReceive);
        }
        catch (SocketException ex)
        {
            Debug.Log("Socket Receive fail" + ex);
        }
    }

    //数据处理（解析协议名和协议体，将消息添加到消息列表中，移动数据）
    private static void HandleReceiveData()
    {
        //消息长度
        if (readBuff.length <= 2)
        {
            return;
        }

        //获取消息体长度
        int readIdx = readBuff.readIdx;
        byte[] bytes = readBuff.bytes;

        // todo: 不懂
        Int16 bodyLength = (Int16)((bytes[readIdx + 1] << 8) | bytes[readIdx]);
        if (readBuff.length < bodyLength)
            return;
        readBuff.readIdx += 2;
        //解析协议名
        int nameCount;
        string s = Encoding.Default.GetString(readBuff.bytes);


        string protoName = MsgBase.DecodeName(readBuff.bytes, readBuff.readIdx, out nameCount);
        if (protoName == "")
        {
            Debug.Log("OnReceiveData MsgBase.DecodeName fail");
            return;
        }

        // readIdx 从协议名的后面开始读取即： 协议体开始的位置
        readBuff.readIdx += nameCount;
        //解析协议体，获取解析体的长度
        int bodyCount = bodyLength - nameCount;
        MsgBase msgBase = MsgBase.Decode(protoName, readBuff.bytes, readBuff.readIdx, bodyCount);
        readBuff.readIdx += bodyCount;

        readBuff.CheckAndMoveBytes();
        //添加到消息队列
        lock (msgList)
        {
            msgList.Add(msgBase);
        }

        msgCount++;

        //继续读取消息
        if (readBuff.length > 2)
        {
            HandleReceiveData();
        }
    }

    #endregion

    #region Update

    //Update
    public static void Update()
    {
        MsgUpdate();
        PingUpdate();
    }

    //更新消息
    public static void MsgUpdate()
    {
        //初步判断，提升效率
        if (msgCount == 0)
        {
            return;
        }

        //重复处理消息
        for (int i = 0; i < MAX_MESSAGE_FIRE; i++)
        {
            //每次都获取第一条消息
            MsgBase msgBase = null;
            lock (msgList)
            {
                if (msgList.Count > 0)
                {
                    msgBase = msgList[0];
                    msgList.RemoveAt(0);
                    msgCount--;
                }
            }

            //分发消息
            if (msgBase != null)
            {
                FireMsg(msgBase.protoName, msgBase);
            }
            //没有消息了
            else
            {
                break;
            }
        }
    }

    //发送PING协议
    private static void PingUpdate()
    {
        //是否启用
        if (!isUsePing)
        {
            return;
        }

        //发送PING
        if (Time.time - lastPingTime > pingInterval)
        {
            MsgPing msgPing = new MsgPing();
            Send(msgPing);
            lastPingTime = Time.time;
        }

        //检测PONG时间
        if (Time.time - lastPongTime > pingInterval * 4)
        {
            Close();
        }
    }

    #endregion

    //监听PONG协议
    private static void OnMsgPong(MsgBase msgBase)
    {
        lastPongTime = Time.time;
    }
}