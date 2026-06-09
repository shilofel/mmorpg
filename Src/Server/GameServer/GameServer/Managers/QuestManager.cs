using Common.Data;
using GameServer.Entities;
using GameServer.Services;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GameServer.Managers
{
    class QuestManager
    {
        Character Owner;

        public QuestManager(Character owner)
        {
            this.Owner = owner;
        }

        public void GetQuestInfos(List<NQuestInfo> list)
        {
            foreach(var quest in this.Owner.Data.Quests)
            {
                list.Add(GetQuestInfo(quest));
            }
        }

        public NQuestInfo GetQuestInfo(TCharacterQuest quest)
        {
            return new NQuestInfo()
            {
                QuestId = quest.QuestID,
                QuestGuid = quest.Id,
                Status = (QuestStatus)quest.Status,
                Targets = new int[3]
                {
                    quest.Target1,
                    quest.Target2,
                    quest.Target3,
                }
            };
        }

        public Result AcceptQuest(NetConnection<NetSession> sender, int questId)
        {
            Character character = sender.Session.Character;

            QuestDefine quest;
            if(DataManager.Instance.Quests.TryGetValue(questId,out quest))
            {
                // 检查是否已有该任务记录（包括已放弃的任务）
                var existingQuest = character.Data.Quests.Where(q => q.QuestID == questId).FirstOrDefault();
                if (existingQuest != null)
                {
                    // 如果任务已完成或已完成交付，不能重新接取
                    if (existingQuest.Status == (int)QuestStatus.Finished)
                    {
                        sender.Session.Response.questAccept.Errormsg = "任务已完成，无法重新接取";
                        return Result.Failed;
                    }
                    
                    // 如果任务进行中或已完成目标，不能重新接取
                    if (existingQuest.Status == (int)QuestStatus.InProgress)
                    {
                        sender.Session.Response.questAccept.Errormsg = "任务已在进行中";
                        return Result.Failed;
                    }
                    
                    // 如果是已放弃的任务(Failed)，先删除旧记录
                    if (existingQuest.Status == (int)QuestStatus.Failed)
                    {
                        character.Data.Quests.Remove(existingQuest);
                        DBService.Instance.Entities.CharacterQuests.Remove(existingQuest);
                    }
                    // 如果是已完成目标但未提交(Completed)，不能重新接取
                    if (existingQuest.Status == (int)QuestStatus.Completed)
                    {
                        sender.Session.Response.questAccept.Errormsg = "任务已完成目标，请提交任务";
                        return Result.Failed;
                    }
                }

                // 创建新的任务记录
                var dbquest = DBService.Instance.Entities.CharacterQuests.Create();
                dbquest.QuestID = quest.ID;
                if(quest.Target1 == QuestTarget.None)
                {
                    dbquest.Status = (int)QuestStatus.Completed;
                }
                else
                {
                    dbquest.Status = (int)QuestStatus.InProgress;
                }

                sender.Session.Response.questAccept.Quest = this.GetQuestInfo(dbquest);
                character.Data.Quests.Add(dbquest);
                DBService.Instance.Save();
                return Result.Success;
            }
            else
            {
                sender.Session.Response.questAccept.Errormsg = "任务不存在";
                return Result.Failed;
            }
        }

        public Result SubmitQuest(NetConnection<NetSession> sender, int questId)
        {
            Character character = sender.Session.Character;

            QuestDefine quest;
            if (DataManager.Instance.Quests.TryGetValue(questId, out quest))
            {
                //数据库不存在
                var dbquest = character.Data.Quests.Where(q => q.QuestID == questId).FirstOrDefault();
                if(dbquest != null)
                {
                    if(dbquest.Status!=(int)QuestStatus.Completed)
                    {
                        sender.Session.Response.questSubmit.Errormsg = "任务未完成";
                        return Result.Failed;
                    }

                    dbquest.Status = (int)QuestStatus.Finished;
                    sender.Session.Response.questSubmit.Quest = this.GetQuestInfo(dbquest);
                    DBService.Instance.Save();

                    if(quest.RewardGold > 0)
                    {
                        character.Gold += quest.RewardGold;
                    }
                    if(quest.RewardExp > 0)
                    {
                        //处理升级
                    }

                    if(quest.RewardItem1 > 0)
                    {
                        character.ItemManager.AddItem(quest.RewardItem1, quest.RewardItem1Count);
                    }
                    if (quest.RewardItem2 > 0)
                    {
                        character.ItemManager.AddItem(quest.RewardItem2, quest.RewardItem2Count);
                    }
                    if (quest.RewardItem3 > 0)
                    {
                        character.ItemManager.AddItem(quest.RewardItem3, quest.RewardItem3Count);
                    }
                    DBService.Instance.Save();
                    return Result.Success;
                }
                sender.Session.Response.questAccept.Errormsg = "任务不存在[2]";
                return Result.Failed;
            }
            else
            {
                //数据表
                sender.Session.Response.questAccept.Errormsg = "任务不存在[1]";
                return Result.Failed;
            }
        }

        public Result AbandonQuest(NetConnection<NetSession> sender, int questId)
        {
            Character character = sender.Session.Character;

            // 从数据库中查找任务
            var dbquest = character.Data.Quests.Where(q => q.QuestID == questId).FirstOrDefault();
            if (dbquest != null)
            {
                // 只有进行中的任务才能放弃
                if (dbquest.Status == (int)QuestStatus.InProgress)
                {
                    dbquest.Status = (int)QuestStatus.Failed;
                    DBService.Instance.Save();
                    
                    // 返回更新后的任务信息
                    sender.Session.Response.questAbandon.Quest = this.GetQuestInfo(dbquest);
                    return Result.Success;
                }
                else if (dbquest.Status == (int)QuestStatus.Finished)
                {
                    sender.Session.Response.questAbandon.Errormsg = "任务已完成，无法放弃";
                    return Result.Failed;
                }
                else if (dbquest.Status == (int)QuestStatus.Failed)
                {
                    sender.Session.Response.questAbandon.Errormsg = "任务已放弃";
                    return Result.Failed;
                }
                else
                {
                    sender.Session.Response.questAbandon.Errormsg = "任务已完成交付，无法放弃";
                    return Result.Failed;
                }
            }
            else
            {
                sender.Session.Response.questAbandon.Errormsg = "任务不存在";
                return Result.Failed;
            }
        }
    }
}