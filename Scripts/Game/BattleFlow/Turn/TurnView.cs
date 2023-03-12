using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Game.BattleFlow.Turn
{
    public sealed class TurnView : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private TextMeshProUGUI turnText;
        [SerializeField] private Color color1;
        [SerializeField] private Color color2;
        
        
        public void SetPanelActive(bool value)
        {
            panel.SetActive(value);
        }

        public void SetText(string text)
        {
            turnText.text = text;
        }

        public async UniTask TextAnimationAsync(PlayerID playerID, CancellationToken token)
        {
            SetPanelActive(true);
            var linkedToken = CancellationTokenSource.CreateLinkedTokenSource(token, this.GetCancellationTokenOnDestroy());
            Color color;
            if (playerID == PlayerID.Player1)
            {
                SetText("Player ターン");
                color = color1;
            }
            else
            {
                SetText("Enemy ターン");
                color = color2;
            }

            turnText.color = color;
            await UniTask.Delay(1000, cancellationToken: linkedToken.Token);
            float a = 1;
            while (turnText.color.a > 0)
            {
                a -= 0.01f;
                turnText.color = new Color(color.r,color.g,color.b,a);
                await UniTask.Delay(1, cancellationToken: linkedToken.Token);
            }
            SetPanelActive(false);
        }

    }
}