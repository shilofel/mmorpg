using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SkillBridge.Message;

using Common;
using Network;
using GameServer.Entities;
using GameServer.Managers;
using Common.Data;

namespace GameServer.Models
{
    class Guild
    {
        public int Id { get { return this.Data.Id; } }

        private Character Leader;

        public string Name { get { return this.Data.Name; } }

        public double timestamp;

        public TGuild Data;

        public Guild(TGuild guild)
        {
            this.Data = guild;
        }

        internal bool JoinApply(NGuildApplyInfo apply)
        {
            var oldApply = this.Data.Applies.FirstOrDefault(v => v.CharacterId == apply.characterId);
            if (oldApply != null)
            {
                return false;
            }

            var dbApply = GuildManager.Instance.CreateApply(apply);
            this.Data.Applies.Add(dbApply);
            this.timestamp = TimeUtil.timestamp;
            return true;
        }

        internal bool JoinAppove(NGuildApplyInfo apply)
        {
            var oldApply = this.Data.Applies.FirstOrDefault(v => v.CharacterId == apply.characterId && v.Result == 0);
            if (oldApply == null)
            {
                return false;
            }
            oldApply.Result = (int)apply.Result;
            if (apply.Result == ApplyResult.Accept)
            {
                this.AddMember(apply.characterId, apply.Name, apply.Class, apply.Level, GuildTitle.None);
            }

            this.timestamp = TimeUtil.timestamp;
            return true;
        }

        public void AddMember(int characterId, string name, int @class, int level, GuildTitle title)
        {
            GuildManager.Instance.AddMember(this, characterId, name, @class, level, title);
            timestamp = TimeUtil.timestamp;
        }

        public void Leave(Character cha)
        {
            if (cha == null) return;

            GuildManager.Instance.RemoveMember(this, cha.Id);

            if (this.Data.LeaderID == cha.Id && this.Data.Members.Count > 0)
            {
                var newLeader = this.Data.Members.FirstOrDefault();
                if (newLeader != null)
                {
                    GuildManager.Instance.TransferLeader(this, newLeader.CharacterId);
                }
            }

            timestamp = TimeUtil.timestamp;
        }

        public void PostProcess(Character from, NetMessageResponse message)
        {
            if (message.Guild == null)
            {
                message.Guild = new GuildResponse();
                message.Guild.Result = Result.Success;
                message.Guild.Guild = this.GuildInfo(from);
            }
        }

        public NGuildInfo GuildInfo(Character from)
        {
            NGuildInfo info = new NGuildInfo()
            {
                Id = this.Id,
                GuildName = this.Name,
                Notice = this.Data.Notice,
                leaderId = this.Data.LeaderID,
                leaderName = this.Data.LeaderName,
                createTime = (long)TimeUtil.GetTimestamp(this.Data.CreateTime),
                memberCount = this.Data.Members.Count
            };

            if (from != null)
            {
                info.Members.AddRange(GetMemberInfos());
                if (from.Id == this.Data.LeaderID)
                    info.Applies.AddRange(GetApplyInfos());
            }

            return info;
        }

        private List<NGuildMemberInfo> GetMemberInfos()
        {
            List<NGuildMemberInfo> members = new List<NGuildMemberInfo>();
            foreach (var member in this.Data.Members)
            {
                var memberInfo = new NGuildMemberInfo()
                {
                    Id = member.Id,
                    characterId = member.CharacterId,
                    Title = (GuildTitle)member.Title,
                    joinTime = (long)TimeUtil.GetTimestamp(member.JoinTime),
                    lastTime = (long)TimeUtil.GetTimestamp(member.LastTime)
                };
                var character = CharacterManager.Instance.GetCharacter(member.CharacterId);
                if (character != null)
                {
                    memberInfo.Info = character.GetBasicInfo();
                    memberInfo.Status = 1;
                    member.Name = character.Data.Name;
                    member.Level = character.Data.Level;
                    member.LastTime = DateTime.Now;
                    if (member.CharacterId == this.Data.LeaderID)
                        this.Leader = character;
                }
                else
                {
                    memberInfo.Info = GetMemberInfo(member);
                    memberInfo.Status = 0;
                    if (member.CharacterId == this.Data.LeaderID)
                        this.Leader = null;
                }
                members.Add(memberInfo);
            }
            return members;
        }

        NCharacterInfo GetMemberInfo(TGuildMember member)
        {
            return new NCharacterInfo()
            {
                Id = member.CharacterId,
                Name = member.Name,
                Class = (CharacterClass)member.Class,
                Level = member.Level
            };
        }

        private List<NGuildApplyInfo> GetApplyInfos()
        {
            List<NGuildApplyInfo> applies = new List<NGuildApplyInfo>();
            foreach (var apply in this.Data.Applies)
            {
                if (apply.Result != (int)ApplyResult.None) continue;
                var applyInfo = new NGuildApplyInfo()
                {
                    characterId = apply.CharacterId,
                    GuildId = apply.GuildId,
                    Name = apply.Name,
                    Class = apply.Class,
                    Level = apply.Level
                };
                applies.Add(applyInfo);
            }
            return applies;
        }

        TGuildMember GetDBMember(int characterId)
        {
            foreach (var member in this.Data.Members)
            {
                if (member.CharacterId == characterId)
                    return member;
            }
            return null;
        }

        internal void ExecuteAdmin(GuildAdminCommand command, int targetId, int sourceId)
        {
            var target = GetDBMember(targetId);
            var source = GetDBMember(sourceId);
            switch (command)
            {
                case GuildAdminCommand.Promote:
                    if (target != null)
                        target.Title = (int)GuildTitle.VicePresident;
                    break;
                case GuildAdminCommand.Depost:
                    if (target != null)
                        target.Title = (int)GuildTitle.None;
                    break;
                case GuildAdminCommand.Transfer:
                    if (target != null)
                    {
                        GuildManager.Instance.TransferLeader(this, target.CharacterId);
                    }
                    break;
                case GuildAdminCommand.Kickout:
                    if (target != null)
                    {
                        GuildManager.Instance.RemoveMember(this, target.CharacterId);
                    }
                    break;
            }
            timestamp = TimeUtil.timestamp;
        }
    }
}
