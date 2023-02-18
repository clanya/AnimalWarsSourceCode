using Game.Audio;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Options
{
    public class OptionView : MonoBehaviour
    {
        [SerializeField] private Button closeButton;
        [SerializeField] private GameObject optionPanel;

        [SerializeField] private Slider BGMSlider;
        [SerializeField] private Slider SESlider;

        [SerializeField] private AudioManager audioManager;

        private bool IsOpningOptionPanel = false;
        private static ISubject<Unit> openOptionPanelSubject = new Subject<Unit>();

        private void Start()
        {
            openOptionPanelSubject
                .Where(_ => !IsOpningOptionPanel)
                .Where(_ => optionPanel != null)
                .Subscribe(_ =>
                {
                    optionPanel?.SetActive(true);
                    IsOpningOptionPanel = true;
                })
                .AddTo(this);

            closeButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    optionPanel.SetActive(false);
                    IsOpningOptionPanel = false;
                    AudioPlayer.PlayClickButtonSE();
                })
                .AddTo(this);

            BGMSlider.onValueChanged
                .AddListener(value =>
                {
                    audioManager.SetBGMVolume(value*0.8f);
                });

            SESlider.onValueChanged
                .AddListener(value =>
                {
                    audioManager.SetSEVolume(value*0.8f);
                });
        }

        public static void OpenOptionPanel()
        {
            openOptionPanelSubject.OnNext(Unit.Default);
        }
    }
}

