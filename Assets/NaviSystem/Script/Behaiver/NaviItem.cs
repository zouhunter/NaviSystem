using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace NaviSystem
{

    public class NaviItem : MonoBehaviour
    {
        public NaviNode nodeInfo { get; set; }
        public UnityAction onComplete { get; set; }
        private void Awake()
        {
            var pass = gameObject.AddComponent<EventPass>();
            pass.onPointClick += OnClickItem;
            InitImage();
        }
        private void InitImage()
        {
            var img = gameObject.AddComponent<Image>();
            img.color = Color.clear;
        }

        private void OnClickItem()
        {
            if (this.onComplete != null)
            {
                this.onComplete.Invoke();
            }
        }
    }

}