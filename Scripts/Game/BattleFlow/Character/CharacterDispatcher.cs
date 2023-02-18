using System.Collections.Generic;
using Game.Character;
using Game.Character.Models;
using Game.Character.Presenter;
using Game.Character.View;
using UnityEngine;

namespace Game.BattleFlow.Character
{
    public sealed class CharacterDispatcher : MonoBehaviour
    {
        [SerializeField] private CharacterPresenter presenter;
        
        public void SetCharacterList(IReadOnlyList<BaseCharacter> characters)
        {
            foreach (var character in characters)
            {
                Dispatch(character);
            }
        }

        private void Dispatch(BaseCharacter character)
        {
            var view = character.gameObject.GetComponent<CharacterView>();
            var commandView = FindObjectOfType<CharacterCommandView>();
            presenter.OnCreateCharacter(character,view,commandView);
        }
    }
}