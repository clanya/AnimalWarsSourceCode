using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Game.Audio;

namespace Game.Titles.Menu
{
    public class MouseCousorMenuSelector : BaseMenuSelector
    {
        [SerializeField] private MenuText[] menuTexts;

        private void Start()
        {
            foreach(var text in menuTexts)
            {
                text.OnMouseCursorObservable
                    .Subscribe(_ =>
                    {
                        selectedMenu.Value = text.MyMenuType;
                        AudioPlayer.PlaySelectButtonSE();
                    })
                    .AddTo(this);
            }
        }
    }
}

