using Game.Character;
using System;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Data.DataTable
{
    //StageManagerから取得する
    [CreateAssetMenu(menuName = "MyScriptable/CharacterInitialPositionDataTable")]
    public class CharacterInitialPositionDataTable : ScriptableObject
    {
        [Serializable]
        public class CharacterInitialPositionData
        {
            [SerializeField] private Vector2Int initialPosition;
            public Vector2Int InitialPosition => initialPosition;
        }

        [Serializable]
        public class FriendCharacterInitialPositionData : CharacterInitialPositionData
        {
            
        }

        [Serializable]
        public class EnemyCharacterInitialPositionData : CharacterInitialPositionData
        {
            [SerializeField] private CharacterType characterType;
            public CharacterType CharacterType => characterType;
        }

        [SerializeField] private int stageNumber;
        [SerializeField] private FriendCharacterInitialPositionData[] friendCharacterPositionDataArray;
        [SerializeField] private EnemyCharacterInitialPositionData[] enemyCharacterPositionDataArray;

        public int StageNumber => stageNumber;
        public ReadOnlyCollection<FriendCharacterInitialPositionData> FriendCharacterPositionDataArray => Array.AsReadOnly(friendCharacterPositionDataArray);
        public ReadOnlyCollection<EnemyCharacterInitialPositionData> EnemyCharacterPositionDataArray => Array.AsReadOnly(enemyCharacterPositionDataArray);
    }
}

