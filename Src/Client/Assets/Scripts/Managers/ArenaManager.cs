using Assets.Scripts.Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Managers
{
    class ArenaManager : Singleton<ArenaManager>
    {
        public int Round = 0;
        ArenaInfo ArenaInfo;

        public ArenaManager()
        {

        }
        //进入时准备状态不准行动，可以考虑给予定身类的负面状态
        internal void EnterArena(ArenaInfo arenaInfo)
        {
            Debug.LogFormat("ArenaManager EnterArena:{0}", this.ArenaInfo.ArenaId);
            this.ArenaInfo = arenaInfo;
        }

        internal void ExitArena(ArenaInfo arenaInfo)
        {
            Debug.LogFormat("ArenaManager ExitArena:{0}", this.ArenaInfo.ArenaId);
            this.ArenaInfo = null;
        }
        internal void SendReady()
        {
            Debug.LogFormat("ArenaManager SendReady:{0}",this.ArenaInfo.ArenaId);
            ArenaService.Instance.SendAreanChallengeRequest(this.ArenaInfo.ArenaId);
        }

        internal void OnReady(int round,ArenaInfo arenaInfo)
        {
            Debug.LogFormat("ArenaManager OnReady:{0} Round:{1}", this.ArenaInfo.ArenaId,round);
            this.Round = round;
            if (UIArena.Instance != null)
                UIArena.Instance.ShowCountDown();
        }
        //解除角色锁定，能够自由行动并作战
        internal void OnRoundStart(int round, ArenaInfo arenaInfo)
        {
            Debug.LogFormat("ArenaManager OnRoundStart:{0} Round:{1}", this.ArenaInfo.ArenaId, round);
            if (UIArena.Instance != null)
                UIArena.Instance.ShowRoundStart(round, arenaInfo);
        }
        //初始化角色至出生点
        internal void OnRoundEnd(int round, ArenaInfo arenaInfo)
        {
            Debug.LogFormat("ArenaManager OnRoundEnd:{0} Round:{1}", this.ArenaInfo.ArenaId, round);
            this.Round = round;
            if (UIArena.Instance != null)
                UIArena.Instance.ShowRoundResult(round, arenaInfo);
        }
    }
}
