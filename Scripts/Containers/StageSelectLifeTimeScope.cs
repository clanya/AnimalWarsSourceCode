using Game.Stages.Select;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class StageSelectLifeTimeScope : LifetimeScope
{
    [SerializeField] private StageSelector stageSelecter;
    [SerializeField] private StageSelectView stageSelectView;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterEntryPoint<StageSelectPresenter>();

        builder.Register<StageSelectService>(Lifetime.Singleton);

        builder.RegisterComponent(stageSelecter);
        builder.RegisterComponent(stageSelectView);
    }
}
