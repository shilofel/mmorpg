using Common.Battle;
using GameServer.AI;
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
        AIAgent AI;
        public Map Map;
        private Vector3Int moveTarget;
        Vector3 movePosition;
        public Monster(int tid, int level, Vector3Int pos, Vector3Int dir) : base(CharacterType.Monster, tid, level, pos, dir)
        {
            this.AI = new AIAgent(this)
;        }

        public void OnEnterMap(Map map)
        {
            this.Map = map;
        }
        public override void Update()
        {
            base.Update();
            this.UpdateMovement();
            this.AI.Update();
        }

        public Skill FindSkill(BattleContext context,SkillType type)
        {
            Skill cancast = null;
            foreach(var skill in this.SkillMgr.Skills)
            {
                if ((skill.Define.Type & type) != skill.Define.Type) continue;
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
            if(this.AI != null)
            {
                //攻击攻击自身的角色
                this.AI.OnDamage(damage,source);
            }
        }

        internal void MoveTo(Vector3Int position)
        {
            if(State == CharacterState.Idle)
            {
                State = CharacterState.Move;
            }
            //已抵达不重复进行
            if(this.moveTarget!=position)
            {
                this.moveTarget = position;
                this.movePosition = Position;
                var dist = (this.moveTarget - this.Position);

                this.Direction = dist.normalized;
                this.Speed = this.Define.Speed;

                NEntitySync sync = new NEntitySync();
                sync.Entity = this.EntityData;
                sync.Event = EntityEvent.MoveFwd;
                sync.Id = this.entityId;

                this.Map.UpdateEntity(sync);
            }
        }

        private void UpdateMovement()
        {
            if (State == CharacterState.Move)
            {
                if(this.Distance(this.moveTarget)<50)
                {
                    this.StopMove();
                }
                //时间帧短会导致无法产生有效移动，需要使用Vector3
                if(this.Speed > 0)
                {
                    Vector3 dir = this.Direction;
                    //Vector3Int 默认*100,需要除以100
                    this.movePosition += dir * this.Speed * TimeUtil.deltaTime / 100f;
                    this.Position = movePosition;
                }
            }
        }

        internal void StopMove()
        {
            throw new NotImplementedException();
        }

    }
}
