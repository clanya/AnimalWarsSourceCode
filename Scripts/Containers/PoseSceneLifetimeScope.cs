using Game.PoseScene;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class PoseSceneLifetimeScope : LifetimeScope
{
    [SerializeField] private PoseSceneView poseSceneView;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterEntryPoint<PoseScenePresenter>();

        builder.Register<PoseSceneService>(Lifetime.Singleton);

        builder.RegisterComponent(poseSceneView);
    }
}
