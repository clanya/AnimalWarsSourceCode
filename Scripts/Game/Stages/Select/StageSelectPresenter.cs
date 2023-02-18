using VContainer;
using VContainer.Unity;
using UniRx;
using Game.Stages.Managers;
using Cysharp.Threading.Tasks;
using Game.Audio;

namespace Game.Stages.Select
{
    public class StageSelectPresenter : ControllerBase, IStartable
    {
        private readonly StageSelectService stageSelectService;
        private readonly StageSelectView stageSelectView;
        private readonly StageSelector stageSelector;
        private readonly StageManager stageManager;

        [Inject]
        public StageSelectPresenter(StageSelectService stageSelectService, StageSelectView stageSelectView, StageSelector stageSelector, StageManager stageManager)
        {
            this.stageSelectService = stageSelectService;
            this.stageSelectView = stageSelectView;
            this.stageSelector = stageSelector;
            this.stageManager = stageManager;
        }

        public void Start()
        {
            stageSelectView.ToNextButtonClickedObservable
                .Subscribe(async _ =>
                {
                    await stageManager.LoadStageData();
                    stageManager.SetStageData();
                    AudioPlayer.PlayClickButtonSE();
                    stageSelectService.NextScnene();
                })
                .AddTo(this);
            
            stageSelector.SelectedStageObservable
                .Subscribe(x =>
                {
                    stageSelectView.MakeUsableToNextButton();
                    AudioPlayer.PlayClickButtonSE();
                    stageManager.SetSelectedStageNumber(x);
                })
                .AddTo(this);

            stageSelectView.BackButtonClickedEvent
                .AddListener(() =>
                {
                    stageSelectService.BackScene();
                });

            stageSelectView.OptionButtonClickedEvent
                .AddListener(() =>
                {
                    stageSelectService.OpenOption();
                });
        }
    }
}

