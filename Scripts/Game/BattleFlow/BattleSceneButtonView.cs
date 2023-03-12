using Game.HowToPlay;
using Game.Options;
using Game.PoseScene;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.BattleFlow
{
    public class BattleSceneButtonView : MonoBehaviour
    {
        [SerializeField] private Button poseButton;
        [SerializeField] private Button optionButton;
        [SerializeField] private Button helpButton;

        private void Start()
        {
            poseButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    PoseScenePanelViewer.OpenPosePanel();
                })
                .AddTo(this);

            optionButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    OptionView.OpenOptionPanel();
                })
                .AddTo(this);

            helpButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    HowToPlayPanelView.OpenPanel();
                })
                .AddTo(this);
        }
    }
}

