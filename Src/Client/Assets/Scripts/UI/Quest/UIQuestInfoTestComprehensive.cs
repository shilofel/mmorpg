using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;
using Models;
using Common.Data;
using UnityEngine.UI;

public class UIQuestInfoTestComprehensive : MonoBehaviour
{
    public UIQuestInfo questInfo;
    public Text debugText;

    private int testCaseIndex = 0;
    private Quest[] testQuests;

    void Start()
    {
        // 初始化所有测试用例
        InitializeTestCases();

        // 运行第一个测试用例
        RunTestCase(testCaseIndex);
    }

    private void InitializeTestCases()
    {
        testQuests = new Quest[5];

        // 测试用例1：三个道具奖励
        testQuests[0] = CreateQuestWithRewards("三道具奖励测试", 1, 5, 2, 3, 3, 1);

        // 测试用例2：只有第一个道具奖励
        testQuests[1] = CreateQuestWithRewards("单道具奖励测试", 1, 10, 0, 0, 0, 0);

        // 测试用例3：只有第二个和第三个道具奖励
        testQuests[2] = CreateQuestWithRewards("双道具奖励测试(2-3)", 0, 0, 2, 5, 3, 2);

        // 测试用例4：无效道具ID（不存在的道具）
        testQuests[3] = CreateQuestWithRewards("无效道具测试", 99999, 1, 88888, 1, 77777, 1);

        // 测试用例5：混合有效和无效道具
        testQuests[4] = CreateQuestWithRewards("混合测试", 1, 2, 99999, 1, 3, 3);
    }

    private Quest CreateQuestWithRewards(string questName, int item1, int count1, int item2, int count2, int item3, int count3)
    {
        QuestDefine define = new QuestDefine();
        define.ID = Random.Range(1000, 9999);
        define.Name = questName;
        define.Type = QuestType.Main;
        define.Overview = "测试任务：" + questName;
        define.Dialog = "测试对话";
        define.DialogFinish = "测试完成";
        define.AcceptNPC = 1;
        define.SubmitNPC = 1;

        define.RewardGold = 1000;
        define.RewardExp = 500;

        define.RewardItem1 = item1;
        define.RewardItem1Count = count1;
        define.RewardItem2 = item2;
        define.RewardItem2Count = count2;
        define.RewardItem3 = item3;
        define.RewardItem3Count = count3;

        Quest quest = new Quest();
        quest.Define = define;
        quest.Info = new SkillBridge.Message.NQuestInfo();
        quest.Info.Status = SkillBridge.Message.QuestStatus.InProgress;
        quest.Info.QuestId = define.ID;

        return quest;
    }

    private void RunTestCase(int index)
    {
        if (index >= 0 && index < testQuests.Length)
        {
            testCaseIndex = index;
            Quest quest = testQuests[index];

            if (questInfo != null)
            {
                questInfo.SetQuestInfo(quest);
                Debug.LogFormat("运行测试用例 {0}: {1}", index + 1, quest.Define.Name);

                // 输出测试信息
                if (debugText != null)
                {
                    debugText.text = string.Format("测试用例 {0}: {1}\n道具1: ID={2}, 数量={3}\n道具2: ID={4}, 数量={5}\n道具3: ID={6}, 数量={7}",
                        index + 1,
                        quest.Define.Name,
                        quest.Define.RewardItem1, quest.Define.RewardItem1Count,
                        quest.Define.RewardItem2, quest.Define.RewardItem2Count,
                        quest.Define.RewardItem3, quest.Define.RewardItem3Count);
                }
            }
            else
            {
                Debug.LogError("UIQuestInfo 引用未设置！");
            }
        }
    }

    void Update()
    {
        // 使用数字键1-5切换测试用例
        if (Input.GetKeyDown(KeyCode.Alpha1)) RunTestCase(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) RunTestCase(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) RunTestCase(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) RunTestCase(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) RunTestCase(4);

        // 使用左右箭头键切换测试用例
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            testCaseIndex = (testCaseIndex - 1 + testQuests.Length) % testQuests.Length;
            RunTestCase(testCaseIndex);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            testCaseIndex = (testCaseIndex + 1) % testQuests.Length;
            RunTestCase(testCaseIndex);
        }
    }

    void OnGUI()
    {
        // 显示测试控制按钮
        GUILayout.BeginArea(new Rect(10, 10, 200, 200));
        GUILayout.Label("任务奖励测试控制");
        GUILayout.Label("当前测试: " + testCaseIndex);

        if (GUILayout.Button("测试1: 三道具")) RunTestCase(0);
        if (GUILayout.Button("测试2: 单道具")) RunTestCase(1);
        if (GUILayout.Button("测试3: 双道具")) RunTestCase(2);
        if (GUILayout.Button("测试4: 无效道具")) RunTestCase(3);
        if (GUILayout.Button("测试5: 混合")) RunTestCase(4);

        GUILayout.Label("\n快捷键:");
        GUILayout.Label("1-5: 选择测试用例");
        GUILayout.Label("左右箭头: 切换");
        GUILayout.EndArea();
    }
}
