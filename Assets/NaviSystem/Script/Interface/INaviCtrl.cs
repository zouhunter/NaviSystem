using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public interface INaviCtrl
{
    UnityAction onComplete { get; set; }
    bool StartNavi();
    void StopNavi();
}
