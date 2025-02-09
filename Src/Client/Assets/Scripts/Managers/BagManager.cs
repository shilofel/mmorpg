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
    class BagManager : Singleton<BagManager>
    {
        public int Unlocked;
        public BagItem[] Items;
        NBagInfo Info;

        unsafe public void Init(NBagInfo info)
        {
            this.Info = info;
            this.Unlocked = info.Unlocked;
            Items = new BagItem[this.Unlocked];
            if(info.Items !=null&& info.Items.Length>= this.Unlocked)
            {
                //字节byte整理成Item数组
                Analyze(info.Items);
            }
            else
            {
                //申请背包内存
                info.Items = new byte[sizeof(BagItem) * this.Unlocked];
                Reset();
            }
        }
        //从ItemMagager中获取道具信息
        public void Reset()
        {
            int i = 0;
            //遍历道具
            foreach (var kv in ItemManager.Instance.Items)
            {
                //道具数量小于单格堆叠限制，直接放置
                //大于进行拆分
                if (kv.Value.Count <= kv.Value.Define.StackLimit)
                {
                    this.Items[i].ItemId = (ushort)kv.Key;
                    this.Items[i].Count = (ushort)kv.Value.Count;
                }
                else
                {
                    int count = kv.Value.Count;
                    while (count > kv.Value.Define.StackLimit)
                    {
                        this.Items[i].ItemId = (ushort)kv.Key;
                        this.Items[i].Count = (ushort)kv.Value.Define.StackLimit;
                        i++;
                        count -= kv.Value.Define.StackLimit;
                    }
                    this.Items[i].ItemId = (ushort)kv.Key;
                    this.Items[i].Count = (ushort)count;
                }
                i++;
            }
        }

        //内存映射到Items数组
        private unsafe void Analyze(byte[] data)
        {
            //指针必须在fixed()内
            fixed(byte * pt = data)
            {
                for(int i=0;i<this.Unlocked;i++)
                {
                    BagItem* item = (BagItem*)(pt + i * sizeof(BagItem));
                    Items[i] = *item;
                }
            }
        }
        //Items数组赋值到内存
        unsafe public NBagInfo GetBagInfo()
        {
            //指针必须在fixed()内
            fixed (byte* pt = Info.Items)
            {
                for (int i = 0; i < this.Unlocked; i++)
                {
                    BagItem* item = (BagItem*)(pt + i * sizeof(BagItem));
                   *item = Items[i];
                }
            }
            return this.Info;
        }

        public void AddItem(int id, int count)
        {
            ushort addCount = (ushort)count;
            for (int i = 0; i < Items.Length; i++)
            {
                if(this.Items[i].ItemId == id)
                {
                    ushort canAdd = (ushort)(DataManager.Instance.Items[id].StackLimit - this.Items[i].Count);
                    if(canAdd >= addCount)
                    {
                        this.Items[i].Count += addCount;
                        addCount = 0;
                        break;
                    }
                    else
                    {
                        this.Items[i].Count += canAdd;
                        addCount -= canAdd;
                    }
                }
            }
            if(addCount>0)
            {
                for (int i = 0; i < Items.Length; i++)
                {
                    if (this.Items[i].ItemId == 0)
                    {
                        this.Items[i].Count = addCount;
                        this.Items[i].ItemId = (ushort)id;
                        break; 
                    }
                }
            }
        }


        public void RemoveItem(int id, int count)
        {
          
        }
    }
}