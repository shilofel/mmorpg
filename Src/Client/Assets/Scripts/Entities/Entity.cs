using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SkillBridge.Message;
using Common;

//位置、速度、方向
namespace Entities
{
    public class Entity
    {
        public int entityId;

        public Vector3Int position;
        public Vector3Int direction;
        public int speed;
        public bool ready = true;

        public IEntityController Controller;

        //从服务端同步的数据
        private NEntity entityData;
        public NEntity EntityData
        {
            get {
                UpdateEntityData();
                return entityData;
            }
            set {
                entityData = value;
                this.SetEntityData(value);
            }
        }

        public Entity(NEntity entity)
        {
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
        }
        //SetEntityData 附带实体数据更新,ready 防止读取状态下进行无效的数据更新
        public void SetEntityData(NEntity entity)
        {
            if (!ready) return;
            this.entityId = entity.Id;
            this.entityData = entity;
            this.position = this.position.FromNVector3(entity.Position);
            this.direction = this.direction.FromNVector3(entity.Direction);
            this.speed = entity.Speed;
        }

        public void UpdateEntityData()
        {
            entityData.Speed = this.speed;
            entityData.Position.FromVector3Int(this.position);
            entityData.Direction.FromVector3Int(this.direction);
        }

    }
}
