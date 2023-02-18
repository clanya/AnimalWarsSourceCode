using System;
using System.Collections.ObjectModel;
using UnityEngine;
using System.Linq;
using Game.Stages.LandscapeData;

namespace Game.Stages.Managers
{
    public class MapManager
    {
        private LandscapeType[][] mapData;
        public ReadOnlyCollection<ReadOnlyCollection<LandscapeType>> MapData { get; private set; }

        public int limitX { get; private set; }
        public int limitY { get; private set; }

        public void SetMapData(LandscapeType[][] mapData)
        {
            this.mapData = mapData;
            MapData = Array.AsReadOnly(mapData.Select(x => Array.AsReadOnly(x)).ToArray());

            limitX = mapData.Length;
            limitY = mapData[0].Length;
        }

        /// <summary>
        /// ステージの範囲外かどうかを調べる
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool CheckOutOfStage(Vector2Int point)
        {
            int limitY = mapData.Length;
            if (point.y < 0 || limitY <= point.y)
                return true;

            int limitX = mapData[0].Length;
            if (point.x < 0 || limitX <= point.x)
                return true;

            if (mapData[point.x][point.y] == LandscapeType.None)
                return true;

            return false;
        }

        public BaseLandscapeData GetLandscapeData(Vector2Int point)
        {
            return LandscapeDataTable.LandscapeDataDic[mapData[point.x][point.y]];
        }

        public LandscapeType GetLandscapeType(Vector2Int point)
        {
            return mapData[point.x][point.y];
        }
    }
}

