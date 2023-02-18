using System;
using Data.DataTable;
using Game.Character.Models;
using Game.Character.View;
using Game.Teams.Edit;
using UniRx;

namespace Game.BattleFlow.Character
{
    public sealed class CharacterStatusObservable : IDisposable
    {
        private readonly CharacterStatusView view;
        private CompositeDisposable compositeDisposable = new();
        public CharacterStatusObservable(CharacterStatusView view)
        {
            this.view = view;
        }

        //キャラクターが攻撃しているときなどに、ステータスUIの値をリアルタイムで変更させるために用意する。
        public void Observable(BaseCharacter character,EditViewParameterDataTable characterInfoTable,CharacterImageTable imageTable)
        {
            var characterType = character.Param.Type;
            var characterInfo = characterInfoTable.GetViewData(characterType);
            view.SetNameText(characterInfo.Name);
            view.SetSprite(imageTable.GetCharacterSprite(characterType));
            view.SetAttackType(character.Param.AttackType);
            view.SetSkillExplainText(characterInfo.SkillExplain);
            
            var param = character.Param;
            
            //各パラメータのReactivePropertyを購読する。
            character.CurrentStatus.Hp
                .Subscribe(value => view.SetHpText(value,param.Hp)).AddTo(compositeDisposable);
            
            character.CurrentStatus.Sp
                .Subscribe(value => view.SetSpText(value, param.Sp)).AddTo(compositeDisposable);

            character.CurrentStatus.AttackPower
                .Subscribe(value => view.SetAttackPowerText(value,param.AttackPower)).AddTo(compositeDisposable);

            character.CurrentStatus.DefensePower
                .Subscribe(value => view.SetDefensePowerText(value, param.DefensePower)).AddTo(compositeDisposable);

            character.CurrentStatus.MagicDefensePower
                .Subscribe(value => view.SetMagicDefensePowerText(value, param.MagicDefensePower)).AddTo(compositeDisposable);

            character.CurrentStatus.Speed
                .Subscribe(value => view.SetSpeed(value, param.Speed)).AddTo(compositeDisposable);

            character.CurrentStatus.MovableRange
                .Subscribe(value => view.SetMovableRange(value, param.MovableRange)).AddTo(compositeDisposable);
        }

        public void Dispose()
        {
            compositeDisposable.Dispose();
        }
    }
}