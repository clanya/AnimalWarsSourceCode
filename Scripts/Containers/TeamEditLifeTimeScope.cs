using Game.Stages.MapInfo;
using Game.Teams.Edit;
using Game.Teams.Edit.Scenes;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class TeamEditLifeTimeScope : LifetimeScope
{
    [SerializeField] private TeamEditSceneView teamEditSceneView;
    [SerializeField] private MapInfoView mapInfoView;
    [SerializeField] private CharacterImageView characterImageView;

    protected override void Configure(IContainerBuilder builder)
    {
        base.Configure(builder);

        builder.RegisterEntryPoint<MapInfoPresenter>();
        builder.RegisterEntryPoint<TeamEditScenePresenter>();
        builder.RegisterEntryPoint<CharacterImagePresenter>();

        builder.Register<TeamEditSceneService>(Lifetime.Singleton);

        builder.RegisterComponent(teamEditSceneView);
        builder.RegisterComponent<IMapInfoView>(mapInfoView);
        builder.RegisterComponent(characterImageView);
    }
}
