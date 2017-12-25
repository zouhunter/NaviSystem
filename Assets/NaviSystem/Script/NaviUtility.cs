using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace NaviSystem
{

    public class NaviUtility
    {
        public static List<NaviNode> LoadNaviNodes(Transform root)
        {
            var naviNodes = new List<NaviNode>();
            RetriveTransform(root, (x) => {
                naviNodes.Add(x);
            });
            naviNodes.Sort((x, y) => { return string.Compare(x.name, y.name); });
            return naviNodes;
        }
        public static void RetriveTransform(Transform root, UnityAction<NaviNode> onRetrive)
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

    }

}
