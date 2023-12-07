using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.EventSystems;


public class MyEvent : UnityEvent<Vector3>
{
}

public class QuatTest : MonoBehaviour
{
    private NavMeshAgent agent;

    public int rayNumbers = 30;
    public float rayLength = 5f;
    public MyEvent Event1 = new();

    private void Start()
    {
        // Event1.AddListener((pos) => { Debug.Log(pos); });
        Event1.AddListener(F1);
    }

    private void F1(Vector3 pos)
    {
        Debug.Log(pos);
    }

    void Update()
    {
        // float v = Input.GetAxis("Vertical");
        // float h = Input.GetAxis("Horizontal");
        //         
        // // 将横向输入转化为左右旋转，将纵向输入转化为俯仰旋转，得到一个很小的旋转四元数
        // Quaternion smallRotate = Quaternion.Euler(v, h, 0);
        //         
        // // 将这个小的旋转叠加到当前旋转位置上
        // if (Input.GetButton("Fire1"))
        // {
        //         
        //     //  按住鼠标左键或Ctrl键时，沿世界坐标轴旋转
        //     transform.rotation = smallRotate * transform.rotation;
        // }
        // else
        // {
        //     //  不按鼠标左键和Ctrl键时，沿局部坐标轴旋转
        //     transform.rotation = transform.rotation * smallRotate;
        // }

        //  前方
        // if (Input.GetButton("Fire1"))
        // {
        //     Quaternion q = Quaternion.identity; // identity相当于Eular(0, 0, 0)，不旋转
        //     //  改变物体的朝向，取当前朝向与正前方之间10%的位置
        //     transform.rotation = Quaternion.Slerp(transform.rotation, q, 0.1f);
        // }

        FieldView();
    }

    private void FieldView()
    {
        Vector3 leftForward = Quaternion.Euler(0, -45, 0) * transform.forward * rayLength;

        for (int i = 0; i < rayNumbers; i++)
        {
            Vector3 v = Quaternion.Euler(0, (90.0f / rayNumbers) * i, 0) * leftForward * rayLength;
            Vector3 pos = transform.position + v;

            Ray ray = new Ray(transform.position, pos);
            if (Physics.Raycast(ray, out RaycastHit hit, rayLength))
            {
                Event1?.Invoke(hit.point);
                pos = hit.point;
                Debug.DrawLine(transform.position, pos, Color.red);
            }
            else
            {
                Debug.DrawRay(transform.position, pos, Color.green);
            }
        }
    }
}