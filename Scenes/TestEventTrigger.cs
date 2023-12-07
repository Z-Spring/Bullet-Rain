using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TestEventTrigger : MonoBehaviour
{
    public Image targetImage; // 将要添加EventTrigger的Image组件
    public Outline outline;

    void Start()
    {
        // 创建一个新的EventTrigger组件
        EventTrigger eventTrigger = targetImage.gameObject.AddComponent<EventTrigger>();

        // 创建一个新的EventTrigger.Entry用于保存事件的类型和响应
        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        entryEnter.eventID = EventTriggerType.PointerEnter; // 设置触发事件的类型，这里是鼠标点击
        entryEnter.callback.AddListener((eventData) => { OnImageEnter(); });
        // 把事件触发器的条目添加到EventTrigger组件中
        eventTrigger.triggers.Add(entryEnter);

        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((eventData) => { OnImageExit(); });
        eventTrigger.triggers.Add(entryExit);

        outline = targetImage.GetComponent<Outline>();
        outline.enabled = false;
    }

    // 当图片被点击时会调用这个方法
    private void OnImageEnter()
    {
        outline.enabled = true;
        Debug.Log("Image was clicked!");
    }

    private void OnImageExit()
    {
        outline.enabled = false;
        Debug.Log("Image was exit!");
    }
}