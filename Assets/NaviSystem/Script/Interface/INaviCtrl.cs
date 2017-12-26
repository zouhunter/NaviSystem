using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using NaviSystem;
using System;

public interface INaviCtrl
{
    UnityAction onComplete { get; set; }
    UnityAction<NaviItem> onStepActive { get; set; }

    bool StartNavi();
    void StopNavi();
}
