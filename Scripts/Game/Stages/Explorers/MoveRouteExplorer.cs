using Game.Character;
using Game.Stages.Managers;
using MyUtil;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Stages.Explorers
{
    public class MoveRouteExplorer
    {
        private MapManager mapManager;

        public MoveRouteExplorer(MapManager mapManager)
        {
            this.mapManager = mapManager;
        }

        public bool FindRoute(Vector2Int start, Vector2Int end, CharacterType characterType, int moveRange, ref Stack<Vector2Int> route)
        {
            if (start == end)
                return true;

            if (mapManager.CheckOutOfStage(end))
                return false;

            if (!mapManager.GetLandscapeData(end).GetCanEnter(characterType))
                return false;

            int deltaX = end.x - start.x;
            int deltaY = end.y - start.y;

            if (moveRange < 0)
            {
                return false;
            }

            if (Mathf.Abs(deltaX + deltaY) > moveRange)
            {
                return false;
            }

            if (new Vector2Int(start.x + 1, start.y) == end ||
                new Vector2Int(start.x - 1, start.y) == end ||
                new Vector2Int(start.x, start.y + 1) == end ||
                new Vector2Int(start.x, start.y - 1) == end)
            {
                route.Push(end);
                return true;
            }

            if (route.Any(x => x == end))
                return false;


            var aroundPoints = AroundPointsFinder.FindSidePoints(end).Where(x=>!mapManager.CheckOutOfStage(x)).Where(x=>mapManager.GetLandscapeData(x).GetCanEnter(characterType));
            route.Push(end);
            foreach(var point in aroundPoints)
            {
                Stack<Vector2Int> stack = new Stack<Vector2Int>(route.Reverse());
                var ret = FindRoute(start, point, characterType, moveRange - 1, ref stack);
                
                if (ret)
                {
                    route = stack;
                    return true;
                }
            }

            return false;
        }
    }
}
