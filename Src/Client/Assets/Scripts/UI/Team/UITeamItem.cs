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

public class UITeamItem : ListView.ListViewItem
{
    public Text nickName;
    public Image classIcon;
    public Image leaderIcon;

    public Image background;

    public override void onSelected(bool selected)
    {
        this.background.enabled = selected ? true : false;
    }
    public int idx;
    public NCharacterInfo info;

    private void Start()
    {
        this.background.enabled = false;
    }

    public void SetMemberInfo(int idx,NCharacterInfo item,bool isLeader)
    {
        this.idx = idx;
        this.info = item;
        if (this.nickName != null) this.nickName.text = this.info.Level.ToString().PadRight(4)+ this.info.Name;
        if (this.classIcon != null) this.classIcon.overrideSprite = SpriteManager.Instance.classIcons[(int)this.info.Class];
        if (this.leaderIcon != null) this.leaderIcon.gameObject.SetActive(isLeader);
    }

}
