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
using Battle;

public class UISkillItem : ListView.ListViewItem
{
    public Text title;
    public Text level;
    public Image icon;

    public Image background;
    public Sprite normalBg;
    public Sprite selectedBg;

    public override void onSelected(bool selected)
    {
        this.background.overrideSprite = selected ? selectedBg : normalBg;
    }
    public Skill item;

    void Start()
    {

    }

    bool isEquiped = false;

    public void SetItem(Skill item, UISkill owner, bool equiped)
    {
        this.item = item;
        if (this.title != null) this.title.text = this.item.Define.Name;
        if (this.level != null) this.level.text = this.item.Info.Level.ToString();
        if (this.icon != null) this.icon.overrideSprite = Resloader.Load<Sprite>(this.item.Define.Icon);
    }
}
