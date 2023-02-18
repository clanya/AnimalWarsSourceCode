using Game.Character;
using Game.Character.Images;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Teams.Edit
{
    public class TeamCharacterImage : MonoBehaviour, IDropHandler, IPointerClickHandler
    {
        [SerializeField] private Image imageComponent;
        [SerializeField] private GameObject frameImageObj;

        private ReactiveProperty<CharacterType> attachedCharacter = new ReactiveProperty<CharacterType>();
        public IReadOnlyReactiveProperty<CharacterType> AttachedCharacter => attachedCharacter;

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag.TryGetComponent<CharacterImage>(out var component))
            {
                attachedCharacter.Value = component.CharacterType;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (attachedCharacter.Value == CharacterType.None) return;

            //ダブルクリックされたとき
            if (eventData.clickCount == 2)
            {
                attachedCharacter.Value = CharacterType.None;
            }
        }

        public void SetCharacterImage(Sprite sprite)
        {
            imageComponent.sprite = sprite;
        }

        public void ViewFrame()
        {
            frameImageObj.SetActive(true);
        }

        public void HideFrame()
        {
            frameImageObj.SetActive(false);
        }
    }
}

