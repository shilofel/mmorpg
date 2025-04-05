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
using SkillBridge.Message;

public class UISkillSlot : MonoBehaviour, IPointerClickHandler
{
    public Image icon;
    public Text cdText;
    public Image overlay;

    Skill skill;

    float overlaySpeed;

    void Start()
    {
        overlay.enabled = false;
        cdText.enabled = false;
    }

    void Update()
    {
        if (this.skill.CD > 0)
        {
            if (!overlay.enabled) overlay.enabled = true;
            if (!cdText.enabled) cdText.enabled = true;

            overlay.fillAmount = this.skill.CD / this.skill.Define.CD;
            this.cdText.text = ((int)Math.Ceiling(this.skill.CD)).ToString();
        }
        else
        {
            if (overlay.enabled) overlay.enabled = false;
            if (this.cdText.enabled) this.cdText.enabled = false;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SkillResult result = this.skill.CanCast(BattleManager.Instance.CurrentTarget);
        switch(result)
        {
            case SkillResult.InvalidTarget:
                MessageBox.Show("技能:" + this.skill.Define.Name + "目标无效");
                break;
            case SkillResult.OutOfMp:
                MessageBox.Show("技能:" + this.skill.Define.Name + "MP不足");
                break;
            case SkillResult.CoolDown:
                MessageBox.Show("技能:" + this.skill.Define.Name + "正在冷却");
                break;
        }

        BattleManager.Instance.CastSkill(skill);
    }

    //public void SetCD(float cd)
    //{
    //    if (!overlay.enabled) overlay.enabled = true;
    //    if (!this.cdText.enabled) this.cdText.enabled = true;
    //    this.cdText.text = ((int)Math.Floor(this.cdRemain)).ToString();
    //    overlay.fillAmount = 1f;
    //    overlaySpeed = 1f / cd;
    //    cdRemain = cd;
    //}

    public void SetSkill(Skill value)
    {
        this.skill = value;
        if (this.icon != null) this.icon.overrideSprite = Resloader.Load<Sprite>(this.skill.Define.Icon);
    }
}
