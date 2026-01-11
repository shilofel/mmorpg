using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Network;
using UnityEngine;
using UnityEngine.Events;

using Models;
using SkillBridge.Message;
using Common.Data;
using Services;

namespace Managers
{
    class StoryManager : Singleton<StoryManager>
    {

        public void Init()
        {
            NPCManager.Instance.RegisterNPCEvent(NPCFunction.InvokeStory, OnOpenStory);
        }

        private bool OnOpenStory(NPCDefine npc)
        {
            this.ShowStoryUI(npc.Param);
            return true;
        }

        public void ShowStoryUI(int storyId)
        {
            StoryDefine story;
            if (DataManager.Instance.Storys.TryGetValue(storyId, out story))
            {
                UIStory uIStory = UIManager.Instance.Show<UIStory>();
                if (uIStory != null)
                {
                    uIStory.SetStory(story);
                }
            }
        }

        public bool StartStory(int storyId)
        {
            StoryService.Instance.SendStartStory(storyId);
            return true;
        }

        internal void OnStoryStart(int storyId)
        {

        }
    }
}