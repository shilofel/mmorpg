using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SkillBridge.Message;
using GameServer.Battle;
using Common;
using Network;
using GameServer.Entities;
using GameServer.Managers;
using Common.Data;
using GameServer.Services;

namespace GameServer.Models
{
    class Map
    {
        internal class MapCharacter
        {
            public NetConnection<NetSession> connection;
            public Character character;

            public MapCharacter(NetConnection<NetSession> conn,Character cha)
            {
                this.connection = conn;
                this.character = cha;
            }
        }

        public int ID
        {
            get { return this.Define.ID; }
        }

        public int InstanceID { get; set; }
        internal MapDefine Define;

        //地图角色使用characterId为key值
        Dictionary<int, MapCharacter> MapCharacters = new Dictionary<int, MapCharacter>();

        SpawnManager SpawnManager = new SpawnManager();

        public MonsterManager MonsterManager = new MonsterManager();
        public Battle.Battle Battle;

        internal Map(MapDefine define,int instance)
        {
            this.Define = define;
            this.InstanceID = instance;
            this.SpawnManager.Init(this);
            this.MonsterManager.Init(this);
            this.Battle = new Battle.Battle(this);
        }

        public void Update()
        {
            SpawnManager.Update();
            this.Battle.Update();
        }

        internal void CharacterEnter(NetConnection<NetSession> conn, Character character)
        {
            Log.InfoFormat("CharacterEnter: Map:{0} characterId:{1}", this.Define.ID, character.Id);
            AddCharacter(conn, character);

            conn.Session.Response.mapCharacterEnter = new MapCharacterEnterResponse();
            conn.Session.Response.mapCharacterEnter.mapId = this.Define.ID;

            //告知其他角色自己进入某地图
            foreach (var kv in this.MapCharacters)
            {
                conn.Session.Response.mapCharacterEnter.Characters.Add(kv.Value.character.Info);
                if (kv.Value.character != character)
                    this.SendCharacterEnterMap(kv.Value.connection, character.Info);
            }
            //通知怪物入场
            foreach (var kv in this.MonsterManager.Monsters)
            {
                conn.Session.Response.mapCharacterEnter.Characters.Add(kv.Value.Info);
            }

            conn.SendResponse();
        }

        public void AddCharacter(NetConnection<NetSession> conn, Character character)
        {
            Log.InfoFormat("AddCharacter: Map:{0} characterId:{1}", this.Define.ID, character.Id);
            character.Info.mapId = this.ID;

            character.OnEnterMap(this);
            //告诉自己进入某地图
            if(this.MapCharacters.ContainsKey(character.Id))
                this.MapCharacters[character.Id] = new MapCharacter(conn, character);
        }

        internal void CharacterLeave(Character character)
        {
            Log.InfoFormat("CharacterLeave: Map:{0} characterId:{1}", this.Define.ID, character.Id);

            character.OnLeaveMap(this);

            //告知其他角色自己离开某地图
            foreach (var kv in this.MapCharacters)
            {
                this.SendCharacterLeaveMap(kv.Value.connection, character);
            }
            this.MapCharacters.Remove(character.Id);
        }

        internal void SendCharacterEnterMap(NetConnection<NetSession> conn, NCharacterInfo character)
        {
            //防止新建将原本的进入消息替换
            if (conn.Session.Response.mapCharacterEnter == null)
            {
                conn.Session.Response.mapCharacterEnter = new MapCharacterEnterResponse();
                conn.Session.Response.mapCharacterEnter.mapId = this.Define.ID;
            }
            conn.Session.Response.mapCharacterEnter.Characters.Add(character);
            conn.SendResponse();
        }

        private void SendCharacterLeaveMap(NetConnection<NetSession> conn, Character character)
        {
            Log.InfoFormat("SendCharacterLeaveMap To {0}:{1} : Map:{2} Character:{3}:{4}", conn.Session.Character.Id, conn.Session.Character.Info.Name, this.Define.ID, character.Id, character.Info.Name);
            conn.Session.Response.mapCharacterLeave = new MapCharacterLeaveResponse();
            conn.Session.Response.mapCharacterLeave.entityId = character.entityId;

            conn.SendResponse();
        }
        //更新自己、通知他人
        internal void UpdateEntity(NEntitySync entity)
        {
            foreach(var kv in this.MapCharacters)
            {
                if(kv.Value.character.entityId == entity.Id)
                {
                    kv.Value.character.Position = entity.Entity.Position;
                    kv.Value.character.Direction = entity.Entity.Direction;
                    kv.Value.character.Speed = entity.Entity.Speed;
                    //坐骑
                    if (entity.Event == EntityEvent.Ride)
                    {
                        kv.Value.character.Ride = entity.Param;
                    }
                }
                else
                {
                    MapService.Instance.SendEntityUpdate(kv.Value.connection, entity);
                }
            }
        }

        //通知他人怪物进入地图
        internal void MonsterEnter(Monster monster)
        {
            Log.InfoFormat("MonsterEnter: Map:{0} monsterId:{1}", this.Define.ID, monster.Id);
            monster.OnEnterMap(this);
            foreach (var kv in this.MapCharacters)
            {
                this.SendCharacterEnterMap(kv.Value.connection, monster.Info);
            }
        }

        internal void BroadcastBattleResponse(NetMessageResponse response)
        {
            //通知同地图所有角色
            foreach(var kv in this.MapCharacters)
            {
                if(response.skillCast!=null)
                    kv.Value.connection.Session.Response.skillCast = response.skillCast;
                if (response.skillHits != null)
                    kv.Value.connection.Session.Response.skillHits = response.skillHits;
                if (response.buffRes != null)
                    kv.Value.connection.Session.Response.buffRes = response.buffRes;
                kv.Value.connection.SendResponse();
            }
        }
    }
}
