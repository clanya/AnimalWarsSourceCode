using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Game.Character;
using Game.Character.Models;
using Game.Stages.Managers;
using MyUtil;
using Game.BattleFlow;
using System;
using Game.BattleFlow.Character;

namespace Game.Stages.Explorers
{
    public class MovablePointsExplorer
    {
        private IEnumerable<Vector2Int> movablePointsList;

        private MapManager mapManager;

        private Func<IEnumerable<BaseCharacter>> GetFriendCharacters;
        private Func<IEnumerable<BaseCharacter>> GetEnemyCharacters;
        private IEnumerable<BaseCharacter> AllCharacters => GetFriendCharacters.Invoke().Union(GetEnemyCharacters.Invoke());

        public MovablePointsExplorer(CharacterManager characterManager, MapManager mapManager, PlayerID playerID)
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
        }

        /// <summary>
        /// 移動可能な位置を見つける
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="moveRange"></param>
        /// <returns></returns>
        public IEnumerable<Vector2Int> FindMovablePoints(Vector2Int startPoint, int moveRange, CharacterType characterType)
        {
            if (mapManager.MapData[startPoint.x][startPoint.y] == LandscapeType.None)
            {
                Debug.LogError("ステージ外の開始位置です");
                return new List<Vector2Int>();
            }
            ExplorMovablePoints(startPoint, moveRange, characterType);
            var result = movablePointsList.Except(AllCharacters.Where(x=>x.Position!=startPoint).Where(x => x.IsDead.Value == false).Select(character => character.Position));
            return result;
        }

        /// <summary>
        /// 移動可能な位置を再帰的に探索する
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="moveRange"></param>
        /// <returns></returns>
        private IEnumerable<Vector2Int> ExplorMovablePoints(Vector2Int startPoint, int moveRange, CharacterType characterType)
        {   
            if (moveRange <= 0)
            {
                return Enumerable.Empty<Vector2Int>(); //適当に空のリストを返す
            }

            int cost = mapManager.GetLandscapeData(startPoint).movingCost;

            if (moveRange - cost < 0)
            {
                movablePointsList= new List<Vector2Int>() { startPoint };
                return movablePointsList;
            }
                

            if (moveRange == 1 || (moveRange == 2 && cost == 2))
            {
                movablePointsList = AroundPointsFinder.FindSidePoints(startPoint);
                movablePointsList.ToList().Add(startPoint);
                //ステージ外、侵入不可エリアを除く
                movablePointsList = movablePointsList
                                    .Where(x => !mapManager.CheckOutOfStage(x))
                                    .Where(x => mapManager.GetLandscapeData(x).GetCanEnter(characterType));
                return movablePointsList;
            }

            //移動範囲が狭い時の移動可能場所（再帰）
            var points = ExplorMovablePoints(startPoint, moveRange - mapManager.GetLandscapeData(startPoint).movingCost, characterType).Where(x=> !mapManager.CheckOutOfStage(x));
            
            
            //移動可能場所を追加する
            var resultHashSet = new HashSet<Vector2Int>();
            foreach(var point in points)
            {
                int RemainingMovement = moveRange - mapManager.GetLandscapeData(point).movingCost; //残りの移動量から地形コストを引いた分を計算
                //移動量が残っていないならその位置は無視する
                if (RemainingMovement < 0)
                    continue;

                //敵のいるところは通行不可
                if (GetEnemyCharacters.Invoke().Any(x => x.Position == point))
                    continue;

                //移動不可の場所は無視
                if (!mapManager.GetLandscapeData(startPoint).GetCanEnter(characterType))
                    continue;

                resultHashSet.UnionWith(AroundPointsFinder.FindSidePoints(point));
            }

            resultHashSet.ExceptWith(points); //新しく追加した場所だけ残す

            resultHashSet = resultHashSet.Where(x => !mapManager.CheckOutOfStage(x)).Where(x => mapManager.GetLandscapeData(x).GetCanEnter(characterType)).ToHashSet();

            //ステージ外、侵入不可エリアを除く
            //TODO: いい感じに関数化
            movablePointsList = movablePointsList.Union(resultHashSet)
                                .Where(x => !mapManager.CheckOutOfStage(x))
                                .Where(x => mapManager.GetLandscapeData(x).GetCanEnter(characterType));

            return resultHashSet;
        }
        
        public bool MovablePointExists(Vector2Int centerPoint, int moveRange,CharacterType characterType)
        {
            var movablePoints = FindMovablePoints(centerPoint, moveRange,characterType);
            if (movablePoints.Any())
            {
                return true;
            }
            return false;
        }
    }
}
