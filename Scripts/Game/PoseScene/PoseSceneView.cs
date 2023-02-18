using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.UI.Button;

namespace Game.PoseScene
{
    public class PoseSceneView : MonoBehaviour
    {
        [SerializeField] private Button ReturnTitleButton;
        [SerializeField] private Button ReturnStageSelectButton;
        [SerializeField] private Button ReturnTeamEditButton;

        public ButtonClickedEvent ReturnTitleButtonClcickedEvent => ReturnTitleButton.onClick;
        public ButtonClickedEvent ReturnStageSelectButtonClcickedEvent => ReturnStageSelectButton.onClick;
        public ButtonClickedEvent ReturnTeamEditButtonClcickedEvent => ReturnTeamEditButton.onClick;

        public IObservable<PointerEventData> ReturnTitleButtonSelectObservable => ReturnTitleButton.OnPointerEnterAsObservable();
        public IObservable<PointerEventData> ReturnStageSelectButtonSelectObservable => ReturnTitleButton.OnPointerEnterAsObservable();
        public IObservable<PointerEventData> ReturnTeamEditButtonSelectObservable => ReturnTitleButton.OnPointerEnterAsObservable();
    }
}

