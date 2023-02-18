using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Character.View
{
    public sealed class CharacterView : MonoBehaviour,IPointerClickHandler
    {
        private Subject<Unit> onPointerClickSubject;
        public IObservable<Unit> OnPointerClickObservable => onPointerClickSubject;
        [SerializeField] private Slider hpSlider;
        [SerializeField] private TextMeshProUGUI hpText;

        private void Awake()
        {
            onPointerClickSubject = new Subject<Unit>();
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                onPointerClickSubject.OnNext(Unit.Default);
            }
        }
        
        public void SetSliderFillColor(Color color)
        {
            var image = hpSlider.transform.GetChild(1).GetChild(0).gameObject.GetComponent<Image>();
            image.color = color;
        }

        public void SetHp(int value,int maxValue)
        {
            hpSlider.value = (float)value / maxValue;
            hpText.text = value.ToString();
        }

        private void OnDestroy()
        {
            onPointerClickSubject.Dispose();
        }
        
    }
}