using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using VContainer;

namespace Game.BattleFlow.Turn
{
    public sealed class TurnPresenter : MonoBehaviour
    {
        [SerializeField] private TurnView view;
        [Inject] private TurnManager model;

        private void Awake()
        {
            model.InitObservable.Subscribe(TurnAnimation).AddTo(this);
        }

        private async void TurnAnimation(Unit _)
        {
            await view.TextAnimationAsync(model.CurrentTurnPlayer.playerID, this.GetCancellationTokenOnDestroy());
        }

        private void Start()
        {
            model.ChangeTurnObservable.Subscribe(async x =>
            {
                await view.TextAnimationAsync(model.CurrentTurnPlayer.playerID, this.GetCancellationTokenOnDestroy());
            }).AddTo(this);
        }
    }
}