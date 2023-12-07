using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace module
{
    public class RegisterPanelInput : MonoBehaviour
    {
        public Button registerBtn;
        public Selectable firstSelected;
        private EventSystem eventSystem;

        private void Start()
        {
            eventSystem = EventSystem.current;
            firstSelected.Select();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab) && Input.GetKey(KeyCode.LeftShift))
            {
                Selectable privious = eventSystem.currentSelectedGameObject.GetComponent<Selectable>()
                    .FindSelectableOnUp();
                if (privious)
                {
                    privious.Select();
                }
                else
                {
                    Debug.Log("next nagivation element not found");
                }
            }
            else if (Input.GetKeyDown(KeyCode.Tab))

            {
                Selectable next = eventSystem.currentSelectedGameObject.GetComponent<Selectable>()
                    .FindSelectableOnDown();
                if (next)
                {
                    next.Select();
                }
                else
                {
                    Debug.Log("next nagivation element not found");
                }
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                registerBtn.onClick.Invoke();
                Debug.Log("Enter");
            }
        }
    }
}