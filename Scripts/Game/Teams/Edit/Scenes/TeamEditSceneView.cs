using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using Game.Options;

namespace Game.Teams.Edit.Scenes
{
    public class TeamEditSceneView : MonoBehaviour
    {
        [SerializeField] private Button buttleStartButton;
        [SerializeField] private Button backButton;
        [SerializeField] private TMP_Text stageText;
        [SerializeField] private Image backGroundImage;

        [SerializeField] private Sprite[] backGroundSprites;
        [SerializeField] private Button optionButton;

        public Button.ButtonClickedEvent ButtleStartButtonClickEvent => buttleStartButton.onClick;
        public Button.ButtonClickedEvent BackButtonClickEvent => backButton.onClick;

        public void Start()
        {
            optionButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    OptionView.OpenOptionPanel();
                })
                .AddTo(this);
        }

        public void SetStageText(int stageNumber)
        {
            stageText.text = $"ステージ {stageNumber}";
        }

        public void SetBackGrount(int stageNumber)
        {
            backGroundImage.sprite = backGroundSprites[stageNumber];
        }

        public void SetStartButtonInteractable(bool interactable)
        {
            buttleStartButton.interactable = interactable;
        }
    }
}

