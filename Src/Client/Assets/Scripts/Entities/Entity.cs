using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SkillBridge.Message;

//位置、速度、方向
namespace Entities
{
    public class Entity
    {
        public int entityId;

        public Vector3Int position;
        public Vector3Int direction;
        public int speed;

        //从服务端同步的数据
        private NEntity entityData;
        public NEntity EntityData
        {
            get {
                return entityData;
            }
            set {
                entityData = value;
                this.SetEntityData(value);
            }
        }

        public Entity(NEntity entity)
        {
            this.entityId = entity.Id;
            this.entityData = entity;
            //进行数据设置更新
            this.SetEntityData(entity);
        }

        public virtual void OnUpdate(float delta)
        {
            if (this.speed != 0)
            {
                Vector3 dir = this.direction;
                this.position += Vector3Int.RoundToInt(dir * speed * delta / 100f);
            }
            entityData.Position.FromVector3Int(this.position);
            entityData.Direction.FromVector3Int(this.direction);
            entityData.Speed = this.speed;
        }

        public void SetEntityData(NEntity entity)
        {
            this.position = this.position.FromNVector3(entity.Position);
            this.direction = this.direction.FromNVector3(entity.Direction);
            this.speed = entity.Speed;
        }
    }
}
