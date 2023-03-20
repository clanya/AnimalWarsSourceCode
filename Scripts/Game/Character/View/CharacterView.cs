using System;
using System.Threading;
using Cysharp.Threading.Tasks;
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
        private CancellationToken token;
        private const float AnimationSpeed = 0.05f;

        private void Awake()
        {
            onPointerClickSubject = new Subject<Unit>();
            token = this.GetCancellationTokenOnDestroy();
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

        public async UniTask SetHpAnimationAsync(int value,int maxValue)
        {
            int currentValue = int.Parse(hpText.text);
            var sliderTask =  SliderAnimationAsync(value, currentValue, maxValue, token);
            var tmpTask = TextAnimationAsync(value, currentValue, token);
            await UniTask.WhenAll(sliderTask, tmpTask);
        }

        /// <summary>
        /// 現在の値から目的の値まで徐々に近づいていくアニメーション
        /// </summary>
        /// <param name="目的の値"></param>
        /// <param name="現在の値"></param>
        /// <param name="最大値"></param>
        private async UniTask SliderAnimationAsync(int targetValue,int currentValue,int maxValue,CancellationToken token)
        {
            var current = currentValue;
            while (current != targetValue)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(AnimationSpeed), cancellationToken: token);
                if (current > targetValue)
                {
                    current--;
                }
                else
                {
                    current++;
                }

                hpSlider.value = (float) current / maxValue;
            }
        }

        private async UniTask TextAnimationAsync(int targetValue,int currentValue,CancellationToken token)
        {
            var current = currentValue;
            while (current != targetValue)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(AnimationSpeed), cancellationToken: token);
                if (current > targetValue)
                {
                    current--;
                }
                else
                {
                    current++;
                }

                hpText.text = current.ToString();
            }
        }

        private void OnDestroy()
        {
            onPointerClickSubject.Dispose();
        }
        
    }
}