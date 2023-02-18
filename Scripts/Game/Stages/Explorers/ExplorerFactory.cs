using Game.BattleFlow;
using Game.Character.Models;
using Game.Stages.Managers;
using System.Collections;
using System.Collections.Generic;
using Game.BattleFlow.Character;
using UnityEngine;
using VContainer;

namespace Game.Stages.Explorers
{
    public class ExplorerFactory
    {
        [Inject] private MapManager mapManager;
        [Inject] private CharacterManager characterManager;

        public ExplorerFactory(MapManager mapManager)
        {
            this.mapManager = mapManager;
        }

        public MovablePointsExplorer GetMovablePointsExplorer(PlayerID user)
        {
            return new MovablePointsExplorer(characterManager, mapManager, user);
        }

        public AttackablePointsExplorer GetAttackPointsExplorer(PlayerID user)
        {
            MovablePointsExplorer movablePointsExplorer = GetMovablePointsExplorer(user);
            return GetAttackPointsExplorer(user, movablePointsExplorer);
        }

        public AttackablePointsExplorer GetAttackPointsExplorer(PlayerID user, MovablePointsExplorer movablePointsExplorer)
        {
            return new AttackablePointsExplorer(characterManager, mapManager, user, movablePointsExplorer);
        }

        public MoveRouteExplorer GetMoveRouteExplorer()
        {
            return new MoveRouteExplorer(mapManager);
        }
    }
}

