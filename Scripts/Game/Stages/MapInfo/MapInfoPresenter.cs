using Game.Character.Managers;
using Game.Stages.Managers;
using System.Linq;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Stages.MapInfo
{
    public class MapInfoPresenter : IStartable
    {
        private readonly IMapInfoView mapInfoView;
        private readonly MapManager mapManager;
        private readonly StageManager stageManager;

        [Inject]
        public MapInfoPresenter(IMapInfoView mapInfoView, MapManager mapManager,
            StageManager stageManager)
        {
            this.mapInfoView = mapInfoView;
            this.mapManager = mapManager;
            this.stageManager = stageManager;
        }

        public void Start()
        {
            mapInfoView.GenerateMapPiece(mapManager.MapData);
            mapInfoView.ViewFriendCharacterPosition(stageManager.characterInitialPositionData.FriendCharacterPositionDataArray.Select(v => new Vector2Int(v.InitialPosition.x, v.InitialPosition.y)).ToList());
            mapInfoView.ViewEnemyCharacterPosition(stageManager.characterInitialPositionData.EnemyCharacterPositionDataArray.ToDictionary(v => new Vector2Int(v.InitialPosition.x, v.InitialPosition.y), v => v.CharacterType));
        }
    }
}

