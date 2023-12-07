using beginGame;
using UnityEngine;

public class BasePanel : MonoBehaviour
{
    public string panelPrefabPath;
    public GameObject panelPrefab;


    // 初始化资源
    public void Init()
    {
        GameObject skinPrefab;
        /*if (panelPrefabPath == "ResultPanel")
        {
            skinPrefab = ResManager.Instance.loadedBundles["result_ui"].LoadAsset<GameObject>(panelPrefabPath);
        }
        else
        {
            skinPrefab = ResManager.Instance.loadedBundles["ui"].LoadAsset<GameObject>(panelPrefabPath);
        }

        if (skinPrefab is null)
        {
            Debug.LogError(panelPrefabPath + " is null");
        }*/
        skinPrefab = BeginGame.Instance.startUIDict[panelPrefabPath];
        if (skinPrefab is null)
        {
            Debug.LogError(panelPrefabPath + " is null");
        }

        panelPrefab = Instantiate(skinPrefab);
    }


    // 关闭资源
    protected void Close()
    {
        string typeName = GetType().ToString();
        Debug.Log("typeName: " + typeName);
        PanelManager.Close(typeName);
    }

    public virtual void OnInit()
    {
    }

    public virtual void OnShow(params object[] para)
    {
    }

    public virtual void OnClose()
    {
    }
}