using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.HowToPlay
{
    public class HowToPlayPanelView : MonoBehaviour
    {
        [SerializeField] private Button closeButton;
        [SerializeField] private GameObject panel;

        private bool IsOpningPanel = false;
        private static ISubject<Unit> openOptionPanelSubject = new Subject<Unit>();

        private void Start()
        {
            openOptionPanelSubject
                .Where(_ => !IsOpningPanel)
                .Where(_ => panel != null)
                .Subscribe(_ =>
                {
                    panel?.SetActive(true);
                    IsOpningPanel = true;
                })
                .AddTo(this);

            closeButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    panel.SetActive(false);
                    IsOpningPanel = false;
                })
                .AddTo(this);
        }

        public static void OpenPanel()
        {
            openOptionPanelSubject.OnNext(Unit.Default);
        }
    }
}

