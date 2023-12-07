using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegisterPanel : BasePanel
{
    private TMP_InputField idInput;
    private TMP_InputField pwInput;
    private TMP_InputField repInput;
    private Button registerBtn;
    private Button closeBtn;

    public override void OnInit()
    {
        panelPrefabPath = "RegisterPanel";
    }

    // private void Update()
    // {
    //     NetManager.Update();
    // }

    public override void OnShow(params object[] para)
    {
        idInput = panelPrefab.transform.Find("IdInput").GetComponent<TMP_InputField>();
        pwInput = panelPrefab.transform.Find("PwInput").GetComponent<TMP_InputField>();
        repInput = panelPrefab.transform.Find("RepInput").GetComponent<TMP_InputField>();

        registerBtn = panelPrefab.transform.Find("RegisterButton").GetComponent<Button>();
        closeBtn = panelPrefab.transform.Find("CloseButton").GetComponent<Button>();

        registerBtn.onClick.AddListener(OnRegisterClick);
        closeBtn.onClick.AddListener(OnCloseClick);

        // 添加网络监听
        NetManager.AddMsgListener("MsgRegister", OnMsgRegister);
        NetManager.AddEventListener(NetManager.NetEvent.ConnectSucc, OnConnectSucc);
        NetManager.AddEventListener(NetManager.NetEvent.ConnectFail, OnConnectFail);
    }

    public override void OnClose()
    {
        NetManager.RemoveMsgListener("MsgRegister", OnMsgRegister);
    }

    private void OnRegisterClick()
    {
        Debug.Log(idInput.text + " " + pwInput.text);
        if (idInput.text == "" || pwInput.text == "" || repInput.text == "")
        {
            PanelManager.Open<TipPanel>("用户名或密码不能为空");
            return;
        }

        if (repInput.text != pwInput.text)
        {
            PanelManager.Open<TipPanel>("两次密码不一致");
            return;
        }

        MsgRegister msgRegister = new MsgRegister
        {
            id = idInput.text,
            pw = pwInput.text
        };

        NetManager.Send(msgRegister);
    }

    private void OnCloseClick()
    {
        Close();
        PanelManager.Open<LoginPanel>();
    }

    private void OnMsgRegister(MsgBase msgBase)
    {
        MsgRegister msg = (MsgRegister)msgBase;
        if (msg.result == 0)
        {
            PanelManager.Open<TipPanel>("注册成功");
            Close();
        }
        else
        {
            PanelManager.Open<TipPanel>("注册失败");
        }
    }

    private void OnConnectSucc(string err)
    {
        Debug.Log("连接成功");
    }

    private void OnConnectFail(string err)
    {
        Debug.Log("连接失败");
    }
}