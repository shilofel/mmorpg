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
    class BattleService:Singleton<BattleService> 
    {
        public BattleService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<SkillCastRequest>(this.OnSkillCast);
            //MessageDistributer.Instance.Subscribe<MapCharacterEnterResponse>(this.OnCharacterEnter);
        }

        public void Init()
        {

        }

        private void OnSkillCast(NetConnection<NetSession> sender, SkillCastRequest request)
        {
            Log.InfoFormat("OnSkillCast: skill :{0} caster:{1} target:{2} pos:{3}"
                , request.castInfo.skillId, request.castInfo.casterId, request.castInfo.targetId, request.castInfo.Position);

            BattleManager.Instance.ProcessBattleMessage(sender, request);
            //sender.Session.Response.skillCast = new SkillCastResponse();
            //sender.Session.Response.skillCast.Result = Result.Success;
            //sender.Session.Response.skillCast.castInfo = request.castInfo;

            //MapManager.Instance[character.Info.mapId].BroadcastBattleResponse(sender.Session.Response);
        }
    }
}
