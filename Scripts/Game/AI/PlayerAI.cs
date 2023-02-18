using Cysharp.Threading.Tasks;
using Game.BattleFlow;
using Game.Character;
using Game.Character.Models;
using Game.Character.Skills;
using Game.Stages;
using Game.Stages.Explorers;
using MyUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Game.BattleFlow.Character;
using Game.BattleFlow.Turn;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.AI
{
    public class PlayerAI : IStartable, IDisposable
    {
        [Inject] private CharacterManager characterManager;
        [Inject] private SkillManager skillManager;
        [Inject] private CameraController cameraController;
        [Inject] private TurnManager turnManager;
        [Inject] private ExplorerFactory explorerFactory;

        private MovablePointsExplorer movablePointsExplorer;
        private MoveRouteExplorer moveRouteExplorer;
        private AttackablePointsExplorer attackablePointsExplorer;
        private CancellationTokenSource token;
        private PlayerID playerID;

        private IEnumerable<BaseCharacter> friendCharacters => characterManager.EnemyCharacterList;
        private IEnumerable<BaseCharacter> enemyCharacters => characterManager.PlayerCharacterList;

        private IEnumerable<Vector2Int> enemyAttackArea;
        private IEnumerable<KeyValuePair<BaseCharacter, IEnumerable<BaseCharacter>>> actionCharactersAndTargetCharacters;
        private IEnumerable<BaseCharacter> fixedActionCharacters;
        private IEnumerable<BaseCharacter> debuffedCharacters;

        public PlayerAI(PlayerID playerID)
        {
            this.playerID = playerID;
        }

        public void Start()
        {
            movablePointsExplorer = explorerFactory.GetMovablePointsExplorer(playerID);
            attackablePointsExplorer = explorerFactory.GetAttackPointsExplorer(playerID, movablePointsExplorer);
            moveRouteExplorer = explorerFactory.GetMoveRouteExplorer();

            token = new CancellationTokenSource();
            StartAI(token.Token);
        }

        private async void StartAI(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                fixedActionCharacters = Enumerable.Empty<BaseCharacter>();

                //自分のターンになるまで待つ
                await UniTask.WaitUntil(() => playerID == turnManager.CurrentTurnPlayer.playerID, cancellationToken: token);

                enemyAttackArea = enemyCharacters.SelectMany(x => attackablePointsExplorer.FindAttackablePoints(x.Position, x.Param));

                //自分のターンになってからターン表示が消えるまで待つ
                await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: token);

                Debug.Log("Start AI turn");

                actionCharactersAndTargetCharacters = friendCharacters.Select(x => new KeyValuePair<BaseCharacter, IEnumerable<BaseCharacter>>(x, enemyCharacters.Where(y => attackablePointsExplorer.FindAttackablePoints(x.Position, x.Param).Any(z => y.Position == z))));

                //回復可能キャラと瀕死キャラを行動させる
                await HealAction(token);
                await MoveDyingCharacter(token);
                await HealAction(token);
                await MoveHealableCharacter(token);

                //バフキャラを行動させる
                var buffedCharacters = await BuffAction(token);
                fixedActionCharacters.Union(buffedCharacters);

                //デバフキャラを行動させる
                debuffedCharacters = await DebuffAction(token);

                foreach (var actionCharacter in fixedActionCharacters)
                {
                    await AttackAction(actionCharacter, token);
                }

                await AttackOrStandbyAction(token);

                //自分のターンが終わるまで待機
                await UniTask.WaitUntil(() => playerID != turnManager.CurrentTurnPlayer.playerID, cancellationToken: token);
            }
        }

        #region HealingAndDyingCharacterAction
        /// <summary>
        /// 回復できるキャラが瀕死のキャラのもとに移動して回復する
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private async UniTask HealAction(CancellationToken token)
        {
            IEnumerable<BaseCharacter> dyingCharacters; //瀕死のキャラクターのリスト
            IEnumerable<BaseCharacter> healableCharacters; //回復可能なキャラクターのリスト

            if (TryGetDyingCharacter(out dyingCharacters))
            {
                if (TryGetHealableCharacter(out healableCharacters))
                {
                    foreach (var actionCharacter in healableCharacters)
                    {
                        var healableArea = attackablePointsExplorer.FindAttackablePoints(actionCharacter.Position, actionCharacter.Param);
                        var targetCharacters = dyingCharacters.Where(x => healableArea.Any(y => y == x.Position));

                        if (targetCharacters.Count() > 0)
                        {
                            var target = targetCharacters.RandomGet();
                            var route = GetMoveRoute(actionCharacter, target.Position);
                            DebugExtension.LogList(route);

                            await actionCharacter.MoveAsync(route, token);

                            await FaceToFace(actionCharacter, target, token);

                            await actionCharacter.UseSkillAsync(target);
                            Debug.Log("Heal");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 瀕死のキャラが回復を受けることができる場所まで移動する
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private async UniTask MoveDyingCharacter(CancellationToken token)
        {
            IEnumerable<BaseCharacter> dyingCharacters; //瀕死のキャラクターのリスト
            IEnumerable<BaseCharacter> healableCharacters; //回復可能なキャラクターのリスト

            if (TryGetDyingCharacter(out dyingCharacters))
            {
                if (TryGetHealableCharacter(out healableCharacters))
                {
                    var healablePoints = healableCharacters.SelectMany(x => attackablePointsExplorer.FindAttackablePoints(x.Position, x.Param));
                    
                    foreach(var dyingCharacter in dyingCharacters)
                    {
                        var movablePoints = movablePointsExplorer.FindMovablePoints(dyingCharacter.Position, dyingCharacter.Param.MovableRange, dyingCharacter.Param.Type);
                        var receivableHealPoints = movablePoints.Where(x => healablePoints.Any(y => x == y));
                        if (receivableHealPoints.Count()>0)
                        {
                            var point=receivableHealPoints.RandomGet();
                            var route = GetRoute(dyingCharacter, point);
                            await MoveCharacter(dyingCharacter, route, token);

                            if(!await AttackAction(dyingCharacter, token))
                            {
                                dyingCharacter.Standby();
                            }

                            Debug.Log("Move dying character");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 回復することができるキャラが敵の攻撃の範囲外に移動する
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private async UniTask MoveHealableCharacter(CancellationToken token)
        {
            IEnumerable<BaseCharacter> healableCharacters; //回復可能なキャラクターのリスト

            if (TryGetHealableCharacter(out healableCharacters))
            {
                foreach(var healableCharacter in healableCharacters)
                {
                    var movablePoints = movablePointsExplorer.FindMovablePoints(healableCharacter.Position, healableCharacter.Param.MovableRange, healableCharacter.Param.Type)
                                            .Where(x=>enemyAttackArea.Any(y=>x==y));

                    if (movablePoints.Count() > 0)
                    {
                        var movePoint = movablePoints.RandomGet();
                        var route = GetRoute(healableCharacter, movePoint);
                        await MoveCharacter(healableCharacter, route, token);
                        if (!await AttackAction(healableCharacter, token))
                        {
                            healableCharacter.Standby();
                        }
                        Debug.Log("Move healable character");
                    }
                }
            }
        }

        //瀕死のキャラクターを取得
        private bool TryGetDyingCharacter(out IEnumerable<BaseCharacter> dyingCharacters)
        {
            dyingCharacters = characterManager.EnemyCharacterList
                .Where(x => (float)x.CurrentStatus.Hp.Value / x.Param.Hp < 0.5f);

            var result = dyingCharacters.Count() > 0 && dyingCharacters != null;
            return result;
        }

        //回復できるキャラを取得
        private bool TryGetHealableCharacter(out IEnumerable<BaseCharacter> healableCharacters)
        {
            //TODO: 回復可能なキャラを取得（生きている、SPがある, 行動していない）
            healableCharacters = friendCharacters.Where(x => x.Param.Skill == Skill.Skill_05)
                                    .Where(x => CheckCanUseSkill(x))
                                    .Where(x => x.IsActionable.Value);

            return healableCharacters.Count() > 0;
        }
        #endregion HealingAndDyingCharacterAction

        #region BuffCharacterAction
        /// <summary>
        /// バフをかけることができるキャラクターを行動させる
        /// </summary>
        /// <param name="token"></param>
        /// <returns>バフをかけられるキャラクター</returns>
        private async UniTask<IEnumerable<BaseCharacter>> BuffAction(CancellationToken token)
        {
            IEnumerable<BaseCharacter> buffableCharacters;

            if(TryGetBuffCharacters(out buffableCharacters))
            {
                foreach(var buffCharacter in buffableCharacters)
                {
                    if (buffCharacter.Param.SkillType == SkillType.BuffForAll)
                    {
                        var attackableCharacters = actionCharactersAndTargetCharacters.Where(x => x.Key.IsActionable.Value).Where(x => x.Value.Count() > 1);
                        if (attackableCharacters.Count() > 2)
                        {
                            await buffCharacter.UseSkillAsync();
                            Debug.Log("Use buff");
                            return attackableCharacters.Select(x=>x.Key);
                        }
                    }

                    var buffablePoints = attackablePointsExplorer.FindAttackablePoints(buffCharacter.Position, buffCharacter.Param);
                    var buffedCharacters = friendCharacters.Where(x => buffablePoints.Any(y => x.Position == y)).Where(x=>x!=buffCharacter);
                    foreach(var buffedCharacter in buffedCharacters)
                    {
                        if (actionCharactersAndTargetCharacters.Single(x => x.Key == buffedCharacter).Value.Count() > 0)
                        {
                            if (buffedCharacter.IsActionable.Value == false)
                                continue;

                            var route = GetMoveRoute(buffCharacter, buffedCharacter.Position);
                            await MoveCharacter(buffCharacter, route, token);
                            await FaceToFace(buffCharacter, buffedCharacter, token);
                            await buffCharacter.UseSkillAsync(buffedCharacter);

                            Debug.Log("Use buff", buffedCharacter.gameObject);
                            return new List<BaseCharacter>() { buffedCharacter };
                        }
                    }
                }
            }
            return Enumerable.Empty<BaseCharacter>();
        }

        private bool TryGetBuffCharacters(out IEnumerable<BaseCharacter> buffCharacter)
        {
            //TODO: バフをかけることができるキャラを取得する
            buffCharacter = friendCharacters.Where(x => x.Param.SkillType == SkillType.BuffForFriend || x.Param.SkillType == SkillType.BuffForFriend)
                                            .Where(x => CheckCanUseSkill(x))
                                            .Where(x => x.IsActionable.Value);

            bool result = buffCharacter != null && buffCharacter.Count() > 0;
            return result;
        }
        #endregion

        #region DebuffCharacterAction
        /// <summary>
        /// デバフをかけられるキャラクターを行動させる
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private async UniTask<IEnumerable<BaseCharacter>> DebuffAction(CancellationToken token)
        {
            IEnumerable<BaseCharacter> debuffableCharacters;

            if (TryGetDebuffCharacters(out debuffableCharacters))
            {
                foreach (var debuffCharacter in debuffableCharacters)
                {
                    var debuffablePoints = attackablePointsExplorer.FindAttackablePoints(debuffCharacter.Position, debuffCharacter.Param);
                    var debuffedCharacters = enemyCharacters.Where(x => debuffablePoints.Any(y => x.Position == y));
                    foreach (var debuffedCharacter in debuffedCharacters)
                    {
                        if (actionCharactersAndTargetCharacters.Where(x=>fixedActionCharacters.Any(y=>y==x.Key)).Any(x=>x.Key==debuffedCharacter))
                        {
                            var route = GetMoveRoute(debuffCharacter, debuffedCharacter.Position);
                            await MoveCharacter(debuffCharacter, route, token);
                            await FaceToFace(debuffCharacter, debuffedCharacter, token);
                            await debuffedCharacter.UseSkillAsync(debuffedCharacter);

                            Debug.Log("Use debuff");
                            return new List<BaseCharacter>() { debuffedCharacter };
                        }
                    }
                }
            }
            return Enumerable.Empty<BaseCharacter>();
        }

        private bool TryGetDebuffCharacters(out IEnumerable<BaseCharacter> buffCharacter)
        {
            //TODO: バフをかけることができるキャラを取得する
            buffCharacter = friendCharacters.Where(x => x.Param.SkillType == SkillType.BuffForFriend || x.Param.SkillType == SkillType.BuffForFriend)
                                            .Where(x => CheckCanUseSkill(x))
                                            .Where(x => x.IsActionable.Value);

            bool result = buffCharacter != null && buffCharacter.Count() > 0;
            return result;
        }
        #endregion

        #region attackCharacterAction

        private async UniTask<bool> AttackAction(BaseCharacter character, CancellationToken token)
        {
            var attackablePoints = attackablePointsExplorer.FindAttackablePoints(character.Position, character.Param);
            var targets = enemyCharacters.Where(x=>attackablePoints.Any(y=>x.Position==y));

            if (targets.Count() <= 0)
            {
                return false;
            }

            BaseCharacter target;
            if (debuffedCharacters == null)
            {
                target = targets.RandomGet();
            }
            else if (targets.Any(x => debuffedCharacters.Any(y => x == y)))
            {
                target = targets.Single(x => debuffedCharacters.Any(y => x == y));
            }
            else
            {
                target = targets.RandomGet();
            }

            var route = GetMoveRoute(character, target.Position);
            await MoveCharacter(character, route, token);
            await FaceToFace(character, target, token);

            await character.AttackAsync(target);
            Debug.Log("Attack", target.gameObject);
            return true;
        }

        private async UniTask AttackOrStandbyAction(CancellationToken token)
        {
            var notActionCharacters = friendCharacters.Where(x => x.IsActionable.Value == true);

            foreach(var character in notActionCharacters)
            {
                if (await AttackAction(character, token))
                    continue;

                enemyAttackArea = enemyCharacters.SelectMany(x => attackablePointsExplorer.FindAttackablePoints(x.Position, x.Param));

                var movablePoints = movablePointsExplorer.FindMovablePoints(character.Position, character.Param.MovableRange, character.Param.Type);
                var movePoints = movablePoints.Except(enemyAttackArea);
                Vector2Int movePoint;
                if (movePoints.Count() > 0)
                {
                    movePoint = movePoints.RandomGet();
                }
                else
                {
                    DebugExtension.LogList(movablePoints);
                    try
                    {
                        movePoint = movablePoints.RandomGet();
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        movePoint = character.Position;
                    }
                }

                var route = GetRoute(character, movePoint);
                await MoveCharacter(character, route, token);
                character.Standby();

                await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: token);
                Debug.Log("Move");
            }
        }
        #endregion

        //移動するべき場所を取得
        private Stack<Vector2Int> GetMoveRoute(BaseCharacter moveCharacter, Vector2Int targetPoint)
        {
            var param = moveCharacter.Param;

            Stack<Vector2Int> route = new Stack<Vector2Int>();

            int deltaX = (int)MathF.Abs(moveCharacter.Position.x - targetPoint.x);
            int deltaY = (int)MathF.Abs(moveCharacter.Position.y - targetPoint.y);
            if (deltaX + deltaY == param.AttackRange)
            {
                return route;
            }

            var sidePoints = AroundPointsFinder.FindSidePoints(targetPoint, param.AttackRange, false);
            var cornersPoints = AroundPointsFinder.FindCornersPoints(targetPoint, param.AttackRange-1, false);
            var aroundPoints = sidePoints.Union(cornersPoints);

            foreach (var point in aroundPoints)
            {
                route = GetRoute(moveCharacter, point);
                if (route.Count() > 0) return route;
            }
            return new Stack<Vector2Int>();
        }

        private Stack<Vector2Int> GetRoute(BaseCharacter moveCharacter, Vector2Int movedPoint)
        {
            var param = moveCharacter.Param;

            var route = new Stack<Vector2Int>();
            if (!characterManager.AllCharacters.Any(x => x.Position == movedPoint) && moveRouteExplorer.FindRoute(moveCharacter.Position, movedPoint, param.Type, param.MovableRange, ref route))
            {
                return route;
            }
            return new Stack<Vector2Int>();
        }

        private async UniTask MoveCharacter(BaseCharacter character, Stack<Vector2Int> route, CancellationToken token)
        {
            cameraController.StartFollow(character.transform, token);
            await character.MoveAsync(route, token);
            cameraController.StopFollow();
        }

        private bool CheckCanUseSkill(BaseCharacter character)
        {
            var skillSetting = character.Param.SkillSetting;
            return character.CurrentStatus.Sp.Value > skillSetting.Cost;
        }

        private async UniTask FaceToFace(BaseCharacter character1, BaseCharacter character2, CancellationToken token)
        {
            Vector2Int lookVec1 = (character2.Position - character1.Position);
            if (lookVec1.x == 0)
            {
                lookVec1.y = (int)Mathf.Sign(lookVec1.y);
            }
            else
            {
                lookVec1.x = (int)Mathf.Sign(lookVec1.x);
            }

            Vector2Int lookVec2 = (character1.Position - character2.Position);
            if (lookVec2.x == 0)
            {
                lookVec2.y = (int)Mathf.Sign(lookVec2.y);
            }
            else
            {
                lookVec2.x = (int)Mathf.Sign(lookVec2.x);
            }

            var forcusPoint = new Vector3((character1.Position.x + character2.Position.x) / 2, 0, (character1.Position.y + character2.Position.y) / 2);
            await cameraController.Forcus(forcusPoint, token);
            await character1.TurnToLookAt(lookVec1, token);
            await character2.TurnToLookAt(lookVec2, token);
        }

        public void Dispose()
        {
            token.Cancel();
        }
    }
}

