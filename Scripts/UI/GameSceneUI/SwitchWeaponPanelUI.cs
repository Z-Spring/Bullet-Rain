using System;
using System.Collections;
using System.Collections.Generic;
using ScriptObject;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using weapon;
using Newtonsoft.Json;

namespace UI.GameSceneUI
{
    public class SwitchWeaponPanelUI : MonoBehaviour
    {
        //com.unity.nuget.Newtonsoft-json
        // com.unity.nuget.newtonsoft-json   correct
        [SerializeField] private Image gunImage;
        [SerializeField] private TextMeshProUGUI remainBulletCountText;
        [SerializeField] private TextMeshProUGUI tipText;
        [SerializeField] private TextMeshProUGUI reloadTimerText;
        [SerializeField] private TextMeshProUGUI currentBulletCountText;
        [SerializeField] private Animator remainBulletAnim;
        [SerializeField] private Transform crossHair;
        [SerializeField] private Image reloadImage;
        [SerializeField] private WeaponListSO weaponListSo;

        [Space(10)] [Header("Weapon")] [SerializeField]
        private WeaponSO currentWeaponSo;


        private float currentReloadTime;
        private int noRemainBulletHash;
        private GameObject reloadPanel;
        private Transform reloadText;
        private Dictionary<string, int[]> bulletDict = new();
        private int lastRemainBulletCount;
        private int lastCurrentBulletCount;

        private void Awake()
        {
            reloadPanel = GameObject.Find("ReloadPanel");
            reloadText = reloadPanel.transform.Find("ReloadText");
            reloadImage = reloadPanel.transform.Find("ReloadImage").GetComponent<Image>();
            tipText = reloadText.transform.Find("TipText").GetComponent<TextMeshProUGUI>();
            reloadTimerText = reloadText.transform.Find("ReloadTimeText").GetComponent<TextMeshProUGUI>();
            crossHair = GameObject.FindWithTag("CrossHair").transform;
            // PlayerPrefs.DeleteKey("bulletDict");
            SetInitWeaponBullets();
            InitWeaponBullets();
        }

        private void Start()
        {
            reloadImage.fillAmount = 0;
            reloadImage.gameObject.SetActive(false);
            reloadTimerText.gameObject.SetActive(false);
            tipText.gameObject.SetActive(false);
            noRemainBulletHash = Animator.StringToHash("NoRemainBullet");
        }

        private void OnEnable()
        {
            WeaponSwitch.OnWeaponSwitch += UpdatePanelUI;
        }

        private void OnDisable()
        {
            WeaponSwitch.OnWeaponSwitch -= UpdatePanelUI;
        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                ReLoadBullet();
            }
        }

        private void SetInitWeaponBullets()
        {
            if (PlayerPrefs.HasKey("bulletDict"))
            {
                return;
            }

            foreach (WeaponSO weaponSo in weaponListSo.weaponList)
            {
                bulletDict.Add(weaponSo.ToString(), new[] { weaponSo.currentBulletCount, weaponSo.remainBulletCount });
            }

            // string json = JsonUtility.ToJson(bullets);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            string json = JsonConvert.SerializeObject(bulletDict, settings);
            PlayerPrefs.SetString("bulletDict", json);
        }

        private void InitWeaponBullets()
        {
            if (!PlayerPrefs.HasKey("bulletDict"))
            {
                return;
            }

            string json = PlayerPrefs.GetString("bulletDict");
            bulletDict = JsonConvert.DeserializeObject<Dictionary<string, int[]>>(json);
            // bulletDict = JsonUtility.FromJson<Bullets>(json).bulletInfo;
            foreach (var weaponSo in weaponListSo.weaponList)
            {
                if (bulletDict.TryGetValue(weaponSo.ToString(), out int[] bulletCount))
                {
                    weaponSo.currentBulletCount = bulletCount[0];
                    weaponSo.remainBulletCount = bulletCount[1];
                }
            }
        }


        private void UpdatePanelUI(WeaponSO weaponSo)
        {
            Debug.Log("切换武器");
            currentWeaponSo = weaponSo;

            gunImage.sprite = currentWeaponSo.weaponIcon;
            int remainBulletCount = currentWeaponSo.remainBulletCount;
            int currentBulletCount = currentWeaponSo.currentBulletCount;
            SetBulletTextColor();

            currentBulletCountText.text = currentBulletCount.ToString();
            remainBulletCountText.text = remainBulletCount.ToString();
        }

        private void SetBulletTextColor()
        {
            if (currentWeaponSo.remainBulletCount != 0)
            {
                remainBulletCountText.color = Color.white;
            }
            else
            {
                remainBulletCountText.color = Color.red;
            }

            if (currentWeaponSo.currentBulletCount != 0)
            {
                currentBulletCountText.color = Color.white;
            }
            else
            {
                currentBulletCountText.color = Color.red;
            }
        }
        


        private void ReLoadBullet()
        {
            if (currentWeaponSo.remainBulletCount == 0)
            {
                remainBulletAnim.SetTrigger(noRemainBulletHash);
                return;
            }

            if (currentWeaponSo.currentBulletCount < currentWeaponSo.bulletCountMax)
            {
                if (currentWeaponSo.bulletCountMax - currentWeaponSo.currentBulletCount >
                    currentWeaponSo.remainBulletCount)
                {
                    currentWeaponSo.currentBulletCount += currentWeaponSo.remainBulletCount;
                    currentWeaponSo.remainBulletCount = 0;
                    Debug.Log(remainBulletCountText.color);
                }
                else
                {
                    currentWeaponSo.remainBulletCount -=
                        currentWeaponSo.bulletCountMax - currentWeaponSo.currentBulletCount;
                    currentWeaponSo.currentBulletCount = currentWeaponSo.bulletCountMax;
                }

                currentReloadTime = 0;
                StartCoroutine(ReLoadProcess());
            }
            else if (currentWeaponSo.currentBulletCount == currentWeaponSo.bulletCountMax)
            {
                StartCoroutine(BulletFull());
            }
        }

        private IEnumerator ReLoadProcess()
        {
            crossHair.gameObject.SetActive(false);
            reloadImage.gameObject.SetActive(true);
            reloadTimerText.gameObject.SetActive(true);

            Debug.Log("开始装弹");
            while (currentReloadTime <= currentWeaponSo.reloadTime)
            {
                currentReloadTime += Time.deltaTime;
                reloadImage.fillAmount = currentReloadTime / currentWeaponSo.reloadTime;
                reloadTimerText.text = (currentWeaponSo.reloadTime - currentReloadTime).ToString("F2");
                yield return null;
            }

            if (currentWeaponSo.remainBulletCount == 0)
            {
                remainBulletCountText.color = Color.red;
            }

            currentBulletCountText.color = Color.white;
            currentBulletCountText.text = currentWeaponSo.currentBulletCount.ToString();
            remainBulletCountText.text = currentWeaponSo.remainBulletCount.ToString();
            reloadImage.gameObject.SetActive(false);
            reloadTimerText.gameObject.SetActive(false);
            crossHair.gameObject.SetActive(true);
        }

        private IEnumerator BulletFull()
        {
            crossHair.gameObject.SetActive(false);
            reloadImage.gameObject.SetActive(true);
            reloadImage.fillAmount = 1;

            tipText.gameObject.SetActive(true);
            reloadTimerText.gameObject.SetActive(false);

            tipText.text = "子弹已满";
            yield return new WaitForSeconds(0.5f);

            tipText.gameObject.SetActive(false);
            reloadImage.gameObject.SetActive(false);
            crossHair.gameObject.SetActive(true);
        }

        private bool CanReload()
        {
            return currentWeaponSo.currentBulletCount < currentWeaponSo.bulletCountMax &&
                   currentWeaponSo.remainBulletCount > 0;
        }
    }
}