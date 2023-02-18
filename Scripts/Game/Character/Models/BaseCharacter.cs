using System;
using Cysharp.Threading.Tasks;
using Game.BattleFlow;
using System.Threading;
using Game.Character.Animations;
using Game.Character.Skills;
using UniRx;
using UnityEngine;
using Game.Audio;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace Game.Character.Models
{
    //Todo: モノビヘに依存せずに作れるはず InitとOnDestroyをPresenterなりで読んであげればいい？
    public class BaseCharacter : MonoBehaviour, IDamageable, IHealble
    {
        private SkillManager skillManager;
        public PlayerID PlayerID { get; private set; }
        private CharacterParam param;
        public CharacterParam Param => param;

        protected CharacterCurrentStatus currentStatus;
        public CharacterCurrentStatus CurrentStatus => currentStatus;
        private readonly BoolReactiveProperty isMovable = new(true);
        public IReadOnlyReactiveProperty<bool> IsMovable => isMovable;
        private readonly BoolReactiveProperty isActionable = new(true);
        public IReadOnlyReactiveProperty<bool> IsActionable => isActionable;
        private readonly BoolReactiveProperty isDead = new(false);
        public IReadOnlyReactiveProperty<bool> IsDead => isDead;

        private readonly ReactiveProperty<CharacterViewState> state = new();
        public IReadOnlyReactiveProperty<CharacterViewState> State => state;
        public Vector2Int Position { get; private set; }

        private CharacterAnimationPlayer animationPlayer;
        private SkillAnimationPlayer skillAnimationPlayer;
        
        private readonly Subject<Unit> dodgeAttackSubject = new();
        public IObservable<Unit> DodgeAttackObservable => dodgeAttackSubject;

        private Vector2Int lookDir { get
            {
                var dir = transform.forward.normalized;
                if (Mathf.Abs(dir.x) > Mathf.Abs(dir.z))
                {
                    return new Vector2Int((int)Mathf.Sign(dir.x), 0);
                }

                return new Vector2Int(0, (int)Mathf.Sign(dir.z));
            }}
        
        public void Initialize()
        {
            currentStatus = new CharacterCurrentStatus(param);
            animationPlayer = GetComponent<CharacterAnimationPlayer>();
            skillAnimationPlayer = FindObjectOfType<SkillAnimationPlayer>();

            isDead
                .Where(x => x)
                .Take(1)
                .Subscribe(async _ =>
                {
                    await animationPlayer.PlayDieAnimation(this.GetCancellationTokenOnDestroy());
                    gameObject.SetActive(false);
                })
                .AddTo(this);
        }

        private void OnDestroy()
        {
            currentStatus.Dispose();
            isMovable.Dispose();
            isActionable.Dispose();
            isDead.Dispose();
            state.Dispose();
        }

        public async UniTask MoveAsync(Stack<Vector2Int> route, CancellationToken token)
        {
            var beforePosition = Position;
            while (route.Count > 0)
            {
                var nextPoint = route.Pop();
                if (beforePosition+lookDir==nextPoint)
                {
                    await MoveAsync(nextPoint, token);
                }
                else
                {
                    animationPlayer.PlayMoveAnimation(0);

                    var vec = nextPoint - Position;
                    await TurnToLookAt(vec, token);
                    
                    await MoveAsync(nextPoint, token);
                }

                beforePosition = nextPoint;
            }

            animationPlayer.PlayMoveAnimation(0);
        }

        //Note: transformはViewにかくべきであり、ここに書くべきはPositionの値を変更するだことだけか。
        private async UniTask MoveAsync(Vector2Int targetPosition, CancellationToken token)
        {
            var linkedToken = CancellationTokenSource.CreateLinkedTokenSource(token, this.GetCancellationTokenOnDestroy());
            AudioPlayer.PlayRunSE();
            animationPlayer.PlayMoveAnimation(1);

            if (Position.x == targetPosition.x)
            {
                if (targetPosition.y > Position.y)
                {
                    await UniTask.WaitUntil(() => transform.position.z > targetPosition.y, cancellationToken: linkedToken.Token);
                }
                else
                {
                    await UniTask.WaitUntil(() => transform.position.z < targetPosition.y, cancellationToken: linkedToken.Token);
                }
            }
            else if (Position.y == targetPosition.y)
            {
                if (targetPosition.x > Position.x)
                {
                    await UniTask.WaitUntil(() => transform.position.x > targetPosition.x, cancellationToken: linkedToken.Token);
                }
                else
                {
                    await UniTask.WaitUntil(() => transform.position.x < targetPosition.x, cancellationToken: linkedToken.Token);
                }
            }

            transform.position = new Vector3(targetPosition.x, 0, targetPosition.y);
            
            AudioPlayer.StopRunSE();
            Position = targetPosition;
        }

        public async UniTask AttackAsync(BaseCharacter targetCharacter)
        {
            int value;
            if (currentStatus.AttackType.Value == AttackType.Physical)
            {
                value = Mathf.Max(0,currentStatus.AttackPower.Value - targetCharacter.currentStatus.DefensePower.Value);
            }
            else
            {
                value = Mathf.Max(0,currentStatus.AttackPower.Value - targetCharacter.currentStatus.MagicDefensePower.Value);
            }

            animationPlayer.attackHitTimingObservable
                .Take(1)
                .Subscribe(_ =>
                {
                    targetCharacter.TakeDamage(value);
                })
                .AddTo(this);

            if (param.AttackType == AttackType.Physical)
            {
                await animationPlayer.PlayAttackAnimation(param.Type, this.GetCancellationTokenOnDestroy());
            }
            else if(param.AttackType == AttackType.Magic)
            {
                await animationPlayer.PlayAttackAnimation(param.Type, this.GetCancellationTokenOnDestroy(),targetCharacter.transform);
            }
            
            bool canDoubleAttack = currentStatus.Speed.Value - targetCharacter.CurrentStatus.Speed.Value >= 5 
                                   && targetCharacter.CurrentStatus.Hp.Value > 0;
            if (canDoubleAttack)
            {

                animationPlayer.attackHitTimingObservable
                .Take(1)
                .Subscribe(_ =>
                {
                    targetCharacter.TakeDamage(value);
                })
                .AddTo(this);

                if (currentStatus.AttackType.Value == AttackType.Physical)
                {
                    value = Mathf.Max(0,currentStatus.AttackPower.Value - targetCharacter.currentStatus.DefensePower.Value);
                }
                else
                {
                    value = Mathf.Max(0,currentStatus.AttackPower.Value - targetCharacter.currentStatus.MagicDefensePower.Value);
                }
                if (param.AttackType == AttackType.Physical)
                {
                    await animationPlayer.PlayAttackAnimation(param.Type, this.GetCancellationTokenOnDestroy());
                }
                else
                {
                    await animationPlayer.PlayAttackAnimation(param.Type, this.GetCancellationTokenOnDestroy(),targetCharacter.transform);

                }
            }
            SetIsActionable(false);
        }

        public void Standby()
        {
            if (isMovable.Value)
            {
                SetIsMovable(false);
            }
            SetIsActionable(false);
        }

        public async UniTask UseSkillAsync(BaseCharacter target = null)
        {
            skillManager.Execute(this,target);
            await skillAnimationPlayer.PlaySkillAnimation(this, this.GetCancellationTokenOnDestroy(), target);
            SetIsActionable(false);
        }

        public void TakeDamage(int value)
        {
            var rand = Random.value;
            if (currentStatus.EvadeRate > rand)
            {
                dodgeAttackSubject.OnNext(Unit.Default);
                return;
            }
            animationPlayer.PlayDamagedAnimation(this.GetCancellationTokenOnDestroy()).Forget();
            AudioPlayer.PlayDamageSE();

            //0未満にならないようにダメージを受ける
            int hp = Mathf.Max(currentStatus.Hp.Value - value, 0);
            currentStatus.SetHp(hp);
        }

        public void TakeHeal(int value)
        {
            //ParameterのHPを超えないように回復
            int hp = Mathf.Min(currentStatus.Hp.Value + value, param.Hp);
            currentStatus.SetHp(hp);
        }

        public void SetSkillManager(SkillManager skillManager)
        {
            this.skillManager = skillManager;
        }

        public void SetPlayerID(PlayerID id)
        {
            this.PlayerID = id;
        }

        public void SetParam(CharacterParam value)
        {
            this.param = value;
        }
        
        public void SetIsMovable(bool value)
        {
            isMovable.Value = value;
        }

        public void SetIsActionable(bool value)
        {
            isActionable.Value = value;
        }

        public void SetIsDead(bool value)
        {
            isDead.Value = value;
        }
        
        public void SetState(CharacterViewState state)
        {
            this.state.Value = state;
        }

        public void InitializeCurrentPosition(Vector2Int position)
        {
            Position = position;
        }

        //todo: メソッド名変更
        public void SetProperty(bool value,CharacterViewState state)
        {
            isMovable.Value = value;
            isActionable.Value = value;
            this.state.Value = state;
        }
        
        public async UniTask TurnToLookAt(Vector2Int vec, CancellationToken token)
        {
            if (vec == lookDir) return;

            if (Mathf.Abs(lookDir.x - vec.x) == 2 || Mathf.Abs(lookDir.y - vec.y) == 2)
            {
                await animationPlayer.PlayTurnAnimation(TurnType.LookBack, token);
            }
            else if (lookDir.x == 1)
            {
                if (vec.y == 1)
                {
                    await animationPlayer.PlayTurnAnimation(TurnType.LookLeft, token);
                }
                else
                {
                    await animationPlayer.PlayTurnAnimation(TurnType.LookRight, token);
                }
            }
            else if (lookDir.x == -1)
            {
                if (vec.y == 1)
                {
                    await animationPlayer.PlayTurnAnimation(TurnType.LookRight, token);
                }
                else
                {
                    await animationPlayer.PlayTurnAnimation(TurnType.LookLeft, token);
                }
            }
            else if (lookDir.y == 1)
            {
                if (vec.x == 1)
                {
                    await animationPlayer.PlayTurnAnimation(TurnType.LookRight, token);
                }
                else
                {
                    await animationPlayer.PlayTurnAnimation(TurnType.LookLeft, token);
                }
            }
            else if (lookDir.y == -1)
            {
                if (vec.x == 1)
                {
                    await animationPlayer.PlayTurnAnimation(TurnType.LookLeft, token);
                }
                else
                {
                    await animationPlayer.PlayTurnAnimation(TurnType.LookRight, token);
                }
            }
        }
    }
}