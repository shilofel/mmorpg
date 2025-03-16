using Common.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;
using Models;
using System;

public class NPCController : MonoBehaviour {

    public int npcID;

    NPCDefine npc;

    SkinnedMeshRenderer renderer;
    Animator anim;
    Color orignColor;

    private bool inInteractive = false;
    //添加任务图标的刷新与销毁
    NpcQuestStatus questStatus;
	// Use this for initialization
	void Start () {
        renderer = this.gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        anim = this.gameObject.GetComponentInChildren<Animator>();
        orignColor = renderer.sharedMaterial.color;
        npc = NPCManager.Instance.GetNPCDefine(npcID);
        this.StartCoroutine(Actions());
        RefreshNpcStates();
        QuestManager.Instance.onQuestStatesChanged += OnQuestStatesChanged;
    }

    void OnQuestStatesChanged(Quest quest)
    {
        this.RefreshNpcStates();
    }

    private void RefreshNpcStates()
    {
        questStatus = QuestManager.Instance.GetQuestStatusByNpc(this.npcID);
        UIWorldElementManager.Instance.AddNpcQuestStatus(this.transform, questStatus);
    }

    private void OnDestroy()
    {
        QuestManager.Instance.onQuestStatesChanged -= OnQuestStatesChanged;
        if (UIWorldElementManager.Instance != null)
            UIWorldElementManager.Instance.RemoveNpcQuestStatus(this.transform);
    }
    IEnumerator Actions()
    {
        while(true)
        {
            if (inInteractive)
                yield return new WaitForSeconds(2f);
            else
                yield return new WaitForSeconds(UnityEngine.Random.Range(5f,10f));
            this.Relax();
        }
    }

    private void Relax()
    {
        anim.SetTrigger("Relax");
    }

    // Update is called once per frame
    void Update () {
		
	}

    private void Interactive()
    {
        if(!inInteractive)
        {
            inInteractive = true;
            StartCoroutine(DoInteractive());
        }
    }

    IEnumerator DoInteractive()
    {
        yield return StartCoroutine(FaceToPlayer());
        if (NPCManager.Instance.Interative(npc))
        {
            anim.SetTrigger("Talk");
            yield return new WaitForSeconds(3f);
        }
        inInteractive = false;
    }

    IEnumerator FaceToPlayer()
    {
        //向量减法
        Vector3 faceTo = (User.Instance.CurrentCharacterObject.transform.position - this.transform.position).normalized;
        while(Mathf.Abs(Vector3.Angle(this.gameObject.transform.forward,faceTo))>1f)
        {
            //差值
            this.gameObject.transform.forward = Vector3.Lerp(this.gameObject.transform.forward, faceTo, Time.deltaTime * 5f);
            yield return null;
        }
    }

    private void OnMouseDown()
    {
        Interactive();
    }

    private void OnMouseOver()
    {
        HighLight(true);
    }

    private void OnMouseEnter()
    {
        HighLight(true);
    }

    private void OnMouseExit()
    {
        HighLight(false);
    }

    private void HighLight(bool highLight)
    {
        if(highLight)
        {
            if (renderer.sharedMaterial.color != Color.white)
                renderer.sharedMaterial.color = Color.white;
        }
        else
        {
            if (renderer.sharedMaterial.color != orignColor)
                renderer.sharedMaterial.color = orignColor;
        }
    }
}
