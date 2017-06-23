using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
namespace NaviSystem{
[RequireComponent(typeof(EventPass))]
public class NaviNode : MonoBehaviour
{
    public UnityAction onComplete;
    private void Awake()
    {
        var pass = gameObject.AddComponent<EventPass>();
        pass.onPointClick += OnPointClick;
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