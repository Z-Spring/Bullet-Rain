using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoomPanel : BasePanel
{
    private Button startBtn;
    private Button closeBtn;
    private Transform content;
    private GameObject playerPrefab;

    public override void OnInit()
    {
        panelPrefabPath = "RoomPanel";
    }

    public override void OnShow(params object[] para)
    {
        startBtn = panelPrefab.transform.Find("CtrlPanel/StartBtn").GetComponent<Button>();
        closeBtn = panelPrefab.transform.Find("CtrlPanel/CloseBtn").GetComponent<Button>();
        content = panelPrefab.transform.Find("ListPanel/Scroll View/Viewport/Content");
        playerPrefab = panelPrefab.transform.Find("Player").gameObject;
        playerPrefab.SetActive(false);

        startBtn.onClick.AddListener(OnStartClick);
        closeBtn.onClick.AddListener(OnCloseClick);


        NetManager.AddMsgListener("MsgLeaveRoom", OnMsgLeaveRoom);
        NetManager.AddMsgListener("MsgGetRoomInfo", OnMsgGetRoomInfo);
        NetManager.AddMsgListener("MsgStartBattle", OnMsgStartBattle);

        MsgGetRoomInfo msgGetRoomInfo = new MsgGetRoomInfo();
        NetManager.Send(msgGetRoomInfo);
    }

    public override void OnClose()
    {
        NetManager.RemoveMsgListener("MsgLeaveRoom", OnMsgLeaveRoom);
        NetManager.RemoveMsgListener("MsgGetRoomInfo", OnMsgGetRoomInfo);
        NetManager.RemoveMsgListener("MsgStartBattle", OnMsgStartBattle);
    }

    // 点击“开战”按钮,发送MsgStartBattle消息
    private void OnStartClick()
    {
        MsgStartBattle msgStartBattle = new MsgStartBattle();
        NetManager.Send(msgStartBattle);
    }

    private void OnCloseClick()
    {
        MsgLeaveRoom msgLeaveRoom = new MsgLeaveRoom();
        NetManager.Send(msgLeaveRoom);
    }

    private void OnMsgStartBattle(MsgBase msgBase)
    {
        MsgStartBattle msg = (MsgStartBattle)msgBase;
        if (msg.result == 0)
        {
            Close();
           
            // PanelManager.Open<BattlePanel>();
        }
        else
        {
            PanelManager.Open<TipPanel>(msg.reason);
        }
    }

    // 离开房间
    private void OnMsgLeaveRoom(MsgBase msgBase)
    {
        MsgLeaveRoom msg = (MsgLeaveRoom)msgBase;
        if (content != null)
        {
            for (int i = 0; i < content.childCount; i++)
            {
                Destroy(content.GetChild(i).gameObject);
            }
        }

        if (msg.result == 0)
        {
            // PanelManager.Open<TipPanel>("退出房间成功");
            PanelManager.Open<RoomListPanel>();
            Close();
        }
        else
        {
            PanelManager.Open<TipPanel>("退出房间失败");
        }
    }

    // 收到服务器发来的房间信息
    // todo: 弄懂
    private void OnMsgGetRoomInfo(MsgBase msgBase)
    {
        // content = panelPrefab.transform.Find("ListPanel/Scroll View/Viewport/Content");

        MsgGetRoomInfo msg = (MsgGetRoomInfo)msgBase;
        for (int i = content.childCount - 1; i >= 0; i--)
        {
            GameObject o = content.GetChild(i).gameObject;
            Destroy(o);
        }

        if (msg.players is null)
        {
            return;
        }

        Debug.Log("msg.players.Length: " + msg.players.Length);
        // 显示新的
        // for (int i = 0; i < msg.players.Length; i++)
        // {
        //     GeneratePlayerInfo(msg.players[i]);
        // }
        foreach (PlayerInfo playerInfo in msg.players)
        {
            GeneratePlayerInfo(playerInfo);
        }
    }


    private void GeneratePlayerInfo(PlayerInfo playerInfo)
    {
        Debug.Log("generatePlayerInfo: " + content);
        GameObject player = Instantiate(playerPrefab, content);
        player.SetActive(true);

        player.transform.localScale = Vector3.one;
        Transform trans = player.transform;

        TextMeshProUGUI text = trans.Find("IdText").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI campText = trans.Find("CampText").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI scoreText = trans.Find("ScoreText").GetComponent<TextMeshProUGUI>();

        Debug.Log("[roomPanel] playerInfo.id: " + playerInfo.id);

        if (StartGameMain.playerRecords.ContainsKey(playerInfo.id))
        {
            scoreText.text = StartGameMain.playerRecords[playerInfo.id].win + " 胜 " +
                             StartGameMain.playerRecords[playerInfo.id].loss + " 负";
            Debug.Log("[roomPanel] scoreText.text: " + playerInfo.id + "     " + scoreText.text);
        }
        else
        {
            scoreText.text = playerInfo.win + " 胜 " + playerInfo.lost + " 负";
        }

        text.text = playerInfo.id;
        if (playerInfo.camp == 1)
        {
            campText.text = "红";
        }
        else
        {
            campText.text = "蓝";
        }

        if (playerInfo.isOwner == 1)
        {
            campText.text += "! ";
        }
    }
}