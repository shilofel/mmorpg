using GameServer.Battle;
using GameServer.Core;
using GameServer.Models;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Entities
{
    class Monster : Creature
    {
        //攻击目标
        Creature Target;
        Map Map;
        public Monster(int tid, int level, Vector3Int pos, Vector3Int dir) : base(CharacterType.Monster, tid, level, pos, dir)
        {

        }

        public void OnEnterMap(Map map)
        {
            this.Map = map;
        }
        public override void Update()
        {
            if(this.State == Common.Battle.CharState.InBattle)
            {
                this.UpdateBattle();
            }
            base.Update();
        }

        private void UpdateBattle()
        {
            if(this.Target!=null)
            {
                BattleContext context = new BattleContext(this.Map.Battle)
                {
                    Target = this.Target,
                    Caster = this,
                };
                Skill skill = this.FindSkill(context);
                if (skill != null)
                {
                    this.CastSkill(context,skill.Define.ID);
                }
            }
        }

        private Skill FindSkill(BattleContext context)
        {
            Skill cancast = null;
            foreach(var skill in this.SkillMgr.Skills)
            {
                var result = skill.CanCast(context);
                //正在释放无法释放技能
                if (result == SkillResult.Casting)
                    return null;
                if (result == SkillResult.Ok)
                {
                    cancast = skill;
                }
            }
            return cancast;
        }

        protected override void OnDamege(NDamageInfo damage, Creature source)
        {
            if(this.Target == null)
            {
                //攻击攻击自身的角色
                this.Target = source;
            }
        }
    }
}
