using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunCrossHair : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Camera followCam;

    void Update()
    {
        Ray ray = followCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hitInfo))
        {
            target.position = hitInfo.point;
        }
    }
}