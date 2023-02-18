using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Stages.LandscapeData
{
    public class RockData : BaseLandscapeData
    {
        public RockData()
        {
            movingCost = 1;
            CanEnter = false;
            CanAttack = false;
        }
    }
}

