using System;
using System.Collections.Generic;
using Game.Audio;
using Game.BattleFlow;
using Game.BattleFlow.Turn;
using Game.Character.Models;
using UniRx;
using UnityEngine;
using VContainer;

namespace Game.Character.Skills
{
    public sealed class BuffSkill : BaseSkill
    {
        private TurnManager turnManager;

        public BuffSkill(SkillSetting skillSetting,TurnManager turnManager) : base(skillSetting)
        {
            this.turnManager = turnManager;
        }
        
        /// <summary>
        /// ターゲットが複数の場合
        /// </summary>
        public void Activated(BaseCharacter self, IEnumerable<BaseCharacter> targets)
        {
            foreach (var target in targets)
            {
                Activated(self,target);
            }
        }
        public override void Activated(BaseCharacter self, BaseCharacter target)
        {
            //バフをかけるステータスの種類分ループする
            foreach (var statusType in skillSetting.BuffForStatusTypeList)
            {
                Buff(skillSetting.Amount,statusType,target.CurrentStatus);
            }
            
            turnManager.ChangeTurnObservable
                .Skip((int)skillSetting.ApplicableTurn)
                .Take(1)
                .Subscribe(_ =>
                {
                    foreach (var statusType in skillSetting.BuffForStatusTypeList)
                    {
                        //とりあえず。やっぱBuffとくっつけたほうがいいか？
                        Buff(-skillSetting.Amount, statusType, target.CurrentStatus);
                    }
                    Debug.Log("Clear Buff");
                },() => Debug.Log("Completeed"));
        }
        
        private void Buff(int amount, StatusType type,CharacterCurrentStatus status)
        {
            AudioPlayer.PlayPowerUpSE();
            switch (type)
            {
                case StatusType.Hp:
                    Debug.LogError("選択できません.HealSkillを使用してください");
                    break;
                case StatusType.Sp:
                    Debug.LogError("選択できません.HealSkillを使用してください");
                    break;
                case StatusType.AttackPower:
                    status.AddAttackPower(amount);
                    break;
                case StatusType.DefensePower:
                    status.AddDefensePower(amount);
                    break;
                case StatusType.MagicDefensePower:
                    status.AddMagicDefensePower(amount);
                    break;
                case StatusType.Speed:
                    status.AddSpeed(amount);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

    }
}