using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyUtil
{
    public static class DebugExtension
    {
        /// <summary>
        /// リストの要素を一覧で表示する
        /// </summary>
        /// <param name="list"></param>
        /// <param name="split"></param>
        public static void LogList(IEnumerable list, SplitType split=SplitType.NewLine)
        {
            string logStr=list.ToElementsString(split);
            Debug.Log(logStr);
        }
    }
}
