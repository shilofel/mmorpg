using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using GameServer.Entities;
using GameServer.Models;
using GameServer.Services;
using Network;
using SkillBridge.Message;

namespace GameServer.Managers
{
    //引入副本概念
    class StoryManager : Singleton<StoryManager>
    {
        public const int ManInstance = 100;

        Arena[] Arenas = new Arena[ManInstance];
        public class StoryMap
        {
            public Queue<int> InstanceIndexes = new Queue<int>();
            public Story[] Stories = new Story[ManInstance];
        }

        Dictionary<int, StoryMap> Stories = new Dictionary<int, StoryMap>();

        public void Init()
        {
            foreach (var story in DataManager.Instance.Stories)
            {
                StoryMap map = new StoryMap();
                for(int i=0;i<ManInstance;i++)
                {
                    map.InstanceIndexes.Enqueue(i);
                }
                this.Stories[story.Key] = map;
            }
        }

        public Story NewStory(int storyId, NetConnection<NetSession> owner)
        {
            var storyMap = DataManager.Instance.Stories[storyId].MapID;
            var instance = this.Stories[storyId].InstanceIndexes.Dequeue();
            var map = MapManager.Instance.GetInstance(storyMap, instance);
            Story story = new Story(map, storyId, instance, owner);
            this.Stories[storyId].Stories[instance] = story;
            story.PlayerEnter();
            return story;
        }

        internal void Update()
        {
        }

        public Story GetStory(int storyId,int instanceId)
        {
            return this.Stories[storyId].Stories[instanceId];
        }

        //public Arena NewArena(ArenaInfo info,NetConnection<NetSession> red,NetConnection<NetSession> blue)
        //{
        //    var instance = InstanceIndexes.Dequeue();
        //    var map = MapManager.Instance.GetInstance(ArenaMapId, instance);
        //    Arena arena = new Arena(map, info, red, blue);
        //    this.Arenas[instance] = arena;
        //    arena.PlayerEnter();
        //    return arena;
        //}
    }
}