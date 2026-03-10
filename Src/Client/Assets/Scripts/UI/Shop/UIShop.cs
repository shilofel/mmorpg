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
    public Transform[] itemRoot; // 分页父节点
    public GameObject shopItemPrefab; // ShopItem预制体

    private UIShopItem selectedItem;
    // 核心：全局单对象池（所有分页共用）
    private List<UIShopItem> shopItemPool = new List<UIShopItem>();
    // 缓存当前商店的商品数据（Key=ShopItemID，Value=ShopItemDefine）
    private Dictionary<int, ShopItemDefine> currentShopItems = new Dictionary<int, ShopItemDefine>();
    // 当前显示的分页索引
    private int currentPage = 0;
    // 每页显示数量
    private const int ITEM_PER_PAGE = 10;

    private void Awake()
    {
        // 初始化：隐藏所有分页（默认显示第0页）
        for (int i = 0; i < itemRoot.Length; i++)
        {
            itemRoot[i].gameObject.SetActive(i == 0);
        }
        // 预创建少量Item到池（可选，首次打开更流畅）
        PreCreatePoolItems(ITEM_PER_PAGE);
    }

    private void Start()
    {
    }

    // 预创建基础数量的Item，避免首次打开商店时集中创建
    private void PreCreatePoolItems(int count)
    {
        for (int i = 0; i < count; i++)
        {
            CreateNewPoolItem();
        }
    }

    // 创建新Item并加入对象池
    private UIShopItem CreateNewPoolItem()
    {
        GameObject go = Instantiate(shopItemPrefab);
        go.SetActive(false); // 默认隐藏
        UIShopItem item = go.GetComponent<UIShopItem>();
        shopItemPool.Add(item);
        return item;
    }

    // 清空旧商品列表（核心：解决切换商店不刷新问题）
    private void ClearShopItems()
    {
        foreach (var root in itemRoot)
        {
            // 销毁所有子物体（商品项）
            foreach (Transform child in root)
            {
                Destroy(child.gameObject);
            }
            // 隐藏分页（仅保留第一页激活）
            root.gameObject.SetActive(root == itemRoot[0]);
        }
        selectedItem = null; // 清空选中状态
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
                GameObject go = Instantiate(shopItemPrefab, itemRoot[page]);
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
        // 刷新商品数据 + 渲染当前分页
        RefreshShopData();
        RenderCurrentPage();
    }

    public void SetMoney()
    {
        this.money.text = User.Instance.CurrentCharacterInfo.Gold.ToString();
    }


    // 缓存当前商店的有效商品数据
    private void RefreshShopData()
    {
        currentShopItems.Clear();
        if (DataManager.Instance.ShopItems.ContainsKey(shop.ID))
        {
            foreach (var kv in DataManager.Instance.ShopItems[shop.ID])
            {
                if (kv.Value.Status > 0) // 仅显示有效商品
                {
                    // 核心：Key=ShopItemID（kv.Key），Value=商品配置
                    currentShopItems.Add(kv.Key, kv.Value);
                }
            }
        }
    }

    // 转换字典为有序列表（用于分页渲染）
    private List<KeyValuePair<int, ShopItemDefine>> GetCurrentShopItemList()
    {
        // 转为列表保证分页顺序稳定（字典遍历顺序不固定）
        return new List<KeyValuePair<int, ShopItemDefine>>(currentShopItems);
    }

    // 渲染当前分页的商品（核心复用逻辑，补全kv.Key传递）
    private void RenderCurrentPage()
    {
        // 1. 隐藏所有池内Item（先统一隐藏，再按需激活）
        foreach (var item in shopItemPool)
        {
            item.gameObject.SetActive(false);
        }

        // 2. 获取有序商品列表 + 计算当前分页范围
        var shopItemList = GetCurrentShopItemList();
        int startIndex = currentPage * ITEM_PER_PAGE;
        int endIndex = Mathf.Min(startIndex + ITEM_PER_PAGE, shopItemList.Count);
        int itemIndex = 0; // 池内Item的索引

        // 3. 复用池内Item渲染当前分页商品（传递kv.Key）
        for (int i = startIndex; i < endIndex; i++)
        {
            var kv = shopItemList[i];
            int shopItemID = kv.Key; // 核心：拿到原始的kv.Key（ShopItem唯一ID）
            ShopItemDefine shopItemDef = kv.Value;

            UIShopItem uiShopItem;
            // 池内有闲置Item则复用，无则创建
            if (itemIndex < shopItemPool.Count)
            {
                uiShopItem = shopItemPool[itemIndex];
            }
            else
            {
                uiShopItem = CreateNewPoolItem();
            }

            // 设置Item的父节点（当前分页）+ 传递kv.Key + 显示数据 + 激活
            uiShopItem.transform.SetParent(itemRoot[currentPage], false);
            uiShopItem.SetShopItem(shopItemID, shopItemDef, this); // 第一个参数改为kv.Key
            uiShopItem.gameObject.SetActive(true);
            itemIndex++;
        }

        // 4. 隐藏非当前分页的父节点
        for (int i = 0; i < itemRoot.Length; i++)
        {
            itemRoot[i].gameObject.SetActive(i == currentPage);
        }
    }

    public void SelectShopItem(UIShopItem item)
    {
        // 取消上一个选中项的状态
        if (selectedItem != null)
        {
            selectedItem.Selected = false;
        }
        this.selectedItem = item;
    }

    // 分页切换接口（可绑定UI按钮，如“上一页/下一页”）
    public void SwitchPage(int targetPage)
    {
        // 边界校验
        int maxPage = Mathf.CeilToInt((float)currentShopItems.Count / ITEM_PER_PAGE) - 1;
        if (targetPage < 0 || targetPage > maxPage) return;

        currentPage = targetPage;
        RenderCurrentPage();
    }

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
