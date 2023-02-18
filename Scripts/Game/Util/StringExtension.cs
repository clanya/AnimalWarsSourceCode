using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Util
{
    public static class StringExtension
    {
        public static int ConvertToInt(this string str)
        {
            if (str == "")
                return 0;
            return Convert.ToInt32(str);
        }

        /// <summary>
        /// 文字列がIntに変換可能か調べる
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool CanConvertToInt(this string str)
        {
            if (str == "")
                return true;

            return int.TryParse(str, out int num);
        }
    }
}

