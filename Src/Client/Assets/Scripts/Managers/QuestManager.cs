using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Network;
using UnityEngine;
using UnityEngine.Events;

using Models;
using SkillBridge.Message;
using Common.Data;
using Services;

namespace Managers
{

    public enum NpcQuestStatus
    {
        None = 0,
        Complete = 1,
        Avaliable,
        Incomplete,
    }

    class QuestManager : Singleton<QuestManager>
    {
        public List<NQuestInfo> questInfos;
        public Dictionary<int, Quest> allQuests = new Dictionary<int, Quest>();

        public Dictionary<int, Dictionary<NpcQuestStatus, List<Quest>>> npcQuests = new Dictionary<int, Dictionary<NpcQuestStatus, List<Quest>>>();

        public void Init(List<NQuestInfo> quests)
        {
            this.questInfos = quests;
            allQuests.Clear();
            this.npcQuests.Clear();
            InitQuests();
        }

        private void InitQuests()
        {
            foreach (var info in this.questInfos)
            {
                Quest quest = new Quest(info);
                this.AddNpcQuest(quest.Define.AcceptNPC, quest);
                this.AddNpcQuest(quest.Define.SubmitNPC, quest);
                this.allQuests[quest.Info.QuestId] = quest;
            }

            foreach(var kv in DataManager.Instance.Quests)
            {
                if (kv.Value.LimitClass != CharacterClass.None && kv.Value.LimitClass != User.Instance.CurrentCharacter.Class)
                    continue;
                if (kv.Value.LimitLevel > User.Instance.CurrentCharacter.Level)
                    continue;
                if (this.allQuests.ContainsKey(kv.Key))
                    continue;
                if(kv.Value.PreQuest > 0)
                {
                    Quest preQuest;
                    if (this.allQuests.TryGetValue(kv.Value.PreQuest, out preQuest))
                    {
                        if (preQuest.Info == null)
                            continue;//前置未接
                        if (preQuest.Info.Status != QuestStatus.Finished)
                            continue;//前置未完成
                    }
                    else
                        continue;
                }

                Quest quest = new Quest(kv.Value);
                this.AddNpcQuest(quest.Define.AcceptNPC, quest);
                this.AddNpcQuest(quest.Define.SubmitNPC, quest);
                this.allQuests[quest.Define.ID] = quest;
            }
        }

        private void AddNpcQuest(int npcId, Quest quest)
        {
            if (!this.npcQuests.ContainsKey(npcId))
                this.npcQuests[npcId] = new Dictionary<NpcQuestStatus, List<Quest>>();

            List<Quest> availables;
            List<Quest> completes;
            List<Quest> incompletes;

            if(!this.npcQuests[npcId].TryGetValue(NpcQuestStatus.Avaliable,out availables))
            {
                availables = new List<Quest>();
                this.npcQuests[npcId][NpcQuestStatus.Avaliable] = availables;
            }
            if (!this.npcQuests[npcId].TryGetValue(NpcQuestStatus.Complete, out completes))
            {
                completes = new List<Quest>();
                this.npcQuests[npcId][NpcQuestStatus.Complete] = completes;
            }
            if (!this.npcQuests[npcId].TryGetValue(NpcQuestStatus.Incomplete, out incompletes))
            {
                incompletes = new List<Quest>();
                this.npcQuests[npcId][NpcQuestStatus.Incomplete] = incompletes;
            }

            if(quest.Info == null)
            {
                if(npcId == quest.Define.AcceptNPC&&!this.npcQuests[npcId][NpcQuestStatus.Avaliable].Contains(quest))
                {
                    this.npcQuests[npcId][NpcQuestStatus.Avaliable].Add(quest);
                }
            }
            else
            {
                if (npcId == quest.Define.SubmitNPC && quest.Info.Status == QuestStatus.Completed)
                {
                    if (!this.npcQuests[npcId][NpcQuestStatus.Complete].Contains(quest))
                    {
                        this.npcQuests[npcId][NpcQuestStatus.Complete].Add(quest);
                    }
                }
                if (npcId == quest.Define.SubmitNPC && quest.Info.Status == QuestStatus.InProgress)
                {
                    if (!this.npcQuests[npcId][NpcQuestStatus.Incomplete].Contains(quest))
                    {
                        this.npcQuests[npcId][NpcQuestStatus.Incomplete].Add(quest);
                    }
                }
            }
        }

        public NpcQuestStatus GetQuestStatusByNpc(int npcId)
        {
            Dictionary<NpcQuestStatus, List<Quest>> status = new Dictionary<NpcQuestStatus, List<Quest>>();
            if(this.npcQuests.TryGetValue(npcId,out status))
            {
                if (status[NpcQuestStatus.Complete].Count > 0)
                    return NpcQuestStatus.Complete;
                if (status[NpcQuestStatus.Avaliable].Count > 0)
                    return NpcQuestStatus.Avaliable;
                if (status[NpcQuestStatus.Incomplete].Count > 0)
                    return NpcQuestStatus.Incomplete;
            }
            return NpcQuestStatus.None;
        }

        public bool OpenNpcQuest(int npcId)
        {
            Dictionary<NpcQuestStatus, List<Quest>> status = new Dictionary<NpcQuestStatus, List<Quest>>();
            if (this.npcQuests.TryGetValue(npcId, out status))
            {
                if (status[NpcQuestStatus.Complete].Count > 0)
                    return ShowQuestDialog(status[NpcQuestStatus.Complete].First());
                if (status[NpcQuestStatus.Avaliable].Count > 0)
                    return ShowQuestDialog(status[NpcQuestStatus.Avaliable].First());
                if (status[NpcQuestStatus.Incomplete].Count > 0)
                    return ShowQuestDialog(status[NpcQuestStatus.Incomplete].First());
            }
            return false;
        }

        private bool ShowQuestDialog(Quest quest)
        {
            if(quest.Info == null || quest.Info.Status == QuestStatus.Completed)
            {
                UIQuestDialog dlg = UIManager.Instance.Show<UIQuestDialog>();
                dlg.SetQuest(quest);
                dlg.OnClose += OnQuestDialogClose;
                return true;
            }
            //任务已接受 显示未完成
            if (quest.Info != null || quest.Info.Status == QuestStatus.Completed)
            {
                if (!string.IsNullOrEmpty(quest.Define.DialogIncomplete))
                    MessageBox.Show(quest.Define.DialogIncomplete);
            }
            return true;
        }

        private void OnQuestDialogClose(UIWindow sender, UIWindow.WindowResult result)
        {
            UIQuestDialog dlg = (UIQuestDialog)sender;
            if(result == UIWindow.WindowResult.Yes)
            {
                MessageBox.Show(dlg.quest.Define.DialogAccept);

            }
            else if (result == UIWindow.WindowResult.No)
            {
                MessageBox.Show(dlg.quest.Define.DialogDeny);

            }
        }
    }
}