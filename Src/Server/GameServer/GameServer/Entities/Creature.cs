using Common.Data;
using GameServer.Battle;
using GameServer.Core;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Entities
{
    class Creature : Entity
    {
        public string Name { get { return this.Info.Name; } }
        public int Id
        {
            get;set;
        }
        public NCharacterInfo Info;
        public CharacterDefine Define;

        public SkillManager SkillMgr;

        public Creature(CharacterType type, int configId, int level, Vector3Int pos, Vector3Int dir) :
           base(pos, dir)
        {
            this.Info = new NCharacterInfo();
            this.Info.Type = type;
            this.Info.Level = level;
            //原先的TID等于现在的configId
            this.Info.configId = configId;
            this.Info.Entity = this.EntityData;
            this.Info.entityId = this.entityId;
            this.Define = DataManager.Instance.Characters[this.Info.configId];
            this.Info.Name = this.Define.Name;
            this.InitSkill();
        }

        void InitSkill()
        {
            SkillMgr = new SkillManager(this);
            this.Info.Skills.AddRange(this.SkillMgr.Infos);
        }
    }
}
