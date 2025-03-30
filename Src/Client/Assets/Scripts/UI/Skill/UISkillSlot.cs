using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Managers;
using Models;
using System;
using Common;
using Common.Data;
using Battle;
using Common.Battle;

public class UISkillSlot : MonoBehaviour, IPointerClickHandler
{
    public Image icon;
    public Text cdText;
    public Image overlay;

    Skill skill;

    float overlaySpeed;
    float cdRemain;

    void Start()
    {

    }

    void Update()
    {
        if (overlay.fillAmount > 0)
        {
            overlay.fillAmount = this.cdRemain / this.skill.Define.CD;
            this.cdText.text = ((int)Math.Ceiling(this.cdRemain)).ToString();
            this.cdRemain -= Time.deltaTime;
        }
        else
        {
            if (overlay.enabled) overlay.enabled = false;
            if (this.cdText.enabled) this.cdText.enabled = false;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SkillResult result = this.skill.CanCast();
        switch(result)
        {
            case SkillResult.InvalidTarget:
                MessageBox.Show("技能:" + this.skill.Define.Name + "目标无效");
                break;
            case SkillResult.OutofMP:
                MessageBox.Show("技能:" + this.skill.Define.Name + "MP不足");
                break;
            case SkillResult.Cooldown:
                MessageBox.Show("技能:" + this.skill.Define.Name + "正在冷却");
                break;
        }

        MessageBox.Show("释放技能:" + this.skill.Define.Name);
        this.SetCD(this.skill.Define.CD);
        this.skill.Cast();
    }

    public void SetCD(float cd)
    {
        if (!overlay.enabled) overlay.enabled = true;
        if (!this.cdText.enabled) this.cdText.enabled = true;
        this.cdText.text = ((int)Math.Floor(this.cdRemain)).ToString();
        overlay.fillAmount = 1f;
        overlaySpeed = 1f / cd;
        cdRemain = cd;
    }

    public void SetSkill(Skill value)
    {
        this.skill = value;
        if (this.icon != null) this.icon.overrideSprite = Resloader.Load<Sprite>(this.skill.Define.Icon);
        this.SetCD(this.skill.Define.CD);
    }
}
