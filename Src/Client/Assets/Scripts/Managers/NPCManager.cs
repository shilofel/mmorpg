using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Data;
using UnityEngine;

namespace Managers
{
    class NPCManager : Singleton<NPCManager>
    {

        public delegate bool NPCACtionHandler(NPCDefine npc);

        Dictionary<NPCFunction, NPCACtionHandler> eventMap = new Dictionary<NPCFunction, NPCACtionHandler>();
        Dictionary<int, Vector3> npcPositions = new Dictionary<int, Vector3>();

        public void RegisterNPCEvent(NPCFunction function,NPCACtionHandler action)
        {
            if (!eventMap.ContainsKey(function))
            {
                eventMap[function] = action;
            }
            else
                eventMap[function] += action;
        }

        public NPCDefine GetNPCDefine(int npcID)
        {
            NPCDefine npc = null;
            DataManager.Instance.NPCs.TryGetValue(npcID, out npc);
            return npc;
        }
        //交互，根据类型执行不同交互
        public bool Interative(int npcID)
        {
            if(DataManager.Instance.NPCs.ContainsKey(npcID))
            {
                var npc = DataManager.Instance.NPCs[npcID];
                return Interative(npc);
            }
            return false;
        }

        public bool Interative(NPCDefine npc)
        {
            if (DoTaskInteractive(npc))
            {
                return true;
            }
            else if (npc.Type == NPCType.Functional)
            {
                return DoFunctionInteractive(npc);
            }
            return false;
        }

        private bool DoTaskInteractive(NPCDefine npc)
        {
            var status = QuestManager.Instance.GetQuestStatusByNpc(npc.ID);
            if (status == NpcQuestStatus.None)
                return false;
            return QuestManager.Instance.OpenNpcQuest(npc.ID);
        }

        private bool DoFunctionInteractive(NPCDefine npc)
        {
            if (npc.Type != NPCType.Functional)
                return false;
            if (!eventMap.ContainsKey(npc.Function))
                return false;
            return eventMap[npc.Function](npc);
        }

        internal void UpdateNpcPosition(int npc,Vector3 pos)
        {
            this.npcPositions[npc] = pos;
        }

        public Vector3 GetNpcPosition(int npc)
        {
            return this.npcPositions[npc];
        }
    }
}
