using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Network;
using UnityEngine;
using UnityEngine.Events;

using Entities;
using SkillBridge.Message;
using Managers;
using Models;

namespace Services
{
    class CharacterManager : Singleton<CharacterManager>, IDisposable
    {
        public Dictionary<int, Character> Characters = new Dictionary<int, Character>();


        public UnityAction<Character> OnCharacterEnter;
        public UnityAction<Character> OnCharacterLeave;

        public CharacterManager()
        {

        }

        public void Dispose()
        {
        }

        public void Init()
        {

        }

        public void Clear()
        {
            //int[] keys = this.Characters.Keys.ToArray();
            //通知其他管理器
           // foreach(var key in keys)
            //{
                //this.RemoveCharacter(key);
            //}
            this.Characters.Clear();
        }

        public void AddCharacter(SkillBridge.Message.NCharacterInfo cha)
        {
            Debug.LogFormat("AddCharacter:{0}:{1} Map:{2} Entity:{3}", cha.Id, cha.Name, cha.mapId, cha.Entity.String());
            Character character = new Character(cha);
            this.Characters[cha.entityId] = character;
            EntieyManager.Instance.AddEntity(character);

            if(OnCharacterEnter!=null)
            {
                OnCharacterEnter(character);
            }
            //管理场景中所有角色，是当前palyer赋值
            if (cha.entityId == User.Instance.CurrentCharacterInfo.entityId)
                User.Instance.CurrentCharacter = character;
        }


        public void RemoveCharacter(int entityId)
        {
            Debug.LogFormat("RemoveCharacter:{0}", entityId);

            if(Characters.ContainsKey(entityId))
            {
                EntieyManager.Instance.RemoveEntity(this.Characters[entityId].Info.Entity);
                if(OnCharacterLeave !=null)
                {
                    OnCharacterLeave(this.Characters[entityId]);
                }
                this.Characters.Remove(entityId);
            }
        }

        public Character GetCharacter(int id)
        {
            Character cha;
            this.Characters.TryGetValue(id, out cha);
            return cha;
        }
    }
}
