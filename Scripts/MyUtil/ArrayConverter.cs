using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MyUtil
{
    public static class ArrayConverter
    {
        /// <summary>
        /// Intのジャグ配列を任意の列挙型のジャグ配列に変換する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T[][] ExchangeToEnumArray<T>(int[][] data) where T : Enum
        {
            int maxX = data.Length;
            T[][] result = new T[maxX][];
            for (int i = 0; i < maxX; i++)
            {
                int maxY = data[i].Length;
                result[i] = new T[maxY];
                for (int j = 0; j < maxY; j++)
                {
                    result[i][j] = data[i][j].GetEnum<T>();
                }
            }
            return result;
        }

        /// <summary>
        /// Intのジャグ配列を任意の列挙型の二重配列に変換する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T[,] ExchangeToEnumArray<T>(int[,] data) where T : Enum
        {
            int maxX = data.GetLength(0);
            int maxY = data.GetLength(1);
            T[,] result = new T[maxX,maxY];
            for (int i = 0; i < maxX; i++)
            {
                for (int j = 0; j < maxY; j++)
                {
                    result[i,j] = data[i,j].GetEnum<T>();
                }
            }
            return result;
        }

        /// <summary>
        /// ジャグ配列を二重配列に変換する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jaggedArray"></param>
        /// <returns></returns>
        public static T[,] ConvertToDoubleArray<T>(T[][] jaggedArray)
        {
            int jaggLength = jaggedArray.Length;
            int maxLength = jaggedArray.Select(x => x.Length).Max();
            T[,] resultArray = new T[jaggedArray.Length, maxLength];

            for (int y = 0; y < jaggLength; y++)
            {
                for (int x = 0; x < maxLength; x++)
                {
                    resultArray[x, y] = jaggedArray[x][y];
                }
            }

            return resultArray;
        }
    }

}
