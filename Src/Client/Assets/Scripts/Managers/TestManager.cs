using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Data;
using UnityEngine;

namespace Managers
{
    class TestManager:Singleton<TestManager>
    {
        public void Init()
        {
            NPCManager.Instance.RegisterNPCEvent(Common.Data.NPCFunction.InvokeShop, OnNPInvokeShop);
            NPCManager.Instance.RegisterNPCEvent(Common.Data.NPCFunction.InvokeInsrance, OnInvokeInsrance);
        }

        private bool OnInvokeInsrance(NPCDefine npc)
        {
            Debug.LogFormat("TestManager.OnInvokeInsrnce NPC:[{0}:{1}] Type:{2} Func:{3}", npc.ID, npc.Name, npc.Type, npc.Function);
            MessageBox.Show("点击了NPC" + npc.Name, "NPC对话");
            return true;
        }

        private bool OnNPInvokeShop(NPCDefine npc)
        {
            Debug.LogFormat("TestManager.OnNPInvokeShop NPC:[{0}:{1}] Type:{2} Func:{3}", npc.ID, npc.Name, npc.Type, npc.Function);
            UITest test = UIManager.Instance.Show<UITest>();
            test.SetTitle(npc.Name);
            return true;
        }
    }
}
