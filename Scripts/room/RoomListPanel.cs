using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomListPanel : BasePanel
{
    private TextMeshProUGUI userText;
    private TextMeshProUGUI scoreText;
    private Button createButton;
    private Button refreshButton;
    private Transform content;
    private GameObject roomPrefab;


    public override void OnInit()
    {
        panelPrefabPath = "RoomListPanel";
    }


    public override void OnShow(params object[] para)
    {
        createButton = panelPrefab.transform.Find("CtrlPanel/CreateBtn").GetComponent<Button>();
        refreshButton = panelPrefab.transform.Find("CtrlPanel/RefreshBtn").GetComponent<Button>();

        scoreText = panelPrefab.transform.Find("InfoPanel/Score").GetComponent<TextMeshProUGUI>();
        userText = panelPrefab.transform.Find("InfoPanel/UserText").GetComponent<TextMeshProUGUI>();

        content = panelPrefab.transform.Find("ListPanel/Scroll View/Viewport/Content");
        Debug.Log("content: " + content);

        roomPrefab = panelPrefab.transform.Find("Room").gameObject;
        roomPrefab.SetActive(false);

        createButton.onClick.AddListener(OnCreateClick);
        refreshButton.onClick.AddListener(OnRefreshClick);

        userText.text = StartGameMain.id;

        NetManager.AddMsgListener("MsgGetRoomList", OnMsgGetRoomList);
        NetManager.AddMsgListener("MsgCreateRoom", OnMsgCreateRoom);
        NetManager.AddMsgListener("MsgEnterRoom", OnMsgEnterRoom);
        NetManager.AddMsgListener("MsgGetAchieve", OnMsgGetAchieve);

        if (StartGameMain.playerRecords.ContainsKey(StartGameMain.id))
        {
            Debug.Log("roomListPanel containsKey");
            scoreText.text = StartGameMain.playerRecords[StartGameMain.id].win + " 胜 " +
                             StartGameMain.playerRecords[StartGameMain.id].loss + " 负";
            Debug.Log(StartGameMain.id + "    scoreText.text: " + scoreText.text);
        }
        else
        {
            // 发送查询
            MsgGetAchieve msgGetAchieve = new MsgGetAchieve();
            NetManager.Send(msgGetAchieve);
        }

        MsgGetRoomList msgGetRoomList = new MsgGetRoomList();
        NetManager.Send(msgGetRoomList);
    }

    public override void OnClose()
    {
        NetManager.RemoveMsgListener("MsgGetRoomList", OnMsgGetRoomList);
        NetManager.RemoveMsgListener("MsgCreateRoom", OnMsgCreateRoom);
        NetManager.RemoveMsgListener("MsgEnterRoom", OnMsgEnterRoom);
        NetManager.RemoveMsgListener("MsgGetAchieve", OnMsgGetAchieve);
    }

    private void OnCreateClick()
    {
        MsgCreateRoom msg = new MsgCreateRoom();
        NetManager.Send(msg);
    }

    private void OnRefreshClick()
    {
        // 重新获取房间列表
        MsgGetRoomList msg = new MsgGetRoomList();
        NetManager.Send(msg);
    }

    // 收到查询结果
    private void OnMsgGetRoomList(MsgBase msgBase)
    {
        MsgGetRoomList msg = (MsgGetRoomList)msgBase;
        // 删除旧的
        // if (content.childCount == 0)
        // {
        //     return;
        // }

        if (content != null)
        {
            for (int i = 0; i < content.childCount; i++)
            {
                Destroy(content.GetChild(i).gameObject);
            }
        }

        if (msg.rooms is null)
        {
            return;
        }

        // 显示新的
        RoomInfo[] rooms = msg.rooms;
        for (int i = 0; i < rooms.Length; i++)
        {
            GenerateRoom(rooms[i]);
        }
    }

    private void GenerateRoom(RoomInfo roomInfo)
    {
        // GameObject room = Instantiate(roomPrefab);
        // room.transform.SetParent(content);
        GameObject room = Instantiate(roomPrefab, content);

        room.transform.localScale = Vector3.one;
        room.SetActive(true);

        Transform trans = room.transform;
        TextMeshProUGUI text = trans.Find("IdText").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI countText = trans.Find("CountText").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI statusText = trans.Find("StatusText").GetComponent<TextMeshProUGUI>();
        Button joinBtn = trans.Find("JoinBtn").GetComponent<Button>();

        text.text = roomInfo.id.ToString();
        countText.text = roomInfo.count.ToString();

        if (roomInfo.status == 0)
        {
            statusText.text = "准备中";
        }
        else
        {
            statusText.text = "战斗中";
        }

        // todo: joinBtn.name??
        joinBtn.name = text.text;
        joinBtn.onClick.AddListener(() => { OnJoinClick(joinBtn.name); });
    }

    private void OnJoinClick(string id)
    {
        MsgEnterRoom msg = new MsgEnterRoom();
        msg.id = int.Parse(id);
        NetManager.Send(msg);
    }

    // 收到成绩查询结果
    private void OnMsgGetAchieve(MsgBase msgBase)
    {
        MsgGetAchieve msg = (MsgGetAchieve)msgBase;
        scoreText.text = msg.win + " 胜 " + msg.lost + " 负";

        // scoreText.text = $"{msg.win} 胜 {msg.lost} 负";
    }

    private void OnMsgCreateRoom(MsgBase msgBase)
    {
        MsgCreateRoom msg = (MsgCreateRoom)msgBase;
        if (msg.result == 0)
        {
            // 创建成功
            // PanelManager.Open<TipPanel>("创建成功");
            PanelManager.Open<RoomPanel>();
            Close();
        }
        else
        {
            // 创建失败
            PanelManager.Open<TipPanel>(msg.reason);
        }
    }

    private void OnMsgEnterRoom(MsgBase msgBase)
    {
        MsgEnterRoom msg = (MsgEnterRoom)msgBase;
        if (msg.result == 0)
        {
            // 进入成功
            // PanelManager.Open<TipPanel>("进入成功");
            PanelManager.Open<RoomPanel>();
            Close();
        }
        else
        {
            // 进入失败
            PanelManager.Open<TipPanel>("进入房间失败");
        }
    }
}