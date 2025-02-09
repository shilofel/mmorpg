using Common;
using Network;
using UnityEngine;
using UnityEngine.Events;

using Models;
using SkillBridge.Message;
using Common.Data;
using System.Text;
using System.Collections.Generic;
namespace Managers
{
    class TeamManager : Singleton<TeamManager>
    {
        public void Init()
        {

        }

        public void UpdateTeamInfo(NTeamInfo info)
        {
            User.Instance.TeamInfo = info;
            ShowTeamUI(info != null);
        }

        public void ShowTeamUI(bool show)
        {
            if(UIMain.Instance !=null)
            {
                UIMain.Instance.ShowTeamUI(show);
            }
        }
    }
}