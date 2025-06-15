using System;
using System.Collections.Generic;
using Common;
using GameServer.Entities;
using Network;
using SkillBridge.Message;
using GameServer.Battle;

namespace GameServer.Managers
{
    class BattleManager : Singleton<BattleManager>
    {
        static long hid = 0;

        public void Init()
        {

        }

        public void ProcessBattleMessage(NetConnection<NetSession> sender, SkillCastRequest request)
        {
            Log.InfoFormat("ProcessBattleMessage: skill :{0} caster:{1} target:{2} pos:{3}"
    , request.castInfo.skillId, request.castInfo.casterId, request.castInfo.targetId, request.castInfo.Position.String());
            Character character = sender.Session.Character;
            var battle = MapManager.Instance[character.Info.mapId].Battle;

            battle.ProcessBattleMessage(sender, request);
        }
    }
}