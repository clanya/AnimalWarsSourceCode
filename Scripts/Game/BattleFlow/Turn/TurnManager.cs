using System;
using System.Collections.Generic;
using System.Linq;
using Game.Character;
using Game.Character.Models;
using UniRx;
using UnityEngine;

namespace Game.BattleFlow.Turn
{
    public sealed class TurnManager
    {
        private IReadOnlyList<Player> playerList;
        private Subject<Player> changeTurnSubject;
        public IObservable<Player> ChangeTurnObservable => changeTurnSubject;
        public Player CurrentTurnPlayer { get; private set; }
        
        private readonly ReactiveProperty<Player> result = new(null);
        public IReadOnlyReactiveProperty<Player> Result => result;

        private readonly ISubject<Unit> initSubject = new Subject<Unit>();
        public IObservable<Unit> InitObservable => initSubject;

        public void Init(IReadOnlyList<Player> playerList)
        {
            this.playerList = playerList;
            changeTurnSubject = new Subject<Player>();
            ChangeTurnObservable.Subscribe(_ => { Debug.Log("Change Turn"); });
            CurrentTurnPlayer = playerList.First();

            initSubject.OnNext(Unit.Default);
            initSubject.OnCompleted();
        }
        
        /// <summary>
        /// ターンをチェンジできるかチェックする。
        /// </summary>
        public void CheckSwapTurn()
        {
            //攻撃中のプレイヤーのキャラクターの中に行動可能なキャラクターがいるか？
            var isActionableExists = CurrentTurnPlayer.characterList.Where(x => x.IsDead.Value == false)
                                                             .Any(x => x.IsActionable.Value);
            //いた場合、何もせずreturn.
            if(isActionableExists)
            {
                return;
            }
            //いなかった場合、ターンをチェンジする。
            SwapTurn();
        }
        
        /// <summary>
        /// プレイヤーのキャラクター達が全滅していないか確認
        /// </summary>
        public void CheckResult()
        {
            var notCurrentPlayer = playerList.Single(x => x != CurrentTurnPlayer);  //攻撃された側のプレイヤーを取得
            if (notCurrentPlayer.characterList.All(x => x.IsDead.Value))      //攻撃された側のプレイヤーのキャラクターが全滅しているか？
            {
                result.Value = CurrentTurnPlayer;
            }
        }
        
        /// <summary>
        /// 攻守交替する。ターンをチェンジする。
        /// </summary>
        private void SwapTurn()
        {
            if (playerList.Count != 2)
            {
                Debug.LogError("Count is not 2!!");
                return;
            }
            
            CurrentTurnPlayer = playerList.First(x => x != CurrentTurnPlayer);

            foreach (var character in CurrentTurnPlayer.characterList)
            {
                character.SetProperty(true, CharacterViewState.Normal);
            }
            changeTurnSubject.OnNext(CurrentTurnPlayer);
        }
        
        public void OnDispose()
        {
            changeTurnSubject.Dispose();
            result.Dispose();
        }
        
        public sealed class Player
        {
            public readonly PlayerID playerID;
            public readonly IReadOnlyList<BaseCharacter> characterList;

            public Player(IReadOnlyList<BaseCharacter> characterList, PlayerID playerID)
            {
                this.characterList = characterList;
                this.playerID = playerID;
            }
        }
    }
}

