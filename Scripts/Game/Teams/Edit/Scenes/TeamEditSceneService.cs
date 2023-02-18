using Game.Scenens;
using KanKikuchi.AudioManager;
using UnityEngine.SceneManagement;

namespace Game.Teams.Edit.Scenes
{
    public class TeamEditSceneService
    {
        public void LoadNextScene(int stageNumber)
        {
            if (SceneManager.GetSceneByBuildIndex((int)SceneType.Battle) != new Scene())
                return;

            SceneManager.UnloadSceneAsync((int)SceneType.EditTeam);
            SceneManager.LoadScene((int)SceneType.Battle, LoadSceneMode.Additive);
            PlayBGM(stageNumber);
            //ステージだけのシーンを追加
            SceneManager.LoadScene("Stage_"+stageNumber.ToString("00"), LoadSceneMode.Additive);
            //ポーズ用のシーンを追加
            SceneManager.LoadScene("PoseScene", LoadSceneMode.Additive);
        }

        public void LoadPreviousScene()
        {
            SceneManager.UnloadSceneAsync((int)SceneType.EditTeam);
            SceneManager.LoadScene((int)SceneType.SelectStage, LoadSceneMode.Additive);
        }

        private void PlayBGM(int number)
        {
            switch (number)
            {
                case 0:
                case 1:
                    BGMManager.Instance.Play(BGMPath.BGM_01);
                    break;
                case 2:
                    BGMManager.Instance.Play(BGMPath.BGM_02);
                    break;
                case 3:
                    BGMManager.Instance.Play(BGMPath.BGM_03);
                    break;
            }
        }
    }
}

