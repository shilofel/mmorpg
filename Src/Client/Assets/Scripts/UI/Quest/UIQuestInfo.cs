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

    public GameObject rewardPanel;
    public UIIconItem[] rewardItems;
    public Text rewardMoney;
    public Text rewardExp;

    public Button navButton;
    private int npc = 0;
    private Quest quest;
    private void Start()
    {

    }

    void Update()
    {

    }

    public void SetQuestInfo(Quest quest)
    {
        this.quest = quest;
        this.title.text = string.Format("[{0}]{1}", quest.Define.Type, quest.Define.Name);
        if (overview != null)
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

        // 检查是否有道具奖励
        bool hasRewardItems = HasRewardItems(quest.Define);
            
        // 如果没有道具奖励，隐藏整个奖励面板
        if (rewardPanel != null)
        {
            rewardPanel.SetActive(hasRewardItems);
        }
            
        // 如果有道具奖励，显示对应的道具
        if (hasRewardItems)
        {
            ShowRewardItems(quest.Define.RewardItem1, quest.Define.RewardItem1Count, 0);
            ShowRewardItems(quest.Define.RewardItem2, quest.Define.RewardItem2Count, 1);
            ShowRewardItems(quest.Define.RewardItem3, quest.Define.RewardItem3Count, 2);
        }

        if(quest.Info==null)
        {
            this.npc = quest.Define.AcceptNPC;
        }
        else if (quest.Info.Status == SkillBridge.Message.QuestStatus.Completed)
        {
            this.npc = quest.Define.SubmitNPC;
        }

        //this.navButton.gameObject.SetActive(this.npc > 0);
        foreach (var fitter in this.GetComponentsInChildren<ContentSizeFitter>())
        {
            fitter.SetLayoutVertical();
        }
    }

    public void OnClickAbondon()
    {
        // 检查是否有任务可以放弃
        if (quest == null || quest.Info == null)
        {
            MessageBox.Show("该任务无法放弃", "提示", MessageBoxType.Information);
            return;
        }

        // 检查任务状态，只有进行中的任务才能放弃
        if (quest.Info.Status != SkillBridge.Message.QuestStatus.InProgress)
        {
            MessageBox.Show("只有进行中的任务才能放弃", "提示", MessageBoxType.Information);
            return;
        }

        // 显示确认对话框
        MessageBox.Show($"确定要放弃任务 [{quest.Define.Name}] 吗？放弃后可以重新接取", "确认放弃", MessageBoxType.Confirm,"确认","取消").OnYes= ()=>
            {
                    // 发送放弃任务请求
                    Services.QuestService.Instance.SendQuestAbandon(quest);
                    
                    // 关闭任务详情界面
                    UIManager.Instance.Close<UIQuestSystem>();
            };
    }

    public void OnClickNav()
    {
        Vector3 pos = NPCManager.Instance.GetNpcPosition(this.npc);
        User.Instance.CurrentCharacterObject.StartNav(pos);
        UIManager.Instance.Close<UIQuestSystem>();
    }

    private bool HasRewardItems(QuestDefine define)
    {
        // 检查是否有有效的道具奖励
        bool hasItem1 = define.RewardItem1 > 0 && define.RewardItem1Count > 0 && ItemManager.Instance.Items.ContainsKey(define.RewardItem1);
        bool hasItem2 = define.RewardItem2 > 0 && define.RewardItem2Count > 0 && ItemManager.Instance.Items.ContainsKey(define.RewardItem2);
        bool hasItem3 = define.RewardItem3 > 0 && define.RewardItem3Count > 0 && ItemManager.Instance.Items.ContainsKey(define.RewardItem3);
        
        return hasItem1 || hasItem2 || hasItem3;
    }

    private void ShowRewardItems(int itemId, int count, int index)
    {
        if (rewardItems != null && index < rewardItems.Length && rewardItems[index] != null)
        {
            if (itemId > 0 && count > 0 && ItemManager.Instance.Items.ContainsKey(itemId))
            {
                var item = ItemManager.Instance.Items[itemId];
                var def = item.Define;
                rewardItems[index].SetMainIcon(def.Icon, count.ToString());
                rewardItems[index].gameObject.SetActive(true);
            }
            else
            {
                rewardItems[index].gameObject.SetActive(false);
            }
        }
    }
}
