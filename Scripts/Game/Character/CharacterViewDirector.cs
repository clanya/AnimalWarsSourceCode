using System;
using System.Collections.Generic;
using System.Linq;
using Game.Character.View;
using UniRx;
using UnityEngine;

namespace Game.Character
{
    public sealed class CharacterViewDirector : IDisposable
    {
        private readonly Subject<CharacterView> clickedCharacterSubject = new();
        public IObservable<CharacterView> ClickedCharacterObservable => clickedCharacterSubject;
        
        //それぞれのキャラクターがクリックされたときの処理
        public void OnPointerClickedObservable(CharacterView view)
        {
            view.OnPointerClickObservable
                .Subscribe(_ =>
                {
                    clickedCharacterSubject.OnNext(view);
                }).AddTo(view);
        }

        /// <summary>
        /// 不要になるとき必ず呼び出し。
        /// </summary>
        public void Dispose()
        {
            clickedCharacterSubject.Dispose();
        }

    }
}