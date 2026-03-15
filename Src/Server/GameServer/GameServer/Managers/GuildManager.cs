using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using GameServer.Entities;
using GameServer.Models;
using GameServer.Services;
using Network;
using SkillBridge.Message;
using Common.Data;

namespace GameServer.Managers
{
    class GuildManager : Singleton<GuildManager>
    {
        public HashSet<string> GuildNames = new HashSet<string>();
        public Dictionary<int, Guild> Guilds = new Dictionary<int, Guild>();

        public void Init()
        {
            this.Guilds.Clear();
            foreach (var guild in DBService.Instance.Entities.Guilds)
            {
                this.AddGuild(new Guild(guild));
            }
        }

        public bool CheckNameExisted(string name)
        {
            return GuildNames.Contains(name);
        }

        public void AddGuild(Guild guild)
        {
            this.Guilds.Add(guild.Id, guild);
            this.GuildNames.Add(guild.Name);
            guild.timestamp = TimeUtil.timestamp;
        }

        public bool CreateGuild(string name, string notice, Character leader)
        {
            DateTime now = DateTime.Now;
            TGuild dbGuild = DBService.Instance.Entities.Guilds.Create();
            dbGuild.Name = name;
            dbGuild.Notice = notice;
            dbGuild.LeaderID = leader.Id;
            dbGuild.LeaderName = leader.Name;
            dbGuild.CreateTime = now;
            DBService.Instance.Entities.Guilds.Add(dbGuild);

            Guild guild = new Guild(dbGuild);
            guild.AddMember(leader.Id, leader.Name, leader.Data.Class, leader.Data.Level, GuildTitle.President);
            leader.Guild = guild;
            leader.Data.GuildId = dbGuild.Id;
            this.AddGuild(guild);

            DBService.Instance.Save();
            return true;
        }

        internal Guild GetGuild(int guildId)
        {
            if (guildId == 0)
                return null;
            Guild guild = null;
            this.Guilds.TryGetValue(guildId, out guild);
            return guild;
        }

        internal List<NGuildInfo> GetGuildInfos()
        {
            List<NGuildInfo> result = new List<NGuildInfo>();
            foreach (var kv in this.Guilds)
            {
                result.Add(kv.Value.GuildInfo(null));
            }
            return result;
        }

        internal TGuildApply CreateApply(NGuildApplyInfo apply)
        {
            var dbApply = DBService.Instance.Entities.GuildApplies.Create();
            dbApply.GuildId = apply.GuildId;
            dbApply.CharacterId = apply.characterId;
            dbApply.Class = apply.Class;
            dbApply.Level = apply.Level;
            dbApply.Name = apply.Name;
            dbApply.ApplyTime = DateTime.Now;
            DBService.Instance.Entities.GuildApplies.Add(dbApply);
            return dbApply;
        }

        internal void AddMember(Guild guild, int characterId, string name, int @class, int level, GuildTitle title)
        {
            DateTime now = DateTime.Now;
            TGuildMember dbMember = new TGuildMember()
            {
                CharacterId = characterId,
                Name = name,
                Class = @class,
                Level = level,
                Title = (int)title,
                JoinTime = now,
                LastTime = now
            };
            guild.Data.Members.Add(dbMember);

            var character = CharacterManager.Instance.GetCharacter(characterId);
            if (character != null)
            {
                character.Data.GuildId = guild.Id;
                character.Guild = guild;
            }
            else
            {
                var dbChar = DBService.Instance.Entities.Characters.FirstOrDefault(c => c.ID == characterId);
                if (dbChar != null)
                    dbChar.GuildId = guild.Id;
            }
        }

        internal void RemoveMember(Guild guild, int characterId)
        {
            var member = guild.Data.Members.FirstOrDefault(m => m.CharacterId == characterId);
            if (member == null) return;

            guild.Data.Members.Remove(member);
            DBService.Instance.Entities.GuildMembers.Remove(member);

            var character = CharacterManager.Instance.GetCharacter(characterId);
            if (character != null)
            {
                character.Data.GuildId = 0;
                character.Guild = null;
            }
            else
            {
                var dbChar = DBService.Instance.Entities.Characters.FirstOrDefault(c => c.ID == characterId);
                if (dbChar != null)
                    dbChar.GuildId = 0;
            }
        }

        internal void TransferLeader(Guild guild, int newLeaderId)
        {
            var newLeader = guild.Data.Members.FirstOrDefault(m => m.CharacterId == newLeaderId);
            if (newLeader == null) return;

            var oldLeader = guild.Data.Members.FirstOrDefault(m => m.CharacterId == guild.Data.LeaderID);
            if (oldLeader != null)
                oldLeader.Title = (int)GuildTitle.None;

            newLeader.Title = (int)GuildTitle.President;
            guild.Data.LeaderID = newLeader.CharacterId;
            guild.Data.LeaderName = newLeader.Name;
        }
    }
}
