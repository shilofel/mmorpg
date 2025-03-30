using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Managers;
using Models;
using System;
using Common;
using Common.Data;

public class UISkill : UIWindow
{
    public Text descript;
    public GameObject itemPrefab;

    public ListView listMain;
    public UISkillItem selectedItem;

    private bool showAvailableList = false;
    private void Start()
    {
        RefreshUI();
        this.listMain.onItemSelected += this.OnItemSelected;
    }

    void OnSelectTab(int idx)
    {
        showAvailableList = idx == 1;
        RefreshUI();
    }

    private void OnDestroy()
    {
        //QuestManager.Instance.OnQuestChanaged -= RefreshUI;
    }

    private void RefreshUI()
    {
        ClearItems();
        InitItems();
    }

    private void InitItems()
    {
        var Skills = User.Instance.CurrentCharacter.SkillMgr.Skills;
        foreach (var skill in Skills)
        {
            if (skill.Define.Type == Common.Battle.SkillType.Skill)
            {
                GameObject go = Instantiate(itemPrefab,listMain.transform);
                UISkillItem ui = go.GetComponent<UISkillItem>();
                ui.SetItem(skill, this, false);
                this.listMain.AddItem(ui);
            }
        }
    }

    void ClearItems()
    {
        this.listMain.RemoveAll();
    }

    private void OnItemSelected(ListView.ListViewItem item)
    {
        selectedItem= item as UISkillItem;
        this.descript.text = this.selectedItem.item.Define.Description;
    }
}
