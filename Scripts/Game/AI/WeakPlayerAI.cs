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
using Random = UnityEngine.Random;

namespace Game.AI
{
    public class WeakPlayerAI : IStartable, IDisposable
    {
        private PlayerID playerID;
        [Inject] private ExplorerFactory explorerFactory;
        [Inject] private AroundPointsExplorer aroundPointsExplorer;
        [Inject] private TurnManager turnManager;
        [Inject] private CharacterManager characterManager;
        [Inject] private SkillManager skillManager;
        [Inject] private CameraController cameraController;
        private IEnumerable<BaseCharacter> friendCharacterList => characterManager.EnemyCharacterList;
        private IEnumerable<BaseCharacter> enemyCharacterList => characterManager.PlayerCharacterList;

        private MovablePointsExplorer movablePointsExplorer;
        private MoveRouteExplorer moveRouteExplorer;
        private AttackablePointsExplorer attackablePointsExplorer;
        private CancellationTokenSource token;

        public WeakPlayerAI(PlayerID playerID)
        {
            this.playerID = playerID;
        }

        public void Start()
        {
            movablePointsExplorer = explorerFactory.GetMovablePointsExplorer(playerID);
            moveRouteExplorer = explorerFactory.GetMoveRouteExplorer();
            attackablePointsExplorer = explorerFactory.GetAttackPointsExplorer(playerID, movablePointsExplorer);

            token = new CancellationTokenSource();
            StartAI(token.Token).Forget();
        }

        private async UniTask StartAI(CancellationToken token)
        {
            //TurnManagerの初期化待ち
            await turnManager.InitObservable.ToUniTask(useFirstValue: false, cancellationToken: token);

            while (true)
            {
                //自分のターンになるまで待つ
                await UniTask.WaitUntil(() => playerID == turnManager.CurrentTurnPlayer.playerID, cancellationToken: token);

                //自分のターンになってからターン表示が消えるまで待つ
                await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: token);

                foreach(var character in friendCharacterList)
                {
                    //キャラクターがスキルを使えるかどうか
                    if (skillManager.CanUseSkill(character))
                    {
                        //確率でスキルを使用する
                        var v = Random.value;
                        if (v > 0.7f)
                        {
                            await UseSkill(character, token);
                        }
                        else
                        {
                            await NormalAttack(character, token);
                        }
                    }
                    else
                    {
                        await NormalAttack(character, token);
                    }

                    await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: token);
                }

                //自分のターンが終わるまで待機
                await UniTask.WaitUntil(() => playerID != turnManager.CurrentTurnPlayer.playerID, cancellationToken: token);
            }
        }

        private async UniTask UseSkill(BaseCharacter character, CancellationToken token)
        {
            switch (character.Param.SkillType)
            {
                case SkillType.BuffForFriend:
                    await BuffForFriend(character, token);
                    break;
                case SkillType.DebuffForEnemy:
                    await DebuffForEnemy(character, token);
                    break;
                case SkillType.BuffForYourself:
                    await BuffYourself(character);
                    break;
                case SkillType.BuffForAll:
                    await BuffAll(character);
                    break;
                case SkillType.Attack:
                    await SkillAttack(character, token);
                    break;
            }
        }

        //味方にバフをかける
        private async UniTask BuffForFriend(BaseCharacter character, CancellationToken token)
        {
            var buffArea = attackablePointsExplorer.FindAttackablePoints(character.Position, character.Param.AttackRange);

            //バフをかけられる範囲内に味方がいるなら移動してバフをかける
            if (friendCharacterList.Any(x => buffArea.Any(y => x.Position == y)))
            {
                var targetCharacters = friendCharacterList.Where(x => buffArea.Any(y => x.Position == y));
                var target = targetCharacters.RandomGet();
                var route = GetMoveRoute(character, target.Position);

                await MoveCharacter(character, route, token);
                await FaceToFace(character, target, token);

                await character.UseSkillAsync(target);
                Debug.Log("Buff for friend");
            }
            else
            {
                //範囲内に味方がいないなら通常攻撃をする
                await NormalAttack(character, token);
            }
        }

        //敵にデバフをかける
        private async UniTask DebuffForEnemy(BaseCharacter character, CancellationToken token)
        {
            var debuffArea = attackablePointsExplorer.FindAttackablePoints(character.Position, character.Param.AttackRange);

            //デバフを書けることができる範囲内に敵がいるなら移動してデバフをかける
            if (enemyCharacterList.Any(x => debuffArea.Any(y => x.Position == y)))
            {
                var targetCharacters = enemyCharacterList.Where(x => debuffArea.Any(y => x.Position == y));
                var target = targetCharacters.RandomGet();
                var route = GetMoveRoute(character, target.Position);

                await MoveCharacter(character, route, token);
                await FaceToFace(character, target, token);

                await character.UseSkillAsync(target);
                Debug.Log("Debuff for enemy");
            }
            else
            {
                //範囲内に敵がいないなら通常攻撃をする
                await NormalAttack(character, token);
            }
        }

        //自分自身にバフをかける
        private async UniTask BuffYourself(BaseCharacter character)
        {
            await character.UseSkillAsync(character);
            Debug.Log("Buff for yourself");
        }

        //味方全体にバフをかける
        private async UniTask BuffAll(BaseCharacter character)
        {
            await character.UseSkillAsync();
            Debug.Log("Buff for all");
        }

        //スキルを使った攻撃をする
        private async UniTask SkillAttack(BaseCharacter character, CancellationToken token)
        {
            var skillArea = attackablePointsExplorer.FindAttackablePoints(character.Position, character.Param.AttackRange);

            //スキルの範囲内に敵がいるなら移動して攻撃
            if (enemyCharacterList.Any(x => skillArea.Any(y => x.Position == y)))
            {
                var targetCharacters = enemyCharacterList.Where(x => skillArea.Any(y => x.Position == y));
                var target = targetCharacters.RandomGet();
                var route = GetMoveRoute(character, target.Position);

                await MoveCharacter(character, route, token);
                await FaceToFace(character, target, token);

                await character.UseSkillAsync(target);
                Debug.Log("Skill attack");
            }
            else
            {
                //範囲内に敵がいないなら攻撃できない
                await SelectMoveOrStandby(character, token);
            }
        }

        //通常攻撃を行う
        private async UniTask NormalAttack(BaseCharacter character, CancellationToken token)
        {
            var attackArea= attackablePointsExplorer.FindAttackablePoints(character.Position, character.Param.AttackRange);

            //攻撃範囲内に敵がいるなら移動して攻撃する
            if (enemyCharacterList.Any(x => attackArea.Any(y => x.Position == y)))
            {
                var targetCharacters = enemyCharacterList.Where(x => attackArea.Any(y => x.Position == y));
                var target = targetCharacters.RandomGet();
                var route = GetMoveRoute(character, target.Position);

                await MoveCharacter(character, route, token);
                await FaceToFace(character, target, token);

                await character.AttackAsync(target);
                Debug.Log("Normal attack");
            }
            else
            {
                //範囲内に敵がいないなら攻撃できない
                await SelectMoveOrStandby(character, token);
            }
        }

        private async UniTask SelectMoveOrStandby(BaseCharacter character, CancellationToken token)
        {
            var v = Random.value;
            if (v < 0.5f)
            {
                var movablePoints = movablePointsExplorer.FindMovablePoints(character.Position, character.Param.MovableRange, character.Param.Type);
                if (movablePoints.Any())
                {
                    var movePoint = movablePoints.RandomGet();
                    Stack<Vector2Int> route = GetMoveRoute(character, movePoint);

                    await MoveCharacter(character, route, token);

                    character.Standby();
                }
                else
                {
                    character.Standby();
                    Debug.Log("Stand by");
                }
            }
            else
            {
                character.Standby();
                Debug.Log("Stand by");
            }
        }

        //移動するべき場所を取得
        private Stack<Vector2Int> GetMoveRoute(BaseCharacter moveCharacter, Vector2Int targetPoint)
        {
            var param = moveCharacter.Param;

            Stack<Vector2Int> route = new Stack<Vector2Int>();
            Vector2Int movedPoint;

            movedPoint = moveCharacter.Position;
            route = GetRoute(moveCharacter, movedPoint);
            if (route != null) return route;

            movedPoint = new Vector2Int(targetPoint.x + param.AttackRange, targetPoint.y);
            route = GetRoute(moveCharacter, movedPoint);
            if (route != null) return route;

            movedPoint = new Vector2Int(targetPoint.x - param.AttackRange, targetPoint.y);
            route = GetRoute(moveCharacter, movedPoint);
            if (route != null) return route;

            movedPoint = new Vector2Int(targetPoint.x, targetPoint.y + param.AttackRange);
            route = GetRoute(moveCharacter, movedPoint);
            if (route != null) return route;

            movedPoint = new Vector2Int(targetPoint.x, targetPoint.y - param.AttackRange);
            route = GetRoute(moveCharacter, movedPoint);
            if (route != null) return route;

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
            return null;
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
            await character1.TurnToLookAt(lookVec1, token);
            await character2.TurnToLookAt(lookVec2, token);
        }

        private async UniTask MoveCharacter(BaseCharacter character, Stack<Vector2Int> route, CancellationToken token)
        {
            cameraController.StartFollow(character.transform, token);
            await character.MoveAsync(route, token);
            cameraController.StopFollow();
        }

        public void Dispose()
        {
            token?.Cancel();
        }
    }
}

