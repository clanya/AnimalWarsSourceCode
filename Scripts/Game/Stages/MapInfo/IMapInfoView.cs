using Game.Character;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Stages.MapInfo
{
    public interface IMapInfoView
    {
        public Button.ButtonClickedEvent MapViewButtonClickEvent { get; }

        public void GenerateMapPiece(ReadOnlyCollection<ReadOnlyCollection<LandscapeType>> mapData);
        public void ViewFriendCharacterPosition(IReadOnlyList<Vector2Int> positionList);
        public void ViewEnemyCharacterPosition(IReadOnlyDictionary<Vector2Int, CharacterType> enemyDic);

        public void UpdateMapPiece(Vector2 PreviousPosition, Vector2 newPosition);
    }
}

