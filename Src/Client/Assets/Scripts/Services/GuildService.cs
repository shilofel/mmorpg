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
    class GuildService : Singleton<GuildService>, IDisposable
    {
        public UnityEngine.Events.UnityAction<bool> OnGuildCreateResult;
        public UnityEngine.Events.UnityAction OnGuildUpdate;
        public UnityEngine.Events.UnityAction<List<NGuildInfo>> OnGuildListResult;

        public GuildService()
        {
            MessageDistributer.Instance.Subscribe<GuildCreateResponse>(this.OnGuildCreate);
            MessageDistributer.Instance.Subscribe<GuildJoinRequest>(this.OnGuildJoinReq);
            MessageDistributer.Instance.Subscribe<GuildJoinResponse>(this.OnGuildJoinRes);
            MessageDistributer.Instance.Subscribe<GuildListResponse>(this.OnGuildList);
            MessageDistributer.Instance.Subscribe<GuildResponse>(this.OnGuild);
            MessageDistributer.Instance.Subscribe<GuildLeaveResponse>(this.OnGuildLeave);
            MessageDistributer.Instance.Subscribe<GuildAdminResponse>(this.OnGuildAdmin);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<GuildCreateResponse>(this.OnGuildCreate);
            MessageDistributer.Instance.Unsubscribe<GuildJoinRequest>(this.OnGuildJoinReq);
            MessageDistributer.Instance.Unsubscribe<GuildJoinResponse>(this.OnGuildJoinRes);
            MessageDistributer.Instance.Unsubscribe<GuildListResponse>(this.OnGuildList);
            MessageDistributer.Instance.Unsubscribe<GuildResponse>(this.OnGuild);
            MessageDistributer.Instance.Unsubscribe<GuildLeaveResponse>(this.OnGuildLeave);
            MessageDistributer.Instance.Unsubscribe<GuildAdminResponse>(this.OnGuildAdmin);
        }

        public void Init()
        {

        }

        //发送创建公会信息
        internal void SendGuildCreate(string guildName, string notice)
        {
            Debug.LogFormat("SendGuildCreate::guildName :{0} notice:{1}", guildName, notice);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildCreate = new GuildCreateRequest();
            message.Request.guildCreate.GuildName = guildName;
            message.Request.guildCreate.GuildNotice = notice;
            NetClient.Instance.SendMessage(message);
        }

        private void OnGuildCreate(object sender, GuildCreateResponse response)
        {
            Debug.LogFormat("OnGuildCreate:{0}", response.Result);
            if(OnGuildCreateResult != null)
            {
                this.OnGuildCreateResult(response.Result == Result.Success);
            }
            if(response.Result == Result.Success)
            {
                GuildManager.Instance.Init(response.guildInfo);
                MessageBox.Show(string.Format("{0}公会创建成功", response.guildInfo.GuildName), "公会");
            }
            else
            {
                MessageBox.Show(string.Format("{0}公会创建失败", response.guildInfo.GuildName), "公会");
            }
        }

        internal void SendGuildJoinRequest(int guild)
        {
            Debug.LogFormat("SendGuildJoinRequest::guild :{0}", guild);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildJoinReq = new GuildJoinRequest();
            message.Request.guildJoinReq.Apply = new NGuildApplyInfo();
            message.Request.guildJoinReq.Apply.GuildId = guild;
            NetClient.Instance.SendMessage(message);
        }

        internal void SendAdminCommand(GuildAdminCommand command, int characterId)
        {
            Debug.LogFormat("SendAdminCommand");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildAdmin = new GuildAdminRequest();
            message.Request.guildAdmin.Command = command;
            message.Request.guildAdmin.Target = characterId;
            NetClient.Instance.SendMessage(message);
        }

        private void OnGuildAdmin(object sender, GuildAdminResponse message)
        {
            //待实现
            Debug.LogFormat("OnGuildAdmin:{0} {1}",message.Command,message.Result);
            MessageBox.Show(string.Format("执行操作:{0} 结果:{1}{2}", message.Command,message.Result,message.Errormsg));
        }

        //回复申请加入公会的审批
        internal void SendGuildJoinApply(bool accept,NGuildApplyInfo apply)
        {
            Debug.LogFormat("SendGuildJoinResponse");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildJoinRes = new GuildJoinResponse();
            message.Request.guildJoinRes.Result = Result.Success;
            message.Request.guildJoinRes.Apply = apply;
            message.Request.guildJoinRes.Apply.Result = accept ? ApplyResult.Accept : ApplyResult.Reject;
            NetClient.Instance.SendMessage(message);
        }

        private void OnGuildJoinReq(object sender, GuildJoinRequest request)
        {
            var confirm = MessageBox.Show(string.Format("{0}申请加入公会", request.Apply.Name), "公会申请",MessageBoxType.Confirm,"接受","拒绝");

            confirm.OnYes = () =>
            {
                this.SendGuildJoinApply(true, request.Apply);
            };
            confirm.OnNo = () =>
            {
                this.SendGuildJoinApply(false, request.Apply);
            };
        }

        private void OnGuildJoinRes(object sender, GuildJoinResponse response)
        {
            Debug.LogFormat("OnGuildJoinRes");
            if (response.Result == Result.Success)
            {
                MessageBox.Show("加入公会成功");
            }

            if (response.Result == Result.Failed)
            {
                MessageBox.Show("加入公会失败");
            }
        }

        private void OnGuild(object sender, GuildResponse message)
        {
            Debug.LogFormat("OnGuild:{0} {1} {2}", message.Result,message.Guild.Id,message.Guild.GuildName);
            GuildManager.Instance.Init(message.Guild);
            if (this.OnGuildUpdate != null)
                this.OnGuildUpdate();

        }

        public void SendGuildLeaveRequest()
        {
            Debug.LogFormat("SendGuildLeaveRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildLeave = new GuildLeaveRequest();
            NetClient.Instance.SendMessage(message);
        }

        private void OnGuildLeave(object sender, GuildLeaveResponse response)
        {
            Debug.LogFormat("OnGuildJoinRes");
            if (response.Result == Result.Success)
            {
                MessageBox.Show("离开公会成功");
            }

            if (response.Result == Result.Failed)
            {
                MessageBox.Show("离开公会失败","公会",MessageBoxType.Error);
            }
        }

        private void OnGuildList(object sender, GuildListResponse message)
        {
            Debug.LogFormat("OnGuildList");
            if (this.OnGuildListResult != null)
                this.OnGuildListResult(message.Guilds);
        }



        internal void SendGuildListRequest()
        {
            Debug.LogFormat("SendGuildListRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildList = new GuildListRequest();
            NetClient.Instance.SendMessage(message);
        }
    }
}
