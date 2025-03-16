using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Models;
using System;
using Managers;

public class UIMain : MonoSingleton<UIMain> {

    public Text AvatarName;
    public Text AvatarLevel;

    public UITeam TeamWindow;
    // Use this for initialization
    protected override void OnStart () {
        this.UpdateAvatar();
        TeamManager.Instance.ShowTeamUI(true);
    }

    private void UpdateAvatar()
    {
        this.AvatarName.text = string.Format("{0}[{1}]", User.Instance.CurrentCharacter.Name, User.Instance.CurrentCharacter.Id);
        this.AvatarLevel.text = User.Instance.CurrentCharacter.Level.ToString();
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
    //public void OnClickSkill()
    //{
    //    UIManager.Instance.Show<UIFriends>();
    //}

    public void ShowTeamUI(bool show)
    {
        TeamWindow.ShowTeam(show);
    }
}
