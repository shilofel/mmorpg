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
    class ShopManager : Singleton<ShopManager>
    {
        UIShop uIShop;
        public void Init()
        {
            NPCManager.Instance.RegisterNPCEvent(NPCFunction.InvokeShop, OnOpenShop);
            StatusService.Instance.RegisterStatusNotify(StatusType.Money, OnBuyItem);
        }

        public bool OnBuyItem(NStatus status)
        {
            if(uIShop!=null)
                uIShop.SetMoney();
            return true;
        }

        private bool OnOpenShop(NPCDefine npc)
        {
            this.ShowShop(npc.Param);
            return true;
        }

        public void ShowShop(int shopId)
        {
            ShopDefine shop;
            if(DataManager.Instance.Shops.TryGetValue(shopId,out shop))
            {
                uIShop = UIManager.Instance.Show<UIShop>();
                if(uIShop != null)
                {
                    uIShop.SetShop(shop);
                }
            }
        }

        public bool BuyItem(int shopId, int shopItemId)
        {
            ItemService.Instance.SendBuyItem(shopId,shopItemId);
            return true;
        }
    }
}