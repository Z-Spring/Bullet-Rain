using System;
using ScriptObject;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using weapon;

namespace battle_player
{
    public class Player : BasePlayer
    {
        public float moveSpeed = 8f;
        public const float SyncPosInterval = 0.1f;
        [SerializeField] private bool isGround;
        [SerializeField] private float bulletLifeTime = 0.4f;
        
        private float syncSendTime;
        private bool canShoot;
        private WeaponSO currentWeaponSo;
        private bool canMove;
        private Vector3 point1, point2;
        private Vector3 playerCenter;
        private float playerRadius;

        private ShootLogic shootLogic;

        private void Start()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            
            currentBloodBar.fillAmount = 1;
            shootLogic = GetComponent<ShootLogic>();
        }

        void Update()
        {
            isGround = IsGrounded();
            Move();
            Jump();
            shootLogic.PlayerShoot(this);
            SyncPlayerPosition();
        }


        private void Move()
        {
            float x = Input.GetAxis("Horizontal"); // 输入左右
            float z = Input.GetAxis("Vertical"); // 输入前后


            // 方向永远平行地面，角色不能走到天上去
            // 获得角色前方向量，将Y轴分量设为0
            Vector3 fwd = transform.forward;
            Vector3 f = new Vector3(fwd.x, 0, fwd.z).normalized;

            // 角色的右方向量与右方向的移动直接对应，与抬头无关，可以直接用
            Vector3 r = transform.right;

            // 用f和r作向量的基，组合成移动向量
            Vector3 moveDir = f * z + r * x;

            GetPlayerColliderInfos();
            float moveDistance = moveSpeed * Time.deltaTime;
            // todo: 这里优化一下，感觉走起来很别扭
            canMove = !Physics.CapsuleCast(point1, point2, playerRadius, moveDir, moveDistance,
                LayerMask.GetMask("Wall"));
            // if (!canMove)
            // {
            //     Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            //     canMove = moveDir.x != 0 && !Physics.CapsuleCast(point1, point2, playerRadius, moveDirX, moveDistance,
            //         LayerMask.GetMask("Ground"));
            //     if (canMove)
            //     {
            //         moveDir = moveDirX;
            //     }
            //     else
            //     {
            //         Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
            //         canMove = moveDir.z != 0 && !Physics.CapsuleCast(point1, point2, playerRadius, moveDirZ,
            //             moveDistance, LayerMask.GetMask("Ground"));
            //         if (canMove)
            //         {
            //             moveDir = moveDirZ;
            //         }
            //     }
            // }

            // 直接改变玩家位置
            if (canMove)
            {
                transform.position += moveDir * moveDistance;
            }
        }

        private void GetPlayerColliderInfos()
        {
            playerRadius = capsuleCollider.radius;
            playerCenter = transform.TransformPoint(capsuleCollider.center + 0.3f * Vector3.up);
            float halfHeight = capsuleCollider.height / 2;
            Vector3 up = transform.up;
            point1 = playerCenter - up * halfHeight;
            point2 = playerCenter + up * halfHeight;
        }

        private void Jump()
        {
            if (Input.GetKeyDown(KeyCode.Space) && isGround)
            {
                rb.AddForce(Vector3.up * 4f, ForceMode.Impulse);
            }
        }

        private void SyncPlayerPosition()
        {
            if (Time.time - syncSendTime < SyncPosInterval) return;
            syncSendTime = Time.time;

            var pos = transform.position;
            var rot = transform.eulerAngles;

            MsgSyncPlayer msgSyncPlayer = new MsgSyncPlayer()
            {
                x = pos.x,
                y = pos.y,
                z = pos.z,

                ex = rot.x,
                ey = rot.y,
                ez = rot.z,
            };
            NetManager.Send(msgSyncPlayer);
        }


        // public bool IsDie()
        // {
        //     return hp <= 0;
        // }

        private bool IsGrounded()
        {
            Vector3 offset = new Vector3(0, 0.9f, 0);

            return Physics.OverlapSphere(transform.position - offset, 0.1f, LayerMask.GetMask("Ground")).Length > 0;
        }
    }
}