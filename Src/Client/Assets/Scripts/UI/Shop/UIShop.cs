using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Managers;
using Models;
using System;
using Common;
using Common.Data;

public class UIShop : UIWindow
{
    public Text title;
    public Text money;

    public ShopDefine shop;

    public Transform[] itemRoot;
    //图标
    public GameObject shopItem;

    private void Start()
    {
        StartCoroutine(InitItems());
    }

    IEnumerator InitItems()
    {
        foreach (var kv in DataManager.Instance.ShopItems[shop.ID])
        {
            if (kv.Value.Status > 0)
            {
                GameObject go = Instantiate(shopItem, itemRoot[0]);
                var ui = go.GetComponent<UIShopItem>();
                ui.SetShopItem(kv.Key, kv.Value,this);
            }
        }
        yield return null;
    }

    public void SetShop(ShopDefine shop)
    {
        this.shop = shop;
        this.title.text = shop.Name;
        this.money.text = User.Instance.CurrentCharacter.Gold.ToString();
    }

    public void SelectShopItem(UIShopItem item)
    {
        this.selectedItem = item;
    }

    private UIShopItem selectedItem;

    public void OnClickBuy()
    {
        if(this.selectedItem == null)
        {
            MessageBox.Show("请选择要购买的道具", "购买提示");
            return;
        }
        ShopManager.Instance.BuyItem(shop.ID, selectedItem.ShopItemID);
    }
}
