using System.Collections.Generic;
using pool;
using ScriptObject;
using UnityEngine;
using Newtonsoft.Json;

namespace JustTest
{
    public class TestSom : MonoBehaviour
    {
        public float shakeDuration = 0.5f;
        public float shakeAmount = 0.1f;
        private Vector3 originalPos;

        [SerializeField] private WeaponListSO weaponListSo;
        // [SerializeField] private TestCameraShake t;
        private Dictionary<string, int[]> bulletDict = new();

        // private void Awake()
        // {
        //     PlayerPrefs.DeleteKey("bulletDict");
        // }
        // private void Awake()
        // {
        //     // PlayerPrefs.DeleteKey("bulletDict");
        //     SetInitWeaponBullets();
        // }
        //
        // private void Start()
        // {
        //     InitWeaponBullets();
        // }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                Bullet b = GetBullet(new Vector3(0, 0, 3), Quaternion.identity);
                b.Init(Vector3.up.normalized);
            }

            //
            if (Input.GetKeyDown(KeyCode.G))
            {
                // foreach (WeaponSO weaponSo in weaponListSo.weaponList)
                // {
                //     Debug.Log("weaponSo: " + weaponSo);
                //
                // }
                Shake();
            }
        }

        void Start()
        {
            originalPos = transform.localPosition;
        }

        public void Shake()
        {
            Debug.Log("shake");
            LeanTween.value(gameObject, UpdateShake, 0, 1, shakeDuration);
        }

        // void UpdateShake(float value)
        // {
        //     float xOffset = Random.Range(-shakeAmount, shakeAmount);
        //     float yOffset = Random.Range(-shakeAmount, shakeAmount);
        //     Vector3 newPosition = originalPos + new Vector3(xOffset, yOffset, 0);
        //     transform.localPosition = newPosition;
        // }

        void UpdateShake(float value)
        {
            float effectiveShakeAmount = shakeAmount * (1 - value); // 摇晃效果会随时间逐渐减弱
            float xOffset = Random.Range(-effectiveShakeAmount, effectiveShakeAmount);
            float yOffset = Random.Range(-effectiveShakeAmount, effectiveShakeAmount);
            Vector3 newPosition = originalPos + new Vector3(xOffset, yOffset, 0);
            transform.localPosition = newPosition;
        }


        void ResetCameraPosition()
        {
            // .setOnComplete(ResetCameraPosition);
            transform.localPosition = originalPos;
        }

        private Bullet GetBullet(Vector3 pos, Quaternion rot)
        {
            // GameObject bulletPrefab = BulletPool.Instance.GetBullet();
            GameObject bulletPrefab = GameObjectPool.Instance.GetGameObject<Bullet>();
            bulletPrefab.transform.position = pos;
            bulletPrefab.transform.rotation = rot;
            bulletPrefab.transform.Rotate(Vector3.right * 90);
            Bullet b = bulletPrefab.GetComponent<Bullet>();
            return b;
        }

        private void SetInitWeaponBullets()
        {
            if (PlayerPrefs.HasKey("bulletDict"))
            {
                return;
            }

            Debug.Log("set init weapon bullets");
            foreach (WeaponSO weaponSo in weaponListSo.weaponList)
            {
                if (bulletDict.ContainsKey(weaponSo.ToString()))
                {
                    continue;
                }

                bulletDict.Add(weaponSo.ToString(), new[] { weaponSo.currentBulletCount, weaponSo.remainBulletCount });
            }

            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            string json = JsonConvert.SerializeObject(bulletDict, settings);
            PlayerPrefs.SetString("bulletDict", json);
            Debug.Log("set json: " + json);
        }

        private void InitWeaponBullets()
        {
            if (!PlayerPrefs.HasKey("bulletDict"))
            {
                return;
            }

            Debug.Log("init weapon bullets");
            string json = PlayerPrefs.GetString("bulletDict");
            Debug.Log("get json: " + json);
            bulletDict = JsonConvert.DeserializeObject<Dictionary<string, int[]>>(json);
            foreach (var weaponSo in weaponListSo.weaponList)
            {
                if (bulletDict.TryGetValue(weaponSo.ToString(), out int[] bulletCount))
                {
                    weaponSo.currentBulletCount = bulletCount[0];
                    weaponSo.remainBulletCount = bulletCount[1];
                }
            }
        }
    }
}