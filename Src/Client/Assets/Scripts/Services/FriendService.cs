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
    class FriendService : Singleton<FriendService>, IDisposable
    {
        public UnityEngine.Events.UnityAction OnFriendUpdate;

        public FriendService()
        {
            MessageDistributer.Instance.Subscribe<FriendAddRequest>(this.OnFriendAddRequest);
            MessageDistributer.Instance.Subscribe<FriendAddResponse>(this.OnFriendAddResponse);
            MessageDistributer.Instance.Subscribe<FriendListResponse>(this.OnFriendList);
            MessageDistributer.Instance.Subscribe<FriendRemoveResponse>(this.OnFriendRemove);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<FriendAddRequest>(this.OnFriendAddRequest);
            MessageDistributer.Instance.Unsubscribe<FriendAddResponse>(this.OnFriendAddResponse);
            MessageDistributer.Instance.Unsubscribe<FriendListResponse>(this.OnFriendList);
            MessageDistributer.Instance.Unsubscribe<FriendRemoveResponse>(this.OnFriendRemove);
        }

        public void Init()
        {

        }

        public void SendFriendAddRequest(int friendId, string friendName)
        {
            Debug.LogFormat("SendFriendAddRequest::friendId :{0} friendName:{1}", friendId, friendName);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.friendAddReq = new FriendAddRequest();
            message.Request.friendAddReq.FromId = User.Instance.CurrentCharacterInfo.Id;
            message.Request.friendAddReq.FromName = User.Instance.CurrentCharacterInfo.Name;
            message.Request.friendAddReq.ToId = friendId;
            message.Request.friendAddReq.ToName = friendName;
            NetClient.Instance.SendMessage(message);
        }

        public void SendFriendAddResponse(bool accept, FriendAddRequest request)
        {
            Debug.LogFormat("SendFriendAddResponse");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.friendAddRes = new FriendAddResponse();
            message.Request.friendAddRes.Result = accept? Result.Success:Result.Failed;
            message.Request.friendAddRes.Errormsg = accept ? "对方已同意" : "对方已拒绝";
            message.Request.friendAddRes.Request = request;
            NetClient.Instance.SendMessage(message);
        }

        void OnFriendAddRequest(object sender, FriendAddRequest request)
        {
            var confirm = MessageBox.Show(string.Format("{0}请求添加好友", request.FromName), "好友请求", MessageBoxType.Confirm, "接受", "拒绝");

            confirm.OnYes = () =>
            {
                this.SendFriendAddResponse(true, request);
            };
            confirm.OnNo = () =>
            {
                this.SendFriendAddResponse(false, request);
            };
        }


        void OnFriendAddResponse(object sender, FriendAddResponse message)
        {
            if(message.Result == Result.Success)
            {
                MessageBox.Show(message.Request.ToName + "接受了您的请求","添加好友成功");
                if (OnFriendUpdate != null)
                    OnFriendUpdate();
            }

            if (message.Result == Result.Failed)
            {
                MessageBox.Show(message.Errormsg , "添加好友失败");
            }
        }

        void OnFriendList(object sender, FriendListResponse message)
        {
            Debug.Log("OnFriendList");
            FriendManager.Instance.Init(message.Friends);
            if (this.OnFriendUpdate != null)
                this.OnFriendUpdate();
        }

        public void SendFriendRemoveRequest(int id, int friendId)
        {
            Debug.LogFormat("SendFriendRemoveRequest::id :{0} friendId:{1}", id, friendId);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.friendRemove = new FriendRemoveRequest();
            message.Request.friendRemove.Id = id;
            message.Request.friendRemove.friendId = friendId;
            NetClient.Instance.SendMessage(message);
        }

        void OnFriendRemove(object sender, FriendRemoveResponse message)
        {
            if (message.Result == Result.Success)
            {
                MessageBox.Show("删除成功", "删除好友");
                if (OnFriendUpdate != null)
                    OnFriendUpdate();
            }

            if (message.Result == Result.Failed)
            {
                MessageBox.Show("删除失败", "删除好友", MessageBoxType.Error);
            }
        }


    }
}
