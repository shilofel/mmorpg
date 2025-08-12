using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkillBridge.Message;
using UnityEngine;
using Common.Battle;
using Managers;
using Battle;
using Common.Data;

namespace Entities
{
    public class Creature : Entity
    {
        public NCharacterInfo Info;

        //数据表配置
        public Common.Data.CharacterDefine Define;

        public Attributes Attributes;

        public SkillManager SkillMgr;
        public BuffManager BuffMgr;
        public EffectManager EffectMar;

        public Action<Buff> OnBuffAdd;
        public Action<Buff> OnBuffRemove;

        bool battleStatus = false;
        public bool BattleStatus
        {
            get { return battleStatus; }
            set
            {
                if(battleStatus!=value)
                {
                    battleStatus = value;
                    this.SetStandby(value);
                }
            }
        }

        public int Id
        {
            get
            {
                return this.Info.Id;
            }
        }

        public string Name
        {
            get
            {
                if (this.Info.Type == CharacterType.Player)
                    return this.Info.Name;
                else
                    return this.Define.Name;
            }
        }

        public Skill CastringSkill = null;

        public bool IsPlayer
        {
            get { return this.Info.Type == CharacterType.Player; }
        }

        internal int Distance(Creature target)
        {
            return (int)Vector3Int.Distance(this.position, target.position);
        }

        public bool IsCurrentPlayer
        {
            get
            {
                if (!IsPlayer) return false;
                return this.Info.Id == Models.User.Instance.CurrentCharacterInfo.Id;
            }
        }

        public Creature(NCharacterInfo info) : base(info.Entity)
        {
            this.Info = info;
            this.Define = DataManager.Instance.Characters[info.configId];
            this.Attributes = new Attributes();
            this.Attributes.Init(this.Define, this.Info.Level, GetEquips(),this.Info.attrDynamic);
            this.SkillMgr = new SkillManager(this);
            this.BuffMgr = new BuffManager(this);
            this.EffectMar = new EffectManager(this);
        }
     

        public void UpdateInfo(NCharacterInfo info)
        {
            this.SetEntityData(info.Entity);
            this.Info = info;
            this.Attributes.Init(this.Define, this.Info.Level, this.GetEquips(), this.Info.attrDynamic);
            this.SkillMgr.UpdateSkills();
        }

        public virtual List<EquipDefine> GetEquips()
        {
            return null;
        }

        public void MoveForward()
        {
            Debug.LogFormat("MoveForward");
            this.speed = this.Define.Speed;
        }

        public void MoveBack()
        {
            Debug.LogFormat("MoveBack");
            this.speed = -this.Define.Speed;
        }

        public void Stop()
        {
            Debug.LogFormat("Stop");
            this.speed = 0;
        }

        public void SetDirection(Vector3Int direction)
        {
            Debug.LogFormat("SetDirection:{0}", direction);
            this.direction = direction;
        }

        public void SetPosition(Vector3Int position)
        {
            Debug.LogFormat("SetPosition:{0}", position);
            this.position = position;
        }

        public void CastSkill(int skillId, Creature target, NVector3 position)
        {
            //设置为战斗状态
            this.SetStandby(true);
            var skill = this.SkillMgr.GetSkill(skillId);
            skill.BeginCast(target,position);
        }

        public void SetStandby(bool standby)
        {
            if (this.Controller != null)
                this.Controller.SetStandby(standby);
        }

        public void PlayAnim(string name)
        {
            if (this.Controller != null)
                this.Controller.PlayAnim(name);
        }

        public override void OnUpdate(float delta)
        {
            base.OnUpdate(delta);
            
            this.SkillMgr.OnUpdate(delta);
            this.BuffMgr.OnUpdate(delta);
        }

        public void DoDamage(NDamageInfo damage)
        {
            Debug.LogFormat("DoDamage:{0} DMG:{1} CRIT:{2}",this.Name, damage.Damage,damage.Crit);
            this.Attributes.HP -= damage.Damage;
            this.PlayAnim("Hurt");
        }

        internal void DoSkillHit(NSkillHitInfo hit)
        {
            Debug.LogFormat("DoSkillHit: Caster:{0} Skill:{1} Hit:{2} IsBullet:{3}", hit.casterId,
                hit.skillId,hit.hitId,hit.isBullet);
            var skill = this.SkillMgr.GetSkill(hit.skillId);
            skill.DoHit(hit);
        }

        internal void DoBuffAction(NBuffInfo buff)
        {
            switch(buff.Action)
            {
                case BuffAction.Add:
                    this.AddBuff(buff.buffId, buff.buffType, buff.casterId);
                    break;
                case BuffAction.Remove:
                    this.RemoveBuff(buff.buffId);
                    break;
                case BuffAction.Hit:
                    this.DoDamage(buff.Damage);
                    break;
                default:
                    break;
            }
        }

        public void RemoveBuff(int buffId)
        {
            var buff = this.BuffMgr.RemoveBuff(buffId);
            if (buff != null && this.OnBuffRemove != null)
            {
                this.OnBuffAdd(buff);
            }
        }

        private void AddBuff(int buffId, int buffType, int casterId)
        {
            var buff = this.BuffMgr.AddBuff(buffId, buffType, casterId);
            if (buff != null&& this.OnBuffAdd!=null)
            {
                this.OnBuffAdd(buff);
            }
        }

        internal void RemoveBuffEffect(BuffEffect effect)
        {
            this.EffectMar.RemoveEffect(effect);
        }

        internal void AddEffect(BuffEffect effect)
        {
            this.EffectMar.AddEffect(effect);
        }

        internal void FaceTo(Vector3Int position)
        {
            this.SetDirection(GameObjectTool.WorldToLogic(GameObjectTool.LogicToWorld(position - this.position).normalized));
            this.UpdateEntityData();
            if (this.Controller != null)
                this.Controller.UpdateDirection();
        }

        internal void PlayEffect(EffectType type, string name, Creature target, float duration)
        {
            if (string.IsNullOrEmpty(name)) return;
            if(this.Controller != null)
            {
                this.Controller.PlayEffect(type, name, target, duration);
            }
        }
    }
}
