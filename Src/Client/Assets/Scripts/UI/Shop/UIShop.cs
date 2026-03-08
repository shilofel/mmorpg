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
    //目前只初始化一次，在获取商店道具列表时存在问题
    IEnumerator InitItems()
    {
        int count = 0;
        int page = 0;
        foreach (var kv in DataManager.Instance.ShopItems[shop.ID])
        {
            if (kv.Value.Status > 0)
            {
                GameObject go = Instantiate(shopItem, itemRoot[page]);
                var ui = go.GetComponent<UIShopItem>();
                ui.SetShopItem(kv.Key, kv.Value,this);
                count++;
                if(count>=10)
                {
                    count = 0;
                    page++;
                    itemRoot[page].gameObject.SetActive(true);
                }
            }
        }
        yield return null;
    }

    public void SetShop(ShopDefine shop)
    {
        this.shop = shop;
        this.title.text = shop.Name;
        this.money.text = User.Instance.CurrentCharacterInfo.Gold.ToString();
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
