using System;
using System.Collections.Generic;
using Common;
using GameServer.Entities;
using GameServer.Models;
using SkillBridge.Message;

namespace GameServer.Managers
{
    class StatusManager
    {
        Character Owner;
        private List<NStatus> Status { get; set; }

        public bool HasStatus
        {
            get { return this.Status.Count > 0; }
        }

        public StatusManager(Character owner)
        {
            this.Owner = owner;
            this.Status = new List<NStatus>();
        }

        public void AddStatus(StatusType type, int id, int value, StatusAction action)
        {
            this.Status.Add(new NStatus()
            {
                Type = type,
                Id = id,
                Action = action,
                Value = value
            });
        }

        public void AddGoldChange(int goldDelta)
        {
            if(goldDelta>0)
            {
                this.AddStatus(StatusType.Money, 0, goldDelta, StatusAction.Add);
            }
            else if(goldDelta<0)
            {
                this.AddStatus(StatusType.Money, 0, -goldDelta, StatusAction.Delete);
            }
        }

        public void AddExpChange(int expDelta)
        {
            this.AddStatus(StatusType.Exp, 0, expDelta, StatusAction.Add);
        }

        public void AddLevelUp(int levelDelta)
        {
            this.AddStatus(StatusType.Level, 0, levelDelta, StatusAction.Add);
        }

        public void AddItemChange(int id,int count,StatusAction action)
        {
            this.AddStatus(StatusType.Item, id, count, action);
        }

        public void PostProcess(NetMessageResponse response)
        {
            if (response.statusNotify == null)
                response.statusNotify = new StatusNotify();
            foreach(var status in this.Status)
            {
                response.statusNotify.Status.Add(status);
            }
            this.Status.Clear();
        }
    }

}