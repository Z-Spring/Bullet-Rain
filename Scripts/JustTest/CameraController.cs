using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera moveCamera;
   [SerializeField] private float rotationSpeed = 30f;
   [SerializeField] private float moveSpeed = 8f;


    private void Start()
    {
        // gameObject.SetActive(false);
        mainCamera.enabled = true;
        moveCamera.enabled = false;
    }

    private void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");


        if (Input.GetKeyDown(KeyCode.Space))
        {
            mainCamera.enabled = !mainCamera.enabled;
            moveCamera.enabled = !moveCamera.enabled;
        }

        if (moveCamera.enabled)
        {
            transform.position += new Vector3(x, 0, y) * moveSpeed * Time.deltaTime;
            transform.Rotate(0, x * rotationSpeed * Time.deltaTime, 0);
        }
        
        VerticalMove();
    }

    private void VerticalMove()
    {
        Vector3 pos = Vector3.up * moveSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.Q))
        {
            transform.position += pos;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            transform.position += Vector3.down * moveSpeed * Time.deltaTime;
        }
    }
}