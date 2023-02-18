using Data.DataTable;
using Game.Stages;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using TMPro;
using Game.Character;
using Game.Stages.MapInfo;
using Game.Audio;

namespace Game.Teams.Edit
{
    public class MapInfoView : MonoBehaviour, IMapInfoView
    {
        [SerializeField] private Button mapViewButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private GameObject mapInfoViewPanel;
        [SerializeField] private Transform mapPieceParent;
        [SerializeField] private GameObject mapPiecePrefab;
        [SerializeField] private LandscapeColorTable landscapeColorTable;
        [SerializeField] private CharacterImageTable characterImageTable;

        public Button.ButtonClickedEvent MapViewButtonClickEvent => mapViewButton.onClick;

        private Dictionary<Vector2, GameObject> mapPieceDic = new Dictionary<Vector2, GameObject>();

        private const float PanelSide = 360f;

        private void Start()
        {
            mapViewButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    mapInfoViewPanel.SetActive(true);
                    AudioPlayer.PlayClickButtonSE();
                })
                .AddTo(this);

            closeButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    mapInfoViewPanel.SetActive(false);
                    AudioPlayer.PlayClickButtonSE();
                })
                .AddTo(this);
        }

        //マップ情報を表示するマスを生成する
        public void GenerateMapPiece(ReadOnlyCollection<ReadOnlyCollection<LandscapeType>> mapData)
        {
            int lengthX = mapData.Count;
            int lengthY = mapData[0].Count;
            
            float pieceSizeX = PanelSide /lengthX;
            float pieceSizeY = PanelSide /lengthY;

            for (int x = 0; x < lengthX; x++)
            {
                for(int y = 0; y < lengthY; y++)
                {
                    var piece=Instantiate(mapPiecePrefab);
                    RectTransform rectTransform = piece.GetComponent<RectTransform>();

                    //生成後の位置調整
                    rectTransform.SetParent(mapPieceParent);
                    rectTransform.anchoredPosition = new Vector2(x * pieceSizeX, y * pieceSizeY);
                    rectTransform.sizeDelta = new Vector2(pieceSizeX, pieceSizeY);
                    rectTransform.localScale = Vector3.one;
                    piece.GetComponent<Image>().color = landscapeColorTable.LandscapeColorDataArray.FirstOrDefault(v => v.LandscapeType == mapData[x][y]).LandscapeColor;

                    mapPieceDic.Add(new Vector2Int(x, y), piece);
                }
            }
        }

        //味方キャラの配置位置を表示する
        public void ViewFriendCharacterPosition(IReadOnlyList<Vector2Int> positionList)
        {
            int teamNumber = 1;
            foreach(var position in positionList)
            {
                var piece = mapPieceDic[position];
                SetMapPieceText(piece, teamNumber);
                teamNumber++;
            }
        }

        private void SetMapPieceText(GameObject mapPiece, int number)
        {
            var text = mapPiece.transform.GetChild(0).GetComponent<TMP_Text>();
            text.text = number.ToString();
        }

        //敵キャラの配置位置を表示する
        public void ViewEnemyCharacterPosition(IReadOnlyDictionary<Vector2Int, CharacterType> enemyDic)
        {
            foreach (var position in enemyDic.Keys)
            {
                var piece = mapPieceDic[position];
                var characterType = enemyDic.FirstOrDefault(v => v.Key == position).Value;
                SetMapPieceEnemyImage(piece, characterType);
            }
        }

        private void SetMapPieceEnemyImage(GameObject mapPiece, CharacterType characterType)
        {
            var image = mapPiece.GetComponent<Image>();
            image.sprite = characterImageTable.GetCharacterSprite(characterType);
            image.color = Color.white;
        }

        public void UpdateMapPiece(Vector2 PreviousPosition, Vector2 newPosition)
        {
            //更新されることは無いため何もしない
        }
    }
}

