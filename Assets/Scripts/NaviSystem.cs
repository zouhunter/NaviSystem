using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
/// <summary>
/// 无需加载顺序异步单例
/// </summary>
public class NaviSystem : MonoBehaviour
{
    public GameObject maskPanel;
    public GameObject itemMask;
    public Image image;
    public Text infoText;
    public RectRechange rectRechange;
    [Range(0,2)]
    public float duration;

    private Transform currentoldParent;
    private NaviItem current;
    private static Dictionary<string, List<NaviItem>> naviDic = new Dictionary<string, List<NaviItem>>();
    private static INaviCtrl naviCtrl;

    private Tweener fadeTween;
    private Tweener moveTween;
    private Tweener textTween;

    public event UnityAction OnDelete;

    void Awake()
    {
        fadeTween = image.DOFade(1, duration).SetAutoKill(false).SetLoops(-1).Pause();
        moveTween = itemMask.transform.DOMove(Vector3.zero,duration).SetAutoKill(false).Pause();
        textTween = infoText.DOText("", duration).SetAutoKill(false).Pause();
        maskPanel.SetActive(true);
    }

    private void Quit()
    {
        ClearLast();
        Destroy(gameObject);
    }

    public static void Reset()
    {
        if(naviCtrl != null) naviCtrl.StopNavi();
        naviDic.Clear();
    }

    /// <summary>
    /// 注册导航事件
    /// </summary>
    /// <param name="item"></param>
    /// <param name="onRegister"></param>
    public static void RegisterNaviItem(NaviItem item)
    {
        if (!naviDic.ContainsKey(item.naviName))
        {
            naviDic.Add(item.naviName, new List<NaviItem>() { item });
        }
        else
        {
            if (!naviDic[item.naviName].Contains(item))
            {
                naviDic[item.naviName].Add(item);
            }
        }
        naviDic[item.naviName].Sort();
    }
  

    public void ReStartNavi(string naviName)
    {
        if (naviDic.ContainsKey(naviName))
        {
            var items = naviDic[naviName];
            ClearLast();
            naviCtrl = new NaviCtrl(items);
            if (naviCtrl.ReStartNavi(out current))
            {
                SetActive(current);
            }
            else
            {
                Reset();
                Debug.LogError("001");
            }
        }
       
    }

    private void ClearLast()
    {
        if (current != null)
        {
            current.onClick = null;
            current.onEnd = null;
        }
    }

    private void SetActive(NaviItem item)
    {
        item.onEnd = CompletAction;
        item.onClick = OnSelected;
        currentoldParent = item.transform.parent;
        item.transform.SetParent(transform, true);
        ShowInfomation(item.posId);
    }

    private void OnSelected()
    {
        current.transform.SetParent(currentoldParent);
        itemMask.SetActive(false);
    }

    private void ShowInfomation(RectRechange.PosID posid)
    {
        rectRechange.ResetItemPos(posid);
        //infoText.text = current.information;
        textTween.ChangeEndValue(current.information).Restart();
        moveTween.ChangeValues(itemMask.transform.position,current.transform.position).Restart();
        fadeTween.Restart();
    }

    private void CompletAction()
    {
        current.onEnd = null;

        if (naviCtrl.NextNavi(out current))
        {
            itemMask.SetActive(true);
            SetActive(current);
        }
        else
        {
            //向导完成
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        if (OnDelete != null)
        {
            OnDelete();
        }
    }

    public void HandleMessage(object message)
    {
        ReStartNavi((string) message);
    }
}
