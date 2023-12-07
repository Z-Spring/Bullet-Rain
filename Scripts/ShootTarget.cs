using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class ShootTarget : MonoBehaviour
    {
        [SerializeField] private Camera cam;

        private void Update()
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {
               transform.position = hitInfo.point;
            }
        }
    }
}