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
            view.SetPanelActive(true);
            await view.TextAnimationAsync(model.CurrentTurnPlayer.playerID, this.GetCancellationTokenOnDestroy());
            view.SetPanelActive(false);
        }

        private void Start()
        {
            model.ChangeTurnObservable.Subscribe(async x =>
            {
                view.SetPanelActive(true);
                await view.TextAnimationAsync(model.CurrentTurnPlayer.playerID, this.GetCancellationTokenOnDestroy());
                view.SetPanelActive(false);

            }).AddTo(this);
        }
    }
}