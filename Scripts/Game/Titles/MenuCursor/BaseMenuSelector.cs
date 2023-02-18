using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UnityEngine.EventSystems;
using Game.Audio;

namespace Game.Titles.Menu
{
    public class BaseMenuSelector : MonoBehaviour
    {
        //子クラス間で値を共有したいのでstaticにしてる
        protected static ReactiveProperty<MenuType> selectedMenu = new ReactiveProperty<MenuType>();
        public IReadOnlyReactiveProperty<MenuType> SelectedMenu => selectedMenu;
    }
}

