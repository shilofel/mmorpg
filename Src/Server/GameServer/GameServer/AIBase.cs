using Common.Battle;
using GameServer.Battle;
using GameServer.Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GameServer.AI
{
    class AIBase
    {
        //ai的拥有者
        private Monster owner;
        Creature Target;
        Skill normalSkill;
        public AIBase(Monster monster)
        {
            this.owner = monster;
            normalSkill = this.owner.SkillMgr.NormalSkill;
        }
        internal void Update()
        {
            if(this.owner.BattleState == Common.Battle.BattleState.InBattle)
            {
                this.UpdateBattle();
            }
        }

        private void UpdateBattle()
        {
            if(this.Target == null)
            {
                this.owner.BattleState = Common.Battle.BattleState.Idle;
                return;
            }
            if (!TryCastSkill())
            {
                if(!TryCastNormal())
                {
                    FollowTarget();
                }
            }
        }

        private void FollowTarget()
        {
            int distance = this.owner.Distance(this.Target);
            if (distance > normalSkill.Define.CastRange - 50)
            {
                this.owner.MoveTo(this.Target.Position);
            }
            else
                this.owner.StopMove();
        }

        private bool TryCastNormal()
        {
            if (this.Target != null)
            {
                BattleContext context = new BattleContext(this.owner.Map.Battle)
                {
                    Target = this.Target,
                    Caster = this.owner,
                };
                var result = normalSkill.CanCast(context);
                if(result == SkillResult.Ok)
                {
                    this.owner.CastSkill(context, normalSkill.Define.ID);
                }
                if(result == SkillResult.OutOfRange)
                {
                    return false;
                }
            }
            return true;
        }

        private bool TryCastSkill()
        {
            if (this.Target != null)
            {
                BattleContext context = new BattleContext(this.owner.Map.Battle)
                {
                    Target = this.Target,
                    Caster = this.owner,
                };
                Skill skill = this.owner.FindSkill(context,SkillType.Skill);
                if (skill != null)
                {
                    this.owner.CastSkill(context, skill.Define.ID);
                    return true;
                }
            }
            return false;
        }

        internal void OnDamage(NDamageInfo damage, Creature source)
        {
            if(this.Target==null)
            {
                this.Target = source;
            }
        }
    }
}