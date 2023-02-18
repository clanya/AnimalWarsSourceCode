using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MyUtil
{
    public static class IEnumelableExtension
    {
        /// <summary>
        /// リストの要素を文字列に変換する
        /// </summary>
        /// <param name="list"></param>
        /// <param name="split"></param>
        /// <returns></returns>
        public static string ToElementsString(this IEnumerable list, SplitType split=SplitType.NewLine)
        {
            string resultStr = "";
            string splitStr = ChooseSplitString(split);
            foreach (var x in list)
            {
                resultStr = string.Concat(resultStr, x, splitStr);
            }
            return resultStr;
        }

        private static string ChooseSplitString(SplitType type)
        {
            return type switch
            {
                SplitType.Space => " ",
                SplitType.Comma => ",",
                SplitType.NewLine => "\n",
                _ => ""
            };
        }

        public static T RandomGet<T>(this IEnumerable<T> list)
        {
            var rand = new Random();
            return list.ElementAt(rand.Next(list.Count()));
        }
    }

}
