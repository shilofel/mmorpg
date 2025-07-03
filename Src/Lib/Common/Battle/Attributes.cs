using Common.Data;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Battle
{
    public class Attributes
    {
        //初始属性
        AttributeData Initial = new AttributeData();
        //成长属性
        AttributeData Growth = new AttributeData();
        //装备属性
        AttributeData Equip = new AttributeData();
        //基本属性——初始 成长 装备合计值 
        public AttributeData Basic = new AttributeData();
        //Buff属性
        public AttributeData Buff = new AttributeData();
        //最终属性
        public AttributeData Final = new AttributeData();

        int Level;

        public NAttributeDynamic DynamicAttr;

        public float HP
        {
            get { return DynamicAttr.Hp; }
            set { DynamicAttr.Hp = (int)Math.Min(MaxHP, value); }
        }

        public float MP
        {
            get { return DynamicAttr.Mp; }
            set { DynamicAttr.Mp = (int)Math.Min(MaxMP, value); }
        }

        public float MaxHP { get { return Final.MaxHP; } }
        public float MaxMP { get { return Final.MaxMP; } }
        public float STR { get { return Final.STR; } }
        public float INT { get { return Final.INT; } }
        public float DEX { get { return Final.DEX; } }
        public float AD { get { return Final.AD; } }
        public float AP { get { return Final.AP; } }
        public float DEF { get { return Final.DEF; } }
        public float MDEF { get { return Final.MDEF; } }
        public float SPD { get { return Final.SPD; } }
        public float CRI { get { return Final.CRI; } }

        public void Init(CharacterDefine define,int level,List<EquipDefine> equips,NAttributeDynamic dynamicAttr)
        {
            this.DynamicAttr = dynamicAttr;
            this.LoadInitAttrbute(this.Initial, define);
            this.LoadGrowAttrbute(this.Growth, define);
            this.LoadEquipAttrbute(this.Equip, equips);
            this.Level = level;
            this.InitBasicAttributes();
            this.InitSecondaryAttributes();

            this.InitFinalAttributes();
            if (this.DynamicAttr == null)
            {
                this.DynamicAttr = new NAttributeDynamic();
                this.HP = this.MaxHP;
                this.MP = this.MaxMP;
            }
            else
            {
                this.HP = this.DynamicAttr.Hp;
                this.MP = this.DynamicAttr.Mp;
            }
        }

        private void InitBasicAttributes()
        {
            for (int i = (int)AttributeType.MaxHP; i < (int)AttributeType.MAX; i++) 
            {
                this.Basic.Data[i] = this.Initial.Data[i];
            }
            //计算一级属性合计值
            for (int i = (int)AttributeType.STR; i < (int)AttributeType.DEX; i++)
            {
                this.Basic.Data[i] = this.Initial.Data[i] + this.Growth.Data[i] * (this.Level - 1);
                this.Basic.Data[i] += this.Equip.Data[i];
            }
        }
        //计算basic二级属性合计值
        private void InitSecondaryAttributes()
        {
            this.Basic.MaxHP = this.Basic.STR * 10 + this.Initial.MaxHP + this.Equip.MaxHP;
            this.Basic.MaxMP = this.Basic.INT * 10 + this.Initial.MaxMP + this.Equip.MaxMP;

            this.Basic.AD = this.Basic.STR * 5 + this.Initial.AD + this.Equip.AD;
            this.Basic.AP = this.Basic.INT * 5 + this.Initial.AP + this.Equip.AP;
            this.Basic.DEF = this.Basic.STR * 2 + this.Initial.DEX * 1 + this.Initial.DEF + this.Equip.DEF;
            this.Basic.MDEF = this.Basic.INT * 2 + this.Initial.DEX * 1 + this.Initial.MDEF + this.Equip.MDEF;

            this.Basic.SPD = this.Basic.DEX * 0.2f + this.Initial.SPD + this.Equip.SPD;
            this.Basic.CRI = this.Basic.DEX * 0.0002f + this.Initial.CRI + this.Equip.CRI;
        }

        public void InitFinalAttributes()
        {
            for (int i = (int)AttributeType.MaxHP; i < (int)AttributeType.MAX; i++)
            {
                this.Final.Data[i] = this.Basic.Data[i] + this.Buff.Data[i];
            }
        }

        private void LoadInitAttrbute(AttributeData attr, CharacterDefine define)
        {
            attr.MaxHP = define.MaxHP;
            attr.MaxMP = define.MaxMP;
            attr.STR = define.STR;
            attr.INT = define.INT;
            attr.DEX = define.DEX;
            attr.AD = define.AD;
            attr.AP = define.AP;
            attr.DEF = define.DEF;
            attr.MDEF = define.MDEF;
            attr.SPD = define.SPD;
            attr.CRI = define.CRI;
        }

        private void LoadGrowAttrbute(AttributeData attr, CharacterDefine define)
        {
            attr.STR = define.GrowthSTR;
            attr.INT = define.GrowthINT;
            attr.DEX = define.GrowthDEX;
        }

        private void LoadEquipAttrbute(AttributeData attr, List<EquipDefine> equips)
        {
            attr.Reset();
            if (equips == null) return;
            foreach(var define in equips)
            {
                attr.MaxHP = define.MaxHP;
                attr.MaxMP = define.MaxMP;
                attr.STR = define.STR;
                attr.INT = define.INT;
                attr.DEX = define.DEX;
                attr.AD = define.AD;
                attr.AP = define.AP;
                attr.DEF = define.DEF;
                attr.MDEF = define.MDEF;
                attr.SPD = define.SPD;
                attr.CRI = define.CRI;
            }
        }
    }
}
