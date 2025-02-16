using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SkillBridge.Message;
using Managers;
using Models;
using System;
using Services;


public class UITeam : MonoBehaviour
{
    public Text teamTitle;
    public UITeamItem[] Members;
    public ListView list;

    private void Start()
    {
        if(User.Instance.TeamInfo == null)
        {
            this.gameObject.SetActive(false);
            return;
        }
        foreach(var item in Members)
        {
            this.list.AddItem(item);
        }
    }

    private void OnEnable()
    {
        UpdateTeamUI();
    }

    public void ShowTeam(bool show)
    {
        this.gameObject.SetActive(show);
        if(show)
        {
            UpdateTeamUI();
        }

    }

    public void UpdateTeamUI()
    {
        if (User.Instance.TeamInfo == null) return;
        //显示队伍人数
        this.teamTitle.text = string.Format("我的队伍({0}/5)", User.Instance.TeamInfo.Members.Count);
        for(int i =0;i<5;i++)
        {
            if(i<User.Instance.TeamInfo.Members.Count)
            {
                this.Members[i].SetMemberInfo(i, User.Instance.TeamInfo.Members[i], 
                    User.Instance.TeamInfo.Members[i].Id == User.Instance.TeamInfo.Leader);
                this.Members[i].gameObject.SetActive(true);
            }
            else
                this.Members[i].gameObject.SetActive(false);
        }
    }

    public void OnClickFriendRemove()
    {
        MessageBox.Show("确实要离开队伍吗?", "退出队伍", MessageBoxType.Confirm, "确认", "取消").OnYes = () =>
        {
            TeamService.Instance.SendTeamLeaveRequest();
        };
    }
}