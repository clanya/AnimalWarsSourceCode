using System;
using System.Collections.Generic;
using System.Linq;
using Game.Character;
using Game.Character.Models;
using UnityEngine;

namespace Game.BattleFlow
{
    //Note:プロパティ名はインスペクタ拡張のserializedObject.FindPropertyで文字列検索しているので、変更する際は注意すること.
    [CreateAssetMenu(menuName ="MyScriptableObject/CharacterData")]
    public sealed class StageCharacterData : ScriptableObject
    {
        [SerializeField,EnumIndex(typeof(CharacterType))] private List<Character> characterList;
        public IReadOnlyList<Character> CharacterList => characterList;
        
        [Serializable]
        public class Character
        {
            [SerializeField] private CharacterParam param;
            public CharacterParam Param => param;
            [SerializeField] private BaseCharacter prefab;
            public BaseCharacter Prefab => prefab;
        }

        public CharacterParam GetCharacterParameter(CharacterType characterType)
            => characterList.First(x => x.Param.Type == characterType).Param;
    }
}