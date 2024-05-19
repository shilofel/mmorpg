using System;
using System.Collections.Generic;
using Common;
using GameServer.Entities;
using SkillBridge.Message;

namespace GameServer.Managers
{
    class CharacterManage : Singleton<CharacterManage>
    {
        public Dictionary<int, Character> Characters = new Dictionary<int, Character>();
        public Character AddCharacter(TCharacter cha)
        {
            Character character = new Character(CharacterType.Player, cha);
            this.Characters[cha.ID] = character;
            return character;
        }

        public void Clear()
        {
            Characters.Clear();
        }

        public void Remove(int characterID)
        {
            Characters.Remove(characterID);
        }

    }
}