using Game.Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Stages.LandscapeData
{
    public class RiverData : BaseLandscapeData
    {
        public RiverData()
        {
            movingCost = 1;
            CanEnter = false;

            entarableCharacterList.Add(CharacterType.Crocodile);
        }
    }
}

