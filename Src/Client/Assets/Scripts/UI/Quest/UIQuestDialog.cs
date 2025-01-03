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

class UIQuestDialog : UIWindow
{
    public UIQuestInfo questInfo;

    public Quest quest;

    public GameObject openButton;
    public GameObject submitButton;

    void Start()
    {

    }

    public void SetQuest(Quest quest)
    {
        this.quest = quest;
        this.UpdateQuest();
        if (this.quest.Info == null)
        {
            openButton.SetActive(true);
            submitButton.SetActive(false);
        }
        else
        {
            if(this.quest.Info.Status == SkillBridge.Message.QuestStatus.Completed)
            {
                openButton.SetActive(true);
                submitButton.SetActive(false);
            }
            else
            {
                openButton.SetActive(false);
                submitButton.SetActive(false);
            }
        }
    }

    private void UpdateQuest()
    {
        if(this.quest != null)
        {
            if(this.questInfo !=null)
            {
                this.questInfo.SetQuestInfo(quest);
            }
        }
    }
}
