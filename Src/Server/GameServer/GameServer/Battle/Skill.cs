using Common;
using Common.Battle;
using Common.Data;
using Common.Utils;
using GameServer.Core;
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
        List<Bullet> Bullets = new List<Bullet>();

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
            this.Define = DataManager.Instance.Skills[(int)this.Owner.Define.TID][this.Info.Id];
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

                this.AddBuff(TriggerType.SkillCast,this.Context.Target);
                if (this.Instant)
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
            NSkillHitInfo hitInfo = this.InitHitInfo(false);
            this.Hit++;
            Log.InfoFormat("Skill[{0}] DoHit:[{1}]", this.Define.Name, this.Hit);
            //子弹
            if (this.Define.Bullet)
            {
                CastBullet(hitInfo);
                return;
            }
            //不为子弹
            DoHit(hitInfo);
        }

        //传入hitInfo做伤害计算
        public void DoHit(NSkillHitInfo hitInfo)
        {
            Context.Battle.AddHitInfo(hitInfo);
            Log.InfoFormat("Skill[{0}] DoHit:[{1}] isBullet:{2}", this.Define.Name, this.Hit,hitInfo.isBullet);
            //范围
            if (this.Define.AOERange>0)
            {
                this.HitRange(hitInfo);
                return;
            }

            if(this.Define.CastTarget == Common.Battle.TargetType.Target)
            {
                this.HitTarget(Context.Target, hitInfo); 
            }
        }

        void CastBullet(NSkillHitInfo hitInfo)
        {
            Context.Battle.AddHitInfo(hitInfo);
            Log.InfoFormat("Skill[{0}] CastBullet:[{1}]", this.Define.Name, this.Define.BulletResource);
            Bullet bullet = new Bullet(this,this.Context.Target,hitInfo);
            this.Bullets.Add(bullet);
        }

        void HitRange(NSkillHitInfo hitInfo)
        {
            Vector3Int pos;
            if (this.Define.CastTarget == Common.Battle.TargetType.Target)
            {
                pos = Context.Target.Position;
            }
            else if (this.Define.CastTarget == Common.Battle.TargetType.Position)
            {
                pos = Context.Position;
            }
            else
            {
                pos = this.Owner.Position;
            }

            List<Creature> units = this.Context.Battle.FindUnitsInMapRange(pos, this.Define.AOERange);
            foreach (var target in units)
            {
                this.HitTarget(target, hitInfo);
            }
        }

        void HitTarget(Creature target,NSkillHitInfo hit)
        {
            if (this.Define.CastTarget == Common.Battle.TargetType.Self && (target != Context.Caster)) return;
            else if (target == Context.Caster) return;

            NDamageInfo damage = this.CalcSkillDamage(Context.Caster, target);
            Log.InfoFormat("Skill[{0}] HitTarget:[{1}]  Damage:[{2}]  Crit:[{3}]", this.Define.Name, target.Name, damage.Damage, damage.Crit);
            target.DoDamage(damage);
            hit.Damages.Add(damage);

            this.AddBuff(TriggerType.SkillHit,target);
        }
        //根据属性计算伤害值
        NDamageInfo CalcSkillDamage(Creature caster, Creature target)
        {
            float ad = this.Define.AD + caster.Attributes.AD * this.Define.ADFactor;
            float ap = this.Define.AP + caster.Attributes.AP * this.Define.APFactor;

            float addmg = ad * (1 - target.Attributes.DEF / (target.Attributes.DEF + 100));
            float apdmg = ap * (1 - target.Attributes.MDEF / (target.Attributes.MDEF + 100));

            float final = addmg + apdmg;

            bool isCrit = IsCrit(caster.Attributes.CRI);
            if (isCrit)
                final = final * 2;

            //随机浮动
            final = final * (float)MathUtil.Random.NextDouble() * 0.1f - 0.05f;

            NDamageInfo damage = new NDamageInfo();
            damage.entityId = target.entityId;
            damage.Damage = Math.Max(1, (int)final);
            damage.Crit = isCrit;
            return damage;
        }

        bool IsCrit(float crit)
        {
            return MathUtil.Random.NextDouble() < crit;
        }

        private NSkillHitInfo InitHitInfo(bool isBullet)
        {
            NSkillHitInfo HitInfo = new NSkillHitInfo();
            HitInfo.casterId = this.Context.Caster.entityId;
            HitInfo.skillId = this.Info.Id;
            HitInfo.hitId = this.Hit;
            HitInfo.isBullet = isBullet;
            return HitInfo;
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
                    if (!this.Define.Bullet)
                    {
                        this.Status = SkillStatus.None;
                        Log.InfoFormat("Skill[{0}] UpdateCasting finish", this.Define.Name);
                    }
                }
            }

            if(this.Define.Bullet)
            {
                bool finish = true;
                foreach(Bullet bullet in this.Bullets)
                {
                    bullet.Update();
                    if (!bullet.Stoped) finish = false;
                }
                if(finish && this.Hit>= this.Define.HitTimes.Count)
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

        private void AddBuff(TriggerType trigger,Creature target)
        {
            if (this.Define.Buff == null || this.Define.Buff.Count == 0) return;
            foreach(var buffId in this.Define.Buff)
            {
                var buffDefine = DataManager.Instance.Buffs[buffId];

                if (buffDefine.Trigger != trigger) continue;

                if(buffDefine.Target == TargetType.Self)
                {
                    this.Owner.AddBuff(this.Context, buffDefine);
                }
                else if (buffDefine.Target == TargetType.Target)
                {
                    target.AddBuff(this.Context, buffDefine);
                }
            }
        }
    }
}
