using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class SettingPointerEventHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,
        IPointerClickHandler
    {
        public Outline outline;
        private bool isClick;

        private void Start()
        {
            outline = GetComponent<Outline>();
            outline.enabled = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            outline.enabled = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (isClick)
            {
                return;
            }

            outline.enabled = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            outline.enabled = true;
            isClick = true;
        }
    }
}