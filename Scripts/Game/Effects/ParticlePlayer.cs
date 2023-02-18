using System;
using UnityEngine;

namespace Game.Effects
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticlePlayer : MonoBehaviour
    {
        private ParticleSystem particleSystem;
        private Action<ParticlePlayer> callback;

        private void Awake()
        {
            particleSystem = GetComponent<ParticleSystem>();
        }

        public void Play(Action<ParticlePlayer> callback)
        {
            particleSystem.Play();
            this.callback = callback;
        }

        private void OnParticleSystemStopped()
        {
            callback(this);
        }
    }
}

