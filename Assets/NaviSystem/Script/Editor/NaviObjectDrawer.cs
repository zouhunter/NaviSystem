using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
using NaviSystem;

[CustomEditor(typeof(NaviObject))]
public class NaviObjectDrawer : Editor
{
    public enum Operation
    {
        MoveUp = 0,
        MoveDown = 1,
        Insert = 2,
        Delete = 3,
    }

    private NaviObject naviObj;
    private List<NaviNode> nodeList { get { return naviObj.nodeList; } }

    private const float btnWidth = 20;
    private bool expland;
    private GUIContent[] _contents;
    private GUIContent[] Contents
    {
        get
        {
            if (_contents == null)
            {
                _contents = Array.ConvertAll<string,GUIContent>(System.Enum.GetNames(typeof(Operation)),x=> new GUIContent(x));
            }
            return _contents;
        }
    }
    private void OnEnable()
    {
        naviObj = target as NaviObject;
        _contents = null;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        DrawNaviNodeList();
    }

    private void DrawNaviNodeList()
    {
        if(GUILayout.Button("[导航节点列表]", EditorStyles.toolbarDropDown)){
            expland = !expland;
        }

        for (int i = 0; i < nodeList.Count; i++)
        {
            var rect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth,  EditorGUIUtility.singleLineHeight * (expland ?2.2f : 1.2f));
            var idRect = new Rect(rect.x, rect.y, btnWidth, EditorGUIUtility.singleLineHeight);
            var nameRect = new Rect(rect.x + idRect.width, rect.y, 50, EditorGUIUtility.singleLineHeight);
            var textRect = new Rect(nameRect.width + idRect.width, rect.y, 100, EditorGUIUtility.singleLineHeight);
            var btnRect = new Rect(rect.width -2 * btnWidth, rect.y, btnWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.SelectableLabel(nameRect, "(id)");
            EditorGUI.SelectableLabel(idRect, string.Format("[{0}]", (i + 1)));
            nodeList[i].name = EditorGUI.TextField(textRect, nodeList[i].name);

            if(GUI.Button(btnRect, "u", EditorStyles.miniButtonLeft))
            {
                if(Selection.activeTransform != null && Selection.activeTransform is RectTransform)
                {
                    RecordUtility.TryRecordToNaviNode(nodeList[i], Selection.activeTransform as RectTransform);
                    if(string.IsNullOrEmpty(nodeList[i].name)){
                        nodeList[i].name = Selection.activeTransform.name;
                    }
                }
            }

            if(expland)
            {
                nameRect = new Rect(rect.x , rect.y + EditorGUIUtility.singleLineHeight, 50, EditorGUIUtility.singleLineHeight);
                textRect = new Rect(nameRect.width, nameRect.y, rect.width - nameRect.width, EditorGUIUtility.singleLineHeight);

                EditorGUI.SelectableLabel(nameRect, "info");
                nodeList[i].infomation = EditorGUI.TextField(textRect, nodeList[i].infomation);
            }
           

            var menuRect = new Rect(rect.width - 5 * btnWidth, rect.y, 5 * btnWidth, EditorGUIUtility.singleLineHeight * (expland ?2:1));
            EditorGUIUtility.AddCursorRect(menuRect, MouseCursor.Link);
            TryAddMenuToRect(menuRect, i);

            var lineRect = new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * (expland ? 2 : 1), rect.width, EditorGUIUtility.singleLineHeight * 0.2f);
            GUI.backgroundColor = Color.red;
            GUI.Box(lineRect, "");
            GUI.backgroundColor = Color.white;
        }
        var addone = GUILayout.Button("+", EditorStyles.toolbarButton);
        if (addone)
        {
            nodeList.Add(new NaviNode());
        }
        if(GUILayout.Button("update all", EditorStyles.toolbarButton))
        {
            RecordUtility.UpdateNodeInfos(naviObj);
        }
        using (var hor = new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("import from csv", EditorStyles.toolbarButton))
            {
                ImportNameAndInfo();
            }
            if(GUILayout.Button("exprot to csv",EditorStyles.toolbarButton))
            {
                ExportNameAndInfo();
            }
        }
    
        EditorUtility.SetDirty(naviObj);
    }
    private void TryAddMenuToRect(Rect rect, int index)
    {
        if (Event.current != null && rect.Contains(Event.current.mousePosition)
            && Event.current.button == 1 && Event.current.type <= EventType.mouseUp)
        {
            Vector2 mousePosition = Event.current.mousePosition;
            EditorUtility.DisplayCustomMenu(new Rect(mousePosition.x, mousePosition.y, 0, 0), Contents, -1, CallBack, index);
            Event.current.Use();
        }
    }

    private void CallBack(object userData, string[] options, int selected)
    {
        var index = (int)userData;
        switch ((Operation)selected)
        {
            case Operation.MoveUp:
                if(index > 0)
                {
                    var obj = nodeList[index];
                    nodeList.RemoveAt(index);
                    nodeList.Insert(index - 1, obj);
                }
                break;
            case Operation.MoveDown:
                if(index < nodeList.Count - 1)
                {
                    var obj = nodeList[index];
                    nodeList.RemoveAt(index);
                    nodeList.Insert(index + 1, obj);
                }
                break;
            case Operation.Insert:
                nodeList.Insert(index, new NaviNode());
                break;
            case Operation.Delete:
                nodeList.RemoveAt(index);
                break;
            default:
                break;
        }
    }

    private void ImportNameAndInfo()
    {
        var path = EditorUtility.OpenFilePanel("选择信息文档", Application.dataPath, "csv");
        if (!string.IsNullOrEmpty(path))
        {
            var text = System.IO.File.ReadAllText(path,System.Text.Encoding.GetEncoding("gb2312"));
            var csv = ParserCSV.Parse(text);
            for (int i = 1; i < csv.Length; i++)
            {
                var array = csv[i];
                var name = array[0];
                var infomation = array[1];
                if(nodeList.Count > i - 1)
                {
                    var item = nodeList[i - 1];
                    Debug.Assert(item.name == name, item.name + " != " + name);
                    item.infomation = infomation;
                }
                else
                {
                    nodeList.Add(new NaviNode());
                    var item = nodeList[i - 1];
                    Debug.Log("add:" + item.name);
                    item.name = name;
                    item.infomation = infomation;
                }
            }
        }
    }
    private void ExportNameAndInfo()
    {
        string[][] info = new string[nodeList.Count + 1][];
        info[0] = new string[2];
        info[0][0] = "Name";
        info[0][1] = "Information";
        for (int i = 0; i < nodeList.Count; i++)
        {
            info[i + 1] = new string[2];
            info[i + 1][0] = nodeList[i].name;
            info[i + 1][1] = nodeList[i].infomation;
        }
        var text = UParserCSV.UParser(info);
        var path = EditorUtility.SaveFilePanel("保存csv", Application.dataPath, "doc", "csv");
        Debug.Log(System.IO.Path.GetFullPath(path));
        if (!string.IsNullOrEmpty(path))
        {
            System.IO.File.WriteAllText( path, text, System.Text.Encoding.GetEncoding("gb2312"));
        }
    }
}
