using System;
using Game.Audio;
using Game.BattleFlow;
using Game.BattleFlow.Turn;
using Game.Character.Models;
using UniRx;
using UnityEngine;
using VContainer;

namespace Game.Character.Skills
{
    public sealed class DebuffSkill : BaseSkill
    {
        private TurnManager turnManager;

        public DebuffSkill(SkillSetting skillSetting, TurnManager turnManager) : base(skillSetting)
        {
            this.turnManager = turnManager;
        }

        public override void Activated(BaseCharacter self, BaseCharacter target)
        {
            foreach (var statusType in skillSetting.BuffForStatusTypeList)
            {
                Debuff(skillSetting.Amount,statusType,target.CurrentStatus);
            }

            turnManager.ChangeTurnObservable
                .Skip((int)skillSetting.ApplicableTurn)
                .Take(1)
                .Subscribe(_ =>
                {
                    foreach (var statusType in skillSetting.BuffForStatusTypeList)
                    {
                        //とりあえず。やっぱBuffとくっつけたほうがいいか？
                        Debuff(-skillSetting.Amount, statusType, target.CurrentStatus);
                    }

                    Debug.Log("Clear Debuff");
                },() => Debug.Log("Debuff Completeed"));
            
            
        }
        
        private void Debuff(int amount, StatusType type,CharacterCurrentStatus status)
        {
            AudioPlayer.PlayPowerDownSE();
            switch (type)
            {
                case StatusType.Hp:
                    status.DecreaseHp(amount);
                    break;
                case StatusType.Sp:
                    status.DecreaseSp(amount);
                    break;
                case StatusType.AttackPower:
                    status.DecreaseAttackPower(amount);
                    break;
                case StatusType.DefensePower:
                    status.DecreaseDefensePower(amount);
                    break;
                case StatusType.MagicDefensePower:
                    status.DecreaseMagicDefensePower(amount);
                    break;
                case StatusType.Speed:
                    status.DecreaseSpeed(amount);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

    }
}