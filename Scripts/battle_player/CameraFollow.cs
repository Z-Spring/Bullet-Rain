using UnityEngine;

namespace battle_player
{
    public class CameraFollow : MonoBehaviour
    {
        private void Update()
        {
            MouseLook();
        }

        private void MouseLook()
        {
            float mx = Input.GetAxis("Mouse X");
            float my = -Input.GetAxis("Mouse Y");

            Quaternion qx = Quaternion.Euler(0, mx, 0);
            Quaternion qy = Quaternion.Euler(my, 0, 0);

            transform.rotation = qx * transform.rotation;
            transform.rotation *= qy;

            // angle 是俯仰角度
            float angle = transform.eulerAngles.x;
            // 使用欧拉角时，经常出现-1°和359°混乱等情况，下面对这些情况加以处理
            if (angle > 180)
            {
                angle -= 360;
            }

            if (angle < -180)
            {
                angle += 360;
            }

            // 限制抬头、低头角度
            if (angle > 80)
            {
                transform.eulerAngles = new Vector3(80, transform.eulerAngles.y, 0);
            }

            if (angle < -80)
            {
                transform.eulerAngles = new Vector3(-80, transform.eulerAngles.y, 0);
            }
        }
    }
}