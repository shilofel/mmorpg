using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Managers;
using Models;
using System;
using Common;
using Common.Data;

public class UIQuestInfo : MonoBehaviour
{
    public Text title;

    public Text[] targets;
    public Text description;

    public Text overview;

    public UIIconItem rewardItems;
    public Text rewardMoney;
    public Text rewardExp;

    public Button navButton;
    private int npc = 0;
    private void Start()
    {

    }

    void Update()
    {

    }

    public void SetQuestInfo(Quest quest)
    {
        this.title.text = string.Format("[{0}]{1}", quest.Define.Type, quest.Define.Name);
        if (overview == null)
        {
            this.overview.text = quest.Define.Overview;
        }

        if (description != null)
        {
            if (quest.Info == null)
            {
                this.description.text = quest.Define.Dialog;
            }
            else
            {
                if (quest.Info.Status == SkillBridge.Message.QuestStatus.Finished)
                    this.description.text = quest.Define.DialogFinish;
            }
        }

        this.rewardExp.text = quest.Define.RewardExp.ToString();
        this.rewardMoney.text = quest.Define.RewardGold.ToString();

        if(quest.Info==null)
        {
            this.npc = quest.Define.AcceptNPC;
        }
        else if (quest.Info.Status == SkillBridge.Message.QuestStatus.Completed)
        {
            this.npc = quest.Define.SubmitNPC;
        }

        this.navButton.gameObject.SetActive(this.npc > 0);
        foreach (var fitter in this.GetComponentsInChildren<ContentSizeFitter>())
        {
            fitter.SetLayoutVertical();
        }
    }

    public void OnClickAbondon()
    {

    }

    public void OnClickNav()
    {
        Vector3 pos = NPCManager.Instance.GetNpcPosition(this.npc);
        User.Instance.CurrentCharacterObject.StartNav(pos);
        UIManager.Instance.Close<UIQuestSystem>();
    }
}
