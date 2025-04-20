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
        private int castTime = 0;
        private int hit = 0;

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

                int distance = (int)Vector3Int.Distance(this.Owner.position, target.position); //- this.Owner.Define.Radius - target.Define.Radius;
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
            this.hit = 0;

            this.Owner.PlayAnim(this.Define.SkillAnim);
        }


        public void OnUpdate(float delta)
        {
            if(this.IsCasting)
            {
                this.skillTime += delta;
                if(skillTime > 0.5&& this.hit == 0)
                {
                    this.DoHit();
                }
                if (skillTime >= this.Define.CD)
                {
                    this.skillTime = 0;
                    this.IsCasting = false;
                }
            }

            UpdateCD(delta);
        }

        private void DoHit()
        {
            if(this.Damage != null)
            {
                var cha = CharacterManager.Instance.GetCharacter(Damage.entityId);
                cha.DoDamage(this.Damage);
            }
            this.hit++;
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
    }
}
