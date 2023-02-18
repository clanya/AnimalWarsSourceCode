using Game.Options;
using Game.Scenens;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Stages.Select
{
    public class StageSelectService
    {
        public void NextScnene()
        {
            if (SceneManager.GetSceneByBuildIndex((int)SceneType.EditTeam) != new Scene())
                return;

            SceneManager.UnloadSceneAsync((int)SceneType.SelectStage);
            SceneManager.LoadScene((int)SceneType.EditTeam, LoadSceneMode.Additive);
        }

        public void BackScene()
        {
            if (SceneManager.GetSceneByBuildIndex((int)SceneType.Title) != new Scene())
                return;

            SceneManager.UnloadSceneAsync((int)SceneType.SelectStage);
            SceneManager.LoadScene((int)SceneType.Title, LoadSceneMode.Additive);
        }

        public void OpenOption()
        {
            OptionView.OpenOptionPanel();
        }
    }
}

