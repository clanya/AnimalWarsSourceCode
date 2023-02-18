using Game.Stages;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data.DataTable
{
    //地形タイプに対応するプレハブのデータを保持する
    [CreateAssetMenu(menuName = "MyScriptableObjet/landscapePrefabTable")]
    public class LandscapePrefabTable : ScriptableObject
    {
        [Serializable]
        public class LandscapeData
        {
            [SerializeField] private LandscapeType landscape;
            [SerializeField] private GameObject landscapePrefab;

            public LandscapeType Landscape => landscape;
            public GameObject LandscapePrefab => landscapePrefab;
        }

        [SerializeField] private List<LandscapeData> landscapeDataTable;
        public List<LandscapeData> LandscapeDatTable => landscapeDataTable;
    }
}

