using Game.Stages.Managers;
using Game.Audio;
using VContainer;
using VContainer.Unity;
using Game.Teams.Managers;
using UniRx;

namespace Game.Teams.Edit.Scenes
{
    public class TeamEditScenePresenter : ControllerBase, IStartable
    {
        private readonly TeamEditSceneService teamEditSceneService;
        private readonly TeamEditSceneView teamEditSceneView;
        private readonly StageManager stageManager;
        private readonly TeamManager teamManager;

        [Inject]
        public TeamEditScenePresenter(TeamEditSceneService teamEditSceneService, TeamEditSceneView teamEditSceneView,
            StageManager stageManager, TeamManager teamManager)
        {
            this.teamEditSceneService = teamEditSceneService;
            this.teamEditSceneView = teamEditSceneView;
            this.stageManager = stageManager;
            this.teamManager = teamManager;
        }

        public void Start()
        {
            teamEditSceneView.ButtleStartButtonClickEvent.AddListener(() =>
            {
                teamEditSceneService.LoadNextScene(stageManager.selectedStageNumber);
                AudioPlayer.PlayClickButtonSE();
            });

            teamEditSceneView.BackButtonClickEvent.AddListener(() =>
            {
                teamEditSceneService.LoadPreviousScene();
                AudioPlayer.PlayClickButtonSE();
            });

            teamEditSceneView.SetStageText(stageManager.selectedStageNumber);
            teamEditSceneView.SetBackGrount(stageManager.selectedStageNumber);

            teamManager.ObserveEveryValueChanged(x => x.IsExistTeamMenber)
                .Subscribe(x =>
                {
                    teamEditSceneView.SetStartButtonInteractable(x);
                })
                .AddTo(this);
        }
    }
}

