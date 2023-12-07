using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MainCameraController : MonoBehaviour
{
    public bool zooming = true;
    public float zoomSpeed = 10f;
    [FormerlySerializedAs("camera")] public Camera mainCamera;

    private float zoomMax = 20f;

    private void Update()
    {
        if (zooming)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            float zoomDistance = zoomSpeed * Input.GetAxis("Vertical") * Time.deltaTime;
            // 摄像投射的最远距离 zoomMax，如果射线检测到了物体，就不会移动摄像机
            if (!Physics.Raycast(ray, zoomMax))
                mainCamera.transform.Translate(ray.direction * zoomDistance, Space.World);
        }
    }
}