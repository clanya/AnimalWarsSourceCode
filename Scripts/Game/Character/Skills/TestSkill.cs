using System;
using Game.Character.Models;
using UnityEngine;

namespace Game.Character.Skills
{
    [Serializable]
    public sealed class TestSkill : BaseSkill
    { 
        public TestSkill(SkillSetting skillSetting) : base(skillSetting) { }
        
        public override void Activated(BaseCharacter self, BaseCharacter target = null)
        {
            Debug.Log("TestSkill " + self.Param.SkillSetting.Amount);
            self.CurrentStatus.AddAttackPower(self.Param.SkillSetting.Amount);
        }
    }
}