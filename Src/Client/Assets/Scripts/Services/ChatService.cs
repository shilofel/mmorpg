using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Network;
using UnityEngine;

using SkillBridge.Message;
using Models;
using Managers;

namespace Services
{
    class ChatService : Singleton<ChatService>, IDisposable
    {
        public UnityEngine.Events.UnityAction OnFriendUpdate;

        public ChatService()
        {
            MessageDistributer.Instance.Subscribe<chatResponse>(this.OnChat);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Subscribe<chatResponse>(this.OnChat);
        }

        public void Init()
        {

        }

        internal void SendChat(ChatChannel sendChannel, string content, int toId, string toName)
        {
            Debug.LogFormat("SendChat");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.Chat = new ChatRequest();
            message.Request.Chat.Message = new ChatMessage();
            message.Request.Chat.Message.Channel = sendChannel;
            message.Request.Chat.Message.ToId = toId;
            message.Request.Chat.Message.ToName = toName;
            message.Request.Chat.Message.Message = content;
            NetClient.Instance.SendMessage(message);

        }

        private void OnChat(object sender, chatResponse message)
        {
            if (message.Result == Result.Success)
            {
                ChatManager.Instance.AddMessage(ChatChannel.Local, message.localMessages);
                ChatManager.Instance.AddMessage(ChatChannel.World, message.worldMessages);
                ChatManager.Instance.AddMessage(ChatChannel.System, message.systemMessages);
                ChatManager.Instance.AddMessage(ChatChannel.Private, message.privateMessages);
                ChatManager.Instance.AddMessage(ChatChannel.Team, message.teamMessages);
                ChatManager.Instance.AddMessage(ChatChannel.Guild, message.guildMessages);
            }
            else
            {
                ChatManager.Instance.AddSystemMessage(message.Errormsg);
            }

        }
    }
}
