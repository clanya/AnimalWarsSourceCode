using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyUtil
{
    public static class IntergerExtension
    {
        /// <summary>
        /// 整数を列挙型に変換する
        /// </summary>
        /// <typeparam name="T">変換先のEnum</typeparam>
        /// <param name="integer"></param>
        /// <returns>変換後の列挙型パラメータ</returns>
        public static T GetEnum<T>(this Int32 integer) where T : Enum
        {
            return (T)Enum.ToObject(typeof(T), integer);
        }
    }
}

