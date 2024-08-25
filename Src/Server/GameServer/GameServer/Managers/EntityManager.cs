using System;
using System.Collections.Generic;
using Common;
using GameServer.Entities;
using SkillBridge.Message;

namespace GameServer.Managers
{
    class EntityManager:Singleton<EntityManager>
    {
        private int idx = 0;
        public List<Entity> AllEntities = new List<Entity>();
        public Dictionary<int, List<Entity>> MapEntities = new Dictionary<int, List<Entity>>();

        //记录在一张地图中的所有实体对象
        public void AddEntity(int mapId,Entity entity)
        {
            AllEntities.Add(entity); 
            //加入唯一id,递增键值
            entity.EntityData.Id = ++this.idx;
            List<Entity> entities = null;

            if(!MapEntities.TryGetValue(mapId,out entities))
            {
                entities = new List<Entity>();
                MapEntities[mapId] = entities;
            }
            entities.Add(entity);
        }

        public void RemoveEntity(int mapId, Entity entity)
        {
            this.AllEntities.Remove(entity);
            this.MapEntities[mapId].Remove(entity);
        }
    }
}