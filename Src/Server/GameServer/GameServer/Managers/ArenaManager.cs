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
    class ArenaManager : Singleton<ArenaManager>
    {
        public const int ArenaMapId = 5;
        public const int MaxInstance = 100;

        Queue<int> InstanceIndexes = new Queue<int>();

        Arena[] Arenas = new Arena[MaxInstance];

        public void Init()
        {
            for(int i=0;i<MaxInstance;i++)
            {
                InstanceIndexes.Enqueue(i);
            }
        }

        public Arena NewArena(ArenaInfo info,NetConnection<NetSession> red,NetConnection<NetSession> blue)
        {
            var instance = InstanceIndexes.Dequeue();
            var map = MapManager.Instance.GetInstance(ArenaMapId, instance);
            Arena arena = new Arena(map,info,red,blue
                );
            this.Arenas[instance] = arena;
            arena.PlayerEnter();
            return arena;
        }

        internal void Update()
        {
            for(int i=0;i<Arenas.Length;i++)
            {
                if(Arenas[i]!=null)
                {
                    Arenas[i].Update();
                }
            }
        }

        public Arena GetArena(int arenaId)
        {
            if (arenaId >= 0 && arenaId < this.Arenas.Length)
            {
                return this.Arenas[arenaId];
            }
            return null;
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