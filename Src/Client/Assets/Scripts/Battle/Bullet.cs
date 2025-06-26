using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Battle;
namespace Assets.Scripts.Battle
{
    class Bullet
    {
        private Skill skill;
        bool TimeMode = true;
        int hit = 0;

        float flyTIme = 0;
        float duration = 0;

        public bool Stoped = false;

        public Bullet(Skill skill)
        {
            this.skill = skill;
            var target = skill.Target;
            this.hit = skill.Hit;
            int distance = skill.Owner.Distance(target);
            duration = distance / this.skill.Define.BulletSpeed;
        }
        /*
                public void Update()
                {
                    if (TimeMode)
                    {
                        this.UpdateTime();
                    }
                    else
                    {
                        this.UpdatePos();
                    }
                }
                */

        public void Update()
        {
            if (Stoped) return;
            this.flyTIme += TimeUtil.deltaTime;
            if (this.flyTIme > duration)
            {
                this.skill.DoHitDamages(this.hit);
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