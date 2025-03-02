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

public class UIGuildItem : ListView.ListViewItem
{
    public Text guildId;
    public Text guildName;
    public Text leader;
    public Text memebrNumber;

    public Image background;
    public Sprite normalBg;
    public Sprite selectedBg;

    public override void onSelected(bool selected)
    {
        this.background.overrideSprite = selected ? selectedBg : normalBg;
    }
    public NGuildInfo Info;

    private void Start()
    {
        this.background.enabled = false;
    }

    public void SetGuildInfo(NGuildInfo item)
    {
        this.Info = item;
        if (this.guildId != null) this.guildId.text = item.Id.ToString();
        if (this.guildName != null) this.guildName.text = item.GuildName.ToString();
        if (this.leader != null) this.leader.text = item.leaderName;
        if(this.memebrNumber != null) this.memebrNumber.text = item.memberCount.ToString();
    }

}
