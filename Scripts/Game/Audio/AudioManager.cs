using KanKikuchi.AudioManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public void SetBGMVolume(float volume)
        {
            BGMManager.Instance.ChangeBaseVolume(volume);
        }

        public void SetSEVolume(float volume)
        {
            SEManager.Instance.ChangeBaseVolume(volume);
        }
    }
}

