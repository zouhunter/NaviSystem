using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class NaviNode : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler,IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    public UnityEngine.Events.UnityAction onComplete;

    //监听按下
    public void OnPointerDown(PointerEventData eventData)
    {
        PassEvent(eventData, ExecuteEvents.pointerDownHandler);
    }

    //监听抬起
    public void OnPointerUp(PointerEventData eventData)
    {
        PassEvent(eventData, ExecuteEvents.pointerUpHandler);
    }

    //监听点击
    public void OnPointerClick(PointerEventData eventData)
    {
        PassEvent(eventData, ExecuteEvents.submitHandler);
        PassEvent(eventData, ExecuteEvents.pointerClickHandler);
        onComplete.Invoke();
    }
    //鼠标移入
    public void OnPointerEnter(PointerEventData eventData)
    {
        PassEvent(eventData, ExecuteEvents.pointerEnterHandler);
    }
    //鼠标移出 
    public void OnPointerExit(PointerEventData eventData)
    {
        PassEvent(eventData, ExecuteEvents.pointerExitHandler);
    }

    //把事件透下去
    public void PassEvent<T>(PointerEventData data, ExecuteEvents.EventFunction<T> function)
        where T : IEventSystemHandler
    {
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(data, results);
        GameObject current = data.pointerCurrentRaycast.gameObject;
        for (int i = 0; i < results.Count; i++)
        {
            if (current != results[i].gameObject)
            {
                ExecuteEvents.Execute(results[i].gameObject, data, function);
                //RaycastAll后ugui会自己排序，如果你只想响应透下去的最近的一个响应，这里ExecuteEvents.Execute后直接break就行。
            }
        }
    }

    
}
