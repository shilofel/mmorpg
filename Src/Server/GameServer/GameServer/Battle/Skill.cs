using Common.Data;
using GameServer.Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Battle
{
    class Skill
    {
        public NSkillInfo Info;
        public Creature Owner;
        public SkillDefine Define;

        public Skill(NSkillInfo info, Creature owner)
        {
            this.Owner = owner;
            this.Info = info;
            this.Define = DataManager.Instance.Skills[(int)this.Owner.Define.Class][this.Info.Id];
        }
    }
}
}
