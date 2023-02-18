using System.Collections;
using System.Collections.Generic;
using Game.Character;
using Game.Character.Models;
using UnityEngine;

namespace Game.Stages.LandscapeData
{
    public class FireFloorData : BaseLandscapeData
    {
        public FireFloorData()
        {
            movingCost = 2;
            CanEnter = true;
        }

        public override void Effect(CharacterCurrentStatus currentStatus)
        {
            base.Effect(currentStatus);
            var hp = Mathf.Max(0,currentStatus.Hp.Value - 5);
            currentStatus.SetHp(hp);
        }
    }
}

