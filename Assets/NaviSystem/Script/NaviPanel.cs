using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine;
namespace NaviSystem{
public class NaviPanel : MonoBehaviour {
    public NaviMask navimask;
    public List<NaviNode> naviNodes;
    private int _id = -1;
    private NaviNode node;
    private void Start()
    {
        if (naviNodes == null || navimask == null) Debug.LogError("[emptyerr]:naviNodes or navimask is null");
        foreach (var item in naviNodes)
        {
            item.onComplete = NextNavi;
        }
        NextNavi();
    }

    private void NextNavi()
    {
        _id++;
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
            if (i== _id)
            {
                navimask.Restart(naviNodes[i]);
            }
        }
    }

    private void OnComplete()
    {
        Destroy(gameObject);
    }
}}
