using Data.DataTable;
using Game.Character.Managers;
using Game.Stages.Managers;
using Game.Teams.Managers;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class RootLifeTimeScope : LifetimeScope
{
    [SerializeField] private CharacterInitialPositionDataTable[] initialCharacterPositionDataArray;

    protected override void Configure(IContainerBuilder builder)
    {
        base.Configure(builder);

        builder.Register<MapManager>(Lifetime.Singleton);
        builder.Register<StageManager>(Lifetime.Singleton);
        builder.Register<TeamManager>(Lifetime.Singleton);

        builder.RegisterInstance(initialCharacterPositionDataArray);
    }
}
