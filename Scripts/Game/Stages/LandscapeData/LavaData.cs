using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Stages.LandscapeData
{
    public class LavaData : BaseLandscapeData
    {
        public LavaData()
        {
            movingCost = 1;
            CanEnter = false;
        }
    }
}

