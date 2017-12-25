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
        TryRecordEditorID();
    }

    private void TryRecordEditorID()
    {
        var pfb = PrefabUtility.GetPrefabParent(TargetTrans);
        if (pfb != null)
        {
            var pfbRoot = PrefabUtility.FindPrefabRoot((pfb as Transform).gameObject);
            Node.id = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(pfbRoot));
            Debug.Log("[record id]:" + Node.id);
            Node.path = new List<string>();
            var trans = pfb as Transform;
            while (trans.gameObject != pfbRoot)
            {
                Node.path.Insert(0, trans.name);
                trans = trans.parent;
            }
        }
        else
        {
            var active = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
            Node.id = active.name;
            Debug.Log("[record id(scene)]:" + Node.id);
            Node.path = new List<string>();
            var trans = TargetTrans.transform;
            while (trans != null)
            {
                Node.path.Insert(0, trans.name);
                trans = trans.parent;
            }
        }
    }
}

[InitializeOnLoad]
public class TransMatch
{
    static List<GUIContent> _nodeItemContents;
    static GUIContent[] NodeItemContents
    {
        get
        {
            if (_nodeItemContents == null)
            {
                _nodeItemContents = new List<GUIContent>();
                _nodeItemContents.Add(new GUIContent("拷贝坐标信息"));
            }

            return _nodeItemContents.ToArray();
        }
    }
    static List<GUIContent> _nodePanelContents;
    static GUIContent[] NodePanelContents
    {
        get
        {
            if (_nodePanelContents == null)
            {
                _nodePanelContents = new List<GUIContent>();
                _nodePanelContents.Add(new GUIContent("批量更新"));
            }
            return _nodePanelContents.ToArray();
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
                    EditorUtility.DisplayCustomMenu(new Rect(mousePosition.x, mousePosition.y, 0, 0), NodeItemContents, index, NodeItemCallBack, m);
                    Event.current.Use();
                }

            }
            else if (obj is GameObject)
            {
                var naviPanel = (obj as GameObject).GetComponent<NaviPanel>();
                if (naviPanel != null)
                {
                    EditorUtility.DisplayCustomMenu(new Rect(mousePosition.x, mousePosition.y, 0, 0), NodePanelContents, index, NodePanelCallBack, naviPanel);
                    Event.current.Use();
                }
            }
        }

    }

    private static void NodeItemCallBack(object userData, string[] options, int selected)
    {
        if (selected == 0)
        {
            var match = userData as MatchItem;
            match.Node.naviNodes.Clear();
            var currTrans = match.TargetTrans;
            while (currTrans.GetComponent<Canvas>() == null)
            {
                var tnode = RecordTransform(currTrans);
                match.Node.naviNodes.Insert(0, tnode);
                currTrans = currTrans.parent as RectTransform;
            }
        }
    }
    private static TransNode RecordTransform(RectTransform currTrans)
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
        return tnode;
    }

    private static void NodePanelCallBack(object userData, string[] options, int selected)
    {
        if (selected == 0)
        {
            var panel = userData as NaviPanel;
            var naviNodes = NaviUtility.LoadNaviNodes(panel.transform);
            foreach (var item in naviNodes)
            {
                item.naviNodes.Clear();
                if (item.path == null || item.path.Count < 1) {
                    OnUpdateError(item,"路径信息记录不足");
                    continue;
                }
                if(string.IsNullOrEmpty(item.id))
                {
                    OnUpdateError(item, "id信息记录不足");
                    continue;
                }

                var path = AssetDatabase.GUIDToAssetPath(item.id);
                RectTransform currTrans = null;
                if (string.IsNullOrEmpty(path))
                {
                    var activeScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
                    if (activeScene.name != item.id)
                    {
                        activeScene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(item.id, UnityEditor.SceneManagement.OpenSceneMode.Additive);
                    }
                    var roots = activeScene.GetRootGameObjects();
                    var root = Array.Find(roots, x => x.name == item.path[0]);
                    if (root != null)
                    {
                        var tran = FindTransformDeep(root.transform, item,1);
                        currTrans = tran as RectTransform;
                      
                    }
                    else
                    {
                        OnUpdateError(item,"找不到根目录");
                        continue;
                    }
                }
                else
                {
                    var pfb = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    var tran = FindTransformDeep(pfb.transform, item);
                    currTrans = tran as RectTransform;
                }

                if(currTrans == null)
                {
                    OnUpdateError(item, "找不到目标对象");
                    continue;
                }
                while (currTrans != null && currTrans.GetComponent<Canvas>() == null)
                {
                    var tnode = RecordTransform(currTrans);
                    item.naviNodes.Insert(0, tnode);
                    currTrans = currTrans.parent as RectTransform;
                }

                Debug.Log("[更新:]" + item.name);
            }
        }
    }
    private static void OnUpdateError(NaviNode item,string info)
    {
        Debug.Log(item.name + ":路径记录错误,请重新关联\n" + info);
    }
    private static Transform FindTransformDeep(Transform root,NaviNode item, int startid = 0)
    {
        var path = item.path.ToArray();
        Transform target = root;
        for (int i = startid; i < path.Length; i++)
        {
            if(target == null)
            {
                OnUpdateError(item,"找到到:" + path[i]);
                break;
            }
            else
            {
                target = target.FindChild(path[i]);
            }
        }
        return target;
    }
}
