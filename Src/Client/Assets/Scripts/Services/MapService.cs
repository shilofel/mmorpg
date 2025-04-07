using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Network;
using UnityEngine;

using SkillBridge.Message;
using Models;
using Common.Data;
using Entities;

namespace Services
{
    class MapService : Singleton<MapService>, IDisposable
    {

        public int CurrentMapId { get; set; }

        public MapService()
        {
            MessageDistributer.Instance.Subscribe<MapCharacterEnterResponse>(this.OnMapCharacterEnter);
            MessageDistributer.Instance.Subscribe<MapCharacterLeaveResponse>(this.OnMapCharacterLeave);
            MessageDistributer.Instance.Subscribe<MapEntitySyncResponse>(this.OnMapEntitySync);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<MapCharacterEnterResponse>(this.OnMapCharacterEnter);
            MessageDistributer.Instance.Unsubscribe<MapCharacterLeaveResponse>(this.OnMapCharacterLeave);
            MessageDistributer.Instance.Unsubscribe<MapEntitySyncResponse>(this.OnMapEntitySync);
        }

        public void Init()
        {

        }

        private void OnMapCharacterEnter(object sender, MapCharacterEnterResponse response)
        {
            Debug.LogFormat("OnMapCharacterEnter:Map:{0} Count:{1}", response.mapId, response.Characters.Count);
            //如果当前角色是进入地图的角色，变更地图
            foreach(var cha in response.Characters)
            {
                if(User.Instance.CurrentCharacterInfo == null||(cha.Type == CharacterType.Player&&User.Instance.CurrentCharacterInfo.Id == cha.Id))
                {
                    User.Instance.CurrentCharacterInfo = cha;
                    if (User.Instance.CurrentCharacter == null)
                        User.Instance.CurrentCharacter = new Character(cha);
                    else
                        User.Instance.CurrentCharacter.UpdateInfo(cha);

                    CharacterManager.Instance.AddCharacter(User.Instance.CurrentCharacter);
                    continue;
                }
                CharacterManager.Instance.AddCharacter(new Character(cha));
            }
            if(CurrentMapId != response.mapId)
            {
                this.EnterMap(response.mapId);
                this.CurrentMapId = response.mapId;
            }
        }

        private void OnMapCharacterLeave(object sender, MapCharacterLeaveResponse response)
        {
            Debug.LogFormat("OnMapCharacterLeave:CharId:{0}", response.entityId);
            //如果是自己离开，清除所有；如果不是，清除其他人
            if (response.entityId != User.Instance.CurrentCharacterInfo.entityId)
                CharacterManager.Instance.RemoveCharacter(response.entityId);
            else
                CharacterManager.Instance.Clear();
        }

        private void EnterMap(int mapId)
        {
            if(DataManager.Instance.Maps.ContainsKey(mapId))
            {
                MapDefine map = DataManager.Instance.Maps[mapId];
                User.Instance.CurrentMapData = map;
                SceneManager.Instance.LoadScene(map.Resource);
            }
            else
            {
                Debug.LogErrorFormat("EnterMap:Map {1} not existed", mapId);
            }
        }

        internal void SendMapEntitySync(EntityEvent entityEvent, NEntity entity,int param)
        {
            Debug.LogFormat("MapEntityUpdateSync:ID :{0} POS:{1} DIR:{2} SPD:{3}", entity.Id, entity.Position.ToString(),entity.Direction.ToString(),entity.Speed);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.mapEntitySync = new MapEntitySyncRequest();
            message.Request.mapEntitySync.entitySync = new NEntitySync()
            {
                Id = entity.Id,
                Event = entityEvent,
                Entity = entity,
                Param = param
            };

            NetClient.Instance.SendMessage(message);
        }

        private void OnMapEntitySync(object sender, MapEntitySyncResponse response)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendFormat("MapEntityUpdateResponse:Entity:{0}", response.entitySyncs.Count);
            sb.AppendLine();
            foreach(var entity in response.entitySyncs)
            {
                Managers.EntityManager.Instance.OnEntitySync(entity);
                sb.AppendFormat("   [{0}]evt:{1}  entity:{2}",entity.Id,entity.Event,entity.Entity.String());
                sb.AppendLine();
            }

            Debug.Log(sb.ToString());
        }
        //传送点
        internal void SendMapTeleporter(int teleporterID)
        {
            Debug.LogFormat("MapTeleportRequest: TeleporterID:[{0}]", teleporterID);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.mapTeleport = new MapTeleportRequest();
            message.Request.mapTeleport.teleporterId = teleporterID;
            NetClient.Instance.SendMessage(message);
        }
    }
}
