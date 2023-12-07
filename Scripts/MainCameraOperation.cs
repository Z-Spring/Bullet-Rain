
    using System;
    using UnityEngine;

    public class MainCameraOperation : MonoBehaviour
    {
        public void DisableMainCamera()
        {
            Camera.main.gameObject.SetActive(false);
        }
    }
