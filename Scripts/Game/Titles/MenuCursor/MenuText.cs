using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UniRx;
using System;

namespace Game.Titles.Menu
{
    public class MenuText : MonoBehaviour, IPointerEnterHandler
    {
        [SerializeField] private MenuType myMenuType;
        public MenuType MyMenuType => myMenuType;

        private ISubject<Unit> onMouseCursorSubject = new Subject<Unit>();
        public IObservable<Unit> OnMouseCursorObservable => onMouseCursorSubject.AsObservable();

        public void OnPointerEnter(PointerEventData eventData)
        {
            onMouseCursorSubject.OnNext(Unit.Default);
        }
    }
}

