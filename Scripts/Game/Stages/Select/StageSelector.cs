using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using System;
using UniRx.Triggers;
using Game.Audio;

namespace Game.Stages.Select
{
    public class StageSelector : MonoBehaviour
    {
        [SerializeField] private Button[] stageSelectButtonArray;
        [SerializeField] private BackGroundView backGroundView;

        private ISubject<int> selectedStageSubject = new Subject<int>();
        public IObservable<int> SelectedStageObservable => selectedStageSubject.AsObservable();

        private bool HadSelected = false;

        private void Start()
        {
            int maxLength = stageSelectButtonArray.Length;
            for(int i = 0; i < maxLength; i++)
            {
                int stageNumber = i;

                stageSelectButtonArray[i]
                    .OnClickAsObservable()
                    .Subscribe(_ =>
                    {
                        HadSelected = true;
                        backGroundView.ApplyStageBackGround(stageNumber);
                        selectedStageSubject.OnNext(stageNumber);
                        AudioPlayer.PlayClickButtonSE();
                    })
                    .AddTo(this);

                stageSelectButtonArray[i]
                    .OnPointerEnterAsObservable()
                    .TakeWhile(_=>!HadSelected)
                    .Subscribe(_ =>
                    {
                        backGroundView.ApplyStageBackGround(stageNumber);
                        AudioPlayer.PlaySelectButtonSE();
                    })
                    .AddTo(this);
            }
        }
    }
}

