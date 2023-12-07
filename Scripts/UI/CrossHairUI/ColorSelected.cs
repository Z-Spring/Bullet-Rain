using UnityEngine;
using UnityEngine.UI;

namespace UI.CrossHairUI
{
    public class ColorSelected : MonoBehaviour
    {
        public Outline CurrentActiveOutline { get; private set; }

        internal void SetActiveOutline(Outline outline)
        {
            if (CurrentActiveOutline)
            {
                CurrentActiveOutline.enabled = false;
            }

            outline.enabled = true;
            CurrentActiveOutline = outline;
        }
    }
}