using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;

namespace NaviSystem
{

    public class RecordUtility
    {
        private static void TryRecordTransInfo(NaviNode node, RectTransform trans)
        {
            node.naviNodes.Clear();
            var currTrans = trans;
            while (currTrans.GetComponent<Canvas>() == null)
            {
                var tnode = RecordOneTransform(currTrans);
                node.naviNodes.Insert(0, tnode);
                currTrans = currTrans.parent as RectTransform;
            }
        }
        public static TransNode RecordOneTransform(RectTransform currTrans)
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
        public static void TryRecordToNaviNode(NaviNode naviNode, RectTransform targetTrans)
        {
            Debug.Log("[record]:" + targetTrans.name);
            TryRecordTransInfo(naviNode, targetTrans);
            var pfb = PrefabUtility.GetPrefabParent(targetTrans);
            if (pfb != null)
            {
                var pfbRoot = PrefabUtility.FindPrefabRoot((pfb as Transform).gameObject);
                naviNode.id = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(pfbRoot));
                naviNode.path = new List<string>();
                var trans = pfb as Transform;
                while (trans.gameObject != pfbRoot)
                {
                    naviNode.path.Insert(0, trans.name);
                    trans = trans.parent;
                }
            }
            else
            {
                var active = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
                naviNode.id = active.name;
                naviNode.path = new List<string>();
                var trans = targetTrans.transform;
                while (trans != null)
                {
                    naviNode.path.Insert(0, trans.name);
                    trans = trans.parent;
                }
            }
        }
        public static void UpdateNodeInfos(NaviObject naviObj)
        {
            var naviNodes = naviObj.nodeList;
            foreach (var item in naviNodes)
            {
                item.naviNodes.Clear();
                if (item.path == null || item.path.Count < 1)
                {
                    OnUpdateError(item, "路径信息记录不足");
                    continue;
                }
                if (string.IsNullOrEmpty(item.id))
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
                        var tran = FindTransformDeep(root.transform, item, 1);
                        currTrans = tran as RectTransform;

                    }
                    else
                    {
                        OnUpdateError(item, "找不到根目录");
                        continue;
                    }
                }
                else
                {
                    var pfb = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    var tran = FindTransformDeep(pfb.transform, item);
                    currTrans = tran as RectTransform;
                }

                if (currTrans == null)
                {
                    OnUpdateError(item, "找不到目标对象");
                    continue;
                }

                while (currTrans != null && currTrans.GetComponent<Canvas>() == null)
                {
                    var tnode = RecordUtility.RecordOneTransform(currTrans);
                    item.naviNodes.Insert(0, tnode);
                    currTrans = currTrans.parent as RectTransform;
                }

                Debug.Log("[更新:]" + item.name);
            }
        }
        private static void OnUpdateError(NaviNode item, string info)
        {
            Debug.Log(item.name + ":路径记录错误,请重新关联\n" + info);
        }

        public static Transform FindTransformDeep(Transform root, NaviNode item, int startid = 0)
        {
            var path = item.path.ToArray();
            Transform target = root;
            for (int i = startid; i < path.Length; i++)
            {
                if (target == null)
                {
                    OnUpdateError(item, "找到到:" + path[i]);
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

}
