using System;
using System.Collections;
using System.Linq;
using battle_player;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI.GameSceneUI
{
    public class ScorePanelUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI blueScoreText;
        [SerializeField] private TextMeshProUGUI redScoreText;
        [SerializeField] private TextMeshProUGUI timeText;

        [FormerlySerializedAs("winBottomImage")] [SerializeField]
        private Image winCampColor;

        [SerializeField] private float gameTime;

        private void Start()
        {
            NetManager.AddMsgListener("MsgGetGameTime", OnMsgGetGameTime);
            EventManager.OnPlayerDeadGlobal += OnPlayerDead;
        }

        private void Update()
        {
            SetWinCampImageColor();
        }

        private void OnMsgGetGameTime(MsgBase msgBase)
        {
            MsgGetGameTime msg = (MsgGetGameTime)msgBase;
            gameTime = msg.gameTime;
            timeText.text = TimeSpan.FromSeconds(gameTime).ToString("mm':'ss");

        }
        

        private void OnPlayerDead(string id)
        {
            // 这里转为数组，然后在删除时就不会破坏原来的字典的状态
            // foreach (var playerId in BattleManager.players.Keys.ToArray())
            // {
            //     if (BattleManager.deadPlayerDic.TryGetValue(playerId, out int camp))
            //     {
            //         if (camp == 1)
            //         {
            //             redScoreText.text = (int.Parse(redScoreText.text) + 1).ToString();
            //         }
            //         else if (camp == 2)
            //         {
            //             blueScoreText.text = (int.Parse(blueScoreText.text) + 1).ToString();
            //         }
            //
            //         BattleManager.RemovePlayer(playerId);
            //     }
            // }

            // foreach (BasePlayer player in BattleManager.players.Values)
            // {
            //     if (player.IsDie())
            //     {
            //         if (player.camp == 1)
            //         {
            //             redScoreText.text = (int.Parse(redScoreText.text) + 1).ToString();
            //         }
            //         else if (player.camp == 2)
            //         {
            //             blueScoreText.text = (int.Parse(blueScoreText.text) + 1).ToString();
            //         }
            //
            //     }
            // }
            
            int camp = BattleManager.GetPlayer(id).camp;
            if (camp == 1)
            {
                redScoreText.text = (int.Parse(redScoreText.text) + 1).ToString();
            }
            else if (camp == 2)
            {
                blueScoreText.text = (int.Parse(blueScoreText.text) + 1).ToString();
            }
            
            
        }

        private void SetWinCampImageColor()
        {
            int redScore = int.Parse(redScoreText.text);
            int blueScore = int.Parse(blueScoreText.text);
            if (redScore > blueScore)
            {
                winCampColor.color = new Color32(197, 91, 93, 255);
            }
            else if (redScore < blueScore)
            {
                winCampColor.color = new Color32(32, 126, 236, 255);
            }
            else if (redScore == blueScore)
            {
                winCampColor.color = Color.white;
            }
        }
        
        private void OnDestroy()
        {
            EventManager.OnPlayerDeadGlobal -= OnPlayerDead;
        }
    }
}