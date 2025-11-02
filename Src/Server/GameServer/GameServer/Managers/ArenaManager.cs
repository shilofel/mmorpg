using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using GameServer.Entities;
using GameServer.Models;
using GameServer.Services;
using Network;
using SkillBridge.Message;

namespace GameServer.Managers
{
    //引入副本概念
    class ArenaManager
    {
        public const int ArenaMapId = 5;
        public const int ManInstance = 100;

        Queue<int> InstanceIndexes = new Queue<int>();

        public void Init()
        {
            for(int i=0;i<ManInstance;i++)
            {
                InstanceIndexes.Enqueue(i);
            }
        }

        //public Arena NewArena(ArenaInfo info,NetConnection<NetSession> red,NetConnection<NetSession> blue)
        //{
        //    var instance = InstanceIndexes.Dequeue();
        //    var map = MapManager.Instance.GetInstance(ArenaMapId, instance);
        //    Arena arena = new Arena(map, info, red, blue);
        //    this.Arenas[instance] = arena;
        //    arena.PlayerEnter();
        //    return arena;
        //}
    }
}