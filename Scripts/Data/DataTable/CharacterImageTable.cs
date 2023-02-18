using Game.Character;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Data.DataTable
{
    [CreateAssetMenu(menuName = "MyScriptable/CharacterImageTable")]
    public class CharacterImageTable : ScriptableObject
    {
        [Serializable]
        public class CharacterImageData
        {
            [SerializeField] private CharacterType characterType;
            [SerializeField] private Sprite characterSprite;

            public CharacterType CharacterType => characterType;
            public Sprite CharacterSprite => characterSprite;
        }

        [SerializeField] private CharacterImageData[] characterImageArray;
        public CharacterImageData[] CharacterImageDataArray => characterImageArray;

        public Sprite GetCharacterSprite(CharacterType characterType)
            => characterImageArray.FirstOrDefault(x => x.CharacterType == characterType).CharacterSprite;
    }
}

