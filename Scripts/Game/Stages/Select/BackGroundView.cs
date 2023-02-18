using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Stages.Select
{
    public class BackGroundView : MonoBehaviour
    {
        [SerializeField] private Sprite[] backGroundSprites;
        [SerializeField] private Image image;

        public void ApplyStageBackGround(int stageNum)
        {
            image.sprite = backGroundSprites[stageNum];
        }
    }
}

