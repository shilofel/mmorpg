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
            EntityManager.Instance.AddEntity(cha.MapID, character);
            this.Characters[cha.ID] = character;
            return character;
        }

        public void Clear()
        {
            Characters.Clear();
        }

        public void Remove(int characterID)
        {
            var cha = this.Characters[characterID];
            EntityManager.Instance.RemoveEntity(cha.Data.MapID, cha);
            Characters.Remove(characterID);
        }

    }
}