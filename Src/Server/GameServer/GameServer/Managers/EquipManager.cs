using System;
using System.Collections.Generic;
using Common;
using Common.Data;
using GameServer.Entities;
using GameServer.Services;
using Network;
using SkillBridge.Message;

namespace GameServer.Managers
{
    class EquipManager : Singleton<EquipManager>
    {
        public Result EquipItem(NetConnection<NetSession> sender, int slot, int itemId,bool isEquip)
        {
            Character character = sender.Session.Character;
            if (!character.ItemManager.Items.ContainsKey(itemId))
                return Result.Failed;

            UpdateEquip(character.Info.Equips,slot,itemId,isEquip);
            
            // 重新计算角色属性
            character.RefreshAttributes();

            DBService.Instance.Save();
            return Result.Success;
        }

        unsafe void UpdateEquip(byte[] equipData, int slot,int itemId,bool isEquip)
        {
            fixed(byte* pt = equipData)
            {
                //获取装备槽位指针，如果穿装备，slotId赋值为装备道具id
                int* slotId = (int*)(pt + slot * sizeof(int));
                if (isEquip)
                    *slotId = itemId;
                else
                    *slotId = 0;
            }
        }
    }
}