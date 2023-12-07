using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    private static readonly Dictionary<string, BasePanel> panels = new();
    private static Transform ui;


    public static void Init()
    {
        panels.Clear();
        Debug.Log("panels.Count: " + panels.Count);
        ui = GameObject.Find("UI").transform;
    }

    // 初始化资源，设置层级和父节点
    public static void Open<T>(params object[] para) where T : BasePanel
    {
        string name = typeof(T).ToString();

        if (panels.ContainsKey(name))
        {
            Debug.Log("panel is already open");
            return;
        }

        BasePanel panel = ui.gameObject.AddComponent<T>();
        // 初始化资源路径，设置层级
        panel.OnInit();
        // 加载资源并实例化 
        panel.Init();

        // 将panelPrefab放到对应的层级下
        panel.panelPrefab.transform.SetParent(ui, false);

        panels.TryAdd(name, panel);
        panel.OnShow(para);
    }


    public static void Close(string name)
    {
        if (!panels.ContainsKey(name)) return;
        BasePanel panel = panels[name];
        panel.OnClose();

        panels.Remove(name);
        Debug.Log("panel.panelPrefab: " + panel.panelPrefab);
        Debug.Log("panel:" + panel);
        Destroy(panel.panelPrefab);
        // Destroy(panel);
    }
}