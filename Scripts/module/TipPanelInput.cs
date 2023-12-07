using System;
using UnityEngine;
using UnityEngine.UI;

namespace module
{
    public class TipPanelInput : MonoBehaviour
    {
        public Button confirmBtn;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                confirmBtn.onClick.Invoke();
            }
        }
    }
}