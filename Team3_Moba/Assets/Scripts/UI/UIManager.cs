using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;

public class UIManager : MonoSingleton<UIManager>
{
    public Transform UIOpenedTransform;
    public Transform UIClosedTransform;

    private UIBase frontUI;

    private Dictionary<Type, GameObject> OpenUIPool = new Dictionary<Type, GameObject>();
    private Dictionary<Type, GameObject> CloseUIPool = new Dictionary<Type, GameObject>();

    private UIBase GetUI<T>(out bool isAlreadyOpen)
    {
        Type uiType = typeof(T);
        UIBase ui = null;
        isAlreadyOpen = false;

        if(OpenUIPool.ContainsKey(uiType))
        {
            ui = OpenUIPool[uiType].GetComponent<UIBase>();
            isAlreadyOpen = true;
        }
        else if(CloseUIPool.ContainsKey(uiType))
        {
            ui = CloseUIPool[uiType].GetComponent<UIBase>();
            CloseUIPool.Remove(uiType);
        }
        else
        {
            var uiObj = Instantiate(Resources.Load($"UI/{uiType}", typeof(GameObject))) as GameObject;
            ui = uiObj.GetComponent<UIBase>();
        }

        return ui;
    }

    public void OpenUI<T>(UIBaseData uiData)
    {
        Type uiType = typeof(T);
        bool isAlreadyOpen = false;
        var ui = GetUI<T>(out isAlreadyOpen);   

        if(ui == null)
        {
            Logger.LogError($"{uiType} prefab dosen't exist in Resources");
            return;
        }

        if(isAlreadyOpen)
        {
            return;
        }

        var sibilingIndex = UIOpenedTransform.childCount;
        ui.Initialize(UIOpenedTransform);
        ui.transform.SetSiblingIndex(sibilingIndex);
        ui.gameObject.SetActive(true);
        ui.SetInfo(uiData);
        ui.OpenUI();

        frontUI = ui;
        OpenUIPool.Add(uiType, ui.gameObject);

    }

    public void CloseUI(UIBase ui)
    {
        Type uiType = ui.GetType();
        ui.gameObject.SetActive(false);
        OpenUIPool.Remove(uiType);
        CloseUIPool.Add(uiType, ui.gameObject);
        ui.transform.SetParent(UIClosedTransform);
        frontUI = null;

        if(UIOpenedTransform.childCount > 0)
        {
            var lastChild = UIOpenedTransform.GetChild(UIOpenedTransform.childCount - 1);
            if(lastChild != null)
            {
                frontUI = lastChild.GetComponent<UIBase>();
            }
        }
    }
}
