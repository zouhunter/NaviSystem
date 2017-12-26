using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace NaviSystem
{

    [CreateAssetMenu(menuName = "创建/引导对象")]
    public class NaviObject : ScriptableObject
    {
        public Material material;
        [HideInInspector]
        public List<NaviNode> nodeList = new List<NaviNode>();
    }

}