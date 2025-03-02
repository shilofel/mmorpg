using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Network;
using SkillBridge.Message;
using GameServer.Entities;
using GameServer.Managers;

namespace GameServer.Services
{
    class GuildService : Singleton<GuildService>
    {

        public GuildService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildCreateRequest>(this.OnGuildCreate);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildJoinRequest>(this.OnGuildJoinRequest);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildJoinResponse>(this.OnGuildJoinResponse);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildListRequest>(this.OnGuildList);

            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildLeaveRequest>(this.OnGuildLeave);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildAdminRequest>(this.OnGuildAdmin);
        }

        public void Init()
        {
            GuildManager.Instance.Init();
        }

        private void OnGuildCreate(NetConnection<NetSession> sender, GuildCreateRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnGuildCreate: GuildName:{0},Id:{1},Name:{2}",
                request.GuildName, character.Id, character.Name);
            sender.Session.Response.guildCreate = new GuildCreateResponse();
            if (character.Guild != null)
            {
                sender.Session.Response.guildCreate.Errormsg = "已经有公会";
                sender.Session.Response.guildCreate.Result = Result.Failed;
                sender.SendResponse();
                return;
            }

            if (GuildManager.Instance.CheckNameExisted(request.GuildName))
            {
                sender.Session.Response.guildCreate.Errormsg = "公会名称已存在";
                sender.Session.Response.guildCreate.Result = Result.Failed;
                sender.SendResponse();
                return;
            }

            GuildManager.Instance.CreateGuild(request.GuildName, request.GuildNotice, character);
            sender.Session.Response.guildCreate.guildInfo = character.Guild.GuildInfo(character);
            sender.Session.Response.guildCreate.Result = Result.Success;
            sender.SendResponse();
        }

        private void OnGuildList(NetConnection<NetSession> sender, GuildListRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnGuildList: Id:{1},Name:{2}",
                character.Id, character.Name);
            sender.Session.Response.guildList = new GuildListResponse();
            sender.Session.Response.guildList.Guilds.AddRange(GuildManager.Instance.GetGuildInfos());
            sender.Session.Response.guildList.Result = Result.Success;
            sender.SendResponse();
        }

        private void OnGuildJoinRequest(NetConnection<NetSession> sender, GuildJoinRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnGuildJoinRequest: GuildId:{0},characterId:{1},Name:{2}",
                request.Apply.GuildId, request.Apply.characterId, request.Apply.Name);
            var guild = GuildManager.Instance.GetGuild(request.Apply.GuildId);
            if (guild == null)
            {
                sender.Session.Response.guildJoinRes = new GuildJoinResponse();
                sender.Session.Response.guildJoinRes.Errormsg = "公会不存在";
                sender.Session.Response.guildJoinRes.Result = Result.Failed;
                sender.SendResponse();
                return;
            }
            request.Apply.characterId = character.Data.ID;
            request.Apply.Name = character.Data.Name;
            request.Apply.Class = character.Data.Class;
            request.Apply.Level = character.Data.Level;
            if (guild.JoinApply(request.Apply))
            {
                var leader = SessionManager.Instance.GetSession(guild.Data.LeaderID);
                if(leader!=null)
                {//给公会长发送请求
                    leader.Session.Response.guildJoinReq = request;
                    leader.SendResponse();
                }
            }
            else 
            {
                sender.Session.Response.guildJoinRes = new GuildJoinResponse();
                sender.Session.Response.guildJoinRes.Errormsg = "请勿重复申请";
                sender.Session.Response.guildJoinRes.Result = Result.Failed;
                sender.SendResponse();
            }
        }

        private void OnGuildJoinResponse(NetConnection<NetSession> sender, GuildJoinResponse response)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnGuildJoinResponse: GuildId:{0},characterId:{1},Name:{2}",
        response.Apply.GuildId, response.Apply.characterId, response.Apply.Name);
            var guild = GuildManager.Instance.GetGuild(response.Apply.GuildId);

            if (response.Result == Result.Success)
            {   //接受请求
                guild.JoinAppove(response.Apply);
            }
            var requester = SessionManager.Instance.GetSession(response.Apply.characterId);

            if (requester != null)
            {
                requester.Session.Character.Guild = guild;
                //回发消息给请求者
                requester.Session.Response.guildJoinRes = response;
                requester.Session.Response.guildJoinRes.Result = Result.Success;
                requester.Session.Response.guildJoinRes.Errormsg = "加入公会成功";
                requester.SendResponse();
            }
            sender.SendResponse();
        }

        private void OnGuildLeave(NetConnection<NetSession> sender, GuildLeaveRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnGuildLeave: characterId:{1}",character.Id);

            sender.Session.Response.guildLeave = new GuildLeaveResponse();

            character.Guild.Leave(character);
            sender.Session.Response.guildLeave.Result = Result.Success;


            DBService.Instance.Save();
            sender.SendResponse();
        }

        private void OnGuildAdmin(NetConnection<NetSession> sender, GuildAdminRequest message)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnGuildAdmin: characterId:{1}", character.Id);

            sender.Session.Response.guildAdmin = new GuildAdminResponse();
            if (character.Guild == null)
            {
                sender.Session.Response.guildAdmin.Errormsg = "没有公会，无法操作";
                sender.Session.Response.guildAdmin.Result = Result.Failed;
                sender.SendResponse();
                return;
            }
            //需要追加角色是否有权限执行对应操作的检查
            character.Guild.ExecuteAdmin(message.Command, message.Target, character.Id);

            var target = SessionManager.Instance.GetSession(message.Target);
            if(target != null)
            {
                target.Session.Response.guildAdmin = new GuildAdminResponse();
                target.Session.Response.guildAdmin.Command = message;
                target.Session.Response.guildAdmin.Result = Result.Success;
                target.SendResponse();
            }

            sender.Session.Response.guildAdmin.Command = message;
            sender.Session.Response.guildAdmin.Result = Result.Success;
        }
    }
}
