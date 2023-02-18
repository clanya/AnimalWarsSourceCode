using KanKikuchi.AudioManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Audio
{
    public static class AudioPlayer
    {
        public static void PlayClickButtonSE()
        {
            SEManager.Instance.Play(SEPath.CLICK_BUTTON, pitch: 0.75f);
        }

        public static void PlaySelectButtonSE()
        {
            SEManager.Instance.Play(SEPath.SELECT_BUTTON, pitch: 1.2f);
        }

        public static void PlayDamageSE()
        {
            SEManager.Instance.Play(SEPath.DAMAGE, pitch: 1.7f);
        }

        public static void PlayPowerUpSE()
        {
            SEManager.Instance.Play(SEPath.POWER_UP, pitch: 1.1f);
        }

        public static void PlayPowerDownSE()
        {
            SEManager.Instance.Play(SEPath.POWER_DOWN, pitch: 1.4f);
        }

        public static void PlayDownSE()
        {
            SEManager.Instance.Play(SEPath.DOWN, pitch: 1.1f);
        }

        public static void PlayRunSE()
        {
            SEManager.Instance.Play(SEPath.RUN, pitch:1.25f, isLoop:true);
        }

        public static void StopRunSE()
        {
            SEManager.Instance.Stop(SEPath.RUN);
        }

        public static void PlayHealSE()
        {
            SEManager.Instance.Play(SEPath.HEAL, pitch: 1.8f);
        }

        public static void PlayMagicSE()
        {
            SEManager.Instance.Play(SEPath.MAGIC, pitch: 1.8f);
        }

        public static void PlaySetSE()
        {
            SEManager.Instance.Play(SEPath.SET);
        }

        public static void PlayPopSE()
        {
            SEManager.Instance.Play(SEPath.POP);
        }

        public static void PlayPiSE()
        {
            SEManager.Instance.Play(SEPath.PI);
        }
    }
}

