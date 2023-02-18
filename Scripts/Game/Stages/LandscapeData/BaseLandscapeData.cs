using Game.Character;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Game.Character.Models;
using UnityEngine;

namespace Game.Stages.LandscapeData
{
    public abstract class BaseLandscapeData
    {
        public int movingCost { get; protected set; }
        protected bool CanEnter;
        public bool CanAttack { get; protected set; } = true;

        protected List<CharacterType> entarableCharacterList =new();

        public BaseLandscapeData()
        {
            movingCost = 1;
            CanEnter = true;
            CanAttack = true;
        }

        public bool GetCanEnter(CharacterType characterType)
        {
            if (CanEnter)
            {
                return true;
            }

            return entarableCharacterList.Any(x => x == characterType);
        }

        public virtual void Effect(CharacterCurrentStatus currentStatus)
        {
            currentStatus.SetEvadeRate(0);
        }
    }
}

