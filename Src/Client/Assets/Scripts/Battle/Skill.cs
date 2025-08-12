using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Battle;
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
        public Creature Target;
        private NVector3 TargetPosition;
        public bool IsCasting = false;
        private float castTime = 0;
        public int Hit = 0;
        private SkillStatus Status;

        Dictionary<int, List<NDamageInfo>> HitMap = new Dictionary<int, List<NDamageInfo>>();

        List<Bullet> Bullets = new List<Bullet>();
        public float CD
        {
            get { return cd; }
        }

        public Skill(NSkillInfo info,Creature owner)
        {
            this.Owner = owner;
            this.Info = info;
            this.cd = 0;
            this.Define = DataManager.Instance.Skills[(int)this.Owner.Define.TID][this.Info.Id];
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

        public void BeginCast(Creature target,NVector3 pos)
        {
            this.IsCasting = true;
            this.castTime = 0;
            this.cd = this.Define.CD;
            this.Target = target;
            this.TargetPosition = pos;
            this.Owner.PlayAnim(this.Define.SkillAnim);
            this.skillTime = 0;
            this.Bullets.Clear();
            this.HitMap.Clear();

            if(this.Define.CastTarget == Common.Battle.TargetType.Position)
            {
                this.Owner.FaceTo(this.TargetPosition.ToVector3Int());
            }
            else if (this.Define.CastTarget == Common.Battle.TargetType.Target)
            {
                this.Owner.FaceTo(this.Target.position);
            }

            if (this.Define.CastTime > 0)
                this.Status = SkillStatus.Casting;
            else
                this.Status = SkillStatus.Running;

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
                    if (!this.Define.Bullet)
                    {
                        this.Status = SkillStatus.None;
                        this.IsCasting = false;
                        Debug.LogFormat("Skill[{0}] UpdateCasting finish", this.Define.Name);
                    }
                }
            }

            if (this.Define.Bullet)
            {
                bool finish = true;
                foreach (Bullet bullet in this.Bullets)
                {
                    bullet.Update();
                    if (!bullet.Stoped) finish = false;
                }
                if (finish && this.Hit >= this.Define.HitTimes.Count)
                {
                    this.Status = SkillStatus.None;
                    this.IsCasting = false;
                    Debug.LogFormat("Skill[{0}] UpdateCasting finish", this.Define.Name);
                }
            }
        }
        private void DoHit()
        {
            if (this.Define.Bullet)
            {
                this.CastBullet();
            }
            else
                this.DoHitDamages(this.Hit);
            this.Hit++;
        }

        public void DoHitDamages(int hit)
        {
            List<NDamageInfo> damages;
            if(this.HitMap.TryGetValue(hit, out damages))
            {
                DoHitDamages(damages);
            }
            this.Hit++;
        }

        private void CastBullet()
        {
            Bullet bullet = new Bullet(this);
            Debug.LogFormat("Skill[{0}] CastBullet:[{1}]", this.Define.Name, this.Define.BulletResource);
            this.Bullets.Add(bullet);
            this.Owner.PlayEffect(EffectType.Bullet,this.Define.BulletResource,this.Target,bullet.duration);
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

        //服务器传回的hit信息
        internal void DoHit(NSkillHitInfo hit)
        {
            if(hit.isBullet|| !this.Define.Bullet)
            {
                this.DoHit(hit.hitId, hit.Damages);
            }
        }

        internal void DoHit(int hitId, List<NDamageInfo> damages)
        {
            //服务端信息早到时，置入Map暂存
            //hit是本地攻击id  当未来的hitid提前到达时暂存
            if (hitId > this.Hit)
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
