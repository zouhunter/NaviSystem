using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
using NaviSystem;

public class MatchItem
{
    public NaviNode Node { get; private set; }
    public RectTransform TargetTrans { get; private set; }
    public MatchItem(NaviNode node, RectTransform targetTrans)
    {
        this.Node = node;
        this.TargetTrans = targetTrans;
    }
}

[InitializeOnLoad]
public class TransMatch
{
    static List<GUIContent> _contents;
    static GUIContent[] Contents
    {
        get
        {
            if (_contents == null)
            {
                _contents = new List<GUIContent>();
                _contents.Add(new GUIContent("拷贝坐标信息"));
            }

            return _contents.ToArray();
        }
    }
    private static int index = 0;
    [InitializeOnLoadMethod]
    static void MenuToNaviNode()
    {
        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItem;
    }

    private static void OnHierarchyWindowItem(int instanceID, Rect selectionRect)
    {
        Vector2 mousePosition = Event.current.mousePosition;

        if (Event.current.type == EventType.MouseUp && Event.current.button == 1 && selectionRect.Contains(mousePosition))
        {
            var obj = EditorUtility.InstanceIDToObject(instanceID);
            var trans = Selection.GetTransforms(SelectionMode.TopLevel);
            if (obj is GameObject && trans != null && trans.Length == 2)
            {
                var naviNode = (obj as GameObject).GetComponent<NaviNode>();
                if (naviNode != null)
                {
                    var tran = trans[0] == (obj as GameObject).transform ? trans[1] : trans[0];
                    var m = new MatchItem(naviNode, tran as RectTransform);
                    EditorUtility.DisplayCustomMenu(new Rect(mousePosition.x, mousePosition.y, 0, 0), Contents, index, CallBack, m);
                    Event.current.Use();
                }

            }
        }
     
    }

    private static void CallBack(object userData, string[] options, int selected)
    {
        if(selected == 0)
        {
            var match = userData as MatchItem;
            match.Node.naviNodes.Clear();
            var currTrans = match.TargetTrans;
            while (currTrans.GetComponent<Canvas>() == null)
            {
                var tnode = new TransNode();
                tnode.name = currTrans.name;
                tnode.anchoredPosition = currTrans.anchoredPosition;
                tnode.anchorMax = currTrans.anchorMax;
                tnode.anchorMin = currTrans.anchorMin;
                tnode.localPosition = currTrans.localPosition;
                tnode.localRotation = currTrans.localRotation;
                tnode.localScale = currTrans.localScale;
                tnode.pivot = currTrans.pivot;
                tnode.sizeDelta = currTrans.sizeDelta;
                match.Node.naviNodes.Insert(0, tnode);
                currTrans = currTrans.parent as RectTransform;
            }
        }
    }
}
