using System;
using System.Collections;
using battle_player;
using ScriptObject;
using UnityEngine;

namespace weapon
{
    public class WeaponSwitch : MonoBehaviour
    {
        public static event Action<WeaponSO> OnWeaponSwitch;
        public static event Action<Transform> OnChangeFirePoint;

        private Transform currentWeapon;
        private WeaponSO currentWeaponSo;

        public WeaponListSO weaponListSo;
        public Transform m4;
        public Transform l96;
        public Transform rifle;
        public Transform scifi;
        public Transform firePoint;
        public Camera scopeCamera;
        public GameObject scopeCameraPlane;

        private void Awake()
        {
            m4 = gameObject.transform.Find("M4");
            l96 = gameObject.transform.Find("L96");
            rifle = gameObject.transform.Find("Rifle");
            scifi = gameObject.transform.Find("SciFi");
        }

        private void OnEnable()
        {
            BattleManager.InitWeapon += InitWeapon;
            ShootLogic.ShowScopeCamera += ShowScopeCamera;
        }

        private void OnDisable()
        {
            BattleManager.InitWeapon -= InitWeapon;
            ShootLogic.ShowScopeCamera -= ShowScopeCamera;
        }

        private void InitWeapon()
        {
            SetActiveWeapon(rifle);
            currentWeaponSo = weaponListSo.weaponList[2];
            OnWeaponSwitch?.Invoke(currentWeaponSo);
            StartCoroutine(SyncSwitchWeaponCoroutine());
        }

        private IEnumerator SyncSwitchWeaponCoroutine()
        {
            yield return new WaitForSeconds(2f);
            SyncSwitchWeapon();
        }

        private void Update()
        {
            // if (parentPlayer is null)
            // {
            //     return;
            // }

            CheckWeaponSwitch(KeyCode.Alpha1, m4, weaponListSo.weaponList[0]);
            CheckWeaponSwitch(KeyCode.Alpha2, l96, weaponListSo.weaponList[1]);
            CheckWeaponSwitch(KeyCode.Alpha3, rifle, weaponListSo.weaponList[2]);
            CheckWeaponSwitch(KeyCode.Alpha4, scifi, weaponListSo.weaponList[3]);
        }

        private void CheckWeaponSwitch(KeyCode key, Transform weapon, WeaponSO weaponSo)
        {
            if (Input.GetKeyDown(key))
            {
                SetActiveWeapon(weapon);
                currentWeaponSo = weaponSo;
                SyncSwitchWeapon();

                OnWeaponSwitch?.Invoke(currentWeaponSo);
            }
        }


        private void SetActiveWeapon(Transform gun)
        {
            if (currentWeapon)
            {
                currentWeapon.gameObject.SetActive(false);
            }

            currentWeapon = gun;
            currentWeapon.gameObject.SetActive(true);
            firePoint = currentWeapon.Find("FirePoint");
            OnChangeFirePoint?.Invoke(firePoint);
        }

        private void ShowScopeCamera(bool isMouseRightDown)
        {
            scopeCamera.gameObject.SetActive(isMouseRightDown);
            scopeCameraPlane.SetActive(isMouseRightDown);
        }

        // private void HideScopeCamera()
        // {
        //     scopeCamera.gameObject.SetActive(false);
        // }

        private void SyncSwitchWeapon()
        {
            MsgSwitchWeapon msg = new MsgSwitchWeapon
            {
                weaponId = currentWeaponSo.weaponId
            };
            Debug.Log("Player.count [WeaponSwitch] " + BattleManager.players.Count);
            NetManager.Send(msg);
        }
    }
}