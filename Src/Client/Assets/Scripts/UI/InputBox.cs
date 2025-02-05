using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Models;
using Services;
using SkillBridge.Message;
using UnityEngine.Events;

public class InputBox
{

    static Object cacheObject = null;

    public static UIInputBox Show(string title, string message, string btnOK = "", string btnCancel = "", string emptyTips = "")
    {
        if (cacheObject == null)
        {
            cacheObject = Resloader.Load<Object>("UI/UIInputBox");
        }

        GameObject go = (GameObject)GameObject.Instantiate(cacheObject);
        UIInputBox msgbox = go.GetComponent<UIInputBox>();
        msgbox.Init(title, message, btnOK, btnCancel, emptyTips);
        return msgbox;
    }
}