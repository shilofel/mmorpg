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

namespace Services
{
    class StatusService : Singleton<StatusService>, IDisposable
    {
        public delegate bool StatusNotifyHandler(NStatus status);

        Dictionary<StatusType, StatusNotifyHandler> eventMap = new Dictionary<StatusType, StatusNotifyHandler>();
        HashSet<StatusNotifyHandler> notifyHandlers = new HashSet<StatusNotifyHandler>();

        public void Init()
        {

        }

        //防止角色进入，重复注册事件
        public void RegisterStatusNotify(StatusType function, StatusNotifyHandler action)
        {
            if (notifyHandlers.Contains(action))
                return;
            if (!eventMap.ContainsKey(function))
            {
                eventMap[function] = action;
            }
            else
                eventMap[function] += action;
            notifyHandlers.Add(action);
        }
        public StatusService()
        {
            MessageDistributer.Instance.Subscribe<StatusNotify>(this.OnStatusNotify);
        }

        private void OnStatusNotify(object sender, StatusNotify notify)
        {
            if (notify == null)
            {
                Debug.LogWarning("OnStatusNotify called with null notify");
                return;
            }
            if (notify.Status == null)
            {
                Debug.LogWarning("OnStatusNotify called with null Status list");
                return;
            }
            foreach( NStatus status in notify.Status)
            {
                Notify(status);
            }
        }

        private void Notify(NStatus status)
        {
            if (status == null)
            {
                Debug.LogWarning("Notify called with null status");
                return;
            }
            Debug.LogFormat("StatusNotify:[{0}][{1}]{2}:{3}",status.Type,status.Action,status.Id,status.Value);

            if(status.Type == StatusType.Money)
            {
                if (User.Instance == null)
                {
                    Debug.LogWarning("User.Instance is null when processing Money status");
                    return;
                }
                if (User.Instance.CurrentCharacterInfo == null)
                {
                    Debug.LogWarning("User.Instance.CurrentCharacterInfo is null when processing Money status");
                    return;
                }
                if (status.Action == StatusAction.Add)
                    User.Instance.AddGold(status.Value);
                else if (status.Action == StatusAction.Delete)
                    User.Instance.AddGold(-status.Value);
            }

            StatusNotifyHandler handler;
            if (eventMap.TryGetValue(status.Type, out handler) && handler != null)
            {
                try
                {
                    handler(status);
                }
                catch (Exception ex)
                {
                    Debug.LogErrorFormat("StatusNotifyHandler exception: {0}, {1}", ex.Message, ex.StackTrace);
                }
            }
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<StatusNotify>(this.OnStatusNotify);
        }

    }
}
