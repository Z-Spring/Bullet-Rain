using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratePrefab : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private float radius;
    private void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 position = new Vector3(Mathf.Cos(i*(2 * Mathf.PI)/10),0, Mathf.Sin(i*(2 * Mathf.PI)/10));
            position *= radius;
            Instantiate(prefab, position, Quaternion.identity);
        }
    }
}
