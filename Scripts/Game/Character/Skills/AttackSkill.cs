using Game.Character.Models;
using UnityEngine;

namespace Game.Character.Skills
{
    public sealed class AttackSkill : BaseSkill
    {
        public AttackSkill(SkillSetting skillSetting) : base(skillSetting) { }

        public override void Activated(BaseCharacter self,BaseCharacter target)
        {
            if(target is null)
            {
                Debug.LogError("TargetCharacter is null");
            }
            
            var selfCurrentStatus = self.CurrentStatus;
            int amount = selfCurrentStatus.AttackPower.Value * skillSetting.Amount; 
            
            var attackType = self.Param.SkillSetting.AttackSkillType;
            switch (attackType)
            {
                case AttackType.Physical:
                {
                    int value = Mathf.Max(0,amount - target.CurrentStatus.DefensePower.Value);
                    target.TakeDamage(value);
                    break;
                }
                case AttackType.Magic:
                {
                    int value = Mathf.Max(0,amount - target.CurrentStatus.MagicDefensePower.Value);
                    target.TakeDamage(value);
                    break;
                }
                default:
                    Debug.LogError("Not exists AttackType");
                    break;
            }
        }
    }
}