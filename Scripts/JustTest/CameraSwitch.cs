using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CameraSwitch : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        mainCamera.enabled = false;
    }
}