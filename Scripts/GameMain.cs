using System;
using UI.GameSceneUI;
using UnityEngine;
using weapon;

public class GameMain : MonoBehaviour
{
    private void Update()
    {
        // if (GameObject.FindGameObjectWithTag("WeaponHolder"))
        // {
        //     
        // }
        

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        if (Input.GetMouseButton(1))
        {
            // 隐藏鼠标指针
            Cursor.visible = false;
            // 锁定鼠标指针到屏幕中央
            Cursor.lockState = CursorLockMode.Locked;
        }

        NetManager.Update();
    }
}