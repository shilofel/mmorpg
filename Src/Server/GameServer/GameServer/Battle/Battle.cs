using Common.Data;
using GameServer.Entities;
using GameServer.Models;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Managers;

namespace GameServer.Battle
{
    class Battle
    {
        public Map Map;

        Dictionary<int, Creature> AllUnits = new Dictionary<int, Creature>();

        Queue<NSkillCastInfo> Actions = new Queue<NSkillCastInfo>();

        List<Creature> DeahPool = new List<Creature>();

        public Battle(Map map)
        {
            Map = map;
        }

        public void ProcessBattleMessage(NetConnection<NetSession> sender, SkillCastRequest request)
        {
            Character character = sender.Session.Character;
            if(request.castInfo!=null)
            {
                if (character.entityId != request.castInfo.casterId)
                    return;
                this.Actions.Enqueue(request.castInfo);
            }
        }

        public void Update()
        {
            if(this.Actions.Count!=0)
            {
                NSkillCastInfo skillCast = this.Actions.Dequeue();
                this.ExecuteAction(skillCast);
            }
            this.UpdateUnits();
        }

        public void JoinBattle(Creature unit)
        {
            this.AllUnits[unit.entityId] = unit;
        }

        private void LeaveBattle(Creature unit)
        {
            this.AllUnits.Remove(unit.entityId);
        }

        private void ExecuteAction(NSkillCastInfo skillCast)
        {
            BattleContext context = new BattleContext(this);
            context.Caster = EntityManager.Instance.GetCreature(skillCast.casterId);
            context.Target = EntityManager.Instance.GetCreature(skillCast.targetId);
            context.CastSkill = skillCast;
            if (context.Caster != null)
                this.JoinBattle(context.Caster);
            if (context.Target != null)
                this.JoinBattle(context.Target);

            context.Caster.CastSkill(context, skillCast.skillId);
            NetMessageResponse message = new NetMessageResponse();
            message.skillCast = new SkillCastResponse();
            message.skillCast.castInfo = skillCast;
            message.skillCast.Damage = context.Damage;
            message.skillCast.Result = context.Result == SkillResult.Ok ? Result.Success : Result.Failed;
            message.skillCast.Errormsg = context.Result.ToString();
            this.Map.BroadcastBattleResponse(message);
        }

        private void UpdateUnits()
        {
            //更新角色死亡信息，加入死亡池并脱离战斗
            this.DeahPool.Clear();
            foreach (var kv in this.AllUnits)
            {
                kv.Value.Update();
                if (kv.Value.IsDeath)
                    this.DeahPool.Add(kv.Value);
            }

            foreach (var unit in DeahPool)
            {
                this.LeaveBattle(unit);
            }
        }
    }
}