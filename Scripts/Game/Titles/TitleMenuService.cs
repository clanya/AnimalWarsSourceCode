using Game.Options;
using Game.Scenens;
using UnityEngine;
using UnityEngine.SceneManagement;
using Game.HowToPlay;

namespace Game.Titles
{
    public class TitleMenuService
    {
        public void LoadNextScenen()
        {
            if (SceneManager.GetSceneByBuildIndex((int)SceneType.SelectStage) != new Scene())
                return;

            SceneManager.UnloadSceneAsync((int)SceneType.Title);
            SceneManager.LoadScene((int)SceneType.SelectStage, LoadSceneMode.Additive);
        }

        public void OpenHowToPlay()
        {
            HowToPlayPanelView.OpenPanel();
        }

        public void OpenOption()
        {
            OptionView.OpenOptionPanel();
        }

        public void ExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
            Application.Quit();//ゲームプレイ終了
#endif
        }

        public void LoadOptionScene()
        {
            SceneManager.LoadScene((int)SceneType.Option, LoadSceneMode.Additive);
        }

        public void LoadHowToPlayScene()
        {
            SceneManager.LoadScene("HowToPlayScene", LoadSceneMode.Additive);
        }
    }
}

