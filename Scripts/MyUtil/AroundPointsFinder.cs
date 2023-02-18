using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyUtil
{
    public static class AroundPointsFinder
    {
        /// <summary>
        /// 上下左右の位置を探す
        /// </summary>
        /// <param name="center"></param>
        /// <returns></returns>
        public static IEnumerable<Vector2Int> FindSidePoints(Vector2Int center)
        {
            List<Vector2Int> result = new List<Vector2Int>();
            result.Add(new Vector2Int(center.x + 1, center.y));
            result.Add(new Vector2Int(center.x - 1, center.y));
            result.Add(new Vector2Int(center.x, center.y + 1));
            result.Add(new Vector2Int(center.x, center.y - 1));
            return result;
        }

        /// <summary>
        /// 指定した長さまで上下左右の位置を探す
        /// </summary>
        /// <param name="center"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static IEnumerable<Vector2Int> FindSidePoints(Vector2Int center, int length, bool within)
        {
            if (length == 0) return new List<Vector2Int>();

            List<Vector2Int> result = new List<Vector2Int>();

            int i = within ? 1 : length;

            for(; i <= length; i++)
            {
                result.Add(new Vector2Int(center.x + i, center.y));
                result.Add(new Vector2Int(center.x - i, center.y));
                result.Add(new Vector2Int(center.x, center.y + i));
                result.Add(new Vector2Int(center.x, center.y - i));
            }

            return result;
        }

        /// <summary>
        /// 斜めの位置を探す
        /// </summary>
        /// <param name="center"></param>
        /// <returns></returns>
        public static IEnumerable<Vector2Int> FindCornersPoints(Vector2Int center)
        {
            List<Vector2Int> result = new List<Vector2Int>();
            result.Add(new Vector2Int(center.x + 1, center.y + 1));
            result.Add(new Vector2Int(center.x + 1, center.y - 1));
            result.Add(new Vector2Int(center.x - 1, center.y + 1));
            result.Add(new Vector2Int(center.x - 1, center.y - 1));
            return result;
        }

        /// <summary>
        /// 指定した長さまで斜めの位置を探す
        /// </summary>
        /// <param name="center"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static IEnumerable<Vector2Int> FindCornersPoints(Vector2Int center, int length, bool within)
        {
            if (length == 0) return new List<Vector2Int>();

            int i = within ? 1 : length;

            List<Vector2Int> result = new List<Vector2Int>();
            for (; i <= length; i++)
            {
                result.Add(new Vector2Int(center.x + i, center.y + i));
                result.Add(new Vector2Int(center.x + i, center.y - i));
                result.Add(new Vector2Int(center.x - i, center.y + i));
                result.Add(new Vector2Int(center.x - i, center.y - i));
            }

            return result;
        }
    }
}

