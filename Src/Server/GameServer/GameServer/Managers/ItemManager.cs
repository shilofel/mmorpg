using System;
using System.Collections.Generic;
using Common;
using GameServer.Entities;
using GameServer.Models;
using GameServer.Services;
using SkillBridge.Message;

namespace GameServer.Managers
{
    class ItemManager
    {
        //道具拥有者
        Character Owner;
        //道具列表
        public Dictionary<int, Item> Items = new Dictionary<int, Item>();
        public ItemManager(Character owner)
        {
            this.Owner = owner;
            foreach(var item in owner.Data.Items)
            {
                this.Items.Add(item.ItemID, new Item(item));
            }
        }
        //数量为0，不清道具表的键值对
        public bool UseItem(int itemId,int count = 1)
        {
            Log.InfoFormat("[{0}]UseItem[{1}:{2}]", this.Owner, itemId, count);
            Item item = null;
            if(Items.TryGetValue(itemId,out item))
            {
                if (item.Count < count)
                    return false;

                //to do 增加使用逻辑
                item.Remove(count);
                return true;
            }
            return false;
        }

        public bool HasItem(int itemId)
        {
            Item item = null;
            if (Items.TryGetValue(itemId, out item))
            {
                return item.Count > 0;
            }
            return false;
        }

        public Item GetItem(int itemId)
        {
            Item item = null;
            Items.TryGetValue(itemId, out item);
            Log.InfoFormat("[{0}]UseItem[{1}:{2}]", this.Owner, itemId, item);
            return item;
        }

        public bool AddItem(int itemId, int count)
        {
            Item item = null;
            if(this.Items.TryGetValue(itemId,out item))
            {
                item.Add(count);
            }
            else
            {
                TCharacterItem dbItem = new TCharacterItem();
                dbItem.CharacterID = Owner.Data.ID;
                dbItem.Owner = this.Owner.Data;
                dbItem.ItemID = itemId;
                dbItem.ItemCount = count;
                Owner.Data.Items.Add(dbItem);
                item = new Item(dbItem);
                this.Items.Add(itemId, item);
            }
            Log.InfoFormat("[{0}]AddItem[{1}] addCount:{2}]", this.Owner, itemId, count);
            //同步数据库，异步
            //DBService.Instance.Save();
            return true;
        }

        public bool RemoveItem(int itemId, int count)
        {
            if (!Items.ContainsKey(itemId))
            {
                return false;
            }
            Item item = this.Items[itemId];
            if (item.Count < count)
                return false;
            item.Remove(count);
            Log.InfoFormat("[{0}]RemoveItem[{1}] RemoveCount:{2}]", this.Owner, itemId, count);
            DBService.Instance.Save();
            return true;
        }
        //内存数据转换为网络数据
        //T to N
        public void GetItemInfos(List<NItemInfo> list)
        {
            foreach(var item in this.Items)
            {
                list.Add(new NItemInfo() { Id = item.Value.ItemID, Count = item.Value.Count });
            }
        }
    }
}