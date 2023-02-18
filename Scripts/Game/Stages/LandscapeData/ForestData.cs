using System.Collections;
using System.Collections.Generic;
using Game.Character;
using Game.Character.Models;
using UnityEngine;

namespace Game.Stages.LandscapeData
{
    public class ForestData : BaseLandscapeData
    {
        public ForestData()
        {
            movingCost = 1;
            CanEnter = true;
        }

        public override void Effect(CharacterCurrentStatus currentStatus)
        {
            currentStatus.SetEvadeRate(0.3f);
        }
    }
}

