using System;
using System.Linq;
using Game.BattleFlow.Character;
using Game.Stages;
using Game.Stages.Managers;
using UniRx;
using UnityEngine;
using VContainer;

namespace Game.BattleFlow
{
    public sealed class LandScapeEffectDispatcher: MonoBehaviour
    {
        [SerializeField] private CharacterManager characterManager;
        [Inject] private MapManager mapManager;

        public void Start()
        {
            ApplyLandScapeEffect();
        }

        private void ApplyLandScapeEffect()
        {
            foreach (var character in characterManager.AllCharacters)
            {
                character.IsMovable.Where(x => !x)
                    .Subscribe(_ =>
                    {
                        mapManager.GetLandscapeData(character.Position).Effect(character.CurrentStatus);
                    }).AddTo(character.gameObject);
            }
        }
    }
}