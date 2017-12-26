using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using UnityEngine.Events;

namespace NaviSystem
{
    [Serializable]
    public class NaviNode
    {
        public string name;
        public string infomation;
        [HideInInspector]
        public List<TransNode> naviNodes = new List<TransNode>();
#if UNITY_EDITOR //用于快速更新
        public string id;//指定预制体
        public List<string> path = new List<string>();//指定路径
#endif
    }
}