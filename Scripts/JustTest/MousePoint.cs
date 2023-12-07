using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePoint : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 从摄像机发出到点击坐标的射线
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 1000, Color.red);
            RaycastHit hit;
            // 如果射线击中物体
            if (Physics.Raycast(ray, out hit, 1000))
            {
                // 打印射线击中的物体名称
                Debug.Log(hit.collider.gameObject.name);
                // 在击中位置生成一个预制体
                if (hit.collider.gameObject.name == "Plane")
                    Instantiate(Resources.Load<GameObject>("Prefabs/Cube"), hit.point, Quaternion.identity);
            }
        }
    }
}