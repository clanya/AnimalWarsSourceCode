using Game.Titles;
using Game.Titles.Menu;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Containers
{
    public class TitleLifeTimeScope : LifetimeScope
    {
        [SerializeField] private TitleMenuView titleMenuView;
        [SerializeField] private TitleMenuCursorView titleMenuCursorView;
        [SerializeField] private BaseMenuSelector[] baseMenuSelectors;

        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            builder.RegisterEntryPoint<TitleMenuPresenter>();
            builder.RegisterEntryPoint<TitleMenuCursorPresenter>();

            builder.Register<TitleMenuService>(Lifetime.Singleton);

            builder.RegisterComponent(titleMenuView);
            builder.RegisterComponent(titleMenuCursorView);
            foreach(var x in baseMenuSelectors)
            {
                builder.RegisterComponent(x);
            }
        }
    }
}

