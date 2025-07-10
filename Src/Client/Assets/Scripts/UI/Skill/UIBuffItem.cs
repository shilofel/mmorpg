using Battle;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBuffItem : MonoBehaviour {

    public Image icon;
    public Image overlay;
    public Text cdText;
    Buff buff;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (this.buff == null) return;
        if (this.buff.time > 0)
        {
            if (!overlay.enabled) overlay.enabled = true;
            if (!cdText.enabled) cdText.enabled = true;

            overlay.fillAmount = this.buff.time / this.buff.Define.Duration;
            this.cdText.text = ((int)Math.Ceiling(this.buff.Define.Duration - this.buff.time)).ToString();
        }
        else
        {
            if (overlay.enabled) overlay.enabled = false;
            if (this.cdText.enabled) this.cdText.enabled = false;
        }
    }

    public void SetItem(Buff buff)
    {
        this.buff = buff;
        if (this.icon != null)
        {
            this.icon.overrideSprite = Resloader.Load<Sprite>(this.buff.Define.Icon);
            this.icon.SetAllDirty();
        }
    }
}
