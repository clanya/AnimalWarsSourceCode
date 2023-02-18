using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Game.BattleFlow;
using Game.BattleFlow.Character;
using Game.BattleFlow.Turn;
using Game.Character.Managers;
using Game.Character.Models;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using VContainer;

namespace Game.Character.Skills
{ 
    public class SkillManager
    {
        [Inject] private CharacterManager characterManager;
        [Inject] private TargetCharacterExplorer targetCharacterExplorer;
        [Inject] private TurnManager turnManager;
        //BaseSkillをかえしたほうがいい？　BaseCharacterのパラメータとしてBaseSkillのインスタンスを持つべき？
        //相互参照になりそう、Skillのメソッドの引数をCharacterParameterとうにすればいける？
        public void Execute(BaseCharacter user,BaseCharacter target = null)
        {
            var skillSetting = user.Param.SkillSetting;
            user.CurrentStatus.DecreaseSp(skillSetting.Cost);
            switch (user.Param.Skill)
            {
                case Skill.Skill_00:
                    var testSkill = new TestSkill(skillSetting);
                    testSkill.Activated(user);
                    break;
                case Skill.Skill_01:
                    var skill01 = new BuffSkill(skillSetting,turnManager);
                    skill01.Activated(user,characterManager.AllCharacters.Where(x => x.PlayerID == user.PlayerID));
                    break;
                case Skill.Skill_02:
                    var skill02 = new AttackSkill(skillSetting);
                    skill02.Activated(user,target);
                    break;
                case Skill.Skill_03:
                    var skill03 = new AttackSkill(skillSetting);
                    skill03.Activated(user,target);
                    break;
                case Skill.Skill_04:
                    var skill04 = new HealSkill(skillSetting);
                    skill04.Activated(user,user);
                    break;
                case Skill.Skill_05:
                    var skill05 = new HealSkill(skillSetting);
                    skill05.Activated(user,target);
                    break;
                case Skill.Skill_06:
                    var skill06 = new DebuffSkill(skillSetting,turnManager);
                    skill06.Activated(user,target);
                    break;
                case Skill.Skill_07:
                    var skill07 = new BuffSkill(skillSetting,turnManager);
                    skill07.Activated(user, target);
                    break;
                case Skill.Skill_08:
                    var skill08 = new AttackSkill(skillSetting);
                    skill08.Activated(user,target);
                    break;
                case Skill.Skill_09:
                    var skill09 = new AttackSkill(skillSetting);
                    skill09.Activated(user,target);
                    break;
                case Skill.Skill_10:
                    var speedBuffSkill = new BuffSkill(skillSetting,turnManager);
                    speedBuffSkill.Activated(user,user);
                    user.AttackAsync(target).Forget();
                    break;
                case Skill.Skill_11:
                    var skill11 = new BuffSkill(skillSetting,turnManager);
                    skill11.Activated(user,user);
                    break;
                case Skill.Skill_12:
                    var skill12 = new DebuffSkill(skillSetting,turnManager);
                    skill12.Activated(user,target);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// ターゲットの選択する時間が必要かどうか。　良い命名が思いつかなかった。
        /// </summary>
        public bool IsTargetSelectionRequired(TargetType type)
        {
            return type switch
            {
                TargetType.Self => false,
                TargetType.OneFriend => true,
                TargetType.OneEnemy => true,
                TargetType.AllFriends => false,
                TargetType.AllEnemies => false,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }

        private bool IsTargetCharacterExists(BaseCharacter character)
        {
            TargetType type = character.Param.SkillSetting.TargetType;
            Vector2Int selfPosition = character.Position;
            int skillTargetRange = character.Param.SkillSetting.TargetRange;
            return type switch
            {
                TargetType.Self => true,
                TargetType.OneFriend => targetCharacterExplorer.TargetCharacterExists(selfPosition,skillTargetRange,TargetPlayerID(character)),
                TargetType.OneEnemy => targetCharacterExplorer.TargetCharacterExists(selfPosition,skillTargetRange,TargetPlayerID(character)),
                TargetType.AllFriends => true,
                TargetType.AllEnemies => true,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }

        public PlayerID TargetPlayerID(BaseCharacter self)
        {
            var skillSetting = self.Param.SkillSetting;
            //仲間をターゲットとするとき
            if (skillSetting.TargetType is TargetType.OneFriend)
            {
                if (self.PlayerID == PlayerID.Player1)
                {
                    return PlayerID.Player1;
                }

                if (self.PlayerID == PlayerID.Player2)
                {
                    return PlayerID.Player2;
                }
            }
            //敵をターゲットとするとき
            if (skillSetting.TargetType is TargetType.OneEnemy)
            {
                if (self.PlayerID == PlayerID.Player1)
                {
                    return PlayerID.Player2;
                }

                if (self.PlayerID == PlayerID.Player2)
                {
                    return PlayerID.Player1;
                }
            }
            
            Debug.Log("Targetを必要としないスキルです。");
            return PlayerID.None;
        }

        public bool CanUseSkill(BaseCharacter character)
        {
            //IsActionableがtrueか
            if (character.IsActionable.Value == false)
            {
                return false;
            }
            
            var skillSetting = character.Param.SkillSetting;
            
            //コストが足りるか
            if (character.CurrentStatus.Sp.Value - skillSetting.Cost < 0)
            {
                return false;
            }
            //スキルのターゲットが存在するか
            return IsTargetCharacterExists(character);
        }
    }
}