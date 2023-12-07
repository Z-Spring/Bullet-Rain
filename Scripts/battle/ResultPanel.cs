using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// call from BattleManager.cs
public class ResultPanel : BasePanel
{
    private Image winImage;
    private Image lostImage;
    private Button okBtn;

    //初始化
    public override void OnInit()
    {
        panelPrefabPath = "ResultPanel";
    }

    //显示
    public override void OnShow(params object[] args)
    {
        //寻找组件
        winImage = panelPrefab.transform.Find("WinImage").GetComponent<Image>();
        lostImage = panelPrefab.transform.Find("LostImage").GetComponent<Image>();
        okBtn = panelPrefab.transform.Find("OkBtn").GetComponent<Button>();
        //监听
        okBtn.onClick.AddListener(OnOkClick);
        //显示哪个图片
        if (args.Length == 1)
        {
            bool isWin = (bool)args[0];
            if (isWin)
            {
                winImage.gameObject.SetActive(true);
                lostImage.gameObject.SetActive(false);
            }
            else
            {
                winImage.gameObject.SetActive(false);
                lostImage.gameObject.SetActive(true);
            }
        }
    }
    

    //当按下确定按钮
    private void OnOkClick()
    {
        PanelManager.Open<RoomPanel>();
        Close();
    }
}