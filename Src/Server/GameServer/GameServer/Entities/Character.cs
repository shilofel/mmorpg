using Common.Data;
using GameServer.Core;
using GameServer.Managers;
using SkillBridge.Message;
using System;
using Network;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using GameServer.Models;

namespace GameServer.Entities
{
    class Character : Creature,IPostResponser
    {
       
        public TCharacter Data;
        public ItemManager ItemManager;
        public StatusManager StatusManager;
        public QuestManager QuestManager;
        public FriendManager FriendManager;
        
        public Guild Guild;
        public Team Team;

        public Chat Chat;

        public double TeamUpdateTS;
        public double GuildUpdateTS;

        public Character(CharacterType type,TCharacter cha):
            base(type,cha.TID, cha.Level,new Core.Vector3Int(cha.MapPosX, cha.MapPosY, cha.MapPosZ),new Core.Vector3Int(100,0,0))
        {
            this.Data = cha;
            this.Id = cha.ID;
            
            this.Info.Id = cha.ID;
            this.Info.Class = (CharacterClass)cha.Class;
            this.Info.Exp = cha.Exp;
            this.Info.mapId = cha.MapID;
            this.Info.Gold = cha.Gold;
            this.Info.Ride = 0;
            this.Info.Name = cha.Name;

            this.ItemManager = new ItemManager(this);
            this.ItemManager.GetItemInfos(this.Info.Items);
            this.Info.Bag = new NBagInfo();
            this.Info.Bag.Unlocked = this.Data.Bag.Unlocked;
            this.Info.Bag.Items = this.Data.Bag.Items;
            this.Info.Equips = this.Data.Equips;
            this.QuestManager = new QuestManager(this);
            this.QuestManager.GetQuestInfos(this.Info.Quests);
            this.StatusManager = new StatusManager(this);
            this.FriendManager = new FriendManager(this);
            this.FriendManager.GetFriendInfos(this.Info.Friends);

            this.Guild = GuildManager.Instance.GetGuild(this.Data.GuildId);
            this.Chat = new Chat(this);
            this.Info.attrDynamic = new NAttributeDynamic();
            this.Info.attrDynamic.Hp = cha.HP;
            this.Info.attrDynamic.Mp = cha.MP;
        }

        internal void AddExp(int exp)
        {
            this.Exp = exp;
            this.CheckLevelUp();
        }

        private void CheckLevelUp()
        {
            //经验公式 exp = power(LV,3)*10+LV*40+50
            long needExp = (long)Math.Pow(this.Level, 3) * 10 + this.Level * 40 + 50;
            if(Exp>needExp)
            {
                this.LevelUp();
            }
        }

        private void LevelUp()
        {
            this.Level++;
            Log.InfoFormat("character{0}:{1} Level UP",this.Info.Id,this.Info.Name);
            CheckLevelUp();
        }

        public long Gold
        {
            get { return this.Data.Gold; }
            set
            {
                if (this.Data.Gold == value)
                    return;
                this.StatusManager.AddGoldChange((int)(value - this.Data.Gold));
                this.Data.Gold = value;
                this.Info.Gold = value;
            }
        }

        public int Ride
        {
            get { return this.Info.Ride; }
            set
            {
                if (this.Info.Ride == value)
                    return;
                this.Info.Ride = value;
            }
        }

        public long Exp
        {
            get { return this.Info.Exp; }
            set
            {
                if (this.Data.Gold == value)
                    return;
                this.StatusManager.AddExpChange((int)(value - this.Data.Exp));
                this.Data.Exp = value;
                this.Info.Exp = value;
            }
        }

        public int Level
        {
            get { return this.Data.Level; }
            set
            {
                if (this.Data.Level == value)
                    return;
                this.StatusManager.AddLevelUp((int)(value - this.Data.Level));
                this.Data.Level = value;
                this.Info.Level = value;
            }
        }

        public void PostProcess(NetMessageResponse message)
        {
            this.FriendManager.PostProcess(message);
            if(this.Team !=null)
            {
                //更新时间在变更时间之前，更新队伍信息
                if(TeamUpdateTS < this.Team.timestamp)
                {
                    Log.InfoFormat("PostProcess > team:character:{0}:{1} {2}<{3}", this.Id, this.Info.Name,TeamUpdateTS, Team.timestamp);
                    TeamUpdateTS = Team.timestamp;
                    this.Team.PostProcess(message);
                }
            }

            if (this.Guild != null)
            {
                //更新公会信息
                Log.InfoFormat("PostProcess > Guild:character:{0}:{1} {2}<{3}", this.Id, this.Info.Name, GuildUpdateTS, Guild.timestamp);
                if (this.Info.Guild == null)
                {
                    this.Info.Guild = this.Guild.GuildInfo(this);
                    if (message.mapCharacterEnter != null)
                        GuildUpdateTS = Guild.timestamp;
                }
                if (GuildUpdateTS < this.Guild.timestamp)
                {
                    GuildUpdateTS = Guild.timestamp;
                    this.Guild.PostProcess(this,message);
                }
            }
            if (this.StatusManager.HasStatus)
            {
                this.StatusManager.PostProcess(message);
            }

            this.Chat.PostProcess(message);
        }

        public NCharacterInfo GetBasicInfo()
        {
            return new NCharacterInfo()
            {
                Id = Info.Id,
                Name = Info.Name,
                Class = Info.Class,
                Level = Info.Level
            };
        }

        public void Clear()
        {
            this.FriendManager.OffLineNotify();
        }
    }
}
