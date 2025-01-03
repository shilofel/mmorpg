using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Models;
using System;

public class UIMain : MonoSingleton<UIMain> {

    public Text AvatarName;
    public Text AvatarLevel;
    // Use this for initialization
    protected override void OnStart () {
        this.UpdateAvatar();
	}

    private void UpdateAvatar()
    {
        this.AvatarName.text = string.Format("{0}[{1}]", User.Instance.CurrentCharacter.Name, User.Instance.CurrentCharacter.Id);
        this.AvatarLevel.text = User.Instance.CurrentCharacter.Level.ToString();
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void BakToCharacterSelect()
    {
        SceneManager.Instance.LoadScene("CharSelect");
        Services.UserService.Instance.SendGameLeave();
    }

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
}
