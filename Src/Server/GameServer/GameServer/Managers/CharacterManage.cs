using System;
using System.Collections.Generic;
using Common;
using GameServer.Entities;
using SkillBridge.Message;

namespace GameServer.Managers
{
    class CharacterManager : Singleton<CharacterManager>
    {
        public Dictionary<int, Character> Characters = new Dictionary<int, Character>();
        public Character AddCharacter(TCharacter cha)
        {
            Character character = new Character(CharacterType.Player, cha);
            EntityManager.Instance.AddEntity(cha.MapID, 0,character);
            character.Info.entityId = character.entityId;
            this.Characters[character.Id] = character;
            return character;
        }

        public void Clear()
        {
            Characters.Clear();
        }

        public void Remove(int characterID)
        {
            var cha = this.Characters[characterID];
            EntityManager.Instance.RemoveEntity(cha.Data.MapID, 0,cha);
            Characters.Remove(characterID);
        }

        public Character GetCharacter(int characterId)
        {
            Character character = null;
            this.Characters.TryGetValue(characterId, out character);
            return character;
        }
    }
}