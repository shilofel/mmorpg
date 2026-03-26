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
        public ArenaInfo ArenaInfo;
        
        // 是否可以操作
        public bool CanOperate { get; private set; } = false;

        public ArenaManager()
        {

        }

        public void Init()
        {

        }
        //进入时准备状态不准行动，可以考虑给予定身类的负面状态
        internal void EnterArena(ArenaInfo arenaInfo)
        {
            this.ArenaInfo = arenaInfo;
            Debug.LogFormat("ArenaManager EnterArena:{0}", this.ArenaInfo.ArenaId);
        }

        internal void ExitArena(ArenaInfo arenaInfo)
        {
            if (this.ArenaInfo != null)
            {
                Debug.LogFormat("ArenaManager ExitArena:{0}", this.ArenaInfo.ArenaId);
            }
            this.ArenaInfo = null;
            // 重置操作权限
            this.CanOperate = false;
        }
        internal void SendReady()
        {
            if (this.ArenaInfo != null)
            {
                Debug.LogFormat("ArenaManager SendReady:{0}",this.ArenaInfo.ArenaId);
                ArenaService.Instance.SendAreanChallengeRequest(this.ArenaInfo.ArenaId);
            }
        }

        internal void OnReady(int round,ArenaInfo arenaInfo)
        {
            if (this.ArenaInfo != null)
            {
                Debug.LogFormat("ArenaManager OnReady:{0} Round:{1}", this.ArenaInfo.ArenaId,round);
            }
            this.Round = round;
            // 倒计时期间不可操作
            this.CanOperate = false;
            if (UIArena.Instance != null)
                UIArena.Instance.ShowCountDown();
        }
        //解除角色锁定，能够自由行动并作战
        internal void OnRoundStart(int round, ArenaInfo arenaInfo)
        {
            if (this.ArenaInfo != null)
            {
                Debug.LogFormat("ArenaManager OnRoundStart:{0} Round:{1}", this.ArenaInfo.ArenaId, round);
            }
            // 回合开始，可以操作
            this.CanOperate = true;
            if (UIArena.Instance != null)
                UIArena.Instance.ShowRoundStart(round, arenaInfo);
        }
        //初始化角色至出生点
        internal void OnRoundEnd(int round, ArenaInfo arenaInfo)
        {
            if (this.ArenaInfo != null)
            {
                Debug.LogFormat("ArenaManager OnRoundEnd:{0} Round:{1}", this.ArenaInfo.ArenaId, round);
            }
            this.Round = round;
            if (UIArena.Instance != null)
                UIArena.Instance.ShowRoundResult(round, arenaInfo);
        }
    }
}
