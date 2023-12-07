using System;
using UI.CrossHairUI;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [Serializable]
    public class ColorSetting
    {
        public Color color;
    }

    public class OptionsUI : MonoBehaviour
    {
        private bool isOpened;
        private Transform settingPanel;
        private Button saveButton;
        private Button defaultButton;
        private ColorSetting colorSetting;
        [SerializeField] private Image testImage;
        [SerializeField] private ColorSelected colorSelected;

        private void Start()
        {
            LoadCrossHairColor();

            settingPanel = transform.Find("SettingPanel");
            settingPanel.gameObject.SetActive(false);
            saveButton = settingPanel.Find("SaveButton").GetComponent<Button>();
            defaultButton = settingPanel.Find("DefaultButton").GetComponent<Button>();
            colorSelected = settingPanel.Find("CrossHairSettings/CrossHairPanel/ColorSetting/ColorSelect")
                .GetComponent<ColorSelected>();

            saveButton.onClick.AddListener(() =>
            {
                testImage.color = colorSelected.CurrentActiveOutline.effectColor;
                Debug.Log("Save");
                colorSetting = new ColorSetting()
                {
                    color = testImage.color
                };
                string colorJson = JsonUtility.ToJson(colorSetting);
                PlayerPrefs.SetString("CrossHairColor", colorJson);
                PlayerPrefs.Save();
            });

            defaultButton.onClick.AddListener(() => { Debug.Log("Default"); });
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                isOpened = !isOpened;
                settingPanel.gameObject.SetActive(isOpened);
            }
        }

        // 设置应该是要用Json持久化到本地文件中
        private void LoadCrossHairColor()
        {
            string s = PlayerPrefs.GetString("CrossHairColor");
            if (!string.IsNullOrEmpty(s))
            {
                colorSetting = JsonUtility.FromJson<ColorSetting>(s);
                if (colorSetting != null)
                {
                    testImage.color = colorSetting.color;
                }
                else
                {
                    // 处理无法反序列化的情况，比如设置默认颜色
                    testImage.color = Color.white;
                }
            }
            else
            {
                // 处理字符串为空的情况，比如设置默认颜色
                testImage.color = Color.white;
            }
        }
    }
}