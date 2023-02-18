using Game.Character.Managers;
using Game.Teams.Managers;
using VContainer;
using VContainer.Unity;
using UniRx;
using Game.Stages.Managers;

namespace Game.Teams.Edit
{
    public class CharacterImagePresenter : ControllerBase, IStartable
    {
        private readonly CharacterImageView characterImageView;
        private readonly TeamManager teamManager;
        private readonly StageManager stageManager;

        [Inject]
        public CharacterImagePresenter(CharacterImageView characterImageView, TeamManager teamManager,
            StageManager stageManager)
        {
            this.characterImageView = characterImageView;
            this.teamManager = teamManager;
            this.stageManager = stageManager;
        }

        public void Start()
        {
            teamManager.ClearTeam();

            int teamMenberCount = stageManager.characterInitialPositionData.FriendCharacterPositionDataArray.Count;
            characterImageView.ViewTeamImage(teamMenberCount);

            characterImageView.AttachedObservable
                .Subscribe(v =>
                {
                    teamManager.SetTeamMember(v.Key, v.Value);
                    characterImageView.SetSelectableCharacterImage(teamManager.teamCharacterTypes);
                })
                .AddTo(this);
        }
    }
}

