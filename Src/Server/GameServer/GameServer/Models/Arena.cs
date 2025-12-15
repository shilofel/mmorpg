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
    class Arena
    {
        const float READY_TIME = 11f;
        const float ROUND_TIME = 60f;
        const float RESULT_TIME = 5f;

        public Map Map;
        public ArenaInfo ArenaInfo;
        public NetConnection<NetSession> Red;
        public NetConnection<NetSession> Blue;

        Map SourceMapRed;
        Map SourceMapBlue;

        int RedPoint = 9;
        int BluePoint = 10;

        public int Round { get; internal set; }
        public bool Ready { get { return this.redReady && blueReady; } }

        private bool redReady;
        private bool blueReady;

        private ArenaStatus ArenaStatus;
        private ArenaRoundStatus RoundStatus;

        private float timer;
        public Arena(Map map, ArenaInfo info, NetConnection<NetSession> red, NetConnection<NetSession> blue)
        {
            this.Map = map;
            this.ArenaInfo = info;
            this.Red = red;
            this.Blue = blue;
            info.ArenaId = map.InstanceID;
            this.ArenaStatus = ArenaStatus.Wait;
            this.RoundStatus = ArenaRoundStatus.None;
        }
        //需要记录进入前地图
        internal void PlayerEnter()
        {
            this.SourceMapRed = PlayerLeaveMap(this.Red);
            this.SourceMapBlue = PlayerLeaveMap(this.Blue);

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
            TeleporterDefine redPoint = DataManager.Instance.Teleporters[this.RedPoint];
            this.Red.Session.Character.Position = redPoint.Position;
            this.Red.Session.Character.Direction = redPoint.Direction;

            TeleporterDefine bluePoint = DataManager.Instance.Teleporters[this.BluePoint];
            this.Blue.Session.Character.Position = bluePoint.Position;
            this.Blue.Session.Character.Direction = bluePoint.Direction;

            this.Map.AddCharacter(this.Red, this.Red.Session.Character);
            this.Map.AddCharacter(this.Blue, this.Blue.Session.Character);

            this.Map.CharacterEnter(this.Red, this.Red.Session.Character);
            this.Map.CharacterEnter(this.Blue, this.Blue.Session.Character);

            EntityManager.Instance.AddMapEntity(this.Map.ID, this.Map.InstanceID, this.Red.Session.Character);
            EntityManager.Instance.AddMapEntity(this.Map.ID, this.Map.InstanceID, this.Blue.Session.Character);
        }

        internal void Update()
        {
            if(this.ArenaStatus == ArenaStatus.Game)
            {
                UpdateRound();
            }
        }

        private void UpdateRound()
        {
            if(this.RoundStatus == ArenaRoundStatus.Ready)
            {
                this.timer -= TimeUtil.deltaTime;
                if(timer<0)
                {
                    this.RoundStatus = ArenaRoundStatus.Fight;
                    this.timer = ROUND_TIME;
                    Log.InfoFormat("Arena:[{0}] Round Start",
                       this.ArenaInfo.ArenaId);
                    ArenaService.Instance.SendArenaRoundStart(this);
                }
            }
            else if(this.RoundStatus == ArenaRoundStatus.Fight)
            {
                this.timer -= TimeUtil.deltaTime;
                if (timer < 0)
                {
                    this.RoundStatus = ArenaRoundStatus.Result;
                    this.timer = RESULT_TIME;
                    Log.InfoFormat("Arena:[{0}] Round End",
                       this.ArenaInfo.ArenaId);
                    ArenaService.Instance.SendArenaRoundEnd(this);
                }
            }
            else if (this.RoundStatus == ArenaRoundStatus.Result)
            {
                this.timer -= TimeUtil.deltaTime;
                if (timer < 0)
                {
                    if (this.Round >= 3)
                    {
                        ArenaResult();
                    }
                    else
                        NextRound();
                }
            }
        }

        private void ArenaResult()
        {
            this.ArenaStatus = ArenaStatus.Result;
            //执行最终结算
        }

        internal void EntityReady(int entityId)
        {
            if(this.Red.Session.Character.entityId == entityId)
            {
                redReady = true;
            }
            if (this.Blue.Session.Character.entityId == entityId)
            {
                blueReady = true;
            }

            if(this.Ready)
            {
                this.ArenaStatus = ArenaStatus.Game;
                this.Round = 0;
                NextRound();
            }
        }

        private void NextRound()
        {
            this.Round++;
            this.timer = READY_TIME;
            this.RoundStatus = ArenaRoundStatus.Ready;
            Log.InfoFormat("Arena:{0},Round:{1} Ready",
               this.ArenaInfo.ArenaId,this.Round);
            ArenaService.Instance.SendArenaReady(this);
        }
    }
}