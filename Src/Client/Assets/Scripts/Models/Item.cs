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
        public ItemDefine Define;

        internal Item(NItemInfo item)
        {
            this.Id = (short)item.Id;
            this.Count = (short)item.Count;
            this.Define = DataManager.Instance.Items[item.Id];
        }

        public override string ToString()
        {
            return string.Format("ID:{0},Count:{1}", this.Id, this.Count);
        }
    }
}
