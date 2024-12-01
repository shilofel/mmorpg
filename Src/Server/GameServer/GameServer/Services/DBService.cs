using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Common;

namespace GameServer.Services
{
    class DBService : Singleton<DBService>
    {
        ExtremeWorldEntities entities;

        public ExtremeWorldEntities Entities
        {
            get { return this.entities; }
        }

        public void Init()
        {
            entities = new ExtremeWorldEntities();
        }
        //设定间隔存储，缓解服务器压力
        //float time = 0;
        public void Save(bool async = false)
        {
            //DateTime.Now.Ticks - time > xxx;
            if (async)
                entities.SaveChangesAsync();
            else
                entities.SaveChanges();
        }
    }
}
