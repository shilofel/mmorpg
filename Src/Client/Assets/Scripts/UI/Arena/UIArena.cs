using Managers;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class UIArena:MonoSingleton<UIArena>
{
    public Text roundText;
    public Text countDownText;

    protected override void OnStart()
    {
        roundText.enabled = false;
        countDownText.enabled = false;
        ArenaManager.Instance.SendReady();
    }

    public void ShowCountDown()
    {
        StartCoroutine(CountDown(10));
    }

    IEnumerator CountDown(int seconds)
    {
        int total = seconds;
        roundText.enabled = true;
        countDownText.enabled = true;
        while(total>0)
        {
            countDownText.text = total.ToString();
            yield return new WaitForSeconds(1f);
            total--;
        }
        countDownText.text = "READY";
        // 等待1秒后隐藏倒计时文本
        yield return new WaitForSeconds(1f);
        countDownText.text = "";
    }

    private void Update()
    {
        
    }

    internal void ShowRoundStart(int round, ArenaInfo arenaInfo)
    {
        countDownText.enabled = true;
        countDownText.text = "FIGHT";
        // 等待1秒后隐藏FIGHT文本并隐藏整个UI
        StartCoroutine(HideFightText());
    }
    
    IEnumerator HideFightText()
    {
        yield return new WaitForSeconds(1f);
        countDownText.text = "";
        // 隐藏整个UI
        roundText.enabled = false;
        countDownText.enabled = false;
    }

    internal void ShowRoundResult(int round, ArenaInfo arenaInfo)
    {
        countDownText.enabled = true;
        countDownText.text = "YOU WIN";
    }
}
