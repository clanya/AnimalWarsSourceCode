using Game.Character;
using UnityEngine;

namespace Game.Effects
{
    public class EffectPlayer : MonoBehaviour
    {
        [SerializeField] private ParticlePrefabTable attackEffectPrefabTable;
        [SerializeField] private ParticleSystem buffEffectPrefab;
        [SerializeField] private ParticleSystem debuffEffectPrefab;
        [SerializeField] private ParticleSystem healEffectPrefab;
        
        [SerializeField] private ParticleSystem damagedEffectPrefab;
        [SerializeField] private ParticleSystem deathEffectPrefab;

        public void PlayAttackEffect(CharacterType characterType, Vector3 playPosition)
        {
            var prefab = attackEffectPrefabTable.GetAttackParticlePrefab(characterType);
            var effect=Instantiate(prefab);
            effect.transform.position = playPosition;
        }

        public void PlayDamagedEffect(Vector3 playPosition)
        {
            var effect = Instantiate(damagedEffectPrefab);
            effect.transform.position = playPosition;
        }

        public void PlayDeathEffect(Vector3 playPosition)
        {
            var effect = Instantiate(deathEffectPrefab);
            effect.transform.position = playPosition;
        }

        public void PlayBuffEffect(Vector3 playPosition)
        {
            var effect = Instantiate(buffEffectPrefab);
            effect.transform.position = playPosition;
        }

        public void PlayDebuffEffect(Vector3 playPosition)
        {
            var effect = Instantiate(debuffEffectPrefab);
            effect.transform.position = playPosition;
        }

        public void PlayHealEffect(Vector3 playPosition)
        {
            var effect = Instantiate(healEffectPrefab);
            effect.transform.position = playPosition;
        }
    }
}

