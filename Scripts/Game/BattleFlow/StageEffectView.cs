using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.BattleFlow
{
    public sealed class StageEffectView :MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private TextMeshProUGUI effectText;
        [SerializeField] private Color textColor;
        [SerializeField] private Image image;
        
        public void SetPanelActive(bool value)
        {
            panel.SetActive(value);
        }
        
        public async UniTask TextAnimationAsync(CancellationToken token)
        {
            var linkedToken = CancellationTokenSource.CreateLinkedTokenSource(token, this.GetCancellationTokenOnDestroy());
            effectText.color = textColor;
            image.color = Color.white;
            await UniTask.Delay(1000, cancellationToken: linkedToken.Token);
            float alpha = 1;
            while (effectText.color.a > 0)
            {
                alpha -= 0.01f;
                effectText.color = new Color(textColor.r,textColor.g,textColor.b,alpha);
                image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
                await UniTask.Delay(1, cancellationToken: linkedToken.Token);
            }
        }
    }
}