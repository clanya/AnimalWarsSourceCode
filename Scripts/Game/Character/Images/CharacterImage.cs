using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Character.Images
{
    public class CharacterImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private Image imageComponent;
        [SerializeField] private CharacterType characterType;
        public CharacterType CharacterType => characterType;

        //ポインターが乗ってるときその位置を送る
        private ISubject<Vector2> onPointerPositionSubject = new Subject<Vector2>();
        public IObservable<Vector2> OnPointerPositionObservable => onPointerPositionSubject;

        //ドラッグしてるときそのポインターの位置を送る
        private ISubject<Vector2> dragPointerPositionSubject = new Subject<Vector2>();
        public IObservable<Vector2> DragPointerPositionObservable => dragPointerPositionSubject;

        //ポインターが出て行ったタイミングを流す
        private ISubject<Unit> exitPointerSubject = new Subject<Unit>();
        public IObservable<Unit> ExitPointerObservable => exitPointerSubject;

        //ドラッグが始まったタイミングを流す
        private ISubject<Unit> startDragSubject = new Subject<Unit>();
        public IObservable<Unit> StartDragObservable => startDragSubject;

        //ドラッグが終わったタイミングを流す
        private ISubject<Unit> endDragSubject = new Subject<Unit>();
        public IObservable<Unit> EndDragObservable => endDragSubject;

        private Coroutine sendPointerPositionCoroutine;
        public bool IsDragging { get; private set; }
        public bool IsSelectable { get; private set; } = true;

        private void Start()
        {
            this.ObserveEveryValueChanged(x => x.IsSelectable)
                .Subscribe(x =>
                {
                    if (x)
                    {
                        var color = imageComponent.color;
                        color.a = 1f;
                        imageComponent.color = color;
                    }
                    else
                    {
                        var color = imageComponent.color;
                        color.a = 0.5f;
                        imageComponent.color = color;
                    }
                })
                .AddTo(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            onPointerPositionSubject.OnNext(eventData.position);
            sendPointerPositionCoroutine = StartCoroutine(SendPointerPosition());
        }

        private IEnumerator SendPointerPosition()
        {
            while (true)
            {
                onPointerPositionSubject.OnNext(Input.mousePosition);
                yield return null;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            StopCoroutine(sendPointerPositionCoroutine);
            exitPointerSubject.OnNext(Unit.Default);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            IsDragging = true;
            dragPointerPositionSubject.OnNext(eventData.position);
            startDragSubject.OnNext(Unit.Default);
        }

        public void OnDrag(PointerEventData eventData)
        {
            dragPointerPositionSubject.OnNext(eventData.position);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            IsDragging = false;
            endDragSubject.OnNext(Unit.Default);
        }

        public void SetCharacterImage(Sprite sprite)
        {
            imageComponent.sprite = sprite;
        }

        public void SetSrelrctable(bool selectable)
        {
            IsSelectable = selectable;
        }
    }
}

