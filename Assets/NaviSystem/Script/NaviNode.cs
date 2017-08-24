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
    public class NaviNode : MonoBehaviour
    {
        public UnityAction onComplete;
        public string infomation;
        private Text infoText;
        private float delyTime = 1f;
        private float showTime = 2f;
        private void Awake()
        {
            var pass = gameObject.AddComponent<EventPass>();
            pass.onPointClick += OnPointClick;
        }

        IEnumerator Start()
        {
            yield return new WaitForSeconds(delyTime);
            //infoText = GetComponentInChildren<Text>();
            //if (infoText != null)
            //{
            //    infoText.DOText(infomation, showTime, true);
            //}
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