using System;
using System.Collections.Generic;
using battle_player;
using UnityEngine;

public class StartGameMain : MonoBehaviour
{
    // 客户端记录玩家的id
    public static string id = "";

    // todo； 存储位置记得更改
    public const string BATTLERESULT_PATH = "C:\\ProgramData/battleResult";
    public static string path = "";
    public static Dictionary<string, PlayerData> playerRecords = new();
    public static event Action InitBattleSceneResources;

    private void Start()
    {
        if (id != "")
        {
            PanelManager.Init();
            // 不需要加载场景
            return;
        }
        NetManager.AddEventListener(NetManager.NetEvent.Close, OnConnectClose);
        NetManager.AddMsgListener("MsgKick", OnMsgKick);
        
        PanelManager.Init();
        BattleManager.Init();
        PanelManager.Open<LoginPanel>();
        InitBattleSceneResources?.Invoke();
    }


    private void Update()
    {
        NetManager.Update();
    }

    private void OnConnectClose(string err)
    {
        PanelManager.Open<TipPanel>("连接关闭");
    }

    private void OnMsgKick(MsgBase msgBase)
    {
        MsgKick msg = (MsgKick)msgBase;
        PanelManager.Open<TipPanel>("被踢下线: " + msg.reason);
    }
}