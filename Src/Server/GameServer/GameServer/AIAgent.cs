using GameServer.Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GameServer.AI
{
    class AIAgent
    {
        private Monster monster;
        private AIBase ai;

        public AIAgent(Monster monster)
        {
            this.monster = monster;
            string aiName = monster.Define.AI;
            if(string.IsNullOrEmpty(aiName))
            {
                aiName = AIMonsterPassive.ID;
            }
            switch (aiName)
            {
                case AIMonsterPassive.ID:
                    this.ai = new AIMonsterPassive(monster);
                    break;
                case AIBoss.ID:
                    this.ai = new AIBoss(monster);
                    break;
            }
        }
        internal void Update()
        {
            if(this.ai !=null)
            {
                this.ai.Update();
            }
        }

        internal void OnDamage(NDamageInfo damage, Creature source)
        {
            if(this.ai!=null)
            {
                this.ai.OnDamage(damage, source);
            }
        }
    }
}