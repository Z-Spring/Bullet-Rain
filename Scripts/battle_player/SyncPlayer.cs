using System;
using pool;
using UnityEngine;

namespace battle_player
{
    public class SyncPlayer : BasePlayer
    {
        [SerializeField] private float bulletLifeTime = 0.4f;

        private Vector3 lastPos;
        private Vector3 lastRot;
        private Vector3 forecastPos;
        private Vector3 forecastRot;
        private float forecastTime;
        private Transform currentWeapon;
        private Animator anim;
        private Transform firePoint;
        private int scopeHash;

        public override void Init(string playerPrefab)
        {
            base.Init(playerPrefab);
            Vector3 pos = transform.position;
            Vector3 rot = transform.eulerAngles;
            lastPos = pos;
            lastRot = rot;
            forecastPos = pos;
            forecastRot = rot;
            forecastTime = Time.time;
        }

        private void Start()
        {
            anim = GetComponentInChildren<Animator>();
            scopeHash = Animator.StringToHash("Scope");
        }

        private void Update()
        {
            ForecastUpdate();
        }

        public void SyncPos(MsgSyncPlayer msg)
        {
            Vector3 pos = new Vector3(msg.x, msg.y, msg.z);
            Vector3 rot = new Vector3(msg.ex, msg.ey, msg.ez);
            forecastPos = pos + 2 * (pos - lastPos);
            forecastRot = rot + 2 * (rot - lastRot);
            forecastTime = Time.time;

            lastPos = pos;
            lastRot = rot;
            forecastTime = Time.time;
        }

        public void SyncFire(MsgFire msg)
        {
            Vector3 pos = new Vector3(msg.x, msg.y, msg.z);
            Quaternion rot = Quaternion.Euler(msg.ex, msg.ey, msg.ez);

            Vector3 dir = firePoint.forward;
            dir.Normalize();

            Bullet bullet = GetBullet(pos, rot);
            bullet.Init(dir, bulletLifeTime);
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

        public void SyncWeaponSwitch(MsgSwitchWeapon msg)
        {
            int weaponId = msg.weaponId;

            if (weaponIdToGameObject.TryGetValue(weaponId, out Transform weapon))
            {
                SetActiveWeapon(weapon);
            }
            else
            {
                Debug.LogWarningFormat("SyncSwitchWeapon: weaponId {0} is not found", weaponId);
            }
        }

        public void SyncWeaponPosition(MsgSyncWeaponPosition msg)
        {
            anim.SetBool(scopeHash, msg.isScope);
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
        }

        private void ForecastUpdate()
        {
            float t = (Time.time - forecastTime) / Player.SyncPosInterval;
            t = Mathf.Clamp(t, 0, 1);
            transform.position = Vector3.Lerp(transform.position, forecastPos, t);

            // todo: 旋转插值
            Quaternion forecastQuaternion = Quaternion.Euler(forecastRot);
            transform.rotation = Quaternion.Lerp(transform.rotation, forecastQuaternion, t);
        }
    }
}