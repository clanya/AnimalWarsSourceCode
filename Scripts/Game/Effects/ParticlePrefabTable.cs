using Game.Character;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Game.Effects
{
    [CreateAssetMenu(menuName ="MyScriptable/AttackEffectPrefabTable")]
    public class ParticlePrefabTable : ScriptableObject
    {
        [Serializable]
        private class EffectPrefabData
        {
            [SerializeField] private CharacterType characterType;
            [SerializeField] private ParticleSystem attackParticlePrefab;
            
            public CharacterType CharacterType => characterType;
            public ParticleSystem AttackParticlePrefab => attackParticlePrefab; 
        }

        [SerializeField] private List<EffectPrefabData> prefabDataList;

        public ParticleSystem GetAttackParticlePrefab(CharacterType characterType)
            => prefabDataList.FirstOrDefault(x => x.CharacterType == characterType).AttackParticlePrefab;
    }
}

