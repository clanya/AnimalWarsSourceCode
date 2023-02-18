using Game.Audio;
using Game.Titles.Menu;
using VContainer;
using VContainer.Unity;
using UnityEngine.SceneManagement;
using Game.Scenens;

namespace Game.Titles
{
    public class TitleMenuPresenter : IStartable
    {
        private readonly TitleMenuService titleMenuService;
        private readonly TitleMenuView titleMenuView;

        [Inject]
        public TitleMenuPresenter(TitleMenuService titleMenuService, TitleMenuView titleMenuView)
        {
            this.titleMenuService = titleMenuService;
            this.titleMenuView = titleMenuView;
        }

        void IStartable.Start()
        {
            //ボタンクリック時の処理を登録
            titleMenuView.GameStartButtonClickedEvent.AddListener(() =>
            {
                titleMenuService.LoadNextScenen();
                AudioPlayer.PlayClickButtonSE();
            });

            titleMenuView.HowToPlayButtonClickedEvent.AddListener(() =>
            {
                titleMenuService.OpenHowToPlay();
                AudioPlayer.PlayClickButtonSE();
            });

            titleMenuView.OptiontButtonClickedEvent.AddListener(() =>
            {
                titleMenuService.OpenOption();
                AudioPlayer.PlayClickButtonSE();
            });

            titleMenuView.ExitButtonClickedEvent.AddListener(() =>
            {
                titleMenuService.ExitGame();
                AudioPlayer.PlayClickButtonSE();
            });

            //オプションのシーンがなければロードする
            if (SceneManager.GetSceneByBuildIndex((int)SceneType.Option) == new Scene())
            {
                titleMenuService.LoadOptionScene();
            }

            //遊び方のシーンがなければロード
            if (SceneManager.GetSceneByName("HowToPlayScene") == new Scene())
            {
                titleMenuService.LoadHowToPlayScene();
            }
        }
    }
}

