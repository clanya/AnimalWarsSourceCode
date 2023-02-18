using System.Collections;
using System.Collections.Generic;
using UniRx.Triggers;
using UnityEngine;
using UniRx;

namespace Game.Character.UI
{
    public class CharacterUIController : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        private Transform characterTransform;

        private void Start()
        {
            characterTransform = transform;

            this.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    canvas.transform.position = characterTransform.position - Vector3.forward*0.45f;
                    canvas.transform.rotation = Quaternion.Euler(new Vector3(70, 0, 0));
                })
                .AddTo(this);
        }
    }
}

