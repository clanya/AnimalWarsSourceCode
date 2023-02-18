using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.HowToPlay
{
    public class HowToPlayView : MonoBehaviour
    {
        [SerializeField] private Button[] menuButtons;
        [SerializeField] private GameObject[] explanePrefabs;
        [SerializeField] private Transform parent;

        private List<GameObject> explaneObjects = new List<GameObject>();
        private GameObject currentExplane;

        void Start()
        {
            foreach(var obj in explanePrefabs)
            {
                var explaneObj=Instantiate(obj, parent);
                explaneObj.SetActive(false);
                explaneObjects.Add(explaneObj);
                var rectTransform = explaneObj.GetComponent<RectTransform>();
                //rectTransform.anchorMin = new Vector2(0.3f, 0.082f);
            }

            for(int i = 0; i < menuButtons.Length; i++)
            {
                int index = i;

                menuButtons[index].OnClickAsObservable()
                    .Subscribe(_ =>
                    {
                        currentExplane?.SetActive(false);
                        explaneObjects[index].SetActive(true);
                        currentExplane = explaneObjects[index];
                    })
                    .AddTo(this);
            }
        }
    }
}

