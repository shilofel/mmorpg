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
using Services;

public class UIGuildApplyItem : ListView.ListViewItem
{
    public Text nickname;
    public Text @class;
    public Text level;

    public NGuildApplyInfo Info;

    private void Start()
    {
        
    }

    public void SetItemInfo(NGuildApplyInfo item)
    {
        this.Info = item;
        if (this.nickname != null) this.nickname.text = item.Name;
        if (this.@class != null) this.@class.text = item.Class.ToString();
        if (this.level != null) this.level.text = item.Level.ToString();
    }

    public void OnAccept()
    {
        MessageBox.Show(string.Format("要通过[{0}]加入公会的申请吗?", this.Info.Name), "审批申请", MessageBoxType.Confirm, "确定", "取消").OnYes = () =>
        {
            GuildService.Instance.SendGuildJoinApply(true,this.Info);
        };
    }

    public void OnDecline()
    {
        MessageBox.Show(string.Format("要拒绝[{0}]加入公会的申请吗?", this.Info.Name), "审批申请", MessageBoxType.Confirm, "确定", "取消").OnYes = () =>
        {
            GuildService.Instance.SendGuildJoinApply(false, this.Info);
        };
    }
}
