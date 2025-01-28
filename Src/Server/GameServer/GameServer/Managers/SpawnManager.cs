using System;
using System.Collections.Generic;
using Common;
using GameServer.Entities;
using GameServer.Models;
using SkillBridge.Message;

namespace GameServer.Managers
{
    class SpawnManager
    {
        private List<Spawner> Rules = new List<Spawner>();
        private Map Map;

        public void Init(Map map)
        {
            this.Map = map;
            if(DataManager.Instance.SpawnRules.ContainsKey(map.Define.ID))
            {
                foreach(var define in DataManager.Instance.SpawnRules[map.Define.ID].Values)
                {
                    this.Rules.Add(new Spawner(define, this.Map));
                }
            }
        }
        public void Update()
        {
            if (Rules.Count == 0)
                return;

            for (int i=0; i < Rules.Count;i++)
            {
                this.Rules[i].Update();
            }
        }
    }
}