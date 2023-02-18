using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Character.View
{
    public sealed class CharacterCommandView : MonoBehaviour
    {
        [SerializeField] private GameObject panel;

        [SerializeField] private Button moveButton;
        public IObservable<Unit> MoveButtonObservable => moveButton.OnClickAsObservable();

        [SerializeField] private Button attackButton;
        public IObservable<Unit> AttackButtonObservable => attackButton.OnClickAsObservable();
        [SerializeField] private Button standbyButton;
        public IObservable<Unit> StandbyButtonObservable => standbyButton.OnClickAsObservable();
        [SerializeField] private Button useSkillButton;
        public IObservable<Unit> UseSkillButtonObservable => useSkillButton.OnClickAsObservable();
        [SerializeField] private Button escapeButton;
        public IObservable<Unit> EscapeButtonObservable => escapeButton.OnClickAsObservable();
        [SerializeField] private Color buttonTextActiveColor;
        [SerializeField] private Color buttonTextNonActiveColor;
        
        private void Start()
        {
            ShowView(false);
            escapeButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    ShowView(false);
                });
        }

        public void ShowView(bool value)
        {
            panel.SetActive(value);
        }
        
        public void SetMoveButtonInteractable(bool value)
        {
            moveButton.interactable = value;
            ChangeButtonTextColor(moveButton, value);
        }

        public void SetAttackButtonInteractable(bool value)
        {
            attackButton.interactable = value;
            ChangeButtonTextColor(attackButton, value);
        }

        public void SetStandbyButtonInteractable(bool value)
        {
            standbyButton.interactable = value;
            ChangeButtonTextColor(standbyButton, value);
        }
        
        public void SetUseSkillButtonInteractable(bool value)
        {
            useSkillButton.interactable = value;
            ChangeButtonTextColor(useSkillButton, value);
        }
        
        public void SetEnableEscapeButtonInteractable(bool value)
        {
            escapeButton.interactable = value;
            ChangeButtonTextColor(escapeButton, value);
        }

        /// <summary>
        /// Buttonの階層下の0番目にTextMeshProGUIがあること前提。
        /// </summary>
        private void ChangeButtonTextColor(Button button,bool value)
        {
            if (button.transform.GetChild(0).TryGetComponent(out TextMeshProUGUI tmp))
            {
                if (value)
                {
                    tmp.color = buttonTextActiveColor;
                    return;
                }
                tmp.color = buttonTextNonActiveColor;
                return;
            }
            Debug.LogError("Not Found TextMeshProUGUI");
        }

        public void SetAllActionButtonInteractable(bool value)
        {
            SetAttackButtonInteractable(value);
            SetStandbyButtonInteractable(value);
            SetUseSkillButtonInteractable(value);
        }
    }
}
