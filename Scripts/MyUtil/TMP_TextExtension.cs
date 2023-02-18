using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using TMPro;

namespace MyUtil
{
    public static class TMP_TextExtension
    {
        /// <summary>
        /// アニメーションカーブに従ってテキストを点滅させる
        /// </summary>
        /// <param name="textMesh"></param>
        /// <param name="curve"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async UniTaskVoid Blink(this TMP_Text textMesh, AnimationCurve curve, CancellationToken token)
        {
            float time = 0;
            while (true)
            {
                float alpha = curve.Evaluate(time);
                Color color = textMesh.color;
                color.a = alpha;
                textMesh.color = color;
                await UniTask.Delay(1, cancellationToken: token);
                time += Time.deltaTime;

                if (token.IsCancellationRequested)
                    break;
            }
        }

        /// <summary>
        /// 指定がなければデフォルトのライナーに従って点滅させる
        /// </summary>
        /// <param name="textMesh"></param>
        /// <param name="token"></param>
        public static void Blink(this TMP_Text textMesh, CancellationToken token)
        {
            AnimationCurve curve = AnimationCurve.Linear(0, 1, 1, 0);
            curve.preWrapMode = WrapMode.PingPong;
            curve.postWrapMode = WrapMode.PingPong;
            textMesh.Blink(curve, token).Forget();
        }
    }
}

