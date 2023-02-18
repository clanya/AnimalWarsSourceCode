using Game.BattleFlow;
using Game.Character;
using Game.Character.Models;
using Game.Stages.Managers;
using MyUtil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.BattleFlow.Character;
using UnityEngine;

namespace Game.Stages.Explorers
{
    public class AttackablePointsExplorer
    {
        private MovablePointsExplorer movablePointsExplorer;
        private MapManager mapManager;

        private Func<IEnumerable<BaseCharacter>> GetFriendCharacters;
        private Func<IEnumerable<BaseCharacter>> GetEnemyCharacters;
        private IEnumerable<BaseCharacter> AllCharacters => GetFriendCharacters.Invoke().Union(GetEnemyCharacters.Invoke());

        public AttackablePointsExplorer(CharacterManager characterManager, MapManager mapManager, PlayerID playerID, MovablePointsExplorer movablePointsExplorer)
        {
            if (playerID == PlayerID.Player1)
            {
                GetFriendCharacters += (() => characterManager.PlayerCharacterList);
                GetEnemyCharacters += (() => characterManager.EnemyCharacterList);
            }
            else
            {
                GetFriendCharacters += (() => characterManager.EnemyCharacterList);
                GetEnemyCharacters += (() => characterManager.PlayerCharacterList);
            }

            this.mapManager = mapManager;
            this.movablePointsExplorer = movablePointsExplorer;
        }

        //攻撃できる場所を取得
        public IEnumerable<Vector2Int> FindAttackablePoints(IEnumerable<Vector2Int> movablePoints, int attackRange)
        {
            IEnumerable<Vector2Int> result = new HashSet<Vector2Int>();
            foreach (var point in movablePoints)
            {
                var points1 = AroundPointsFinder.FindSidePoints(point, attackRange, false);
                var points2 = AroundPointsFinder.FindCornersPoints(point, attackRange - 1, false);

                result = result.Union(points1);
                result = result.Union(points2);
            }

            result = result.Where(x => !mapManager.CheckOutOfStage(x)).Where(x => mapManager.GetLandscapeData(x).CanAttack).Where(x => mapManager.GetLandscapeType(x) != LandscapeType.Lava);
            return result;
        }

        public IEnumerable<Vector2Int> FindAttackablePoints(Vector2Int centerPoint, CharacterParam param)
        {
            var movablePoints = movablePointsExplorer.FindMovablePoints(centerPoint, param.MovableRange, param.Type);
            return FindAttackablePoints(movablePoints, param.AttackRange);
        }

        public IEnumerable<Vector2Int> FindAttackablePoints(Vector2Int centerPoint, int attackRange)
        {
            var movablePoints = new List<Vector2Int>() { centerPoint };
            return FindAttackablePoints(movablePoints, attackRange);
        }
    }
}
