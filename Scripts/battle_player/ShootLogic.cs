using System;
using pool;
using ScriptObject;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using weapon;

namespace battle_player
{
    public class ShootLogic : MonoBehaviour
    {
        public static event Action<Color> ChangeCrossHairColor;
        public static event Action<bool> ShowScopeCamera;
        public static event Action HideScopeCamera;

        [SerializeField] private float nextShootTime;
        [SerializeField] private float bulletLifeTime = 0.4f;

        private WeaponSO currentWeaponSo;
        private TextMeshProUGUI currentBulletCountText;
        private Animator currentBulletAnim;
        private int noBulletHash;
        private Transform firePoint;
        private Animator scopeAnim;
        private int scopeHash;
        private Vector3 shootTargetPosition;
        private WeaponListSO weaponListSo;
        private Color lastCrossHairColor;
        private WeaponSwitch weaponSwitch;
        private Transform crossHair;
        private Camera followCamera;
        private CameraShake cameraShake;

        private void Start()
        {
            currentBulletCountText = GameObject.Find("CurrentBulletCountText").GetComponent<TextMeshProUGUI>();
            currentBulletAnim = currentBulletCountText.GetComponent<Animator>();
            scopeAnim = GetComponentInChildren<Animator>();
            AssignAnimationIDs();

            shootTargetPosition = Vector3.zero;
            crossHair = GameObject.FindWithTag("CrossHair").transform;

            lastCrossHairColor = crossHair.GetChild(0).GetComponent<Image>().color;

            GameObject followCameraTransform = GameObject.FindWithTag("FollowCamera");
            followCamera = followCameraTransform.GetComponent<Camera>();
            cameraShake = followCameraTransform.GetComponent<CameraShake>();

            weaponSwitch = GetComponentInChildren<WeaponSwitch>();
            weaponListSo = weaponSwitch.weaponListSo;
        }

        private void Update()
        {
            SetShootAnimation();
            SetShootDirection();
        }

        private void OnEnable()
        {
            WeaponSwitch.OnWeaponSwitch += OnSetCurrentWeapon;
            WeaponSwitch.OnChangeFirePoint += OnChangeFirePoint;
        }

        private void OnDisable()
        {
            WeaponSwitch.OnWeaponSwitch -= OnSetCurrentWeapon;
            WeaponSwitch.OnChangeFirePoint -= OnChangeFirePoint;
        }

        private void OnSetCurrentWeapon(WeaponSO weaponSo)
        {
            currentWeaponSo = weaponSo;
        }

        private void OnChangeFirePoint(Transform firePoint)
        {
            this.firePoint = firePoint;
        }

        private void AssignAnimationIDs()
        {
            noBulletHash = Animator.StringToHash("NoBullet");
            scopeHash = Animator.StringToHash("Scope");
        }


        public void PlayerShoot(Player player)
        {
            if (IsRepeaterWeapon() && Input.GetMouseButton(0))
            {
                HandleShoot(player);
            }
            else if (!IsRepeaterWeapon() && Input.GetMouseButtonDown(0))
            {
                HandleShoot(player);
            }
        }

        private void HandleShoot(Player player)
        {
            if (Time.time < nextShootTime)
            {
                return;
            }

            if (currentWeaponSo.currentBulletCount == 0)
            {
                SetCurrentBulletCountTextColor(Color.red);
                currentBulletAnim.SetTrigger(noBulletHash);
                return;
            }

            SetCurrentBulletCountTextColor(Color.white);
            cameraShake.Shake();
            InitBullet(player);
            UpdateBulletCount();

            nextShootTime = Time.time + currentWeaponSo.shootInterval;
            SyncFire();
        }

        private void SyncFire()
        {
            Vector3 pos = firePoint.position;
            Vector3 rot = firePoint.eulerAngles;
            MsgFire msgFire = new MsgFire
            {
                x = pos.x,
                y = pos.y,
                z = pos.z,

                ex = rot.x,
                ey = rot.y,
                ez = rot.z,
            };

            NetManager.Send(msgFire);
        }

        private void SetCurrentBulletCountTextColor(Color color)
        {
            currentBulletCountText.color = color;
        }

        private void InitBullet(Player player)
        {
            Bullet b = GetBullet(firePoint.position, firePoint.rotation);
            b.firePlayer = player;
            b.Init(shootTargetPosition, bulletLifeTime);
        }

        private Bullet GetBullet(Vector3 pos, Quaternion rot)
        {
            GameObject bulletPrefab = GameObjectPool.Instance.GetGameObject<Bullet>();
            bulletPrefab.transform.position = pos;
            bulletPrefab.transform.rotation = rot;
            bulletPrefab.transform.Rotate(Vector3.right * 90);

            Bullet b = bulletPrefab.GetComponent<Bullet>();
            return b;
        }

        private bool IsRepeaterWeapon()
        {
            return currentWeaponSo == weaponListSo.weaponList[0];
        }


        private void UpdateBulletCount()
        {
            currentWeaponSo.currentBulletCount--;
            if (currentWeaponSo.currentBulletCount < 0)
            {
                currentWeaponSo.currentBulletCount = 0;
            }

            currentBulletCountText.text = currentWeaponSo.currentBulletCount.ToString();
        }

        private void SetShootDirection()
        {
            int weaponLayerIndex = LayerMask.NameToLayer("WeaponHolder");
            int weaponLayer = ~(1 << weaponLayerIndex);
            Ray camRay = followCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
            if (Physics.Raycast(camRay, out RaycastHit hit, Mathf.Infinity, weaponLayer))
            {
                Debug.Log("hit: " + hit.collider.tag);
                ChangeColor(hit);

                Vector3 pos = firePoint.position;
                shootTargetPosition = hit.point - pos;
                shootTargetPosition.Normalize();
            }
            else
            {
                shootTargetPosition = camRay.direction;
                shootTargetPosition.Normalize();
            }
        }

        private void ChangeColor(RaycastHit hit)
        {
            //
            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("hit player");
                ChangeCrossHairColor?.Invoke(Color.red);
            }
            else
            {
                ChangeCrossHairColor?.Invoke(lastCrossHairColor);
            }
        }

        private void SetShootAnimation()
        {
            bool isMouseDown = Input.GetMouseButtonDown(1);
            bool isMouseUp = Input.GetMouseButtonUp(1);
            if (isMouseDown || isMouseUp)
            {
                HandleInput(isMouseDown);
            }
        }

        private void HandleInput(bool isMouseDown)
        {
            scopeAnim.SetBool(scopeHash, isMouseDown);
            crossHair.gameObject.SetActive(!isMouseDown);
            if (currentWeaponSo.weaponName == "L96")
            {
                ShowScopeCamera?.Invoke(isMouseDown);
            }
            //
            // else
            // {
            //     HideScopeCamera?.Invoke();
            // }

            SyncWeaponPosition(isMouseDown);
        }

        private void SyncWeaponPosition(bool isScope)
        {
            MsgSyncWeaponPosition msgSyncWeaponPosition = new MsgSyncWeaponPosition
            {
                isScope = isScope
            };
            NetManager.Send(msgSyncWeaponPosition);
        }
    }
}