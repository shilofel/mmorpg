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
    public class Character:Creature
    {
        public bool mapTeleport = false;

        public Character(NCharacterInfo info):base(info)
        {

        }

        public override List<EquipDefine> GetEquips()
        {
            return EquipManager.Instance.GetEquipedDefines();
        }
        
        /// <summary>
        /// 重写UpdateInfo方法，根据角色类型决定是否应用装备属性
        /// </summary>
        public override void UpdateInfo(NCharacterInfo info)
        {
            this.SetEntityData(info.Entity);
            this.Info = info;
            
            // 只有玩家角色才应用装备属性，怪物不应用
            if (this.Info.Type == CharacterType.Player)
            {
                this.Attributes.Init(this.Define, this.Info.Level, this.GetEquips(), this.Info.attrDynamic);
            }
            else
            {
                // 怪物不应用装备属性
                this.Attributes.Init(this.Define, this.Info.Level, null, this.Info.attrDynamic);
            }
            
            this.SkillMgr.UpdateSkills();
        }
    }
}