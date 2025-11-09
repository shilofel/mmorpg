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
        //loadingDone为false，不进行实体的位置同步
        bool loadingDone = true;
        public MapService()
        {
            MessageDistributer.Instance.Subscribe<MapCharacterEnterResponse>(this.OnMapCharacterEnter);
            MessageDistributer.Instance.Subscribe<MapCharacterLeaveResponse>(this.OnMapCharacterLeave);
            MessageDistributer.Instance.Subscribe<MapEntitySyncResponse>(this.OnMapEntitySync);

            SceneManager.Instance.onSceneLoadDone += OnLoadDone;
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
                    //角色开始进入地图，ready取消
                    User.Instance.CurrentCharacter.ready = false;

                    User.Instance.CharacterInited();
                    CharacterManager.Instance.AddCharacter(User.Instance.CurrentCharacter);
                    //修复别人切换地图导致自身切换地图的bug
                    if (CurrentMapId != response.mapId)
                    {
                        this.EnterMap(response.mapId);
                        this.CurrentMapId = response.mapId;
                    }

                    continue;
                }
                CharacterManager.Instance.AddCharacter(new Character(cha));
            }
            
        }

        private void OnMapCharacterLeave(object sender, MapCharacterLeaveResponse response)
        {
            Debug.LogFormat("OnMapCharacterLeave:CharId:{0}", response.entityId);
            //如果是自己离开，清除所有；如果不是，清除其他人
            if (response.entityId != User.Instance.CurrentCharacterInfo.entityId)
                CharacterManager.Instance.RemoveCharacter(response.entityId);
            else
            {
                if(User.Instance.CurrentCharacterObject !=null)
                {
                    User.Instance.CurrentCharacterObject.OnLevelLevel();
                }
                CharacterManager.Instance.Clear();
            }
        }

        private void EnterMap(int mapId)
        {
            if(DataManager.Instance.Maps.ContainsKey(mapId))
            {
                loadingDone = false;
                MapDefine map = DataManager.Instance.Maps[mapId];
                User.Instance.CurrentMapData = map;
                SceneManager.Instance.LoadScene(map.Resource);
                //User.Instance.CurrentCharacter.SetPosition(GameObjectTool.WorldToLogic(User.Instance.CurrentCharacter.position));
            }
            else
            {
                Debug.LogErrorFormat("EnterMap:Map {1} not existed", mapId);
            }
        }

        internal void SendMapEntitySync(EntityEvent entityEvent, NEntity entity,int param)
        {
            if (!loadingDone) return;
            Debug.LogFormat("MapEntityUpdateSync:ID :{0} POS:{1} DIR:{2} SPD:{3}", entity.Id, entity.Position.String(),entity.Direction.String(),entity.Speed);
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
            if (!loadingDone) return;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendFormat("OnMapEntitySync:Entity:{0}", response.entitySyncs.Count);
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
            Debug.LogFormat("SendMapTeleporter: TeleporterID:[{0}]", teleporterID);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.mapTeleport = new MapTeleportRequest();
            message.Request.mapTeleport.teleporterId = teleporterID;
            NetClient.Instance.SendMessage(message);
        }

        private void OnLoadDone()
        {
            if (User.Instance.CurrentCharacter != null)
                User.Instance.CurrentCharacter.ready = true;

            if (User.Instance.CurrentCharacterObject != null)
            {
                User.Instance.CurrentCharacterObject.OnEnterLevel();
            }
            loadingDone = true;
        }
    }
}
