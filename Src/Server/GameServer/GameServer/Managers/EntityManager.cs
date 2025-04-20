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
        public Dictionary<int, Entity> AllEntities = new Dictionary<int, Entity>();
        public Dictionary<int, List<Entity>> MapEntities = new Dictionary<int, List<Entity>>();

        //记录在一张地图中的所有实体对象
        public void AddEntity(int mapId,Entity entity)
        {
            //加入唯一id,递增键值
            entity.EntityData.Id = ++this.idx;
            AllEntities.Add(entity.EntityData.Id,entity);
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
            this.AllEntities.Remove(entity.entityId);
            this.MapEntities[mapId].Remove(entity);
        }

        private Entity GetEntity(int entityId)
        {
            Entity result = null;
            this.AllEntities.TryGetValue(entityId, out result);
            return result;
        }

        public Creature GetCreature(int entityId)
        {
            return GetEntity(entityId) as Creature;
        }
    }
}