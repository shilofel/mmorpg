using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SkillBridge.Message;
using Managers;
using Models;
using System;
using Services;
using Assets.Scripts.Services;

public class UIFriends : UIWindow
{
    public GameObject itemPrefab;
    public ListView listMain;
    public Transform itemRoot;

    public UIFriendItem selectedItem;

    private void Start()
    {
        //FriendService.Instance.OnFriendUpdate = RefreshUI;
        this.listMain.onItemSelected += this.OnFriendSelected;
        RefreshUI();
    }

    private void Update()
    {
        
    }

    public void OnFriendSelected(ListView.ListViewItem item)
    {
        this.selectedItem = item as UIFriendItem;
    }

    public void OnClickFriendAdd()
    {
        InputBox.Show("输入要添加的好友名称或ID", "添加好友").OnSubmit += OnFriendAddSubmit;
    }

    private bool OnFriendAddSubmit(string input,out string tips)
    {
        tips = "";
        int friendId = 0;
        string friendName = "";
        if (!int.TryParse(input, out friendId))
            friendName = input;
        if(friendId == User.Instance.CurrentCharacterInfo.Id||friendName == User.Instance.CurrentCharacterInfo.Name)
        {
            tips = "不能添加自己";
            return false;
        }

        FriendService.Instance.SendFriendAddRequest(friendId, friendName);
        return true;
    }

    public void OnClickFriendChat()
    {
        MessageBox.Show("暂未开发");
    }

    public void OnClickFriendRemove()
    {
        if (selectedItem == null)
        {
            MessageBox.Show("请选择要删除的好友");
            return;
        }
        MessageBox.Show(string.Format("确实要删除好友[{0}]吗?", selectedItem.info.friendInfo.Name), "删除好友", MessageBoxType.Confirm, "删除", "取消").OnYes = () =>
        {
            FriendService.Instance.SendFriendRemoveRequest(this.selectedItem.info.Id, this.selectedItem.info.friendInfo.Id);
        };
    }

    public void OnClickFriendTeamInvite()
    {
        if (selectedItem == null)
        {
            MessageBox.Show("请选择要邀请的好友");
            return;
        }
        if (selectedItem.info.Status == 0)
        {
            MessageBox.Show("请选择在线好友");
            return;
        }
        MessageBox.Show(string.Format("确实要邀请好友[{0}]加入队伍吗?", selectedItem.info.friendInfo.Name), "邀请好友组队", MessageBoxType.Confirm, "邀请", "取消").OnYes = () =>
        {
            TeamService.Instance.SendTeamInviteRequest(this.selectedItem.info.friendInfo.Id, this.selectedItem.info.friendInfo.Name);
        };
    }

    public void OnClickChallenge()
    {
        if (selectedItem == null)
        {
            MessageBox.Show("请选择要挑战的好友");
            return;
        }
        if (selectedItem.info.Status == 0)
        {
            MessageBox.Show("请选择在线好友");
            return;
        }
        MessageBox.Show(string.Format("确实要与好友[{0}]进行竞技PK吗?", selectedItem.info.friendInfo.Name), "竞技场挑战", MessageBoxType.Confirm, "挑战", "取消").OnYes = () =>
        {
            ArenaService.Instance.SendAreanChallengeRequest(this.selectedItem.info.friendInfo.Id, this.selectedItem.info.friendInfo.Name);
        };
    }

    void RefreshUI()
    {
        ClearFriendList();
        InitFriendItems();
    }

    void InitFriendItems()
    {
        foreach (var item in FriendManager.Instance.allFriends)
        {
            GameObject  go = Instantiate(itemPrefab,this.listMain.transform);
            UIFriendItem ui = go.GetComponent<UIFriendItem>();
            ui.SetFriendInfo(item);
            this.listMain.AddItem(ui);
        }
    }

    void ClearFriendList()
    {
        this.listMain.RemoveAll();
    }
}
