using UnityEngine;
using UniRx;
using MyUtil;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine.UI;

namespace Game.Titles.Menu
{
    public class TitleMenuView : MonoBehaviour
    {
        [SerializeField] private GameObject menuPanel;
        [SerializeField] private GameObject toStartTextObj;
        [SerializeField] private TMP_Text toStartText;
        [SerializeField] private Button gameStartButton;
        [SerializeField] private Button howToPlayButton;
        [SerializeField] private Button optionButton;
        [SerializeField] private Button exitButton;

        //ボタンのクリック時イベントを登録用に公開
        public Button.ButtonClickedEvent GameStartButtonClickedEvent => gameStartButton.onClick;
        public Button.ButtonClickedEvent HowToPlayButtonClickedEvent => howToPlayButton.onClick;
        public Button.ButtonClickedEvent OptiontButtonClickedEvent => optionButton.onClick;
        public Button.ButtonClickedEvent ExitButtonClickedEvent => exitButton.onClick;

        private CancellationTokenSource linkedToken;

        private bool PressedAnyKey => Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1);

        private void Start()
        {
            linkedToken = CancellationTokenSource.CreateLinkedTokenSource(new CancellationToken(), this.GetCancellationTokenOnDestroy());

            toStartText.Blink(linkedToken.Token);
            ShowMenuPanelObservable();
        }

        private void ShowMenuPanelObservable()
        {
            //何かしらのキーを押したときにメニューを表示する
            this.ObserveEveryValueChanged(x => x.PressedAnyKey)
                .Where(x => x)
                .Take(1)
                .Subscribe(_ =>
                {
                    toStartTextObj.SetActive(false);
                    toStartText.gameObject.SetActive(false);
                    menuPanel.SetActive(true);

                    linkedToken.Cancel();
                })
                .AddTo(this);
        }
    }
}

