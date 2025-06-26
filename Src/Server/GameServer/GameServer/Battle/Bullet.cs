using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Common;
using GameServer.Entities;
using SkillBridge.Message;

namespace GameServer.Battle
{
    class Bullet
    {
        private Skill skill;
        private Creature target;
        bool TimeMode = true;

        float flyTIme = 0;
        float duration = 0;
        NSkillHitInfo hitInfo;

        public bool Stoped = false;

        public Bullet(Skill skill, Creature target, NSkillHitInfo hitInfo)
        {
            this.skill = skill;
            this.target = target;
            this.hitInfo = hitInfo;
            int distance = skill.Owner.Distance(target);

            if(TimeMode)
            {
                //时间模式需要根据距离计算飞行时间
                duration = distance / this.skill.Define.BulletSpeed;
            }
            Log.InfoFormat("Bullet[{0}],CastBullet[{1}] Target:{2} Distance:{3} Time:{4}", this.skill.Define.Name, this.skill.Define.BulletResource,
                target.Name, distance, duration);
        }

        public void Update()
        {
            if(TimeMode)
            {
                this.UpdateTime();
            }
            else
            {
                this.UpdatePos();
            }
        }

        private void UpdateTime()
        {
            if (Stoped) return;
            this.flyTIme += TimeUtil.deltaTime;
            if(this.flyTIme > duration)
            {
                this.hitInfo.isBullet = true;
                this.skill.DoHit(this.hitInfo);
                this.Stoped = true;
            }
        }

        private void UpdatePos()
        {
            /*
            int distance = skill.Owner.Distance(target);
            if(distance > 50) 
            {
                pos += speed * TimeUtil.deltaTime;
            }
            else
            {
                this.hitInfo.isBullet = true;
                this.skill.DoHit(this.hitInfo);
                this.stoped = true;
            }*/
        }
    }
}