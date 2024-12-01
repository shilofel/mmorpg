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
using Services;

namespace Managers
{
    class EquipManager : Singleton<EquipManager>
    {
        public delegate void OnEquipChangeHandle();

        public event OnEquipChangeHandle OnEquipChanged;

        public Item[] Equips = new Item[(int)EquipSlot.SlotMax];

        byte[] Data;

        unsafe public void Init(byte[] data)
        {
            this.Data = data;
            this.ParseEquipData(data);
        }

        public bool Contains(int equipId)
        {
            for(int i=0;i<Equips.Length;i++)
            {
                if (this.Equips[i] != null && Equips[i].Id == equipId)
                    return true;
            }
            return false;
        }

        public Item GetEquip(EquipSlot slot)
        {
            return Equips[(int)slot];
        }

        unsafe private void ParseEquipData(byte[] data)
        {
            fixed (byte* pt = this.Data)
            {
                for (int i = 0; i < this.Equips.Length; i++)
                {
                    int itemId = *(int*)(pt + i * sizeof(int));
                    if (itemId > 0)
                        Equips[i] = ItemManager.Instance.Items[itemId];
                    else
                        Equips[i] = null;
                }
            }
        }

        unsafe public byte[] GetEquipData()
        {
            //指针必须在fixed()内
            fixed (byte* pt = Data)
            {
                for (int i = 0; i < (int)EquipSlot.SlotMax; i++)
                {
                    int* itemId = (int*)(pt + i * sizeof(int));
                    if (Equips[i] == null)
                        *itemId = 0;
                    else
                        *itemId = Equips[i].Id;
                }
            }
            return this.Data;
        }

        public void EquipItem(Item equip)
        {
            ItemService.Instance.SendItemEquip(equip, true);
        }

        public void UnEquipItem(Item equip)
        {
            ItemService.Instance.SendItemEquip(equip, false);
        }

        public void OnEquipItem(Item equip)
        {
            if(this.Equips[(int)equip.equipInfo.Slot] != null && this.Equips[(int)equip.equipInfo.Slot].Id == equip.Id)
            {
                return;
            }

            this.Equips[(int)equip.equipInfo.Slot] = ItemManager.Instance.Items[equip.Id];

            if (OnEquipChanged != null)
                OnEquipChanged();
        }

        public void OnUnEquipItem(EquipSlot slot)
        {
            if(this.Equips[(int)slot] != null)
            {
                this.Equips[(int)slot] = null;
                if (OnEquipChanged != null)
                    OnEquipChanged();
            }
        }
    }
}