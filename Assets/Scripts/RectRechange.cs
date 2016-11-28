using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;


public class RectRechange : MonoBehaviour {
    public enum PosID
    {
        ru,
        rd,
        lu,
        ld
    }


    public Transform ru;
    public Transform rd;
    public Transform lu;
    public Transform ld;

    public RectTransform item;
    private Transform temp;

    public void ResetItemPos(PosID posid)
    {
        switch (posid)
        {
            case PosID.ru:
                temp = ru.GetChild(0);
                SetActive(rubool:true);
                SetLeftCenter();
                break;
            case PosID.rd:
                temp = rd.GetChild(0);
                SetActive(rdbool: true);
                SetLeftCenter();
                break;
            case PosID.lu:
                temp = lu.GetChild(0);
                SetActive(lubool: true);
                SetRightCenter();
                break;
            case PosID.ld:
                temp = ld.GetChild(0);
                SetActive(ldbool: true);
                SetRightCenter();
                break;
            default:
                break;
        }
        item.SetParent(temp,false);
        item.transform.localPosition = Vector3.zero;
    }

    void SetLeftCenter()
    {
        item.pivot =new Vector2(0, 0.5f);
    }

    void SetRightCenter()
    {
        item.pivot = new Vector2(1, 0.5f);
    }
    void SetActive(bool rubool = false,bool rdbool = false,bool lubool = false,bool ldbool= false)
    {
        ru.gameObject.SetActive(rubool);
        rd.gameObject.SetActive(rdbool);
        lu.gameObject.SetActive(lubool);
        ld.gameObject.SetActive(ldbool);
    }
}
