using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using UnityEngine.Events;

namespace NaviSystem
{
    [RequireComponent(typeof(EventPass))]
    public class NaviNode : UIBehaviour
    {
        public UnityAction onComplete;
        public string infomation;
        public List<TransNode> naviNodes;
        public Func<List<string>,RectTransform> FindTran { get; set; }
        private bool inited;
        protected override void Awake()
        {
            base.Awake();
            var pass = gameObject.AddComponent<EventPass>();
            pass.onPointClick += OnPointClick;
        }

        public void OnActive()
        {
            if (inited) return;
            var parent = transform.parent;
            List<string> path = new List<string>();
            for (int i = 0; i < naviNodes.Count; i++)
            {
                var item = naviNodes[i];
                path.Add(item.name);
                RectTransform rect = null;
                if (i != naviNodes.Count - 1){
                    rect = FindTran(path);
                }
                else{
                    rect = GetComponent<RectTransform>();
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
            inited = true;
        }

        public void OnPointClick()
        {
            if (onComplete != null)
            {
                onComplete.Invoke();
            }
        }
    }
}