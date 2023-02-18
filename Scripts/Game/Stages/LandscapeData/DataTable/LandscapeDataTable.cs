using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Stages.LandscapeData
{
    public static class LandscapeDataTable
    {
        public static readonly Dictionary<LandscapeType, BaseLandscapeData> LandscapeDataDic = new Dictionary<LandscapeType, BaseLandscapeData>()
        {
            //{LandscapeType.None, new NoneData() },
            {LandscapeType.Plain, new PlainData() },
            {LandscapeType.Forest, new ForestData() },
            {LandscapeType.River, new RiverData() },
            //{LandscapeType.RaggedRock, new RaggedRockData() },
            {LandscapeType.Rock, new RockData() },
            {LandscapeType.Lava, new LavaData() },
            {LandscapeType.FireFloor, new FireFloorData() },
        };
    }
}

