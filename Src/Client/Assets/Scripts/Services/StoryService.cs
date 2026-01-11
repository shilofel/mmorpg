using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Network;
using UnityEngine;

using SkillBridge.Message;
using Models;
using Common.Data;
using Managers;

namespace Services
{
    class StoryService : Singleton<StoryService>, IDisposable
    {
        public StoryService()
        {
            MessageDistributer.Instance.Subscribe<StoryStartResponse>(this.OnStoryStart);
            MessageDistributer.Instance.Subscribe<StoryEndResponse>(this.OnStoryEnd);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<StoryStartResponse>(this.OnStoryStart);
            MessageDistributer.Instance.Unsubscribe<StoryEndResponse>(this.OnStoryEnd);
        }

        public void Init()
        {
            StoryManager.Instance.Init();
        }

        public bool SendStartStory(int storyId)
        {
            Debug.Log("SendStartStory");

            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.storyStart = new StoryStartRequest();
            message.Request.storyStart.storyId = storyId;

            NetClient.Instance.SendMessage(message);
            return true;
        }

        private void OnStoryStart(object sender, StoryStartResponse message)
        {
            Debug.Log("OnStoryStart" + message.storyId);

            StoryManager.Instance.OnStoryStart(message.storyId);
        }

        public bool SendEndStory(int storyId)
        {
            Debug.Log("SendEndStory" + storyId);

            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.storyEnd = new StoryEndRequest();
            message.Request.storyEnd.storyId = storyId;

            NetClient.Instance.SendMessage(message);
            return true;
        }
        private void OnStoryEnd(object sender, StoryEndResponse message)
        {
            Debug.Log("OnStoryEnd" + message.storyId);

            if(message.Result == Result.Success)
            {

            }
        }
    }
}
