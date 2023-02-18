using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using MyUtil;

namespace Game.Util
{
    public static class CSVConverter
    {
        /// <summary>
        /// TextAssetを整数のジャグ配列に変換する
        /// </summary>
        /// <param name="textAsset">変換後の数値データ</param>
        /// <returns></returns>
        public static int[][] ConvertToDoubleArray(TextAsset textAsset)
        {
            //テキストデータを改行で分ける
            var splitText = textAsset.ToString().Split('\n');
            int[][] jaggedArray = new int[splitText.Length-1][];
            int length = splitText.Length - 1; //CSVの最後の行は空行なので無視する

            //テキストデータから数値に変換できないものを取り除く
            for (int i = 0; i < length; i++)
            {
                string[] strArray = splitText[i].Split(',').Where(s => s.CanConvertToInt()).ToArray();

                //CSVの文字データを数値に変換して配列に代入
                int[] intData = strArray.Select(x => x.ConvertToInt()).ToArray();
                jaggedArray[length - i - 1] = intData;
            }

            jaggedArray = ReverseArray(jaggedArray);
            //var resultArray = ArrayConverter.ConvertToDoubleArray<int>(jaggedArray);
            return jaggedArray;
        }

        //二重配列の縦と横を入れ替える
        //NOTE: 参照渡しのほうがいいか？
        private static int[][] ReverseArray(int[][] array)
        {
            int maxX = array.Select(x => x.Length).Max();
            int maxY = array.Length;
            int[][] resultArray = new int[maxX][];

            for (int i = 0; i < maxX; i++)
            {
                resultArray[i] = new int[maxY];

                for (int j = 0; j < maxY; j++)
                {
                    //空白は0で埋める
                    try
                    {
                        resultArray[i][j] = array[j][i];
                    }
                    catch(IndexOutOfRangeException)
                    {
                        resultArray[i][j] = 0;
                    }
                }
            }

            return resultArray;
        }
    }
}

