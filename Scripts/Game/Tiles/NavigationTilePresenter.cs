using Game.BattleFlow;
using Game.Character;
using Game.Stages.Explorers;
using System;
using System.Collections.Generic;
using System.Linq;
using Game.BattleFlow.Character;
using Game.BattleFlow.Turn;
using Game.Character.Models;
using UniRx;
using UnityEngine;
using VContainer;
using VContainer.Unity;


namespace Game.Tiles
{
    public class NavigationTilePresenter : ControllerBase, IStartable,IDisposable
    {
        private readonly NavigationTileViewDirector tileViewDirector;
        private readonly CharacterModelDirector characterModelDirector;
        private readonly MovablePointsExplorer movablePointsExplorer;
        private readonly AttackablePointsExplorer attackablePointsExplorer;
        private readonly CharacterManager characterManager;
        private readonly TurnManager turnManager;
        
        [Inject]
        public NavigationTilePresenter(NavigationTileViewDirector tileViewDirector, CharacterModelDirector characterModelDirector,
            ExplorerFactory explorerFactory, CharacterManager characterManager, TurnManager turnManager)
        {
            this.tileViewDirector = tileViewDirector;
            this.characterModelDirector = characterModelDirector;

            movablePointsExplorer = explorerFactory.GetMovablePointsExplorer(PlayerID.Player1);
            attackablePointsExplorer = explorerFactory.GetAttackPointsExplorer(PlayerID.Player1, movablePointsExplorer);

            this.characterManager = characterManager;
            this.turnManager = turnManager;
        }

        public void Start()
        {
            //敵の攻撃範囲を表示
            ViewDangerArea();

            //選択したキャラの移動範囲と攻撃範囲を表示
            characterModelDirector.SelectedCharacter
                .Skip(1)
                .Where(character=>character!=null)
                .Where(character=>character.IsMovable.Value)
                .Subscribe(character =>
                {
                    tileViewDirector.SetNormalNotDangerTiles();

                    var param = character.Param;
                    var movablePoints=movablePointsExplorer.FindMovablePoints(character.Position, param.MovableRange, param.Type);
                    var attackablePoins = attackablePointsExplorer.FindAttackablePoints(movablePoints, param.AttackRange);
                    tileViewDirector.ViewMovableAndAttackablePoints(movablePoints, attackablePoins);

                    character.IsMovable
                        .Where(x => !x)
                        .Take(1)
                        .Subscribe(_ =>
                        {
                            tileViewDirector.SetNormalNotDangerTiles();
                        },
                        ()=>
                        {
                            var attackablePoints=attackablePointsExplorer.FindAttackablePoints(character.Position, character.Param.AttackRange);
                            tileViewDirector.ViewAttackablePoins(attackablePoints);
                        })
                        .AddTo(this);

                    character.IsActionable
                        .Where(x => !x)
                        .Take(1)
                        .Subscribe(_ =>
                        {
                            tileViewDirector.SetNormalNotDangerTiles();
                        })
                        .AddTo(this);
                })
                .AddTo(this);

            characterModelDirector.SelectedCharacter
                .Skip(1)
                .Where(character => character == null)
                .Subscribe(character =>
                {
                    tileViewDirector.SetNormalNotDangerTiles();
                })
                .AddTo(this);

            //自分のターンになったとき敵の攻撃範囲を表示
            turnManager.InitObservable
                .Take(1)
                .Subscribe(_ =>
                {
                    turnManager.ChangeTurnObservable
                        .Where(x => x.playerID == PlayerID.Player1)
                        .Subscribe(_ =>
                        {
                            ViewDangerArea();
                        })
                        .AddTo(this);
                })
                .AddTo(this);

            foreach(var enemy in characterManager.EnemyCharacterList)
            {
                enemy.IsDead
                    .Where(x => x)
                    .Subscribe(_ =>
                    {
                        tileViewDirector.SetNormalAllTiles();
                        ViewDangerArea();
                    })
                    .AddTo(this);
            }
        }

        //敵の攻撃範囲を表示
        private void ViewDangerArea()
        {
            IEnumerable<Vector2Int> allEnemyAttackablePoints = new List<Vector2Int>();
            foreach (var enemy in characterManager.EnemyCharacterList.Where(x=>!x.IsDead.Value))
            {
                allEnemyAttackablePoints=allEnemyAttackablePoints.Union(attackablePointsExplorer.FindAttackablePoints(enemy.Position, enemy.Param));
            }
            tileViewDirector.ViewDangerPoints(allEnemyAttackablePoints);
        }
    }
}