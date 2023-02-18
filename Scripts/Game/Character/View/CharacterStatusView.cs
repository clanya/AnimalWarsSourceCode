using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Character.View
{
    public sealed class CharacterStatusView : MonoBehaviour
    {
        #region SerializeField
        [SerializeField] private GameObject panel;
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI nameTMP;
        [SerializeField] private TextMeshProUGUI attackTypeTMP;
        [SerializeField] private TextMeshProUGUI hpTMP;
        [SerializeField] private TextMeshProUGUI spTMP;
        [SerializeField] private TextMeshProUGUI attackPowerTMP;
        [SerializeField] private TextMeshProUGUI defensePowerTMP;
        [SerializeField] private TextMeshProUGUI magicDefensePowerTMP;
        [SerializeField] private TextMeshProUGUI speedTMP;
        [SerializeField] private TextMeshProUGUI movableRangeTMP;
        
        [SerializeField] private Button skillShowButton;
        [SerializeField] private GameObject skillIntroducePanel;
        [SerializeField] private TextMeshProUGUI skillExplainTMP;
        
        [SerializeField] private string skillButtonStr1;
        [SerializeField] private string skillButtonStr2;
        #endregion
        
        public void Initialize()
        {
            ButtonClickAsObservable();
        }

        public void ShowView(bool value)
        {
            panel.SetActive(value);
        }
        
        //Spriteを各キャラクタータイプと紐づけているならばタイプを引数に取ったほうがよさげ？
        public void SetSprite(Sprite sprite)
        {
            image.sprite = sprite;
        }
        public void SetNameText(string text)
        {
            nameTMP.text = text;
        }

        public void SetAttackType(AttackType type)
        {
            attackTypeTMP.text = type switch
            {
                AttackType.Physical => "<color=#FF8B00>物理</color>",
                AttackType.Magic => "<color=#D700FF>魔法</color>",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }

        public void SetHpText(int currentHp,int maxHp)
        {
            hpTMP.text = $"HP {FormatText(currentHp,maxHp)}/{maxHp}";
        }

        public void SetSpText(int currentSp,int maxSp)
        {
            spTMP.text = $"SP {FormatText(currentSp,maxSp)}/{maxSp}";
        }

        public void SetAttackPowerText(int currentValue,int maxValue)
        {
            attackPowerTMP.text = $"攻撃 {FormatText(currentValue, maxValue)}";
        }

        public void SetDefensePowerText(int currentValue,int maxValue)
        {
            defensePowerTMP.text = $"防御 {FormatText(currentValue, maxValue)}";
        }

        public void SetMagicDefensePowerText(int currentValue,int maxValue)
        {
            magicDefensePowerTMP.text = $"魔防 {FormatText(currentValue, maxValue)}";
        }

        public void SetSpeed(int currentValue,int maxValue)
        {
            speedTMP.text = $"速さ {FormatText(currentValue, maxValue)}";
        }

        public void SetMovableRange(int currentValue,int maxValue)
        {
            movableRangeTMP.text = $"移動範囲 {FormatText(currentValue, maxValue)}";
        }

        public void SetSkillExplainText(string text)
        {
            skillExplainTMP.text = text;
        }
        
        public void ShowSkillPanelView(bool value)
        {
            skillIntroducePanel.SetActive(value);
        }
        
        /// <summary>
        /// Skillの説明を表示・日表示するボタン
        /// </summary>
        private void ButtonClickAsObservable()
        {
            skillShowButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    bool isActive = skillIntroducePanel.activeSelf;
                    ChangeButtonText(skillShowButton,!isActive);
                    ShowSkillPanelView(!isActive);
                }).AddTo(this);
        }
        
        /// <summary>
        /// Buttonの階層下の0番目にTextMeshProGUIがあること前提。
        /// </summary>
        private void ChangeButtonText(Button button,bool value)
        {
            if (button.transform.GetChild(0).TryGetComponent(out TextMeshProUGUI tmp))
            {
                if (value)
                {
                    tmp.text = skillButtonStr2;
                    return;
                }
                tmp.text = skillButtonStr1;
                return;
            }
            Debug.LogError("Not found TextMeshProUGUI");
        }


        //Note: メソッド名変更 文字の大きさも変えたいかも？
        private string FormatText(int currentValue, int maxValue)
        {
            string color = "white";
            
            if (currentValue < maxValue)
            {
                color = "#FF3333";
            }
            else if (currentValue > maxValue)
            {
                color = "#33FF33";
            }
            
            return $"<color={color}>{currentValue}</color>";
        }
    }
}