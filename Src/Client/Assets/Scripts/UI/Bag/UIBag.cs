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

        for (int i = 0; i < BagManager.Instance.Items.Length; i++)
        {
            var item = BagManager.Instance.Items[i];
            if (item.ItemId > 0)
            {
                // 检查格子是否存在，防止数组越界
                if (i < slots.Count)
                {
                    GameObject go = Instantiate(bagItem, slots[i].transform);
                    // 将生成的物品对象加入列表，方便后续清理
                    spawnedItems.Add(go);
                    var ui = go.GetComponent<UIIconItem>();
                    // 增加空值检查，避免空引用异常
                    if (ui != null && ItemManager.Instance.Items.ContainsKey(item.ItemId))
                    {
                        var def = ItemManager.Instance.Items[item.ItemId].Define;
                        ui.SetMainIcon(def.Icon, item.Count.ToString());
                    }
                }
            }
        }
        // 未解锁的格子设为灰色
        for (int i = BagManager.Instance.Items.Length; i < slots.Count; i++)
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
}
