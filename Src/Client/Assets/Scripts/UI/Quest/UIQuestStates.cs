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

class UIQuestStates : MonoBehaviour
{

    public Image[] statusImage;

    private NpcQuestStatus questStatus;

    void Start()
    {

    }

    public void SetQuestStatus(NpcQuestStatus status)
    {
        this.questStatus = status;

        for (int i = 0; i < 4; i++) 
        {
            if (this.statusImage[i] != null)
                this.statusImage[i].gameObject.SetActive(i == (int)status);
        }
    }

}
