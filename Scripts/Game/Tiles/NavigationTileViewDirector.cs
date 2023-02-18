using Game.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace Game.Tiles
{
    public sealed class NavigationTileViewDirector : MonoBehaviour
    {
        [SerializeField] private NavigationTileGenerator navigationTileGenerator;

        private ISubject<Vector2Int> clickedMovePointSubject = new Subject<Vector2Int>();
        public IObservable<Vector2Int> ClickedMovePointOservable => clickedMovePointSubject.AsObservable();

        private NavigationTileView[,] tileViewArray;

        private int lengthX;
        private int lengthY;

        private IEnumerable<Vector2Int> dangerPoints;
        public IEnumerable<Vector2Int> movablePoints;

        //パネルの色
        private Dictionary<TileType, Color> colorDic = new Dictionary<TileType, Color>()
        {
            {TileType.Normal, new Color(1,1,1,1) },
            {TileType.Movable, new Color(0,0,1,1)},
            {TileType.Attackable, new Color(1,0f,0f,1) },
            {TileType.Danger, new Color(1,0.9f,0f,1f) },
            {TileType.MovableAndDanger, new Color(0.6f,0.25f,1,1) },
            {TileType.AttackableAndDanger, new Color(1f,0.5f,0.5f,1) }
        };

        private enum TileType
        {
            Normal,
            Movable,
            Attackable,
            Danger,
            MovableAndDanger,
            AttackableAndDanger,
        }

        public void InitTileArray(int x, int y)
        {
            tileViewArray = new NavigationTileView[x, y];

            lengthX = x;
            lengthY = y;
        }

        public void InitTile(Vector2Int tilePosition)
        {
            var tile = navigationTileGenerator.GenerateTile(tilePosition);
            var tileView = tile.GetComponent<NavigationTileView>();
            tileView.SetImageColor(colorDic[TileType.Normal]);

            tileViewArray[tilePosition.x, tilePosition.y] = tileView;

            tileView.OnPointerClickAsObservable
                .Where(_=>movablePoints!=null)
                .Where(_=>movablePoints.Any(x=>x==tilePosition))
                .Subscribe(_ =>
                {
                    clickedMovePointSubject.OnNext(tilePosition);
                })
                .AddTo(this);
        }

        //移動範囲を表示
        public void ViewMovablePoints(IEnumerable<Vector2Int> movablePoints)
        {
            this.movablePoints = movablePoints;

            foreach(var point in movablePoints)
            {
                if (dangerPoints.Any(x => x == point))
                {
                    tileViewArray[point.x, point.y].SetImageColor(colorDic[TileType.MovableAndDanger]);
                }
                else
                {
                    tileViewArray[point.x, point.y].SetImageColor(colorDic[TileType.Movable]);
                }
            }
        }

        public void ViewAttackablePoins(IEnumerable<Vector2Int> attackablePoints)
        {
            foreach (var point in attackablePoints)
            {
                if (dangerPoints.Any(x => x == point))
                {
                    tileViewArray[point.x, point.y].SetImageColor(colorDic[TileType.AttackableAndDanger]);
                }
                else
                {
                    tileViewArray[point.x, point.y].SetImageColor(colorDic[TileType.Attackable]);
                }
            }
        }

        //キャラの移動範囲と攻撃範囲を表示
        public void ViewMovableAndAttackablePoints(IEnumerable<Vector2Int> movablePoints, IEnumerable<Vector2Int> attackablePoints)
        {
            this.movablePoints = movablePoints;
            var viewAttackablePoints = attackablePoints.Except(movablePoints);

            ViewMovablePoints(movablePoints);
            ViewAttackablePoins(viewAttackablePoints);
        }

        public void SetNormalAllTiles()
        {
            for (int i = 0; i < lengthX; i++)
            {
                for (int j = 0; j < lengthY; j++)
                {
                    tileViewArray[i, j].SetImageColor(colorDic[TileType.Normal]);
                }
            }
        }

        public void SetNormalNotDangerTiles()
        {
            for (int i = 0; i < lengthX; i++)
            {
                for (int j = 0; j < lengthY; j++)
                {
                    if (dangerPoints.Any(x => x == new Vector2Int(i, j)))
                    {
                        tileViewArray[i, j].SetImageColor(colorDic[TileType.Danger]);
                    }
                    else
                    {
                        tileViewArray[i, j].SetImageColor(colorDic[TileType.Normal]);
                    }
                }
            }
        }

        //すべてのパネルを隠す
        public void HideAllTiles()
        {
            for(int i = 0; i < lengthX; i++)
            {
                for(int j = 0; j < lengthY; j++)
                {
                    tileViewArray[i, j].Hide();
                }
            }
        }

        //敵の攻撃範囲でない部分を隠す
        public void HideNotDangerTiles()
        {
            for (int i = 0; i < lengthX; i++)
            {
                for (int j = 0; j < lengthY; j++)
                {
                    if (dangerPoints.Any(x => x == new Vector2Int(i, j)))
                    {
                        tileViewArray[i, j].SetImageColor(colorDic[TileType.Danger]);
                    }
                    else
                    {
                        tileViewArray[i, j].Hide();
                    }
                }
            }
        }

        //敵の攻撃範囲を表示
        public void ViewDangerPoints(IEnumerable<Vector2Int> dangerPoints)
        {
            HideAllBorder();

            this.dangerPoints = dangerPoints;

            foreach(var point in dangerPoints)
            {
                tileViewArray[point.x, point.y].SetImageColor(colorDic[TileType.Danger]);
                tileViewArray[point.x, point.y].SetBorder(GetBorders(point));
            }
        }

        public void HideAllBorder()
        {
            for (int i = 0; i < lengthX; i++)
            {
                for (int j = 0; j < lengthY; j++)
                {
                    tileViewArray[i, j].HideBorder();
                }
            }
        }

        //境界かどうかを調べる
        private IEnumerable<BorderType> GetBorders(Vector2Int point)
        {
            List<BorderType> result = new List<BorderType>();
            if (point.x + 1 >= lengthX || !dangerPoints.Any(x => x == new Vector2Int(point.x + 1, point.y)))
            {
                result.Add(BorderType.Right);
            }

            if (point.x - 1 < 0 || !dangerPoints.Any(x => x == new Vector2Int(point.x - 1, point.y)))
            {
                result.Add(BorderType.Left);
            }

            if (point.y + 1 >= lengthY || !dangerPoints.Any(x => x == new Vector2Int(point.x, point.y + 1)))
            {
                result.Add(BorderType.Top);
            }

            if (point.y - 1 < 0 || !dangerPoints.Any(x => x == new Vector2Int(point.x, point.y - 1)))
            {
                result.Add(BorderType.Bottom);
            }

            return result;
        }
    }
}