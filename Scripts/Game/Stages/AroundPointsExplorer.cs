using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Game.Character;
using Game.Character.Models;
using Game.Stages.Managers;
using VContainer;
using Game.Character.Managers;
using VContainer.Unity;

namespace Game.Stages
{
    public class AroundPointsExplorer
    {
        private IEnumerable<Vector2Int> movablePointsList;

        [Inject] private MapManager mapManager;

        /// <summary>
        /// あるポイントから指定した範囲のエリアを取得
        /// </summary>
        /// <param name="targetPoint"></param>
        /// <param name="attackRange"></param>
        /// <returns></returns>
        public IEnumerable<Vector2Int> FindAroundPoints(Vector2Int targetPoint, int range)
        {
            if (mapManager.MapData[targetPoint.x][targetPoint.y] == LandscapeType.None)
            {
                Debug.LogError("ステージ外の開始位置です");
                return new List<Vector2Int>();
            }
            ExplorAroundPoints(targetPoint, range);
            return movablePointsList;
        }

        /// <summary>
        /// 再帰的に探索する
        /// </summary>
        /// <param name="targetPoint"></param>
        /// <param name="attackRange"></param>
        /// <returns></returns>
        private IEnumerable<Vector2Int> ExplorAroundPoints(Vector2Int targetPoint, int range)
        {
            if (range <= 0)
            {
                return new List<Vector2Int>(); //適当に空のリストを返す
            }

            if (range == 1)
            {

                movablePointsList = FindClossPoints(targetPoint);
                //ステージ外、侵入不可エリアを除く
                movablePointsList = movablePointsList
                                    .Where(x => !mapManager.CheckOutOfStage(x)).ToArray();
                return movablePointsList;
            }

            //移動範囲が狭い時の移動可能場所（再帰）
            var points = ExplorAroundPoints(targetPoint, range - 1);
            
            //移動可能場所を追加する
            var resultHashSet = new HashSet<Vector2Int>();
            foreach(var point in points)
            {
                int RemainingMovement = range - 1; //残りの移動量から地形コストを引いた分を計算
                //移動量が残っていないならその位置は無視する
                if (RemainingMovement < 0)
                    continue;

                resultHashSet.UnionWith(FindClossPoints(point));
            }
            resultHashSet.Remove(targetPoint);
            resultHashSet.ExceptWith(points); //新しく追加した場所だけ残す

            //ステージ外、侵入不可エリアを除く
            //TODO: いい感じに関数化
            movablePointsList = movablePointsList.Union(resultHashSet)
                                .Where(x => !mapManager.CheckOutOfStage(x)).ToArray();

            return resultHashSet;
        }

        /// <summary>
        /// 上下左右の位置のリストを返す
        /// </summary>
        /// <param name="centerPoint">中心となる位置</param>
        /// <returns></returns>
        private IEnumerable<Vector2Int> FindClossPoints(Vector2Int centerPoint)
        {
            List<Vector2Int> resultList = new List<Vector2Int>();
            resultList.Add(new Vector2Int(centerPoint.x + 1, centerPoint.y));
            resultList.Add(new Vector2Int(centerPoint.x - 1, centerPoint.y));
            resultList.Add(new Vector2Int(centerPoint.x, centerPoint.y + 1));
            resultList.Add(new Vector2Int(centerPoint.x, centerPoint.y - 1));
            return resultList;
        }
    }
}
