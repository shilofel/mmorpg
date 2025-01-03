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
        public EquipDefine equipInfo;
        public ItemDefine Define;

        public Item(NItemInfo item):this(item.Id,item.Count)
        {

        }

        public Item(int id, int count)
        {
            this.Id = id;
            this.Count = count;
            DataManager.Instance.Items.TryGetValue(this.Id, out this.Define);
            DataManager.Instance.Equips.TryGetValue(this.Id, out this.equipInfo);
        }

        public override string ToString()
        {
            return string.Format("ID:{0},Count:{1}", this.Id, this.Count);
        }
    }
}
