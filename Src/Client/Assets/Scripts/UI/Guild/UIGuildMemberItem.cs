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

public class UIGuildMemberItem : ListView.ListViewItem
{
    public Text nickName;
    public Text @class;
    public Text level;
    public Text title;
    public Text joinTime;
    public Text status;

    public Image background;
    public Sprite normalBg;
    public Sprite selectedBg;

    public override void onSelected(bool selected)
    {
        this.background.overrideSprite = selected ? selectedBg : normalBg;
    }

    public NGuildMemberInfo info;

    private void Start()
    {

    }

    public void SetGuildMemberInfo(NGuildMemberInfo item)
    {
        this.info = item;
        if (this.nickName != null) this.nickName.text = this.info.Info.Name;
        if (this.@class != null) this.@class.text = this.info.Info.Class.ToString();
        if (this.level != null) this.level.text = this.info.Info.Level.ToString();
        if (this.joinTime != null) this.joinTime.text = TimeUtil.GetTime(this.info.joinTime).ToShortDateString();
        if (this.title != null) this.title.text = this.info.Title.ToString();
        if (this.status != null) this.status.text = this.info.Status == 1 ? "在线" : TimeUtil.GetTime(this.info.lastTime).ToShortDateString();
    }
}