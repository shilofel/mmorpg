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
            skill.BeginCast();
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
        }
    }
}
