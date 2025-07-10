using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Common.Data;
using Entities;
using UnityEngine;

namespace Battle
{
    public class Buff
    {
        public Creature Owner;
        public int BuffId;
        public BuffDefine Define;
        private int CasterId;
        public bool Stoped;
        public float time;

        public Buff(Creature owner, int buffId, BuffDefine define, int casterId)
        {
            this.Owner = owner;
            this.BuffId = buffId;
            this.Define = define;
            this.CasterId = casterId;
            this.OnAdd();
        }

        private void OnAdd()
        {
            Debug.LogFormat("[{0}],OnAdd {1}", this.Owner.Name, this.BuffId);
            if (this.Define.Effect != Common.Battle.BuffEffect.None)
            {
                this.Owner.AddEffect(this.Define.Effect);
            }

            AddAttr();
        }

        public void OnRemove()
        {
            Debug.LogFormat("[{0}],OnRemove {1}", this.Owner.Name, this.BuffId);
            RemoveAttr();
            Stoped = true;

            if (this.Define.Effect != Common.Battle.BuffEffect.None)
            {
                this.Owner.RemoveBuffEffect(this.Define.Effect);
            }
        }


        private void AddAttr()
        {
            if (this.Define.DEFRatio != 0)
            {
                this.Owner.Attributes.Buff.DEF += this.Owner.Attributes.Basic.DEF * this.Define.DEFRatio;
                this.Owner.Attributes.InitFinalAttributes();
            }

        }

        private void RemoveAttr()
        {
            if (this.Define.DEFRatio != 0)
            {
                this.Owner.Attributes.Buff.DEF -= this.Owner.Attributes.Basic.DEF * this.Define.DEFRatio;
                this.Owner.Attributes.InitFinalAttributes();
            }
        }

        internal void OnUpdate(float delta)
        {
            if (Stoped) return;
            this.time += TimeUtil.deltaTime;

            if (this.time > this.Define.Duration)
            {
                this.OnRemove();
            }
        }
    }
}