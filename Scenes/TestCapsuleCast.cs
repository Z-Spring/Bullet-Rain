using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class TestCapsuleCast : MonoBehaviour
{
    private CapsuleCollider coll;
    private float playerHight;
    private Vector3 playerCenter;
    private float playerRadius;

    private void Start()
    {
        coll = GetComponent<CapsuleCollider>();
        playerRadius = coll.radius;
        playerHight = coll.height;
        playerCenter = coll.center;
    }

    private void Update()
    {
        bool detect = Physics.CapsuleCast(transform.position, transform.position + Vector3.up * 0.5f, 0.5f,
            Vector3.forward, 0.5f);
        if (detect)
        {
            Debug.Log("detect");
        }

        float halfHeight = playerHight / 2;
        Vector3 worldCenter = transform.TransformPoint(playerCenter);
        Vector3 point1 = worldCenter - transform.up * halfHeight;
        Vector3 point2 = worldCenter + transform.up * halfHeight;
        Vector3 direction = transform.forward;
        float distance = 2f;
        float radius = playerRadius;


        Debug.DrawRay(point1, direction * distance, Color.green);
        Debug.DrawRay(point2, direction * distance, Color.green);
        Debug.DrawLine(point1, point2, Color.red);
// 绘制投射前的胶囊轮廓
        for (float t = 0; t <= 1; t += 0.2f)
        {
            Vector3 initialPoint = Vector3.Lerp(point1, point2, t);
            Debug.DrawRay(initialPoint, Vector3.left * radius, Color.blue);
            Debug.DrawRay(initialPoint, Vector3.right * radius, Color.blue);
        }

// 绘制投射后的胶囊轮廓
        for (float t = 0; t <= 1; t += 0.2f)
        {
            Vector3 finalPoint = Vector3.Lerp(point1, point2, t) + direction * distance;
            Debug.DrawRay(finalPoint, Vector3.left * radius, Color.yellow);
            Debug.DrawRay(finalPoint, Vector3.right * radius, Color.yellow);
        }
    }
}