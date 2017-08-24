using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine;
public class Panel : MonoBehaviour {
    [SerializeField]
    private Button m_Confer;
    [SerializeField]
    private Button m_Close;
    
	// Use this for initialization
	void Start () {
        m_Confer.onClick.AddListener(()=>Debug.Log("需要提前将新引导的节点和对应的按扭匹配才能保证不会navi点击和事件分离！！"));
        m_Close.onClick.AddListener(()=> { Destroy(gameObject); });
	}
	
}
