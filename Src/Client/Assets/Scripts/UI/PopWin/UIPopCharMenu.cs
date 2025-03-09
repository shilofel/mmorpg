using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SkillBridge.Message;
using Managers;
using Models;
using System;
using Services;
using UnityEngine.EventSystems;

public class UIPopCharMenu : UIWindow, IDeselectHandler
{
    public int targetId;

    public string targetName;

    //重要
    public void OnDeselect(BaseEventData eventData)
    {
        var ed = eventData as PointerEventData;
        //hover点击事件点在当前界面中
        if (ed.hovered.Contains(this.gameObject))
            return;
        this.Close(WindowResult.None);
    }

    public void OnEnable()
    {
        this.GetComponent<Selectable>().Select();
        this.Root.transform.position = Input.mousePosition + new Vector3(80, 0, 0);
    }

    public void OnChat()
    {
        ChatManager.Instance.StartPrivateChat(targetId, targetName);
        this.Close(WindowResult.No);
    }

    public void OnAddFriend()
    {
        this.Close(WindowResult.No);
    }

    public void OnInviteTeam()
    {
        this.Close(WindowResult.No);
    }
}
