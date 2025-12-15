using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Network;
using UnityEngine;

using SkillBridge.Message;
using Models;
using Managers;

namespace Assets.Scripts.Services
{
    class ArenaService:Singleton<ArenaService>,IDisposable
    {
        public ArenaService()
        {
            MessageDistributer.Instance.Subscribe<ArenaChallengeRequest>(this.OnArenaChallengeRequest);
            MessageDistributer.Instance.Subscribe<ArenaChallengeResponse>(this.OnArenaChallengeResponse);
            MessageDistributer.Instance.Subscribe<ArenaBeginResponse>(this.OnArenaBegin);
            MessageDistributer.Instance.Subscribe<ArenaEndResponse>(this.OnArenaEnd);
            MessageDistributer.Instance.Subscribe<ArenaReadyResponse>(this.OnArenaReady);
            MessageDistributer.Instance.Subscribe<ArenaRoundStartResponse>(this.OnArenaRoundStart);
            MessageDistributer.Instance.Subscribe<ArenaRoundEndResponse>(this.OnArenaRoundEnd);
        }


        internal void Init()
        {

        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<ArenaChallengeRequest>(this.OnArenaChallengeRequest);
            MessageDistributer.Instance.Unsubscribe<ArenaChallengeResponse>(this.OnArenaChallengeResponse);
            MessageDistributer.Instance.Unsubscribe<ArenaBeginResponse>(this.OnArenaBegin);
            MessageDistributer.Instance.Unsubscribe<ArenaEndResponse>(this.OnArenaEnd);
        }

        internal void SendAreanChallengeRequest(int targetId, string targetName)
        {
            Debug.LogFormat("SendAreanChallengeRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.arenaChallengeReq = new ArenaChallengeRequest();
            message.Request.arenaChallengeReq.ArenaInfo = new ArenaInfo();
            message.Request.arenaChallengeReq.ArenaInfo.Red = new ArenaPlayer()
            {
                EntityId = User.Instance.CurrentCharacterInfo.Id,
                Name = User.Instance.CurrentCharacterInfo.Name
            };
            message.Request.arenaChallengeReq.ArenaInfo.Blue = new ArenaPlayer()
            {
                EntityId = targetId,
                Name = targetName
            };
            NetClient.Instance.SendMessage(message);
        }

        internal void SendAreanChallengeResponse(bool accept, ArenaChallengeRequest requset)
        {
            Debug.LogFormat("SendAreanChallengeResponse");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.arenaChallengeRes = new ArenaChallengeResponse();
            message.Request.arenaChallengeRes.Result = accept? Result.Success:Result.Failed;
            message.Request.arenaChallengeRes.Errormsg = accept ? "" : "对方拒绝了请求";
            message.Request.arenaChallengeRes.ArenaInfo = requset.ArenaInfo;
            NetClient.Instance.SendMessage(message);
        }
        //发送挑战响应
        private void OnArenaChallengeRequest(object sender, ArenaChallengeRequest request)
        {
            Debug.LogFormat("OnArenaChallengeRequest");
            var confirm = MessageBox.Show(string.Format("{0}邀请你竞技场对战", request.ArenaInfo.Red.Name), "竞技请求", MessageBoxType.Confirm, "接受", "拒绝");

            confirm.OnYes = () =>
            {
                this.SendAreanChallengeResponse(true, request);
            };
            confirm.OnNo = () =>
            {
                this.SendAreanChallengeResponse(false, request);
            };
        }

        private void OnArenaChallengeResponse(object sender, ArenaChallengeResponse message)
        {
            Debug.LogFormat("OnArenaChallengeResponse");

            if (message.Result == Result.Failed)
            {
                MessageBox.Show(message.Errormsg, "对方拒绝请求");
            }
        }
        //初始竞技开始获得竞技信息
        private void OnArenaBegin(object sender, ArenaBeginResponse message)
        {
            Debug.LogFormat("OnArenaBegin");

            ArenaManager.Instance.EnterArena(message.ArenaInfo);
        }

        private void OnArenaEnd(object sender, ArenaEndResponse message)
        {
            Debug.LogFormat("OnArenaEnd");

            ArenaManager.Instance.ExitArena(message.ArenaInfo);
        }

        internal void SendAreanChallengeRequest(int arenaId)
        {
            Debug.LogFormat("SendAreanChallengeRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.arenaReady = new ArenaReadyRequest();
            message.Request.arenaReady.entityId = User.Instance.CurrentCharacter.entityId;
            message.Request.arenaReady.arenaId = arenaId;
            NetClient.Instance.SendMessage(message);
        }
        private void OnArenaReady(object sender, ArenaReadyResponse message)
        {
            ArenaManager.Instance.OnReady(message.Round,message.ArenaInfo);
        }

        private void OnArenaRoundStart(object sender, ArenaRoundStartResponse message)
        {
            ArenaManager.Instance.OnRoundStart(message.Round, message.ArenaInfo);
        }

        private void OnArenaRoundEnd(object sender, ArenaRoundEndResponse message)
        {
            ArenaManager.Instance.OnRoundEnd(message.Round, message.ArenaInfo);
        }
    }
}