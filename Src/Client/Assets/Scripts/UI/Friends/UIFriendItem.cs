using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Managers;
using Models;
using System;
using Common;
using Common.Data;
using SkillBridge.Message;

public class UIFriendItem:ListView.ListViewItem
{
    public Text nickName;
    public Text @class;
    public Text level;
    public Text status;

    public Image background;
    public Sprite normalBg;
    public Sprite selectedBg;

    public override void onSelected(bool selected)
    {
        this.background.overrideSprite = selected ? selectedBg : normalBg;
    }

    public NFriendInfo info;

    private void Start()
    {
        
    }

    public void SetFriendInfo(NFriendInfo item)
    {
        this.info = item;
        if (this.nickName != null) this.nickName.text = this.info.friendInfo.Name;
        if (this.@class != null) this.@class.text = this.info.friendInfo.Class.ToString();
        if (this.level != null) this.level.text = this.info.friendInfo.Level.ToString();
        if (this.status != null) this.status.text = this.info.Status == 1?"在线":"离线";
    }
}