using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Network;
using SkillBridge.Message;
using GameServer.Managers;
using Common.Data;
using Common;
using GameServer.Services;

namespace GameServer.Models
{
    class Story
    {
        const float READY_TIME = 11f;
        const float ROUND_TIME = 60f;
        const float RESULT_TIME = 5f;

        public Map Map;
        public NetConnection<NetSession> Player;
        public int StoryId;
        public int InstanceId;

        Map SourceMapRed;

        int StartPoint = 12;

        public int Round { get; internal set; }

        private float timer = 0f;

        public Story(Map map, int storyId, int instance, NetConnection<NetSession> owner)
        {
            this.StoryId = storyId;
            this.InstanceId = instance;
            this.Player = owner;
            this.Map = map;
        }
        //需要记录进入前地图
        internal void PlayerEnter()
        {
            this.SourceMapRed = PlayerLeaveMap(this.Player);

            this.PlayerEnterArena();
        }

        private Map PlayerLeaveMap(NetConnection<NetSession> player)
        {
            var currentMap = MapManager.Instance[player.Session.Character.Info.mapId];
            currentMap.CharacterLeave(player.Session.Character);
            EntityManager.Instance.RemoveMapEntity(currentMap.ID, currentMap.InstanceID, player.Session.Character);
            return currentMap;
        }

        private void PlayerEnterArena()
        {
            TeleporterDefine redPoint = DataManager.Instance.Teleporters[this.StartPoint];
            this.Player.Session.Character.Position = redPoint.Position;
            this.Player.Session.Character.Direction = redPoint.Direction;

            this.Map.AddCharacter(this.Player, this.Player.Session.Character);

            this.Map.CharacterEnter(this.Player, this.Player.Session.Character);

            EntityManager.Instance.AddMapEntity(this.Map.ID, this.Map.InstanceID, this.Player.Session.Character);
        }

        internal void Update()
        {

        }

        private void End()
        {

        }
    }
}