using Game.AI;
using Game.BattleFlow;
using Game.BattleFlow.Character;
using Game.BattleFlow.Turn;
using Game.Character;
using Game.Character.Managers;
using Game.Character.Models;
using Game.Character.Skills;
using Game.Stages;
using Game.Stages.Explorers;
using Game.Tiles;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameLifeTimeScope : LifetimeScope
{
    [SerializeField] private CharacterManager characterManager;
    [SerializeField] private NavigationTileViewDirector navigationTileViewDirector;
    [SerializeField] private CameraController cameraController;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<TargetCharacterExplorer>(Lifetime.Singleton);
        builder.Register<TurnManager>(Lifetime.Singleton);
        builder.Register<AroundPointsExplorer>(Lifetime.Transient);
        builder.Register<ExplorerFactory>(Lifetime.Singleton);
        builder.Register<CharacterModelDirector>(Lifetime.Singleton);
        builder.Register<CharacterViewDirector>(Lifetime.Singleton);
        builder.Register<SkillManager>(Lifetime.Singleton);

        builder.RegisterEntryPoint<NavigationTileGeneratePresenter>();
        builder.RegisterEntryPoint<NavigationTilePresenter>();
        builder.RegisterEntryPoint<PlayerAI>()
            .WithParameter(PlayerID.Player2);

        builder.RegisterComponent(characterManager);
        builder.RegisterComponent(navigationTileViewDirector);
        builder.RegisterComponent(cameraController);
    }
}
