using Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Common.Battle;
using SkillBridge.Message;

public class UINameBar : MonoBehaviour {

    public Text avaverName;

    public Character character;

    public UIBuffIcons buffIcons;

    // 职业图标数组：0-战士 1-法师 2-弓箭手 3-怪物
    public Sprite[] classIcons;

    public Image classIcon;

    // Use this for initialization
    void Start () {
		if(this.character!=null)
        {
            this.UpdateInfo();
            buffIcons.SetOwner(this.character);
        }
	}

	// Update is called once per frame
	void Update () {

	}

    void UpdateInfo()
    {
        if (this.character != null)
        {
            string name = this.character.Name + " Lv." + this.character.Info.Level;
            if(name != this.avaverName.text)
            {
                this.avaverName.text = name;
            }

            // 根据职业类型更新图标
            UpdateClassIcon();
        }
    }

    void UpdateClassIcon()
    {
        if (this.classIcon == null || this.classIcons == null || this.classIcons.Length < 4)
            return;

        int iconIndex = GetClassIconIndex();
        if (iconIndex >= 0 && iconIndex < this.classIcons.Length)
        {
            this.classIcon.sprite = this.classIcons[iconIndex];
            this.classIcon.gameObject.SetActive(true);
        }
        else
        {
            this.classIcon.gameObject.SetActive(false);
        }
    }

    int GetClassIconIndex()
    {
        if (this.character == null || this.character.Define == null)
            return -1;

        CharacterClass classType = this.character.Define.Class;

        // 怪物使用独立的图标索引
        if (!this.character.IsPlayer)
        {
            return 0; // 怪物
        }

        // 根据职业类型返回对应的图标索引
        switch (classType)
        {
            case CharacterClass.Warrior:
                return 1; // 战士
            case CharacterClass.Wizard:
                return 2; // 法师
            case CharacterClass.Archer:
                return 3; // 弓箭手
            default:
                return 0;
        }
    }
}
