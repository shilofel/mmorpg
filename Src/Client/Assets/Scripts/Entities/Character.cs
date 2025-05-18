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
    }
}