using Cysharp.Threading.Tasks;
using Game.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Game.Stages
{
    public class LandscapeDataLoader
    {
        private string stageDataName = "StageData_";
        private AsyncOperationHandle<TextAsset> handle;

        //CSVから地形データの読み込み
        public IEnumerator LoadStageData(int stageNumber, Action<int[][]> callback)
        {
            string loadAddress = stageDataName + stageNumber.ToString("D2");
            handle = Addressables.LoadAssetAsync<TextAsset>(loadAddress);

            yield return handle;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                var landscapeData = CSVConverter.ConvertToDoubleArray(handle.Result);
                callback?.Invoke(landscapeData);
            }
            else
            {
                Debug.LogError($"データが存在しません");
            }
            Addressables.Release(handle);
        }
    }
}

