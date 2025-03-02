using Common;
using Network;
using UnityEngine;
using UnityEngine.Events;

using Models;
using SkillBridge.Message;
using Common.Data;
using System.Text;
using System.Collections.Generic;
using System;

namespace Managers
{
    class GuildManager : Singleton<GuildManager>
    {

        public NGuildInfo guildInfo;
        public bool HasGuild
        {
            get { return this.guildInfo != null; }
        }
        public void Init(NGuildInfo guild)
        {
            this.guildInfo = guild;
        }

        public void ShowGuild()
        {
            if (this.HasGuild)
                UIManager.Instance.Show<UIGuild>();
            else
            {
                var win = UIManager.Instance.Show<UIGuildPopNoGuild>();
                win.OnClose += PopNoGuild_OnClose;
            }
        }

        private void PopNoGuild_OnClose(UIWindow sender, UIWindow.WindowResult result)
        {
            //创建公会
            if(result == UIWindow.WindowResult.Yes)
            {
                UIManager.Instance.Show<UIGuildPopCreate>();
            }
            else if(result == UIWindow.WindowResult.No)
            {
                //加入公会
                UIManager.Instance.Show<UIGuildList>();
            }
        }
    }
}