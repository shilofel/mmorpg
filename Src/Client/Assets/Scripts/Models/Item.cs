using Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SkillBridge.Message;

namespace Models
{
    class Item
    {
        public int Id;

        public int Count;

        internal Item(NItemInfo item)
        {
            this.Id = (short)item.Id;
            this.Count = (short)item.Count;
        }

        public override string ToString()
        {
            return string.Format("ID:{0},Count:{1}", this.Id, this.Count);
        }
    }
}
