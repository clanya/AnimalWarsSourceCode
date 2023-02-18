using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Game.BattleFlow.Character;
using UniRx;
using UnityEngine;

namespace Game.BattleFlow
{
    public sealed class StageEffectPresenter : MonoBehaviour
    {
        [SerializeField] private CharacterManager characterManager;
        private readonly IObservable<Unit> dodgeAttackObservable = new Subject<Unit>();
        [SerializeField] private StageEffectView stageEffectView;
        
        public void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            stageEffectView.SetPanelActive(false);
            dodgeAttackObservable.Merge(characterManager.AllCharacters.Select(x => x.DodgeAttackObservable).ToArray()).Subscribe(async _ =>
            {
                stageEffectView.SetPanelActive(true);
                await stageEffectView.TextAnimationAsync(this.GetCancellationTokenOnDestroy());
                stageEffectView.SetPanelActive(false);
            }).AddTo(this);
        }
    }
}