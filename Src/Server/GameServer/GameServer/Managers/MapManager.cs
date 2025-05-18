using System;
using System.Collections.Generic;
using Common;
using GameServer.Entities;
using GameServer.Models;
using SkillBridge.Message;

namespace GameServer.Managers
{
    class MapManager : Singleton<MapManager>
    {
        public Dictionary<int, Map> Maps = new Dictionary<int, Map>();
        public void Init()
        {
            foreach (var mapdefine in DataManager.Instance.Maps.Values)
            {
                Map map = new Map(mapdefine);
                Log.InfoFormat("MapManager: Map:{0}:{1}", map.Define.ID,map.Define.Name);
                this.Maps[mapdefine.ID] = map;
            }
        }

        public Map this[int key]
        {
            get
            {
                return this.Maps[key];
            }
        }

        public void Update()
        {
            foreach(var map in Maps.Values)
            {
                map.Update();
                map.Battle.Update();
            }
        }
    }

}