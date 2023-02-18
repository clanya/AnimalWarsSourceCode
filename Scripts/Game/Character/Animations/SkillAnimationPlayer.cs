using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.BattleFlow;
using Game.BattleFlow.Character;
using Game.Character.Models;
using Game.Character.Skills;
using Game.Effects;
using Game.Util;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Character.Animations
{
    public sealed class SkillAnimationPlayer : MonoBehaviour
    {
        [Inject] private CharacterManager characterManager;
        [SerializeField] private EffectPlayer effectPlayer;
        
        
        public async UniTask PlaySkillAnimation(BaseCharacter user, CancellationToken token,BaseCharacter target = null)
        {
            var skill = user.Param.Skill;
            var animationPlayer = user.gameObject.GetComponent<CharacterAnimationPlayer>();
            switch (skill)
            {
                case Skill.Skill_00:
                    break;
                //Buff
                case Skill.Skill_01:
                    foreach (var targetCharacter in characterManager.AllCharacters.Where(x => x.PlayerID == user.PlayerID))
                    {
                        effectPlayer.PlayBuffEffect(targetCharacter.transform.position);
                    }
                    await animationPlayer.PlaySkillAnimation(token);
                    break;
                case Skill.Skill_11:
                    effectPlayer.PlayBuffEffect(user.transform.position);
                    break;
                case Skill.Skill_07:
                case Skill.Skill_10:
                    if (target is null)
                    {
                        Debug.LogError("Null Reference");
                        return;
                    }
                    effectPlayer.PlayBuffEffect(target.transform.position);
                    await animationPlayer.PlaySkillAnimation(token);
                    break;
                //AttackSkill
                case Skill.Skill_08:
                    await animationPlayer.PlayAttackAnimation(user.Param.Type,token,target.transform);
                    break;
                case Skill.Skill_02:
                case Skill.Skill_03:
                case Skill.Skill_09:
                    await animationPlayer.PlayAttackAnimation(user.Param.Type,token);
                    break;
                //Heal Skill
                case Skill.Skill_04:
                    effectPlayer.PlayHealEffect(user.transform.position);
                    await animationPlayer.PlaySkillAnimation(token);
                    break;

                case Skill.Skill_05:
                    if (target is null)
                    {
                        Debug.LogError("Null Reference");
                        return;
                    }
                    effectPlayer.PlayHealEffect(target.transform.position);
                    await animationPlayer.PlaySkillAnimation(token);
                    break;
                //Debuff
                case Skill.Skill_06:
                case Skill.Skill_12:
                    if (target is null)
                    {
                        Debug.LogError("Null Reference");
                        return;
                    }
                    effectPlayer.PlayDebuffEffect(target.transform.position);
                    await animationPlayer.PlaySkillAnimation(token);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}