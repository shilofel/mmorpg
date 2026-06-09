using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SkillBridge.Message;
using Managers;
using Models;
using System;

public class UIBag :UIWindow
{
    public Text money;

    public Transform[] pages;
    //图标
    public GameObject bagItem;
    // 背包格子
    List<Image> slots;
    // 存储生成的物品图标对象，用于清理
    private List<GameObject> spawnedItems = new List<GameObject>();
    
    // 道具信息面板
    public ItemInfo itemInfoPanel;
    // 存储当前选中的道具ID
    private int selectedItemId = 0;

    private void Start()
    {
        if (slots == null)
        {
            slots = new List<Image>();
            for (int page = 0; page < this.pages.Length; page++)
            {
                slots.AddRange(this.pages[page].GetComponentsInChildren<Image>(true));
            }
        }
        StartCoroutine(InitBags());
        BagManager.Instance.OnItemChanged += OnReset;
    }

    private void OnEnable()
    {
        // 显示界面时隐藏道具信息面板
        HideItemInfo();
        OnReset();
    }

    private void OnDestroy()
    {
        BagManager.Instance.OnItemChanged -= OnReset;
    }

    IEnumerator InitBags()
    {
        // 初始化前先清理旧物品，避免重复生成
        Clear();

        // 过滤掉装备和坐骑，只显示消耗品、材料和任务道具
        int displayIndex = 0;
        
        for (int i = 0; i < BagManager.Instance.Items.Length; i++)
        {
            var item = BagManager.Instance.Items[i];
            if (item.ItemId > 0)
            {
                // 检查是否为可显示的道具类型（排除装备和坐骑）
                if (ItemManager.Instance.Items.ContainsKey(item.ItemId))
                {
                    var def = ItemManager.Instance.Items[item.ItemId].Define;
                    
                    // 过滤掉装备和坐骑
                    if (def.Type == ItemType.Equip || def.Type == ItemType.Ride)
                    {
                        continue;
                    }
                    
                    // 检查格子是否存在，防止数组越界
                    if (displayIndex < slots.Count)
                    {
                        GameObject go = Instantiate(bagItem, slots[displayIndex].transform);
                        // 将生成的物品对象加入列表，方便后续清理
                        spawnedItems.Add(go);
                        var ui = go.GetComponent<UIBagItem>();
                        // 增加空值检查，避免空引用异常
                        if (ui != null)
                        {
                            ui.SetBagItemIcon(def.Icon, item.Count.ToString(), item.ItemId);
                            
                            // 添加点击事件
                            ui.OnItemClick = OnBagItemClick;
                        }
                        displayIndex++;
                    }
                }
            }
        }
        // 未解锁的格子设为灰色
        for (int i = displayIndex; i < slots.Count; i++)
        {
            slots[i].color = Color.gray;
        }
        SetMoney();
        yield return null;
    }

    public void SetMoney()
    {
        // 空值检查，防止UI组件未赋值导致的异常
        if (money != null && User.Instance?.CurrentCharacterInfo != null)
        {
            this.money.text = User.Instance.CurrentCharacterInfo.Gold.ToString();
        }
    }

    /// <summary>
    /// 清理所有生成的物品图标，恢复格子状态
    /// </summary>
    void Clear()
    {
        // 销毁所有生成的物品图标对象
        foreach (var item in spawnedItems)
        {
            if (item != null)
            {
                Destroy(item);
            }
        }
        // 清空列表
        spawnedItems.Clear();

        // 恢复所有格子的颜色为默认状态
        foreach (var slot in slots)
        {
            if (slot != null)
            {
                slot.color = Color.white;
            }
        }
    }

    public void OnReset()
    {
        // 增加空值检查
        if (BagManager.Instance != null)
        {
            BagManager.Instance.Reset();
        }
        this.Clear();
        StartCoroutine(InitBags());
    }
    
    /// <summary>
    /// 背包道具点击事件处理
    /// </summary>
    /// <param name="bagItem">点击的背包道具</param>
    private void OnBagItemClick(UIBagItem bagItem)
    {
        if (bagItem != null)
        {
            Debug.LogFormat("点击道具: {0}", bagItem.ItemId);
            ShowItemInfo(bagItem.ItemId);
        }
    }
    
    /// <summary>
    /// 显示道具信息面板
    /// </summary>
    /// <param name="itemId">道具ID</param>
    public void ShowItemInfo(int itemId)
    {
        // 如果面板不存在，则创建一个新的
        if (itemInfoPanel == null)
        {
            CreateItemInfoPanel();
        }
        
        if (itemInfoPanel != null)
        {
            selectedItemId = itemId;
            itemInfoPanel.gameObject.SetActive(true);
            itemInfoPanel.SetItemInfo(itemId);
        }
    }
    
    /// <summary>
    /// 创建道具信息面板
    /// </summary>
    private void CreateItemInfoPanel()
    {
        // 尝试从Resources加载预制体
        GameObject prefab = Resources.Load<GameObject>("UI/UIItemInfo");
        if (prefab != null)
        {
            // 在当前UIBag的transform下创建面板
            GameObject panelObj = Instantiate(prefab, this.transform);
            itemInfoPanel = panelObj.GetComponent<ItemInfo>();
            panelObj.SetActive(false);
            Debug.Log("已自动创建ItemInfoPanel");
        }
        else
        {
            Debug.LogWarning("未找到UIItemInfo预制体，无法自动创建道具信息面板");
        }
    }
    
    /// <summary>
    /// 隐藏道具信息面板
    /// </summary>
    public void HideItemInfo()
    {
        if (itemInfoPanel != null)
        {
            itemInfoPanel.ClearInfo();
            itemInfoPanel.gameObject.SetActive(false);
            selectedItemId = 0;
        }
    }
    
    /// <summary>
    /// 获取当前选中的道具ID
    /// </summary>
    public int GetSelectedItemId()
    {
        return selectedItemId;
    }
}
