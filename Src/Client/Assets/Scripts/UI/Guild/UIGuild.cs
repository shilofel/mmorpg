using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SkillBridge.Message;
using Managers;
using Models;
using System;
using Services;

public class UIGuild : UIWindow
{
    public GameObject itemPrefab;
    public ListView listMain;
    public Transform itemRoot;

    public UIGuildInfo uiInfo;
    public UIGuildMemberItem selectedItem;

    public GameObject panelAdmin;
    public GameObject panelLeader;

    private void Start()
    {
        GuildService.Instance.OnGuildUpdate = UpdateUI;
        this.listMain.onItemSelected += this.OnGuildMemberSelected;
        this.UpdateUI();
    }

    private void OnDestroy()
    {
        GuildService.Instance.OnGuildUpdate -= UpdateUI;
    }

    private void UpdateUI()
    {
        this.uiInfo.Info = GuildManager.Instance.guildInfo;

        ClearList();
        InitItems();

        this.panelAdmin.SetActive(GuildManager.Instance.myMemberInfo.Title > GuildTitle.None);
        this.panelLeader.SetActive(GuildManager.Instance.myMemberInfo.Title > GuildTitle.President);
    }

    public void OnGuildMemberSelected(ListView.ListViewItem item)
    {
        this.selectedItem = item as UIGuildMemberItem;
    }


    void InitItems()
    {
        foreach (var item in GuildManager.Instance.guildInfo.Members)
        {
            GameObject go = Instantiate(itemPrefab, this.listMain.transform);
            UIGuildMemberItem ui = go.GetComponent<UIGuildMemberItem>();
            ui.SetGuildMemberInfo(item);
            this.listMain.AddItem(ui);

        }
    }

    void ClearList()
    {
        this.listMain.RemoveAll();
    }

    public void OnClickAppliesList()
    {
        UIManager.Instance.Show<UIGuildApplyList>();
    }

    public void OnClickLeave()
    {

    }

    public void OnClickChat()
    {

    }

    public void OnClickKickout()
    {
        if(selectedItem == null )
        {
            MessageBox.Show("请选择踢出公会的成员");
            return;
        }
        MessageBox.Show(string.Format("确定要踢出【{0}】吗？", this.selectedItem.info.Info.Name), "踢出公会", MessageBoxType.Confirm, "确定", "取消").OnYes = () =>
              {
                  GuildService.Instance.SendAdminCommand(GuildAdminCommand.Kickout, this.selectedItem.info.Info.Id);
              };
    }

    public void OnClickPromote()
    {
        if (selectedItem == null)
        {
            MessageBox.Show("请选择晋升的成员");
            return;
        }
        if(selectedItem.info.Title != GuildTitle.None)
        {
            MessageBox.Show("该成员已经无法晋升");
            return;
        }
        MessageBox.Show(string.Format("确定要晋升成员【{0}】吗？", this.selectedItem.info.Info.Name), "晋升", MessageBoxType.Confirm, "确定", "取消").OnYes = () =>
        {
            GuildService.Instance.SendAdminCommand(GuildAdminCommand.Promote, this.selectedItem.info.Info.Id);
        };
    }

    public void OnClickDepost()
    {
        if (selectedItem == null)
        {
            MessageBox.Show("请选择罢免的成员");
            return;
        }
        if (selectedItem.info.Title == GuildTitle.None)
        {
            MessageBox.Show("该成员已经无法罢免");
            return;
        }
        if (selectedItem.info.Title == GuildTitle.President)
        {
            MessageBox.Show("会长无法罢免");
            return;
        }
        MessageBox.Show(string.Format("确定要罢免成员【{0}】吗？", this.selectedItem.info.Info.Name), "罢免", MessageBoxType.Confirm, "确定", "取消").OnYes = () =>
        {
            GuildService.Instance.SendAdminCommand(GuildAdminCommand.Depost, this.selectedItem.info.Info.Id);
        };
    }

    public void OnClickTransfer()
    {
        if (selectedItem == null)
        {
            MessageBox.Show("请选择要转让会长的成员");
            return;
        }

        MessageBox.Show(string.Format("确定要将会长转让给成员【{0}】吗？", this.selectedItem.info.Info.Name), "转让", MessageBoxType.Confirm, "确定", "取消").OnYes = () =>
        {
            GuildService.Instance.SendAdminCommand(GuildAdminCommand.Transfer, this.selectedItem.info.Info.Id);
        };
    }

    public void OnClickSetNotice()
    {

    }
}
