using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Battle;
using Common.Data;
using Entities;
using Managers;
using Services;
using SkillBridge.Message;
using UnityEngine;

namespace Battle
{
    public class Skill
    {
        public NSkillInfo Info;
        public Creature Owner;
        public SkillDefine Define;
        public float cd = 0;

        public float skillTime;
        public NDamageInfo Damage;
        public bool IsCasting = false;
        private float castTime = 0;
        private int Hit = 0;
        private SkillStatus Status;

        Dictionary<int, List<NDamageInfo>> HitMap = new Dictionary<int, List<NDamageInfo>>();
        public float CD
        {
            get { return cd; }
        }

        public Skill(NSkillInfo info,Creature owner)
        {
            this.Owner = owner;
            this.Info = info;
            this.cd = 0;
            this.Define = DataManager.Instance.Skills[(int)this.Owner.Define.Class][this.Info.Id];
        }

        public SkillResult CanCast(Creature target)
        {
            if(this.Define.CastTarget == Common.Battle.TargetType.Target)
            {
                if(target ==null||target == this.Owner)
                return SkillResult.InvalidTarget;

                int distance = this.Owner.Distance(target);
                //int distance = (int)Vector3Int.Distance(this.Owner.position, target.position); //- this.Owner.Define.Radius - target.Define.Radius;
                if (distance > this.Define.CastRange)
                    return SkillResult.OutOfRange;
            }
            if (this.Define.CastTarget == Common.Battle.TargetType.Position && BattleManager.Instance.CurrentPosition == null)
            {
                if (target == null || target == this.Owner)
                    return SkillResult.InvalidTarget;
            }
            if (this.Owner.Attributes.MP<this.Define.MPCost)
            {
                return SkillResult.OutOfMp;
            }
            if(this.cd>0)
            {
                return SkillResult.CoolDown;
            }
            return SkillResult.Ok;
        }

        public void BeginCast(NDamageInfo damage)
        {
            this.IsCasting = true;
            this.castTime = 0;
            this.cd = this.Define.CD;

            this.skillTime = 0;
            this.Damage = damage;
            this.Hit = 0;

            if (this.Define.CastTime > 0)
                this.Status = SkillStatus.Casting;
            else
                this.Status = SkillStatus.Running;

            this.Owner.PlayAnim(this.Define.SkillAnim);
        }


        public void OnUpdate(float delta)
        {
            UpdateCD(delta);

            if (this.Status == SkillStatus.Casting)
                this.UpdateCasting();
            else if (this.Status == SkillStatus.Running)
                this.UpdateSkill();
        }

        private void UpdateCasting()
        {
            if (this.castTime < this.Define.CastTime)
            {
                this.castTime += Time.deltaTime;
            }
            else
            {
                this.castTime = 0;
                this.Status = SkillStatus.Running;
                Debug.LogFormat("Skill[{0}] UpdateCasting finish", this.Define.Name);
            }
        }

        private void UpdateSkill()
        {
            this.skillTime += Time.deltaTime;

            if (this.Define.Duration > 0)
            {//持续技能
                if (this.skillTime > this.Define.Interval * (this.Hit + 1))
                {
                    this.DoHit();
                }

                if (this.skillTime >= this.Define.Duration)
                {
                    this.Status = SkillStatus.None;
                    this.IsCasting = false;
                    Debug.LogFormat("Skill[{0}] UpdateSkill finish", this.Define.Name);
                }
            }
            else if (this.Define.HitTimes != null && this.Define.HitTimes.Count > 0)
            {
                if (this.Hit < this.Define.HitTimes.Count)
                {
                    if (this.skillTime > this.Define.HitTimes[this.Hit])
                    {
                        this.DoHit();
                    }
                }
                else
                {
                    this.Status = SkillStatus.None;
                    this.IsCasting = false;
                    Debug.LogFormat("Skill[{0}] UpdateCasting finish", this.Define.Name);
                }
            }
        }

        private void DoHit()
        {
            List<NDamageInfo> damages;
            if(this.HitMap.TryGetValue(this.Hit,out damages))
            {
                DoHitDamages(damages);
            }
            this.Hit++;
        }

        public void UpdateCD(float delta)
        {
            if(this.cd > 0)
            {
                this.cd -= delta;
            }
            if(cd < 0)
            {
                this.cd = 0;
            }
        }

        internal void DoHit(int hitId, List<NDamageInfo> damages)
        {
            //服务端信息早到时，置入Map暂存
            if (hitId <= this.Hit)
                this.HitMap[hitId] = damages;
            else
                DoHitDamages(damages);
        }

        internal void DoHitDamages(List<NDamageInfo> damages)
        {
            foreach(var dmg in damages)
            {
                Creature target = EntityManager.Instance.GetEntity(dmg.entityId) as Creature;
                if (target == null) continue;
                target.DoDamage(dmg);
            }
        }

    }
}
