using System;
using UnityEngine;

namespace Game.Character
{
    //Todo:命名変更
    public enum CharacterViewState
    {
        Normal = 0,
        HadActed = 1,
        Select = 2,
        Targeted = 3,
    }
    
    public static class CharacterViewStateExtension
    {
        public static Color ConvertToColor(this CharacterViewState state)
        {
            return state switch
            {
                CharacterViewState.Normal => Color.white,
                CharacterViewState.HadActed => Color.black,
                CharacterViewState.Select => Color.red,
                CharacterViewState.Targeted => Color.green,
                _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
            };
        }
    }
}