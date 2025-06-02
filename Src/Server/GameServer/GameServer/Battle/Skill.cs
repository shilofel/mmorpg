using Common;
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

        public SkillStatus Status;

        public float cd = 0;

        public float CD
        {
            get { return cd; }
        }

        private float castingTime = 0;
        private float skillTime = 0;
        private int Hit = 0;
        BattleContext Context;

        public bool Instant
        {
            get
            {
                if (this.Define.CastTime > 0 || this.Define.Bullet || this.Define.Duration > 0) return false;
                if (this.Define.HitTimes != null && this.Define.HitTimes.Count > 0) return false;

                return true;
            }
        }

        public Skill(NSkillInfo info, Creature owner)
        {
            this.Owner = owner;
            this.Info = info;
            this.Define = DataManager.Instance.Skills[(int)this.Owner.Define.Class][this.Info.Id];
        }

        public SkillResult CanCast(BattleContext context)
        {
            if (this.Status != SkillStatus.None)
                return SkillResult.Casting;
            if(this.Define.CastTarget == Common.Battle.TargetType.Target)
            {
                //目标为空或者会技能持有者本身，判断为无效目标
                if (context.Target == null || context.Target == this.Owner)
                    return SkillResult.InvalidTarget;
                //计算自身与目标的距离，判断是否超出施法距离
                int distance = this.Owner.Distance(context.Target);
                if (distance > this.Define.CastRange)
                    return SkillResult.OutOfRange;
            }

            if (this.Define.CastTarget == Common.Battle.TargetType.Position)
            {
                if (context.CastSkill.Position == null)
                    return SkillResult.InvalidTarget;
                if (this.Owner.Distance(context.Position)>this.Define.CastRange)
                    return SkillResult.OutOfRange;
            }

            if (this.Owner.Attributes.MP < this.Define.MPCost)
                return SkillResult.OutOfMp;

            if (this.cd > 0)
                return SkillResult.CoolDown;

            return SkillResult.Ok;
        }

        public SkillResult Cast(BattleContext context)
        {
            SkillResult result = this.CanCast(context);
            if (result == SkillResult.Ok)
            {
                this.castingTime = 0;
                this.Hit = 0;
                this.cd = this.Define.CD;
                this.Context = context;
                this.skillTime = 0;

                if(this.Instant)
                {
                    this.DoHit();
                }
                else
                {
                    if (this.Define.CastTime > 0)
                        this.Status = SkillStatus.Casting;
                    else
                        this.Status = SkillStatus.Running;
                }
            }
            Log.InfoFormat("Skill[{0}] Cast result:[{1}]  Status:[{2}]", this.Define.Name, result, this.Status);
            return result;
        }

        private void DoHit()
        {
            this.Hit++;
            Log.InfoFormat("Skill[{0}] DoHit:[{2}]", this.Define.Name, this.Hit);
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
            if (this.Status == SkillStatus.Casting)
                this.UpdateCasting();
            else if (this.Status == SkillStatus.Running)
                this.UpdateSkill();
        }

        private void UpdateSkill()
        {
            this.skillTime += TimeUtil.deltaTime;

            if(this.Define.Duration>0)
            {//持续技能
                if(this.skillTime>this.Define.Interval*(this.Hit+1))
                {
                    this.DoHit();
                }

                if(this.skillTime>= this.Define.Duration)
                {
                    this.Status = SkillStatus.None;
                    Log.InfoFormat("Skill[{0}] UpdateSkill finish", this.Define.Name);
                }
            }
            else if(this.Define.HitTimes!=null && this.Define.HitTimes.Count>0)
            {
                if(this.Hit<this.Define.HitTimes.Count)
                {
                    if(this.skillTime>this.Define.HitTimes[this.Hit])
                    {
                        this.DoHit();
                    }
                }
                else
                {
                    this.Status = SkillStatus.None;
                    Log.InfoFormat("Skill[{0}] UpdateCasting finish", this.Define.Name);
                }
            }
        }

        private void UpdateCasting()
        {
            if(this.castingTime <this.Define.CastTime)
            {
                this.castingTime += TimeUtil.deltaTime;
            }
            else
            {
                this.castingTime = 0;
                this.Status = SkillStatus.Running;
                Log.InfoFormat("Skill[{0}] UpdateCasting finish", this.Define.Name);
            }
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
