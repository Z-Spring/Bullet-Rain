using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : BasePanel
{
    private TMP_InputField idInput;
    private TMP_InputField pwInput;
    private Button loginBtn;
    private Button regBtn;
    

    public override void OnInit()
    {
        panelPrefabPath = "LoginPanel";
    }

    public override void OnShow(params object[] para)
    {
        // idInput = panelPrefab.transform.Find("IdInput").GetComponent<InputField>();
        idInput = panelPrefab.transform.Find("IdInput").GetComponent<TMP_InputField>();
        pwInput = panelPrefab.transform.Find("PwInput").GetComponent<TMP_InputField>();
        loginBtn = panelPrefab.transform.Find("LoginButton").GetComponent<Button>();
        regBtn = panelPrefab.transform.Find("RegisterButton").GetComponent<Button>();

        loginBtn.onClick.AddListener(OnLoginClick);
        regBtn.onClick.AddListener(OnRegisterClick);

        // 添加网络监听
        NetManager.AddMsgListener("MsgLogin", OnMsgLogin);
        NetManager.AddEventListener(NetManager.NetEvent.ConnectSucc, OnConnectSucc);
        NetManager.AddEventListener(NetManager.NetEvent.ConnectFail, OnConnectFail);
        // 连接服务器
        NetManager.Connect("192.168.2.197", 8080);
    }

    private void OnLoginClick()
    {
        Debug.Log(idInput.text + " " + pwInput.text);
        if (idInput.text == "" || pwInput.text == "")
        {
            PanelManager.Open<TipPanel>("用户名或密码不能为空");
        }
        else
        {
            MsgLogin msgLogin = new MsgLogin()
            {
                id = idInput.text,
                pw = pwInput.text
            };
            NetManager.Send(msgLogin);
        }
    }

    private void OnRegisterClick()
    {
        PanelManager.Open<RegisterPanel>();
        Close();
    }

    // 取消网络监听
    public override void OnClose()
    {
        NetManager.RemoveMsgListener("MsgLogin", OnMsgLogin);
        NetManager.RemoveEventListener(NetManager.NetEvent.ConnectSucc, OnConnectSucc);
        NetManager.RemoveEventListener(NetManager.NetEvent.ConnectFail, OnConnectFail);
    }

    private void OnConnectSucc(string err)
    {
        
        Debug.Log("连接成功");
    }

    private void OnConnectFail(string err)
    {
        Debug.Log("连接失败");
    }

    private void OnMsgLogin(MsgBase msgBase)
    {
        MsgLogin msgLogin = (MsgLogin)msgBase;
        Debug.Log("收到登录消息");
        if (msgLogin.result == 0)
        {
            Debug.Log("登录成功");
            StartGameMain.id = msgLogin.id;
            StartGameMain.path = $"{StartGameMain.BATTLERESULT_PATH}_{StartGameMain.id}.json";

            PanelManager.Open<RoomListPanel>();

            Close();
        }
        else
        {
            PanelManager.Open<TipPanel>("登录失败");
        }
    }
}