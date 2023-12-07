using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TipPanel : BasePanel
{
    private Button okBtn;
    private TextMeshProUGUI text;
    private string tipText;

    public override void OnInit()
    {
        panelPrefabPath = "TipPanel";
    }

    public override void OnShow(params object[] para)
    {
        okBtn = panelPrefab.transform.Find("OkBtn").GetComponent<Button>();
        text = panelPrefab.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        text.text = (string)para[0];
        tipText = (string)para[0];
        okBtn.onClick.AddListener(OnOkClick);
    }


    private void OnOkClick()
    {
        if (tipText.Contains("被踢下线"))
        {
            Application.Quit();
        }

        Close();
    }
}