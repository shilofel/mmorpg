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
    class TeamService : Singleton<TeamService>, IDisposable
    {
        public UnityEngine.Events.UnityAction OnFriendUpdate;

        public TeamService()
        {
            MessageDistributer.Instance.Subscribe<TeamInviteRequest>(this.OnTeamInviteRequest);
            MessageDistributer.Instance.Subscribe<TeamInviteResponse>(this.OnTeamInviteResponse);
            MessageDistributer.Instance.Subscribe<TeamInfoResponse>(this.OnTeamInfo);
            MessageDistributer.Instance.Subscribe<TeamLeaveResponse>(this.OnTeamLeave);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<TeamInviteRequest>(this.OnTeamInviteRequest);
            MessageDistributer.Instance.Unsubscribe<TeamInviteResponse>(this.OnTeamInviteResponse);
            MessageDistributer.Instance.Unsubscribe<TeamInfoResponse>(this.OnTeamInfo);
            MessageDistributer.Instance.Unsubscribe<TeamLeaveResponse>(this.OnTeamLeave);
        }

        public void Init()
        {

        }

        public void SendTeamInviteRequest(int friendId, string friendName)
        {
            Debug.LogFormat("SendTeamInviteRequest::friendId :{0} friendName:{1}", friendId, friendName);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.teamInviteReq = new TeamInviteRequest();
            message.Request.teamInviteReq.FromId = User.Instance.CurrentCharacterInfo.Id;
            message.Request.teamInviteReq.FromName = User.Instance.CurrentCharacterInfo.Name;
            message.Request.teamInviteReq.ToId = friendId;
            message.Request.teamInviteReq.ToName = friendName;
            NetClient.Instance.SendMessage(message);
        }

        public void SendTeamInviteResponse(bool accept, TeamInviteRequest request)
        {
            Debug.LogFormat("SendTeamInviteResponse");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.teamInviteRes = new TeamInviteResponse();
            message.Request.teamInviteRes.Result = accept ? Result.Success : Result.Failed;
            message.Request.teamInviteRes.Errormsg = accept ? "组队成功" : "对方已拒绝";
            message.Request.teamInviteRes.Request = request;
            NetClient.Instance.SendMessage(message);
        }

        void OnTeamInviteRequest(object sender, TeamInviteRequest request)
        {
            var confirm = MessageBox.Show(string.Format("{0}请求组队", request.FromName), "组队请求", MessageBoxType.Confirm, "接受", "拒绝");

            confirm.OnYes = () =>
            {
                this.SendTeamInviteResponse(true, request);
            };
            confirm.OnNo = () =>
            {
                this.SendTeamInviteResponse(false, request);
            };
        }


        void OnTeamInviteResponse(object sender, TeamInviteResponse message)
        {
            if (message.Result == Result.Success)
            {
                MessageBox.Show(message.Request.ToName + "加入您的队伍", "组队成功");
            }

            if (message.Result == Result.Failed)
            {
                MessageBox.Show(message.Errormsg, "组队失败");
            }
        }

        void OnTeamInfo(object sender, TeamInfoResponse message)
        {
            Debug.Log("OnTeamInfo");
            TeamManager.Instance.UpdateTeamInfo(message.Team);
        }

        public void SendTeamLeaveRequest()
        {
            Debug.LogFormat("SendTeamLeaveRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.teamLeave = new TeamLeaveRequest();
            message.Request.teamLeave.characterId = User.Instance.CurrentCharacterInfo.Id;
            message.Request.teamLeave.TeamId = User.Instance.TeamInfo.Id;
            NetClient.Instance.SendMessage(message);
        }

        void OnTeamLeave(object sender, TeamLeaveResponse message)
        {
            if (message.Result == Result.Success)
            {
                TeamManager.Instance.UpdateTeamInfo(null);
                MessageBox.Show("退出成功", "退出队伍");
            }

            if (message.Result == Result.Failed)
            {
                MessageBox.Show("退出失败", "退出队伍", MessageBoxType.Error);
            }
        }
    }
}
