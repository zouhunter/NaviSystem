using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine;
using NaviSystem;
using System;

public class NaviController: INaviCtrl
{
    public UnityAction onComplete { get; set; }
    private Transform root;
    private Transform parent;
    private NaviObject naviObj;
    private NaviMask navimask;
    private Dictionary<int, NaviItem> nodeDic = new Dictionary<int, NaviItem>();

    private Dictionary<string, RectTransform> transDic = new Dictionary<string, RectTransform>();
    private int _id = -1;
    public NaviItem Current { get; private set; }
    public NaviController(Transform startParent, NaviObject naviObj)
    {
        this.root = startParent;
        this.naviObj = naviObj;
        Debug.Assert(naviObj != null, "[emptyerr]:naviNode is null");
        InitNaviMask();
    }

    public bool StartNavi()
    {
        NextNavi();
        return Current != null;
    }

    public void StopNavi()
    {
        if(parent)
        {
            UnityEngine.Object.Destroy(parent.gameObject);
        }
    }

    private void InitNaviMask()
    {
        InitExpandRectTransform(root as RectTransform);
        parent = FindTran("_NaviPanelInternal");
        parent.transform.SetParent(root, false);
        InitExpandRectTransform(parent as RectTransform);
        navimask = parent.gameObject.AddComponent<NaviMask>();
        navimask.material = naviObj.material;
        navimask.rootRect = root as RectTransform;
        var image = parent.gameObject.AddComponent<Image>();
        image.material = naviObj.material;
        var group = parent.gameObject.AddComponent<CanvasGroup>();
        group.ignoreParentGroups = true;
        group.interactable = true;
        group.blocksRaycasts = true;
        var btn = parent.gameObject.AddComponent<Button>();
        btn.onClick.AddListener(navimask.WarningCurrentNode);
    }

    private void InitExpandRectTransform(RectTransform rectTrans)
    {
        rectTrans.anchorMin = Vector2.zero;
        rectTrans.anchorMax = Vector2.one;
        rectTrans.sizeDelta = Vector2.zero;
        rectTrans.anchoredPosition = Vector2.zero;
        rectTrans.anchoredPosition = Vector2.one * 0.5f;
    }

    private NaviItem InitNaviNode(int id)
    {
        NaviItem epass = null;
        Debug.Assert(id < naviObj.nodeList.Count, "out index");
        var nodeInfo = naviObj.nodeList[id];
        if (!nodeDic.ContainsKey(id))
        {
            var parent = this.parent;
            List<string> path = new List<string>();
            for (int i = 0; i < nodeInfo.naviNodes.Count; i++)
            {
                var item = nodeInfo.naviNodes[i];
                path.Add(item.name);
                RectTransform rect = FindTran(path.ToArray());
                if (i == nodeInfo.naviNodes.Count - 1)
                {
                    epass = rect.gameObject.AddComponent<NaviItem>();
                    epass.onComplete = NextNavi;
                    nodeDic.Add(id,epass);
                }
                rect.SetParent(parent, false);
                rect.transform.localPosition = item.localPosition;
                rect.transform.localScale = item.localScale;
                rect.pivot = item.pivot;
                rect.anchoredPosition = item.anchoredPosition;
                rect.anchorMax = item.anchorMax;
                rect.anchorMin = item.anchorMin;
                rect.sizeDelta = item.sizeDelta;
                parent = rect;
            }
        }
        else
        {
            epass = nodeDic[id];
        }

        return epass;
    }

    private RectTransform FindTran(params string[] arg1)
    {
        var path = string.Join("/", arg1);
        var itemName = arg1[arg1.Length - 1];
        if (!transDic.ContainsKey(path))
        {
            var obj = new GameObject(itemName, typeof(RectTransform));
            transDic[path] = obj.GetComponent<RectTransform>();
        }
        return transDic[path];
    }

    private void NextNavi()
    {
        _id++;
        Debug.Log("id" + _id);
        if (_id == naviObj.nodeList.Count)
        {
            OnComplete();
        }
        else
        {
            InitNaviNode(_id);
            ActiveStep(_id);
        }
    }

    private void ActiveStep(int id)
    {
        foreach (var item in nodeDic)
        {
            if (item.Key == id)
            {
                Current = item.Value;
                Current.gameObject.SetActive(true);
                navimask.MoveToNode(Current.GetComponent<RectTransform>());
            }
            else
            {
                item.Value.gameObject.SetActive(false);
            }
        }
    }

    private void OnComplete()
    {
        if (this.onComplete != null)
        {
            onComplete.Invoke();
        }
    }
}
