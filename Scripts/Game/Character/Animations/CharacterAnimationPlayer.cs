using Cysharp.Threading.Tasks;
using Game.Effects;
using Game.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Game.Character.Skills;
using UnityEngine;
using UniRx;
using Game.Audio;

namespace Game.Character.Animations
{
    public enum TurnDirection
    {
        Left=1,
        Right=-1,
    }

    public enum TurnType
    {
        LookLeft,
        LookRight,
        LookBack,
    }

    public class CharacterAnimationPlayer : MonoBehaviour
    {
        //[SerializeField] private CharacterType characterType;
        [SerializeField] private Animator animator;
        [Tooltip("単位はミリ秒"), SerializeField] private float moveSwitchingTime = 500f;

        [Range(0f,1f), SerializeField] private float attackEffectPlayTime = 0.5f;
        [Range(0f,1f), SerializeField] private float damagedEffectPlayTime = 0.5f;

        [SerializeField] private Transform attackEffectPoint;
        [SerializeField] private Transform damagedEffectPoint;
        [SerializeField] private Transform deathEffectPoint;

        private readonly AnimationCurve switchToRunCurve = AnimationCurve.EaseInOut(0, 0, 0.5f, 1);
        private EffectPlayer effectPlayer;

        private ISubject<Unit> attackHitTimingSubject = new Subject<Unit>();
        public IObservable<Unit> attackHitTimingObservable => attackHitTimingSubject;

        private void Start()
        {
            effectPlayer = FindObjectOfType<EffectPlayer>();
        }

        //呼び出し側からスピードを指定できる場合（呼び出し側は複数呼び出す）
        public void PlayMoveAnimation(float moveSpeed)
        {
            animator.SetFloat(AnimatorParameterHash.GetParameterHash(AnimatorParameterType.Move), moveSpeed);
        }

        public async UniTask PlayTurnAnimation(float turnAngle, CancellationToken token, float turnTime=0.5f)
        {
            Quaternion startRotation = transform.localRotation;
            Quaternion endRotation = startRotation * Quaternion.Euler(0, turnAngle, 0);

            float time = 0;
            while (true)
            {
                if (time >= turnTime) break;

                time += Time.deltaTime;
                var rate = Mathf.Clamp01(time / turnTime);
                gameObject.transform.localRotation = Quaternion.Slerp(startRotation, endRotation, rate);

                await UniTask.DelayFrame(1, cancellationToken: token);
            }
        }

        public async UniTask PlayTurnAnimation(float turnAngle, TurnDirection turnDirection, CancellationToken token, float turnTime = 0.5f)
        {
            var angle = turnAngle * (int)turnDirection;
            await PlayTurnAnimation(angle, token, turnTime: turnTime);
        }

        public async UniTask PlayTurnAnimation(TurnType turnType, CancellationToken token, float turnTime = 0.5f)
        {
            switch (turnType)
            {
                case TurnType.LookLeft:
                    await PlayTurnAnimation(-90f, token, turnTime: turnTime);
                    break;
                case TurnType.LookRight:
                    await PlayTurnAnimation(90f, token, turnTime: turnTime);
                    break;
                case TurnType.LookBack:
                    await PlayTurnAnimation(180f, token, turnTime: turnTime);
                    break;
            }
        }

        public async UniTask PlayAttackAnimation(CharacterType characterType, CancellationToken token,Transform magicEffectPointTransform = null)
        {
            animator.SetTrigger(AnimatorParameterHash.GetParameterHash(AnimatorParameterType.Attack));
            await AnimationTransitionWaiter.WaitStateTime(attackEffectPlayTime, (int)AnimatorLayerType.Attack, AnimatorStateTagHash.GetTagHash(AnimatorStateTagType.Attack),  animator, token);
            if (magicEffectPointTransform is not null)
            {
                effectPlayer.PlayAttackEffect(characterType, magicEffectPointTransform.position);
                attackHitTimingSubject.OnNext(Unit.Default);
                AudioPlayer.PlayMagicSE();
                await AnimationTransitionWaiter.WaitAnimationTransition((int)AnimatorLayerType.Attack, AnimatorStateTagHash.GetTagHash(AnimatorStateTagType.Empty), animator, token);
                return;
            }
            effectPlayer.PlayAttackEffect(characterType, attackEffectPoint.position);
            attackHitTimingSubject.OnNext(Unit.Default);
            await AnimationTransitionWaiter.WaitAnimationTransition((int)AnimatorLayerType.Attack, AnimatorStateTagHash.GetTagHash(AnimatorStateTagType.Empty), animator, token);
        }

        public async UniTask PlayDamagedAnimation(CancellationToken token)
        {
            animator.SetTrigger(AnimatorParameterHash.GetParameterHash(AnimatorParameterType.Damaged));
            await AnimationTransitionWaiter.WaitStateTime(damagedEffectPlayTime, (int)AnimatorLayerType.Damaged, AnimatorStateTagHash.GetTagHash(AnimatorStateTagType.Damaged), animator, token);
            effectPlayer.PlayDamagedEffect(damagedEffectPoint.position);
            await AnimationTransitionWaiter.WaitAnimationTransition((int)AnimatorLayerType.Damaged, AnimatorStateTagHash.GetTagHash(AnimatorStateTagType.Empty), animator, token);
        }

        public async UniTask PlayDieAnimation(CancellationToken token)
        {
            animator.SetTrigger(AnimatorParameterHash.GetParameterHash(AnimatorParameterType.Die));
            AudioPlayer.PlayDownSE();
            await AnimationTransitionWaiter.WaitStateTime(1, (int)AnimatorLayerType.Die, AnimatorStateTagHash.GetTagHash(AnimatorStateTagType.Die), animator, token);
            effectPlayer.PlayDeathEffect(deathEffectPoint.position);
        }

        public async UniTask PlaySkillAnimation(CancellationToken token)
        {
            animator.SetTrigger(AnimatorParameterHash.GetParameterHash(AnimatorParameterType.Attack));
            await AnimationTransitionWaiter.WaitAnimationTransition((int)AnimatorLayerType.Attack, AnimatorStateTagHash.GetTagHash(AnimatorStateTagType.Attack), animator, token);
            await AnimationTransitionWaiter.WaitAnimationTransition((int)AnimatorLayerType.Attack, AnimatorStateTagHash.GetTagHash(AnimatorStateTagType.Empty), animator, token);
            await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: token);
        }
    }
}
