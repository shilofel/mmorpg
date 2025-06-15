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

public class UISkillSlots : MonoBehaviour
{
    public UISkillSlot[] slots;

    void Start()
    {
    }

    public void UpdateSkills()
    {
        if (User.Instance.CurrentCharacter == null) return;
        var Skills = User.Instance.CurrentCharacter.SkillMgr.Skills;
        int skillIdx = 0;
        foreach(var skill in Skills)
        {
            slots[skillIdx].SetSkill(skill);
            skillIdx++;
        }
    }
}
