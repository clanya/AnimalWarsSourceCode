using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Character
{
    [CreateAssetMenu(menuName ="MyScriptable/CharacterPrefabTable")]
    public class CharacterPrefabTable : ScriptableObject
    {
        [Serializable]
        private class CharacterPrefabData
        {
            [SerializeField] private CharacterType characterType;
            [SerializeField] private GameObject characterPrefab;

            public CharacterType CharacterType => characterType;
            public GameObject CharacterPrefab => characterPrefab;
        }

        [SerializeField] private List<CharacterPrefabData> characterPrefabDataList = new List<CharacterPrefabData>();

        public GameObject GetCharacterPrefab(CharacterType characterType)
            => characterPrefabDataList.Find(x => x.CharacterType == characterType).CharacterPrefab;
    }
}

