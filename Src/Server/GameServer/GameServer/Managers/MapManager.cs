using System;
using System.Collections.Generic;
using Common;
using GameServer.Entities;
using GameServer.Models;
using Network;
using SkillBridge.Message;

namespace GameServer.Managers
{
    class MapManager : Singleton<MapManager>
    {
        public Dictionary<int, Dictionary<int,Map>> Maps = new Dictionary<int, Dictionary<int, Map>>();
        public void Init()
        {
            foreach (var mapdefine in DataManager.Instance.Maps.Values)
            {
                Log.InfoFormat("MapManager: Map:{0}:{1}", mapdefine.ID, mapdefine.Name);
                int instanceCount = 1;
                if(mapdefine.Type == Common.Data.MapType.Arena)
                {
                    instanceCount = ArenaManager.MaxInstance + 1;
                }
                this.Maps[mapdefine.ID] = new Dictionary<int, Map>();
                for(int i =0;i<instanceCount;i++)
                {
                    this.Maps[mapdefine.ID][i] = new Map(mapdefine, i);
                }
            }
        }
        //提供给只有一个默认副本的地图使用，目前除竞技场以外均是
        public Map this[int key]
        {
            get
            {
                return this.Maps[key][0];
            }
        }

        public Map GetInstance(int mapId,int instance)
        {
            return this.Maps[mapId][instance];
        }

        public void Update()
        {
            foreach(var map in Maps.Values)
            {
                foreach (var instance in map.Values)
                {
                    instance.Update();
                }
            }
        }
    }

}