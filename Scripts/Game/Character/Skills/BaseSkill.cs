using System;
using Game.Character.Models;

namespace Game.Character.Skills
{
    public abstract class BaseSkill
    {
        protected SkillSetting skillSetting;

        protected BaseSkill(SkillSetting skillSetting)
        {
            this.skillSetting = skillSetting;
        }

        public virtual void Activated(BaseCharacter self, BaseCharacter target)
        {
            self.SetIsActionable(false);
        }
    }
}