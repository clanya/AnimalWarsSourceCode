using System;
using UniRx;

namespace Game.Character.Models
{
    public sealed class CharacterModelDirector : IDisposable
    {
        private readonly ReactiveProperty<BaseCharacter> selectedCharacter = new();
        public IReadOnlyReactiveProperty<BaseCharacter> SelectedCharacter => selectedCharacter;
        
        private readonly ReactiveProperty<BaseCharacter> targetedCharacter = new();
        public IReadOnlyReactiveProperty<BaseCharacter> TargetedCharacter => targetedCharacter;
        
        public bool IsLockedSelectedCharacter { get; private set; }
        
        public void SetSelectedCharacter(BaseCharacter character)
        {
            if (selectedCharacter.Value == character)
            {
                selectedCharacter.Value = null;
                return;
            }
            selectedCharacter.Value = character;
        }
        
        public void SetTargetedCharacter(BaseCharacter character)
        {
            if (targetedCharacter.Value == character)
            {
                targetedCharacter.Value = null;
                return;
            }
            targetedCharacter.Value = character;
        }

        public void ClearSelectedCharacter()
        {
            selectedCharacter.Value = null;
        }
        
        public void ClearTargetCharacter()
        {
            targetedCharacter.Value = null;
        }

        public void SetIsLockedSelectedCharacter(bool value)
        {
            IsLockedSelectedCharacter = value;
        }

        public void Dispose()
        {
            selectedCharacter.Dispose();
            targetedCharacter.Dispose();
        }
    }
}