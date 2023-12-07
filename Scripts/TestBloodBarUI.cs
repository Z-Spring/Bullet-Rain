using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class TestBloodBarUI : MonoBehaviour
    {
        [SerializeField] private Image hitHealthBloodBar;
        [SerializeField] private Image currentBloodBar;

        [SerializeField]private float updateSpeed = 0.7f;

        private void Start()
        {
            currentBloodBar.fillAmount = 1f;
            hitHealthBloodBar.fillAmount = 1f;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                currentBloodBar.fillAmount += 0.1f;
            }

            
            if (Input.GetKeyDown(KeyCode.A))
            {
                currentBloodBar.fillAmount -= 0.1f;
                StartCoroutine(UpdateHitHealthBar());
            }
        }

        private IEnumerator UpdateHitHealthBar()
        {
            yield return new WaitForSeconds(1f);
            float currentBloodBarValue = hitHealthBloodBar.fillAmount;
            float escapeTime = 0f;
            while (escapeTime < updateSpeed)
            {
                escapeTime += Time.deltaTime;
                hitHealthBloodBar.fillAmount = Mathf.Lerp(currentBloodBarValue, currentBloodBar.fillAmount,
                    escapeTime / updateSpeed);
                yield return null;
            }

            // hitHealthBloodBar.fillAmount = currentBloodBarValue;
        }
    }
}