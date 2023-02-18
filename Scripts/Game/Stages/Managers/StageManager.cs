using System.Collections;
using UniRx;
using MyUtil;
using VContainer;
using Data.DataTable;
using System.Linq;

namespace Game.Stages.Managers
{
    public class StageManager
    {
        public int selectedStageNumber { get; private set; }
        public CharacterInitialPositionDataTable characterInitialPositionData { get; private set; }

        private readonly MapManager mapManager;
        private readonly CharacterInitialPositionDataTable[] initialCharacterPositionDataList;

        [Inject]
        public StageManager(CharacterInitialPositionDataTable[] initialCharacterPositionDataList, MapManager mapManager)
        {
            this.initialCharacterPositionDataList = initialCharacterPositionDataList;
            this.mapManager = mapManager;
        }

        public void SetSelectedStageNumber(int selectedStageNumber)
        {
            this.selectedStageNumber = selectedStageNumber;
        }

        public IEnumerator LoadStageData()
        {
            LandscapeDataLoader landscapeDataLoader = new LandscapeDataLoader();

            yield return landscapeDataLoader.LoadStageData(selectedStageNumber, SetMapData); //地形データの読み込み
        }

        //取得したステージデータを登録
        public void SetStageData()
        {
            characterInitialPositionData = initialCharacterPositionDataList.FirstOrDefault(x => x.StageNumber == selectedStageNumber);
        }

        //読み込んだ地形データを登録
        private void SetMapData(int[][] loadedData)
        {
            var mapData = ArrayConverter.ExchangeToEnumArray<LandscapeType>(loadedData);
            mapManager.SetMapData(mapData);
        }
    }
}

