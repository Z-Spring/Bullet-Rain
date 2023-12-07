using System.Collections;
using System.Collections.Generic;
using pool;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace battle_player
{
    public class BasePlayer : MonoBehaviour
    {
        public Transform crossHair;

        [Space(10)] [Header("PlayerInfo")] public float hp;
        public string id = "";
        public int camp;
        public float maxHp = 100;

        public Rigidbody rb;
        public CapsuleCollider capsuleCollider;
        public CameraShake cameraShake;
        public Camera followCamera;

        [Space(15)] [Header("BloodBar")] public Image currentBloodBar;
        public Image hitHealthBloodBar;
        public float bloodBarUpdateSpeed = 0.5f;


        [Space(15)] [Header("Weapon")] public Transform m4;
        public Transform l96;
        public Transform rifle;
        public Transform scifi;
        protected Dictionary<int, Transform> weaponIdToGameObject;

        public Transform weaponHolder;
        private Transform bulletPool;

        public virtual void Init(string playerPrefab)
        {
            // GameObject playerPrefabRef = ResManager.LoadPrefab(playerPrefab);
            GameObject asset = ResManager.Instance.loadedBundles["player"].LoadAsset<GameObject>(playerPrefab);
            GameObject player = Instantiate(asset, transform);
            InitPhysicsSettings();
            InitWeapons(player);
            player.transform.localPosition = Vector3.zero;
            player.transform.localEulerAngles = Vector3.zero;

            Transform followCameraTransform = player.transform.Find("FollowCamera");
            cameraShake = followCameraTransform.GetComponent<CameraShake>();
            followCamera = followCameraTransform.GetComponent<Camera>();
            crossHair = GameObject.FindWithTag("CrossHair").transform;
            Transform canvas = player.transform.Find("Canvas");
            InitBloodBarUI(canvas);
            hp = maxHp;
        }

        private void InitWeapons(GameObject player)
        {
            weaponHolder = player.transform.Find("WeaponCamera/WeaponHolder");
            m4 = weaponHolder.Find("M4");
            l96 = weaponHolder.Find("L96");
            rifle = weaponHolder.Find("Rifle");
            scifi = weaponHolder.Find("SciFi");

            weaponIdToGameObject = new Dictionary<int, Transform>
            {
                { 1, m4 },
                { 2, l96 },
                { 3, rifle },
                { 4, scifi }
            };
        }


        private void InitPhysicsSettings()
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.freezeRotation = true;
            rb.mass = Random.Range(70f, 85f);

            capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
            capsuleCollider.height = 2f;
        }

        private void InitBloodBarUI(Transform parent)
        {
            // GameObject prefab = ResManager.LoadPrefab("BloodBarUI");
            GameObject prefab = ResManager.Instance.loadedBundles["battle_scene_ui"]
                .LoadAsset<GameObject>("BloodBarUI");

            GameObject bloodBarUI = Instantiate(prefab, parent);
            currentBloodBar = bloodBarUI.transform.Find("BloodBar").GetComponent<Image>();
            hitHealthBloodBar = bloodBarUI.transform.Find("HitHealthBloodBar").GetComponent<Image>();
            currentBloodBar.fillAmount = 1f;
            hitHealthBloodBar.fillAmount = 1f;
        }

       

        public void TakeDamage(MsgHit msgHit)
        {
            int damage = msgHit.damage;
            if (IsDie())
            {
                return;
            }

            // todo: 暂时注释掉，摄像机抖动感觉不好
            // StartCoroutine(cameraShake.Shake());
            // cameraShake.Shake();
            hp -= damage;
            currentBloodBar.fillAmount = hp / maxHp;
            StartCoroutine(BloodBarFadeOut());
            if (IsDie())
            {
                transform.rotation = Quaternion.Euler(90, 0, 0);
                EventManager.TriggerPlayerDead(msgHit.targetId);
                Destroy(gameObject, 1f);
            }
        }

        // todo: 优化一下，设置成比如一段时间没伤害时，才会血条渐变，现在则是固定等待1s
        private IEnumerator BloodBarFadeOut()
        {
            yield return new WaitForSeconds(1f);
            float currentBloodBarValue = hitHealthBloodBar.fillAmount;
            float escapeTime = 0f;
            while (escapeTime < bloodBarUpdateSpeed)
            {
                escapeTime += Time.deltaTime;
                hitHealthBloodBar.fillAmount = Mathf.Lerp(currentBloodBarValue, currentBloodBar.fillAmount,
                    escapeTime / bloodBarUpdateSpeed);
                yield return null;
            }
        }

        private bool IsDie()
        {
            return hp <= 0;
        }
    }
}