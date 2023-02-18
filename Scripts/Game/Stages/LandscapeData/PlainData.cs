using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Stages.LandscapeData
{
    public class PlainData : BaseLandscapeData
    {
        public PlainData()
        {
            movingCost = 1;
            CanEnter = true;
        }
    }
}

