using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Managers;
using Models;
using System;
using Common;
using Common.Data;

public class UIStory : UIWindow
{
    public Text title;
    public Text descript;

    public StoryDefine story;

    private void Start()
    {

    }

    public void SetStory(StoryDefine story)
    {
        this.story = story;
        this.title.text = story.Name;
        this.descript.text = story.Description;
    }

    public void OnClickStart()
    {
        if (!StoryManager.Instance.StartStory(this.story.ID))
        { 

        }
    }
}
