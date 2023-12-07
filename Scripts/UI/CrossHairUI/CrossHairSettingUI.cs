using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.CrossHairUI
{
    public class CrossHairSettingUI : MonoBehaviour, IPointerClickHandler
    {
        public Outline outline;
        public Transform crossHairPanel;


        private void Start()
        {
            crossHairPanel = transform.parent.Find("CrossHairPanel");
            outline = GetComponent<Outline>();
            crossHairPanel.gameObject.SetActive(false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            outline.enabled = true;
            crossHairPanel.gameObject.SetActive(true);
        }
    }
}