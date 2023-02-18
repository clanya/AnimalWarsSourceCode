using Game.Audio;
using Game.Character.Models;

namespace Game.Character.Skills
{
    public sealed class HealSkill : BaseSkill
    {
        public HealSkill(SkillSetting skillSetting) : base(skillSetting) { }

        public override void Activated(BaseCharacter self, BaseCharacter target)
        {
            AudioPlayer.PlayHealSE();
            target.TakeHeal(skillSetting.Amount);
        }
    }
}