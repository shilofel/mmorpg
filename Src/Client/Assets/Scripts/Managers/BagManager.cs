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
        public delegate void OnItemChangeHandle();

        public event OnItemChangeHandle OnItemChanged;
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
            //指针必须在fixed()内，防止gc移动内存
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

            if (OnItemChanged != null)
                OnItemChanged();
        }

        public void RemoveItem(int id, int count)
        {
            if (count <= 0) return;
            if (!DataManager.Instance.Items.ContainsKey(id))
            {
                Debug.LogError($"移除道具失败：ID={id} 的道具配置不存在");
                return;
            }

            ushort removeCount = (ushort)count;

            // 遍历背包，优先扣除已有同ID道具
            for (int i = 0; i < Items.Length && removeCount > 0; i++)
            {
                if (this.Items[i].ItemId == id)
                {
                    if (this.Items[i].Count > removeCount)
                    {
                        // 当前格子数量足够，直接扣除
                        this.Items[i].Count -= removeCount;
                        removeCount = 0;
                    }
                    else
                    {
                        // 扣除当前格子全部数量，剩余继续扣
                        removeCount -= this.Items[i].Count;
                        this.Items[i].ItemId = 0; // 清空格子
                        this.Items[i].Count = 0;
                    }
                }
            }

            // 剩余数量未扣除（道具不足）
            if (removeCount > 0)
            {
                Debug.LogError($"移除道具失败：ID={id} 不足，需要{count}个，实际缺少{removeCount}个");
            }

            if (OnItemChanged != null)
                OnItemChanged();
        }
    }
}