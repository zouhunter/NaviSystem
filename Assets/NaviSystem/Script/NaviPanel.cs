using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine;
using NaviSystem;

public class NaviPanel : MonoBehaviour
{
    public NaviMask navimask;
    public Transform NaviNodeRoot;
    private List<NaviNode> naviNodes;
    private int _id = -1;
    private void Start()
    {
        LoadNaviNodes();
        if (naviNodes == null || navimask == null) Debug.LogError("[emptyerr]:naviNodes or navimask is null");
        foreach (var item in naviNodes)
        {
            item.onComplete = NextNavi;
        }
        NextNavi();

    }

    private void LoadNaviNodes()
    {
        naviNodes = new List<NaviNode>();
        RetriveTransform(NaviNodeRoot, (x) =>
        {
            naviNodes.Add(x);
        });
        naviNodes.Sort((x, y) => { return string.Compare(x.name, y.name); });
    }
    private void RetriveTransform(Transform root, UnityAction<NaviNode> onRetrive)
    {
        var node = root.GetComponent<NaviNode>();
        if (node != null)
        {
            onRetrive.Invoke(node);
        }
        if (root.childCount > 0)
        {
            foreach (Transform child in root)
            {
                RetriveTransform(child, onRetrive);
            }
        }
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
                navimask.Restart(naviNodes[i]);
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

        //PopupPanel.Data data = new PopupPanel.Data("引导结束", "如需关闭引导系统，请在设置菜单中取消开启选择", ()=> {
        //});
        //BundleUISystem.UIGroup.Open<PopupPanel>(data);
    }
}
