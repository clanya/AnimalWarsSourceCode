using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using VContainer;
using VContainer.Unity;

namespace Game.Titles.Menu
{
    public class TitleMenuCursorPresenter : ControllerBase, IStartable
    {
        private readonly TitleMenuCursorView titleMenuCursorView;
        private readonly IEnumerable<BaseMenuSelector> menuSelectors;

        [Inject]
        public TitleMenuCursorPresenter(TitleMenuCursorView titleMenuCursorView, IEnumerable<BaseMenuSelector> menuSelectors)
        {
            this.titleMenuCursorView = titleMenuCursorView;
            this.menuSelectors = menuSelectors;
        }

        void IStartable.Start()
        {
            foreach(var selecter in menuSelectors)
            {
                selecter.SelectedMenu
                    .Skip(1)
                    .Subscribe(x =>
                    {
                        titleMenuCursorView.MoveCursor(x);
                    })
                    .AddTo(this);
            }
        }
    }
}

