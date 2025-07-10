using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Common.Data;
using Entities;

namespace Battle
{
    public class BuffManager
    {
        private Creature owner;

        public Dictionary<int, Buff> Buffs = new Dictionary<int, Buff>();

        public BuffManager(Creature owner)
        {
            this.owner = owner;
        }

        internal Buff AddBuff(int buffId, int buffType, int casterId)
        {
            BuffDefine define;
            if(DataManager.Instance.Buffs.TryGetValue(buffId,out define))
            {
                Buff buff = new Buff(this.owner, buffId, define, casterId);
                Buffs[buffId] = buff;
                return buff;
            }
            return null;
        }

        internal Buff RemoveBuff(int buffId)
        {
            Buff buff;
            if (Buffs.TryGetValue(buffId, out buff))
            {
                buff.OnRemove();
                this.Buffs.Remove(buffId);
                return buff;
            }
            return null;
        }

        internal void OnUpdate(float delta)
        {

            List<int> needRemove = new List<int>();
            foreach (var kv in this.Buffs)
            {
                kv.Value.OnUpdate(delta);
                if(kv.Value.Stoped)
                {
                    needRemove.Add(kv.Key);
                }
            }
            foreach (var key in needRemove)
            {
                this.owner.RemoveBuff(key);
            }
        }
    }
}