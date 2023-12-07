using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using weapon;

namespace battle_player
{
    public class BattleManager
    {
        public static Dictionary<string, BasePlayer> players = new();
        public static event Action InitWeapon;

        
        //初始化
        public static void Init()
        {
            //添加监听
            NetManager.AddMsgListener("MsgEnterBattle", OnMsgEnterBattle);
            NetManager.AddMsgListener("MsgBattleResult", OnMsgBattleResult);
            NetManager.AddMsgListener("MsgLeaveBattle", OnMsgLeaveBattle);
            NetManager.AddMsgListener("MsgSyncPlayer", OnMsgSyncPlayer);
            NetManager.AddMsgListener("MsgFire", OnMsgFire);
            NetManager.AddMsgListener("MsgHit", OnMsgHit);
            NetManager.AddMsgListener("MsgResult", OnMsgResult);
            NetManager.AddMsgListener("MsgSwitchWeapon", OnMsgSwitchWeapon);
            NetManager.AddMsgListener("MsgSyncWeaponPosition", OnMsgSyncWeaponPosition);
        }

        private static void OnMsgLeaveBattle(MsgBase msgBase)
        {
            MsgLeaveBattle msg = (MsgLeaveBattle)msgBase;
            BasePlayer player = GetPlayer(msg.id);
            if (player == null)
            {
                return;
            }

            RemovePlayer(msg.id);
            player.gameObject.SetActive(false);
        }

        private static async void OnMsgEnterBattle(MsgBase msgBase)
        {
            MsgEnterBattle msg = (MsgEnterBattle)msgBase;
            await LoadSceneAsync("BattleScene");

            EnterBattle(msg);
            // OnMsgEnterBattle(msg);
            InitWeapon?.Invoke();
            ResManager.Instance.UnloadAssetBundle("ui", true);
            ResManager.Instance.UnloadAssetBundle("player", false);
            ResManager.Instance.UnloadAssetBundle("bullet", false);
            ResManager.Instance.UnloadAssetBundle("bullet_scene_ui", false);
            ResManager.Instance.UnloadAssetBundle("effect", false);
        }

        private static async Task LoadSceneAsync(string sceneName)
        {
            var loadOperation = SceneManager.LoadSceneAsync(sceneName);
            while (!loadOperation.isDone)
            {
                await Task.Delay(50); // Wait a short amount of time before checking again.
            }
        }


        private static void EnterBattle(MsgEnterBattle msgEnterBattle)
        {
            Reset();
            PanelManager.Close("RoomPanel");
            PanelManager.Close("ResultPanel");

            foreach (var tankInfo in msgEnterBattle.tanks)
            {
                GeneratePlayer(tankInfo);
            }

            // InitSwitchWeaponUI();
            ResManager.Instance.LoadResultPanel();

            // 隐藏鼠标指针
            Cursor.visible = false;
            // 锁定鼠标指针到屏幕中央
            Cursor.lockState = CursorLockMode.Locked;
        }


        private static void GeneratePlayer(TankInfo tankInfo)
        {
            //GameObject
            string objName = "Player_" + tankInfo.id;
            GameObject playerObj = new GameObject(objName)
            {
                layer = LayerMask.NameToLayer("player"),
                tag = "Player"
            };

            //AddComponent
            BasePlayer player;
            if (tankInfo.id == StartGameMain.id)
            {
                player = playerObj.AddComponent<Player>();
            }
            else
            {
                player = playerObj.AddComponent<SyncPlayer>();
            }

            //属性
            player.camp = tankInfo.camp;
            player.id = tankInfo.id;
            player.hp = tankInfo.hp;
            //pos rotation
            Vector3 pos = new Vector3(tankInfo.x, tankInfo.y, tankInfo.z);
            Vector3 rot = new Vector3(tankInfo.ex, tankInfo.ey, tankInfo.ez);
            player.transform.position = pos;
            player.transform.eulerAngles = rot;

            // AddPlayer(tankInfo.id, player);

            //init
            if (tankInfo.camp == 1)
            {
                player.Init("Player1");
            }
            else
            {
                player.Init("Player2");
            }

            // camera
            if (tankInfo.id == StartGameMain.id)
            {
                playerObj.AddComponent<CameraFollow>();
                playerObj.AddComponent<ShootLogic>();

            }
            else
            {
                player.GetComponentInChildren<WeaponSwitch>().enabled = false;
                var playerPrefab = playerObj.transform.GetChild(0);
                playerPrefab.Find("FollowCamera").gameObject.SetActive(false);
                playerPrefab.Find("Canvas").gameObject.SetActive(false);
            }

            //列表
            AddPlayer(tankInfo.id, player);
        }

        private static void InitSwitchWeaponUI()
        {
            GameObject prefab = ResManager.Instance.loadedBundles["battle_scene_ui"]
                .LoadAsset<GameObject>("SwitchWeaponPanel");

            GameObject gameSceneUi = GameObject.Find("GameSceneUI");
            MonoBehaviour.Instantiate(prefab, gameSceneUi.transform);
        }

        private static void AddPlayer(string id, BasePlayer player)
        {
            players[id] = player;
        }

        public static void RemovePlayer(string id)
        {
            Debug.Log("RemovePlayer: " + id);
            players.Remove(id);
        }

        public static BasePlayer GetPlayer(string id)
        {
            if (players.TryGetValue(id, out BasePlayer player))
            {
                return player;
            }

            return null;
        }

        private static BasePlayer GetCtrlPlayer()
        {
            return GetPlayer(StartGameMain.id);
        }

        private static void Reset()
        {
            //场景
            foreach (BasePlayer player in players.Values)
            {
                if (player != null)
                {
                    MonoBehaviour.Destroy(player.gameObject);
                }
            }

            //列表
            players.Clear();
        }

        private static void OnMsgSyncPlayer(MsgBase msgBase)
        {
            MsgSyncPlayer msg = (MsgSyncPlayer)msgBase;
            if (msg.id == StartGameMain.id)
            {
                return;
            }

            SyncPlayer player = (SyncPlayer)GetPlayer(msg.id);
            if (player == null)
            {
                return;
            }

            player.SyncPos(msg);
        }

        private static void OnMsgFire(MsgBase msgBase)
        {
            MsgFire msg = (MsgFire)msgBase;
            if (msg.id == StartGameMain.id)
            {
                return;
            }

            SyncPlayer player = (SyncPlayer)GetPlayer(msg.id);
            if (player == null)
            {
                return;
            }

            player.SyncFire(msg);
        }


        private static void OnMsgSwitchWeapon(MsgBase msgBase)
        {
            MsgSwitchWeapon msg = (MsgSwitchWeapon)msgBase;

            if (msg.id == StartGameMain.id)
            {
                return;
            }

            SyncPlayer syncPlayer = (SyncPlayer)GetPlayer(msg.id);
            if (syncPlayer == null)
            {
                return;
            }

            syncPlayer.SyncWeaponSwitch(msg);
        }

        private static void OnMsgSyncWeaponPosition(MsgBase msgBase)
        {
            MsgSyncWeaponPosition msg = (MsgSyncWeaponPosition)msgBase;
            if (msg.id == StartGameMain.id)
            {
                return;
            }

            SyncPlayer syncPlayer = (SyncPlayer)GetPlayer(msg.id);
            if (syncPlayer == null)
            {
                return;
            }

            syncPlayer.SyncWeaponPosition(msg);
        }

        private static void OnMsgHit(MsgBase msgBase)
        {
            MsgHit msg = (MsgHit)msgBase;
            BasePlayer targetPlayer = GetPlayer(msg.targetId);
            if (targetPlayer == null)
            {
                return;
            }

            targetPlayer.TakeDamage(msg);
        }

        // todo: 更改此显示方式
        private static async void OnMsgBattleResult(MsgBase msgBase)
        {
            ResManager.Instance.LoadStartSceneResources();
            
            MsgBattleResult msg = (MsgBattleResult)msgBase;
            bool isWin = false;
            BasePlayer player = GetCtrlPlayer();
            if (player != null && player.camp == msg.winCamp)
            {
                isWin = true;
            }

            Debug.Log(isWin);

            await LoadSceneAsync("StartGameUIScene");
            ResManager.Instance.UnloadAssetBundle("player", true);
            ResManager.Instance.UnloadAssetBundle("bullet", true);
            ResManager.Instance.UnloadAssetBundle("bullet_scene_ui", true);
            ResManager.Instance.UnloadAssetBundle("effect", true);

            PanelManager.Open<ResultPanel>(isWin);
            ResManager.Instance.UnloadAssetBundle("result_ui", false);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            // MonoBehaviour.Destroy(tank.gameObject);
        }


        private static async void OnMsgResult(MsgBase msgBase)
        {
            MsgResult msg = (MsgResult)msgBase;

            // 直接将数据存到内存中----字典中
            //  {"id": "1","coin": 0,"text": "","win": 1, "lost": 0}

            foreach (PlayerData playerData in msg.playerData)
            {
                StartGameMain.playerRecords[playerData.id] = playerData;
            }

            //todo: 异步保存到本地文件
            await SaveToLocalFileAsync();
            // PanelManager.Open<RoomListPanel>();
        }

        // text = {"win": 1, "lost": 0}
        private static async Task SaveToLocalFileAsync()
        {
            if (StartGameMain.playerRecords.TryGetValue(StartGameMain.id, out PlayerData playerData))
            {
                await File.WriteAllTextAsync(StartGameMain.path, JsonUtility.ToJson(playerData));
            }
        }
    }
}