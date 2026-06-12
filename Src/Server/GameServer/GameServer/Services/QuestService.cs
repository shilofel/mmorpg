using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Network;
using SkillBridge.Message;
using GameServer.Entities;
using GameServer.Managers;
using Common.Data;

namespace GameServer.Services
{
    class QuestService:Singleton<QuestService>
    {
        public QuestService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<QuestSubmitRequest>(this.OnQuestSubmit);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<QuestAcceptRequest>(this.OnQuestAccept);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<QuestAbandonRequest>(this.OnQuestAbandon);
        }

        public void Init()
        {

        }

        private void OnQuestAccept(NetConnection<NetSession> sender, QuestAcceptRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("QuestAcceptRequest::character:{0};QuestId:{1}", character.Id, request.QuestId);

            sender.Session.Response.questAccept = new QuestAcceptResponse();

            Result result = character.QuestManager.AcceptQuest(sender, request.QuestId);
            sender.Session.Response.questAccept.Result = result;
            sender.SendResponse();
        }

        private void OnQuestSubmit(NetConnection<NetSession> sender, QuestSubmitRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("QuestSubmitRequest::character:{0};QuestId:{1}", character.Id, request.QuestId);

            sender.Session.Response.questSubmit = new QuestSubmitResponse();

            Result result = character.QuestManager.SubmitQuest(sender, request.QuestId);
            sender.Session.Response.questSubmit.Result = result;
            sender.SendResponse();
        }

        private void OnQuestAbandon(NetConnection<NetSession> sender, QuestAbandonRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("QuestAbandonRequest::character:{0};QuestId:{1}", character.Id, request.QuestId);

            sender.Session.Response.questAbandon = new QuestAbandonResponse();

            Result result = character.QuestManager.AbandonQuest(sender, request.QuestId);
            sender.Session.Response.questAbandon.Result = result;
            sender.SendResponse();
        }
    }
}