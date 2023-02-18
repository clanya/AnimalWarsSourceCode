using Game.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using Image = UnityEngine.UI.Image;

namespace Game.Tiles
{
    public class NavigationTileView : MonoBehaviour
    {
        [SerializeField] private Image image;

        [SerializeField] private GameObject rightBorderImage; 
        [SerializeField] private GameObject leftBorderImage; 
        [SerializeField] private GameObject topBorderImage; 
        [SerializeField] private GameObject bottomBorderImage; 

        public IObservable<PointerEventData> OnPointerClickAsObservable => image.OnPointerClickAsObservable().Where(x => x.button == PointerEventData.InputButton.Left);
        
        public void SetImageColor(Color color)
        {
            gameObject.SetActive(true);
            image.color = color;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void HideBorder()
        {
            rightBorderImage.SetActive(false);
            leftBorderImage.SetActive(false);
            topBorderImage.SetActive(false);
            bottomBorderImage.SetActive(false);
        }

        //境界を表示
        public void SetBorder(IEnumerable<BorderType> borders)
        {
            if (borders.Any(x => x == BorderType.Left))
            {
                leftBorderImage.SetActive(true);
            }
            else
            {
                leftBorderImage.SetActive(false);
            }

            if (borders.Any(x => x == BorderType.Right))
            {
                rightBorderImage.SetActive(true);
            }
            else
            {
                rightBorderImage.SetActive(false);
            }

            if (borders.Any(x => x == BorderType.Top))
            {
                topBorderImage.SetActive(true);
            }
            else
            {
                topBorderImage.SetActive(false);
            }

            if (borders.Any(x => x == BorderType.Bottom))
            {
                bottomBorderImage.SetActive(true);
            }
            else
            {
                bottomBorderImage.SetActive(false);
            }
        }
    }
}