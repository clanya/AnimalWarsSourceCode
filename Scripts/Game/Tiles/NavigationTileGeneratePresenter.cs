using Game.BattleFlow;
using Game.Stages.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Tiles
{
    public class NavigationTileGeneratePresenter : IStartable
    {
        private readonly MapManager mapManager;
        private readonly NavigationTileViewDirector navigationTileViewDirector;

        [Inject]
        public NavigationTileGeneratePresenter(MapManager mapManager, NavigationTileViewDirector navigationTileViewDirector)
        {
            this.mapManager = mapManager;
            this.navigationTileViewDirector = navigationTileViewDirector;
        }

        public void Start()
        {
            navigationTileViewDirector.InitTileArray(mapManager.limitX, mapManager.limitY);

            for(int i = 0; i < mapManager.limitX; i++)
            {
                for(int j = 0; j < mapManager.limitY; j++)
                {
                    navigationTileViewDirector.InitTile(new Vector2Int(i, j));
                }
            }
        }
    }
}

