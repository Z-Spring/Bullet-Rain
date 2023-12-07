using System;
using System.Collections;
using System.Collections.Generic;
using UI.CrossHairUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class ColorPointerEventHandler : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
    {
        public ColorSelected colorSelected;
        public Outline outline;

        private void Start()
        {
            colorSelected = transform.parent.GetComponent<ColorSelected>();
            outline = GetComponent<Outline>();
            outline.enabled = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (colorSelected.CurrentActiveOutline != outline)
            {
                outline.enabled = true;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (colorSelected.CurrentActiveOutline != outline)
            {
                outline.enabled = false;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            colorSelected.SetActiveOutline(outline);
        }
    }
}