using Game.Audio;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.PoseScene
{
    public class PoseScenePanelViewer : MonoBehaviour
    {
        [SerializeField] private Button closeButton;
        [SerializeField] private GameObject panel;
        [SerializeField] private TMP_Text text;

        private bool IsOpningPanel = false;
        private static ISubject<string> openPosePanelSubject = new Subject<string>();
        private static ISubject<string> openResultPanelSubject = new Subject<string>();

        private void Start()
        {
            openPosePanelSubject
                .Where(_ => !IsOpningPanel)
                .Where(_ => panel != null)
                .Subscribe(x =>
                {
                    panel?.SetActive(true);
                    IsOpningPanel = true;
                    text.text = x;
                })
                .AddTo(this);

            openResultPanelSubject
                .Where(_ => !IsOpningPanel)
                .Where(_ => panel != null)
                .Subscribe(x =>
                {
                    panel?.SetActive(true);
                    closeButton.gameObject.SetActive(false);
                    IsOpningPanel = true;
                    text.text = x;
                })
                .AddTo(this);

            closeButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    AudioPlayer.PlayClickButtonSE();
                    panel.SetActive(false);
                    IsOpningPanel = false;
                })
                .AddTo(this);
        }

        public static void OpenPosePanel()
        {
            openPosePanelSubject.OnNext("一時停止");
        }

        public static void OpenResultPanel(string text)
        {
            openResultPanelSubject.OnNext(text);
        }
    }
}

