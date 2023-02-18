using Game.Scenens;
using KanKikuchi.AudioManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.PoseScene
{
    public class PoseSceneService
    {
        public void ReturnTitleScene()
        {
            if (SceneManager.GetSceneByBuildIndex((int)SceneType.Title) != new Scene())
                return;

            SceneManager.UnloadSceneAsync((int)SceneType.Battle);
            SceneManager.UnloadSceneAsync("PoseScene");
            UnLoadStageScene();
            BGMManager.Instance.Stop();
            SEManager.Instance.Stop();
            SceneManager.LoadScene((int)SceneType.Title, LoadSceneMode.Additive);
        }

        public void ReturnStageSelectScene()
        {
            if (SceneManager.GetSceneByBuildIndex((int)SceneType.SelectStage) != new Scene())
                return;

            SceneManager.UnloadSceneAsync((int)SceneType.Battle);
            SceneManager.UnloadSceneAsync("PoseScene");
            UnLoadStageScene();
            BGMManager.Instance.Stop();
            SEManager.Instance.Stop();
            SceneManager.LoadScene((int)SceneType.SelectStage, LoadSceneMode.Additive);
        }

        public void ReturnTeamEditScene()
        {
            if (SceneManager.GetSceneByBuildIndex((int)SceneType.EditTeam) != new Scene())
                return;

            SceneManager.UnloadSceneAsync((int)SceneType.Battle);
            SceneManager.UnloadSceneAsync("PoseScene");
            UnLoadStageScene();
            BGMManager.Instance.Stop();
            SEManager.Instance.Stop();
            SceneManager.LoadScene((int)SceneType.EditTeam, LoadSceneMode.Additive);
        }

        public void UnLoadStageScene()
        {
            var noneScene = new Scene();
            for(int i = 0; i < 4; i++)
            {
                var stageScene = SceneManager.GetSceneByName("Stage_" + i.ToString("00"));
                if (stageScene != noneScene)
                {
                    SceneManager.UnloadSceneAsync(stageScene);
                }
            }
        }
    }
}

