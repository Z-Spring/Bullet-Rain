using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletRoation : MonoBehaviour
{
    public Transform bulletPrefab;
    public Transform firePoint;
    void Start()
    {
        
        Instantiate(bulletPrefab, firePoint.position,Quaternion.Euler(0, 0, 90));
    }

    
}
