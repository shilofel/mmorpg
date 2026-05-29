using Common.Battle;
using Common.Data;
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
    class Creature : Entity
    {
        public string Name { get { return this.Info.Name; } }
        public int Id
        {
            get;set;
        }
        public NCharacterInfo Info;
        public CharacterDefine Define;

        public SkillManager SkillMgr;
        public BuffManager BuffMgr;
        public EffectManager EffectMgr;

        public Attributes Attributes;
        public bool IsDeath = false;

        public BattleState BattleState;
        public CharacterState State;
        public Map Map;

        public Creature(CharacterType type, int configId, int level, Vector3Int pos, Vector3Int dir) :
           base(pos, dir)
        {
            this.Info = new NCharacterInfo();
            this.Info.Type = type;
            this.Info.Level = level;
            //原先的TID等于现在的configId
            this.Info.configId = configId;
            this.Info.Entity = this.EntityData;
            this.Info.entityId = this.entityId;
            this.Define = DataManager.Instance.Characters[this.Info.configId];
            this.Info.Name = this.Define.Name;
            this.InitSkill();
            this.InitBuff();

            this.Attributes = new Attributes();
            this.Attributes.Init(this.Define, this.Info.Level, this.GetEquips(), this.Info.attrDynamic);
            this.Info.attrDynamic = this.Attributes.DynamicAttr;
            this.State = CharacterState.Idle;
        }

        public virtual void OnEnterMap(Map map)
        {
            this.Map = map; 
        }

        public virtual void OnLeaveMap(Map map)
        {
            this.Map = null;
        }

        internal int Distance(Creature target)
        {
            return (int)Vector3Int.Distance(this.Position, target.Position);
        }

        internal int Distance(Vector3Int Position)
        {
            return (int)Vector3Int.Distance(this.Position, Position);
        }

        internal void DoDamage(NDamageInfo damage, Creature source)
        {
            this.BattleState = BattleState.InBattle;
            this.Attributes.HP -= damage.Damage;
            if(this.Attributes.HP<0)
            {
                this.IsDeath = true;
                damage.WillDead = true;
            }
            //子类重载
            this.OnDamege(damage, source);
        }

        protected virtual void OnDamege(NDamageInfo damage, Creature source)
        {
            
        }

        virtual public List<EquipDefine> GetEquips()
        {
            return null;
        }

        void InitSkill()
        {
            SkillMgr = new SkillManager(this);
            this.Info.Skills.AddRange(this.SkillMgr.Infos);
        }

        private void InitBuff()
        {
            BuffMgr = new BuffManager(this);
            EffectMgr = new EffectManager(this);
        }

        public void CastSkill(BattleContext context, int skillId)
        {
            Skill skill = this.SkillMgr.GetSkill(skillId);
            context.Result = skill.Cast(context);
            //释放成功进战
            if(context.Result == SkillResult.Ok)
            {
                this.BattleState = BattleState.InBattle;
            }
            //为空代表是怪物释放的，不为空由客户端传递而来是角色释放
            if(context.CastSkill == null)
            {
                if(context.Result == SkillResult.Ok)
                {
                    context.CastSkill = new NSkillCastInfo
                    {
                        casterId = this.entityId,
                        targetId = context.Target.entityId,
                        skillId = skill.Define.ID,
                        Position = new NVector3(),
                        Result = context.Result
                    };
                    context.Battle.AddCastSkillInfo(context.CastSkill);
                }
            }
            else
            {
                context.CastSkill.Result = context.Result;
                context.Battle.AddCastSkillInfo(context.CastSkill);
            }
        }

        public override void Update()
        {
            this.SkillMgr.Update();
            this.BuffMgr.Update();
        }

        internal void AddBuff(BattleContext context, BuffDefine buffDefine)
        {
            this.BuffMgr.AddBuff(context, buffDefine);
        }
    }
}
