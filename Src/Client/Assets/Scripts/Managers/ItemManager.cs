using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Network;
using UnityEngine;
using UnityEngine.Events;

using Models;
using SkillBridge.Message;
using Common.Data;

namespace Managers
{
    class ItemManager : Singleton<ItemManager>
    {
        public Dictionary<int, Item> items = new Dictionary<int, Item>();

        public void init(List<NItemInfo> items)
        {
            this.items.Clear();
            foreach (var info in items)
            {
                Item item = new Item(info);
                this.items.Add(item.Id, item);

                Debug.LogFormat("ItemManager:Init[{0}]", item);
            }
        }

        public ItemDefine GetItem(int itemId)
        {
            return null;
        }

        //数量为0，不清道具表的键值对
        public bool UseItem(int itemId)
        {
            return false;
        }

        public bool UseItem(ItemDefine item)
        {
            return false;
        }
    }
}