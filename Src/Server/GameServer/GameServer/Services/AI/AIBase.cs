using Common.Battle;
using GameServer.Battle;
using GameServer.Entities;
using GameServer.Managers;
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
        // ai的拥有者
        private Monster owner;
        
        // 当前攻击目标
        Creature Target;
        
        // 普通攻击技能
        Skill normalSkill;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="monster">AI所属的怪物</param>
        public AIBase(Monster monster)
        {
            this.owner = monster;
            normalSkill = this.owner.SkillMgr.NormalSkill;
        }
        
        /// <summary>
        /// AI更新逻辑
        /// </summary>
        internal void Update()
        {
            if(this.owner.BattleState == Common.Battle.BattleState.InBattle)
            {
                this.UpdateBattle();
            }
        }

        /// <summary>
        /// 战斗状态更新
        /// </summary>
        private void UpdateBattle()
        {
            // 检查目标是否存在
            if(this.Target == null)
            {
                this.owner.BattleState = Common.Battle.BattleState.Idle;
                // 回到初始刷新点
                ReturnToSpawnPoint();
                return;
            }
            
            // 检查目标是否有效
            if (!IsTargetValid())
            {
                this.Target = null; // 清空无效目标
                this.owner.BattleState = Common.Battle.BattleState.Idle; // 回到 idle 状态
                // 回到初始刷新点
                ReturnToSpawnPoint();
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

        /// <summary>
        /// 检查目标是否有效
        /// </summary>
        /// <returns>目标是否有效</returns>
        private bool IsTargetValid()
        {
            // 检查目标是否为 null
            if (this.Target == null)
                return false;
            
            // 检查目标是否已经死亡
            if (this.Target.IsDeath)
                return false;
            
            // 检查目标是否还在同一个地图
            if (this.Target.Map != this.owner.Map)
                return false;
            
            // 检查目标是否是玩家且已经离线
            Character character = this.Target as Character;
            if (character != null)
            {
                // 检查玩家是否在线（通过 SessionManager）
                return SessionManager.Instance.GetSession(character.Id) != null;
            }
            
            return true;
        }

        /// <summary>
        /// 回到初始刷新点
        /// </summary>
        private void ReturnToSpawnPoint()
        {
            // 检查是否已经在刷新点附近
            if (owner.Distance(owner.SpawnPosition) < 50)
            {
                owner.StopMove();
                return;
            }
            
            // 移动回初始刷新点
            owner.MoveTo(owner.SpawnPosition);
        }

        /// <summary>
        /// 跟随目标
        /// </summary>
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

        /// <summary>
        /// 尝试释放普通攻击
        /// </summary>
        /// <returns>是否成功释放</returns>
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

        /// <summary>
        /// 尝试释放技能
        /// </summary>
        /// <returns>是否成功释放</returns>
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

        /// <summary>
        /// 受到伤害时的处理
        /// </summary>
        /// <param name="damage">伤害信息</param>
        /// <param name="source">伤害来源</param>
        internal void OnDamage(NDamageInfo damage, Creature source)
        {
            if(this.Target==null)
            {
                this.Target = source;
            }
        }
    }
}
