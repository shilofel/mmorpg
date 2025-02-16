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
    class TeamService : Singleton<TeamService>
    {
        public TeamService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<TeamInviteRequest>(this.OnTeamInviteRequest);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<TeamInviteResponse>(this.OnTeamInviteResponse);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<TeamLeaveRequest>(this.OnTeamLeave);
        }

        public void Init()
        {
            TeamManager.Instance.Init();
        }

        private void OnTeamInviteRequest(NetConnection<NetSession> sender, TeamInviteRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnTeamInviteRequest: FromId:{0},FromName:{1},ToId:{2},ToName:{3}",
                request.FromId, request.FromName, request.ToId, request.ToName);

            NetConnection<NetSession> target = SessionManager.Instance.GetSession(request.ToId);
            //邀请目标不在线
            if (target == null)
            {
                sender.Session.Response.teamInviteRes = new TeamInviteResponse();
                sender.Session.Response.teamInviteRes.Errormsg = "好友不在线";
                sender.Session.Response.teamInviteRes.Result = Result.Failed;
                sender.SendResponse();
                return;
            }
            //邀请角色已有队伍
            if(target.Session.Character.Team!=null)
            {
                sender.Session.Response.teamInviteRes = new TeamInviteResponse();
                sender.Session.Response.teamInviteRes.Errormsg = "好友已经有队伍";
                sender.Session.Response.teamInviteRes.Result = Result.Failed;
                sender.SendResponse();
                return;
            }
            //转发给目标
            Log.InfoFormat("TeamInviteRequest: FromId:{0},FromName:{1},ToId:{2},ToName:{3}",
                request.FromId, request.FromName, request.ToId, request.ToName);
            target.Session.Response.teamInviteReq = request;
            target.SendResponse();
        }

        private void OnTeamInviteResponse(NetConnection<NetSession> sender, TeamInviteResponse response)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnTeamInviteResponse: character:{0},Result:{1},FromId:{2},ToId:{3}",
                character.Id, response.Result, response.Request.FromId, response.Request.ToId);
            sender.Session.Response.teamInviteRes = response;
            if (response.Result == Result.Success)
            {//接受请求
                var requester = SessionManager.Instance.GetSession(response.Request.FromId);
                if (requester == null)
                {
                    sender.Session.Response.friendAddRes.Result = Result.Failed;
                    sender.Session.Response.friendAddRes.Errormsg = "请求者已下线";
                }
                else
                {
                    //组队成功,回发邀请方
                    TeamManager.Instance.AddTeamMember(requester.Session.Character, character);
                    requester.Session.Response.teamInviteRes = response;
                    requester.SendResponse();
                }
            }
            //发送给接受方
            sender.SendResponse();
        }

        private void OnTeamLeave(NetConnection<NetSession> sender, TeamLeaveRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnTeamLeave: character:{0},leader:{1},team:{2}",
                character.Id, character.Team.Leader.Id, character.Team.Id);
            sender.Session.Response.teamLeave = new TeamLeaveResponse();
            sender.Session.Response.teamLeave.characterId = request.characterId;
            if (character.Team != null)
            {
                character.Team.Leave(character);
                sender.Session.Response.teamLeave.Result = Result.Success;
                sender.Session.Response.teamLeave.Errormsg = "None";
            }
            else
            {
                sender.Session.Response.teamLeave.Result = Result.Failed;
                sender.Session.Response.teamLeave.Errormsg = "请求者无队伍";
            }
            sender.SendResponse();
        }
    }
}
