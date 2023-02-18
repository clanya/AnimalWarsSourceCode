using Game.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using UniRx;

namespace Game.PoseScene
{
    public class PoseScenePresenter : ControllerBase, IStartable
    {
        private readonly PoseSceneView poseSceneView;
        private readonly PoseSceneService poseSceneService;

        [Inject]
        public PoseScenePresenter(PoseSceneView poseSceneView, PoseSceneService poseSceneService)
        {
            this.poseSceneView = poseSceneView;
            this.poseSceneService = poseSceneService;
        }

        public void Start()
        {
            poseSceneView.ReturnTitleButtonClcickedEvent.AddListener(() =>
            {
                AudioPlayer.PlayClickButtonSE();
                poseSceneService.ReturnTitleScene();
            });

            poseSceneView.ReturnStageSelectButtonClcickedEvent.AddListener(() =>
            {
                AudioPlayer.PlayClickButtonSE();
                poseSceneService.ReturnStageSelectScene();
            });

            poseSceneView.ReturnTeamEditButtonClcickedEvent.AddListener(() =>
            {
                AudioPlayer.PlayClickButtonSE();
                poseSceneService.ReturnTeamEditScene();
            });

            poseSceneView.ReturnTitleButtonSelectObservable
                .Subscribe(_ => AudioPlayer.PlaySelectButtonSE())
                .AddTo(this);

            poseSceneView.ReturnStageSelectButtonSelectObservable
                .Subscribe(_ => AudioPlayer.PlaySelectButtonSE())
                .AddTo(this);

            poseSceneView.ReturnTeamEditButtonSelectObservable
                .Subscribe(_ => AudioPlayer.PlaySelectButtonSE())
                .AddTo(this);
        }
    }
}

