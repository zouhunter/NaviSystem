using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class NaviItem : MonoBehaviour,/*IPointerDownHandler ,*/IComparable<NaviItem>{

    public string naviName;
    public int order;
    public RectRechange.PosID posId = RectRechange.PosID.ru;
    [Multiline(5)]
    public string information;
    public int delyCompletTime = 2;

    public UnityAction onEnd;
    public UnityAction onClick;
    public Selectable btn;
    void Start()
    {
        btn = GetComponent<Selectable>();
        NaviSystem.RegisterNaviItem(this);
        if (btn is Toggle)
        {
            ((Toggle)btn).onValueChanged.AddListener(OnValueChanged);
        }
        else
        {
            ((Button)btn).onClick.AddListener(OnPointerDown);
        }
    }

    public int CompareTo(NaviItem other)
    {
        return other.order == order ? 0: other.order > order ? -1 : 1;
    }

    public void OnValueChanged(bool isOn)
    {
        if (onClick != null)
        {
            onClick.Invoke();
        }
        if (onEnd != null)
        {
            //TimerInfo info = new TimerInfo(System.Guid.NewGuid().ToString(), onEnd, delyCompletTime * 60);
            //TimerManager.Instance.AddTimerEvent(info);
        }
    }

    public void OnPointerDown(/*PointerEventData eventData*/)
    {
        if (onClick != null)
        {
            onClick.Invoke();
        }
        if (onEnd !=null)
        {
            //TimerInfo info = new TimerInfo(System.Guid.NewGuid().ToString(), onEnd, delyCompletTime * 60);
            //TimerManager.Instance.AddTimerEvent(info);
        }
    }
}
