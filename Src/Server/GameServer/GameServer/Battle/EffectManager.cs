using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Common;
using Common.Battle;
using GameServer.Entities;

namespace GameServer.Battle
{
    class EffectManager
    {
        private Creature Owner;

        Dictionary<BuffEffect,int> Effects = new Dictionary<BuffEffect, int>();

        internal bool HasEffect(BuffEffect effect)
        {
            if(this.Effects.TryGetValue(effect,out int val))
            {
                return val > 0;
            }
            return false;
        }

        public EffectManager(Creature owner)
        {
            this.Owner = owner;
        }

        internal void AddEffect(BuffEffect effect)
        {
            Log.InfoFormat("[{0}],AddEffect {1}", this.Owner.Name, effect);
            if (!this.Effects.ContainsKey(effect))
                this.Effects[effect] = 1;
            else
                this.Effects[effect]++;
        }

        internal void RemoveBuffEffect(BuffEffect effect)
        {
            Log.InfoFormat("[{0}],RemoveBuffEffect {1}", this.Owner.Name, effect);
            if (this.Effects[effect] > 0)
                this.Effects[effect]--;
        }
    }
}