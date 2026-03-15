using GameServer.Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GameServer.Models
{
    class Team
    {
        public int Id;
        public Character Leader;

        public List<Character> members = new List<Character>();

        public double timestamp;

        public Team(Character leader)
        {
            this.AddMember(leader);
        }

        public void AddMember(Character member)
        {
            if(members.Count == 0)
            {
                this.Leader = member;
            }
            this.members.Add(member);
            member.Team = this;
            timestamp = TimeUtil.timestamp;
        }

        public void Leave(Character member)
        {
            if(members.Contains(member))
            {
                this.members.Remove(member);
                if(this.Leader == member)
                {
                    if (this.members.Count > 0)
                    {
                        this.Leader = this.members[0];
                    }
                    else
                        this.Leader = null;
                }
                member.Team = null;
                timestamp = TimeUtil.timestamp;
                
            }
        }

        public void PostProcess(NetMessageResponse message)
        {
            if(message.teamInfo == null)
            {
                message.teamInfo = new TeamInfoResponse();
                message.teamInfo.Team = new NTeamInfo();
                message.teamInfo.Result = Result.Success;
                message.teamInfo.Team.Id = this.Id;
                message.teamInfo.Team.Leader = this.Leader.Id;
                
                foreach(var member in this.members)
                {
                    message.teamInfo.Team.Members.Add(member.GetBasicInfo());
                }
            }
        }
    }
}