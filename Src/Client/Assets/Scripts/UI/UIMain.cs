using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Models;
using System;
using Managers;
using Entities;

public class UIMain : MonoSingleton<UIMain> {

    public Text AvatarName;
    public Text AvatarLevel;

    public UITeam TeamWindow;

    public UICreatureInfo targetUI;

    public UISkillSlots skillSlots;
    // Use this for initialization
    protected override void OnStart () {
        this.UpdateAvatar();
        //TeamManager.Instance.ShowTeamUI(true);
        //this.targetUI.gameObject.SetActive(true);
        BattleManager.Instance.OnTargetChanged += OnTargetChanged;
        User.Instance.OnCharacterInit += this.skillSlots.UpdateSkills;
        this.skillSlots.UpdateSkills();
    }

    private void UpdateAvatar()
    {
        this.AvatarName.text = string.Format("{0}[{1}]", User.Instance.CurrentCharacterInfo.Name, User.Instance.CurrentCharacterInfo.Id);
        this.AvatarLevel.text = User.Instance.CurrentCharacterInfo.Level.ToString();
    }

    // Update is called once per frame
    void Update () {
		
	}

    //public void BakToCharacterSelect()
    //{
    //    SceneManager.Instance.LoadScene("CharSelect");
    //    Services.UserService.Instance.SendGameLeave();
    //}

    public void OnClickBag()
    {
        UIManager.Instance.Show<UIBag>();
    }

    public void OnClickChar()
    {
        UIManager.Instance.Show<UICharEquip>();
    }

    public void OnClickQuest()
    {
        UIManager.Instance.Show<UIQuestSystem>();
    }

    public void OnClickFriend()
    {
        UIManager.Instance.Show<UIFriends>();
    }

    public void OnClickGuild()
    {
        GuildManager.Instance.ShowGuild();
    }
    public void OnClickRide()
    {
        UIManager.Instance.Show<UIRide>();
    }
    public void OnClickSetting()
    {
        UIManager.Instance.Show<UISetting>();
    }
    public void OnClickSkill()
    {
        UIManager.Instance.Show<UISkill>();
    }

    public void ShowTeamUI(bool show)
    {
        TeamWindow.ShowTeam(show);
    }

    private void OnTargetChanged(Creature  target)
    {
       if(target!=null)
        {
            if (!targetUI.isActiveAndEnabled) targetUI.gameObject.SetActive(true);
            targetUI.Target = target;
        }
       else
        {
            targetUI.gameObject.SetActive(false);
        }
    }
}
