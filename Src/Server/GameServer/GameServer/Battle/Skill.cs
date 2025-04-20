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

        public float cd = 0;

        public float CD
        {
            get { return cd; }
        }

        public Skill(NSkillInfo info, Creature owner)
        {
            this.Owner = owner;
            this.Info = info;
            this.Define = DataManager.Instance.Skills[(int)this.Owner.Define.Class][this.Info.Id];
        }

        public SkillResult Cast(BattleContext context)
        {
            SkillResult result = SkillResult.Ok;
            if(context.Target !=null)
            {
                this.DoSkillDamage(context);
            }
            else
                result = SkillResult.InvalidTarget;


            this.cd = this.Define.CD;

            return result;
        }

        private void DoSkillDamage(BattleContext context)
        {
            context.Damage = new NDamageInfo();
            context.Damage.entityId = context.Target.entityId;
            context.Damage.Damage = 100;
            context.Target.DoDamage(context.Damage);
        }

        //更新cd
        internal void Update()
        {
            UpdateCD();
        }

        private void UpdateCD()
        {
            if (this.cd > 0)
            {
                this.cd -= TimeUtil.deltaTime;
            }
            if (cd < 0)
            {
                this.cd = 0;
            }
        }
    }
}
