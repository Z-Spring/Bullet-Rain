using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 后面逻辑得改，写的不好
/// </summary>
public class LookAtCamera : MonoBehaviour
{
    public Camera followCamera;
    public Camera birdViewCamera;
    private Camera mainCamera;

    public enum LookCamera
    {
        FollowCamera,
        MainCamera,
        BirdViewCamera
    }

    public LookCamera lookCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        followCamera = GameObject.FindGameObjectWithTag("Player").transform.GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        switch (lookCamera)
        {
            case LookCamera.FollowCamera:

                if (followCamera)
                {
                    Debug.Log("followCamera is not null");
                    transform.LookAt(followCamera.transform);
                }
                else
                {
                    lookCamera = LookCamera.BirdViewCamera;
                }

                break;
            case LookCamera.MainCamera:
                if (mainCamera is null)
                {
                    return;
                }

                transform.LookAt(mainCamera.transform);
                break;
            case LookCamera.BirdViewCamera:
                transform.LookAt(birdViewCamera.transform);
                break;
        }
    }
}