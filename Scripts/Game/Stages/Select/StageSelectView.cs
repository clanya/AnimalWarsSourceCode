using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Stages.Select
{
    public class StageSelectView : MonoBehaviour
    {
        [SerializeField] private Button toNextButton;
        [SerializeField] private StageSelector stageSelector;
        [SerializeField] private Button backButton;
        [SerializeField] private Button optionButton;

        public Button.ButtonClickedEvent ToNextButtonClickedEvent => toNextButton.onClick;
        public Button.ButtonClickedEvent BackButtonClickedEvent => backButton.onClick;
        public Button.ButtonClickedEvent OptionButtonClickedEvent => optionButton.onClick;

        public IObservable<Unit> ToNextButtonClickedObservable => toNextButton.OnClickAsObservable();

        private void Awake()
        {
            toNextButton.interactable = false;
        }

        public void MakeUsableToNextButton()
        {
            toNextButton.interactable = true;
        }
    }
}

