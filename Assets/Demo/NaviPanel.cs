using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using NaviSystem;

public class NaviPanel : MonoBehaviour {
    public NaviObject naviObj;
    private INaviCtrl naviCtrl;

    private void Awake()
    {
        naviCtrl = new NaviController(transform, naviObj);
        naviCtrl.onComplete = () => { naviCtrl.StopNavi(); };
    }
    private void Start()
    {
        naviCtrl.StartNavi();
    }
}
