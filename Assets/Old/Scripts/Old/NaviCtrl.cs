using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
public interface INaviCtrl
{
    bool ReStartNavi(out NaviItem item);
    void StopNavi();
    bool NextNavi(out NaviItem item);
}

public class NaviCtrl : INaviCtrl
{
    public int index;
    public IList<NaviItem> naviList;
    private bool stop;
    public NaviCtrl(List<NaviItem> naviList)
    {
        this.naviList = naviList;
    }

    public bool NextNavi(out NaviItem item)
    {
        if (stop)
        {
            item = null;
            return false;
        }
        if (index < naviList.Count)
        {
            item = naviList[index];
            index++;
            return true;
        }
        else
        {
            //StopNavi();
            item = null;
            return false;
        }
    }

    public bool ReStartNavi(out NaviItem item)
    {
        stop = false;
         index = 0;
        return NextNavi(out item);
    }

    public void StopNavi()
    {
        stop = true;
    }
}
