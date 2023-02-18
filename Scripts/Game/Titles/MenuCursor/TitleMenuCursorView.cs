using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Titles.Menu
{
    public class TitleMenuCursorView : MonoBehaviour
    {
        [SerializeField] private RectTransform cursor;

        private const float MenuTextHeight=52.5f;

        public void MoveCursor(float height)
        {
            Vector3 movePos = new Vector3(0, height, 0);
            cursor.localPosition = movePos;
        }

        public void MoveCursor(MenuType menuType)
        {
            MoveCursor(80f-((int)menuType * MenuTextHeight));
        }
    }
}

