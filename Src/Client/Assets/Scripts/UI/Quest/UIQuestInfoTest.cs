using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;
using Models;
using Common.Data;

public class UIQuestInfoTest : MonoBehaviour
{
    public UIQuestInfo questInfo;

    void Start()
    {
        // 创建测试任务数据
        Quest testQuest = CreateTestQuestWithThreeRewards();
        
        // 设置任务信息
        if (questInfo != null)
        {
            questInfo.SetQuestInfo(testQuest);
            Debug.Log("测试任务已设置，包含三个道具奖励");
        }
        else
        {
            Debug.LogError("UIQuestInfo 引用未设置！");
        }
    }

    private Quest CreateTestQuestWithThreeRewards()
    {
        // 创建任务定义
        QuestDefine define = new QuestDefine();
        define.ID = 9999;
        define.Name = "测试任务 - 三道具奖励";
        define.Type = QuestType.Main;
        define.Overview = "这是一个测试任务，用于验证三个道具奖励的显示逻辑";
        define.Dialog = "欢迎接受测试任务！";
        define.DialogFinish = "任务完成！";
        define.AcceptNPC = 1;
        define.SubmitNPC = 1;

        // 设置奖励
        define.RewardGold = 1000;
        define.RewardExp = 500;

        // 设置三个道具奖励（使用现有道具ID）
        // 假设道具ID 1, 2, 3 存在于ItemDefine.txt中
        define.RewardItem1 = 1;    // 红瓶
        define.RewardItem1Count = 5;
        define.RewardItem2 = 2;    // 蓝瓶
        define.RewardItem2Count = 3;
        define.RewardItem3 = 3;    // 宝箱
        define.RewardItem3Count = 1;

        // 创建任务对象
        Quest quest = new Quest();
        quest.Define = define;
        quest.Info = new SkillBridge.Message.NQuestInfo();
        quest.Info.Status = SkillBridge.Message.QuestStatus.InProgress;
        quest.Info.QuestId = define.ID;

        return quest;
    }

    void Update()
    {
        // 按空格键重新测试
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Quest testQuest = CreateTestQuestWithThreeRewards();
            if (questInfo != null)
            {
                questInfo.SetQuestInfo(testQuest);
                Debug.Log("重新设置测试任务");
            }
        }
    }
}
