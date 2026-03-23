using System;
using System.Collections.Generic;
using Common;
using GameServer.Entities;
using GameServer.Models;
using SkillBridge.Message;

namespace GameServer.Managers
{
    class MonsterManager 
    {
        private Map Map;
        public Dictionary<int, Monster> Monsters = new Dictionary<int, Monster>();

        public void Init(Map map)
        {
            this.Map = map;
        }
        internal Monster Create(int spawnMonID,int spawnLevel,NVector3 position, NVector3 direction, int spawnPointID = 0)
        {
            Monster monster = new Monster(spawnMonID, spawnLevel, position, direction);
            monster.SpawnPointID = spawnPointID;
            EntityManager.Instance.AddEntity(this.Map.ID, this.Map.InstanceID, monster);
            //怪物没有dbID，直接使用实体id
            monster.Id = monster.entityId;
            monster.Info.entityId = monster.entityId;
            monster.Info.mapId = this.Map.ID;
            this.Monsters[monster.Id] = monster;

            this.Map.MonsterEnter(monster);
            return monster;
        }
    }
}