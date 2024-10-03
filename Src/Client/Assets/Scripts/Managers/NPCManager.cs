using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Data;

namespace Managers
{
    class NPCManager : Singleton<NPCManager>
    {

        public delegate bool NPCACtionHandler(NPCDefine npc);

        Dictionary<NPCFunction, NPCACtionHandler> eventMap = new Dictionary<NPCFunction, NPCACtionHandler>();

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
            if(npc.Type == NPCType.Task)
            {
                return DoTaskInteractive(npc);
            }
            else if (npc.Type == NPCType.Functional)
            {
                return DoFunctionInteractive(npc);
            }
            return false;
        }

        private bool DoTaskInteractive(NPCDefine npc)
        {
            MessageBox.Show("点击了NPC:" + npc.Name, "NPC对话");
            return true;
        }

        private bool DoFunctionInteractive(NPCDefine npc)
        {
            if (npc.Type != NPCType.Functional)
                return false;
            if (!eventMap.ContainsKey(npc.Function))
                return false;
            return eventMap[npc.Function](npc);
        }
    }
}
