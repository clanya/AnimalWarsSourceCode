using Game.Stages;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Data.DataTable
{
    [CreateAssetMenu(menuName ="MyScriptable/LandscapeColorTable")]
    public class LandscapeColorTable : ScriptableObject
    {
        [Serializable]
        public class LandscapeColorData
        {
            [SerializeField] private LandscapeType landscapeType;
            [SerializeField] private Color landscapeColor;

            public LandscapeType LandscapeType => landscapeType;
            public Color LandscapeColor => landscapeColor;
        }

        [SerializeField] private LandscapeColorData[] landscapeColorDataArray;
        public ReadOnlyCollection<LandscapeColorData> LandscapeColorDataArray => Array.AsReadOnly(landscapeColorDataArray);
    }
}

