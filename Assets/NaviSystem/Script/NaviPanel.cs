using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine;
using NaviSystem;
using System;

public class NaviPanel : MonoBehaviour
{
    public NaviMask navimask;
    private Transform naviNodeRoot { get { return transform; } }
    private List<NaviNode> naviNodes;
    private Dictionary<string, RectTransform> transDic = new Dictionary<string, RectTransform>();
    private int _id = -1;

    private void Awake()
    {
        naviNodes =NaviUtility.LoadNaviNodes(naviNodeRoot);
        if (naviNodes == null || navimask == null) Debug.LogError("[emptyerr]:naviNodes or navimask is null");
        foreach (var item in naviNodes)
        {
            item.onComplete = NextNavi;
            item.FindTran = FindTran;
        }
    }
    private void Start()
    {
        NextNavi();
    }

    private RectTransform FindTran(List<string> arg1)
    {
        var path = string.Join("/", arg1.ToArray());
        var itemName = arg1[arg1.Count - 1];
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
        if (_id == naviNodes.Count)
        {
            OnComplete();
        }
        else
        {
            ActiveStep();
        }
    }

    private void ActiveStep()
    {
        for (int i = 0; i < naviNodes.Count; i++)
        {
            naviNodes[i].gameObject.SetActive(i == _id);
            if (i == _id)
            {
                naviNodes[i].OnActive();
                navimask.MoveToNode(naviNodes[i]);
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnComplete();
        }
    }

    private void OnComplete()
    {
        Destroy(gameObject);
    }
}
