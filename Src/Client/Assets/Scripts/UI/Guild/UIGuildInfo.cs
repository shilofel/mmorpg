using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Managers;
using Models;
using System;
using Common;
using Common.Data;
using SkillBridge.Message;

public class UIGuildInfo : MonoBehaviour
{
    public Text guildName;
    public Text guildID;
    public Text leader;
    public Text notice;
    public Text memebrNumber;

    public NGuildInfo info;

    public NGuildInfo Info
    {
        get { return this.info; }
        set { this.info = value;this.UpdateUI(); }
    }


    void UpdateUI()
    {
        if (this.Info == null)
        {
            this.guildName.text = "无";
            this.guildID.text = "ID:0";
            this.leader.text = "会长:无";
            this.notice.text = "";
            this.memebrNumber.text = string.Format("成员数量(0/{0})", 40);
        }
        else
        {
            this.guildName.text = this.info.GuildName;
            this.guildID.text = "ID:" + this.info.Id;
            this.leader.text = "会长:" + this.info.leaderName;
            this.notice.text = this.info.Notice;
            this.memebrNumber.text = string.Format("成员数量({0}/{1})",this.info.memberCount,40);
        }
    }


}
