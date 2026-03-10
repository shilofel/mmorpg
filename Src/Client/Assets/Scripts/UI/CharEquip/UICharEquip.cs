using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SkillBridge.Message;
using Managers;
using Models;
using System;
using Common.Battle;

class UICharEquip : UIWindow
{
    public Text title;
    public Text money;

    public GameObject itemPrefab;
    public GameObject itemEquipedPrefab;

    public Transform itemListRoot;

    public List<Transform> slots;

    public Text hp;
    public Slider hpBar;

    public Text mp;
    public Slider mpBar;

    public Text[] attrs;

    private UIEquipItem currentSelectedItem;
    public void SelectItem(UIEquipItem item)
    {
        if (currentSelectedItem != null && currentSelectedItem != item)
            currentSelectedItem.Selected = false;
        currentSelectedItem = item;
    }

    private void Start()
    {
        RefreshUI();
        EquipManager.Instance.OnEquipChanged += RefreshUI;
    }

    private void OnDestroy()
    {
        EquipManager.Instance.OnEquipChanged -= RefreshUI;
    }

    private void RefreshUI()
    {
        ClearAllEquipList();
        InitAllEquipItems();
        ClearEquipedList();
        InitEquipedItems();
        this.money.text = User.Instance.CurrentCharacterInfo.Gold.ToString();

        InitAttributes();
    }

    private void InitAllEquipItems()
    {
        foreach(var kv in ItemManager.Instance.Items)
        {
            if(kv.Value.Define.Type == ItemType.Equip && kv.Value.Define.LimitClass == User.Instance.CurrentCharacterInfo.Class)
            {
                //已经装备不显示
                if (EquipManager.Instance.Contains(kv.Key))
                    continue;
                GameObject go = Instantiate(itemPrefab, itemListRoot);
                UIEquipItem ui = go.GetComponent<UIEquipItem>();
                ui.SetEquipItem(kv.Key, kv.Value, this, false);
            }
        }
    }

    void ClearAllEquipList()
    {
        foreach(var item in itemListRoot.GetComponentsInChildren<UIEquipItem>())
        {
            Destroy(item.gameObject);
        }
    }

    private void ClearEquipedList()
    {
        foreach(var item in slots)
        {
            if (item.childCount > 0)
                Destroy(item.GetChild(0).gameObject);
        }
    }

    private void InitEquipedItems()
    {
        for(int i=0;i<(int)EquipSlot.SlotMax;i++)
        {
            var item = EquipManager.Instance.Equips[i];
            {
                if(item !=null)
                {
                    GameObject go = Instantiate(itemEquipedPrefab, slots[i]);
                    UIEquipItem ui = go.GetComponent<UIEquipItem>();
                    ui.SetEquipItem(i, item, this, true);
                }
            }
        }
    }

    public void DoEquip(Item item)
    {
        EquipManager.Instance.EquipItem(item);
    }

    public void UnEquip(Item item)
    {
        EquipManager.Instance.UnEquipItem(item);
    }

    private void InitAttributes()
    {
        var character = User.Instance.CurrentCharacter.Attributes;
        this.hp.text = string.Format("{0}/{1}", character.HP, character.MaxHP);
        this.mp.text = string.Format("{0}/{1}", character.MP, character.MaxMP);
        this.hpBar.maxValue = character.MaxHP;
        this.hpBar.value = character.HP;
        this.mpBar.maxValue = character.MaxMP;
        this.mpBar.value = character.MP;

        for (int i = (int)AttributeType.STR; i < (int)AttributeType.MAX; i++) 
        {
            if (i == (int)AttributeType.CRI)
                this.attrs[i - 2].text = string.Format("{0:f2}%", character.Final.Data[i] * 100);
            else
                this.attrs[i - 2].text = ((int)character.Final.Data[i]).ToString();
        }
    }
}
