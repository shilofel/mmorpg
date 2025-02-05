using System;
using Services;
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
        public Dictionary<int, Item> Items = new Dictionary<int, Item>();

        public void Init(List<NItemInfo> items)
        {
            this.Items.Clear();
            foreach (var info in items)
            {
                Item item = new Item(info);
                this.Items.Add(item.Id, item);

                Debug.LogFormat("ItemManager:Init[{0}]", item);
            }
            StatusService.Instance.RegisterStatusNotify(StatusType.Item, OnItemNotify);
        }

        public ItemDefine GetItem(int itemId)
        {
            return null;
        }

        private bool OnItemNotify(NStatus status)
        {
            if(status.Action == StatusAction.Add)
            {
                this.AddItem(status.Id, status.Value);
            }
            else if(status.Action == StatusAction.Delete)
            {
                this.RemoveItem(status.Id, status.Value);
            }
            return true;
        }

        void AddItem(int id,int count)
        {
            Item item = null;
            if(this.Items.TryGetValue(id,out item))
            {
                item.Count += count;
            }
            else
            {
                item = new Item(id, count);
                this.Items.Add(id, item);
            }
            BagManager.Instance.AddItem(id, count);
        }


        private void RemoveItem(int id, int count)
        {
            if (!this.Items.ContainsKey(id))
            {
                return;
            }
            Item item = this.Items[id];
            if (item.Count < count)
                return;
            item.Count -= count;
            BagManager.Instance.RemoveItem(id, count);
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