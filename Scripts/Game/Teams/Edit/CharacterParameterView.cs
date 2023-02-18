using Game.BattleFlow;
using Game.Character;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Character.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Game.Teams.Edit.EditViewParameterDataTable;

namespace Game.Teams.Edit
{
    public class CharacterParameterView : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private GameObject parameterPanel;
        [SerializeField] private RectTransform parameterPanelTransform;
        [SerializeField] private StageCharacterData stageCharacterData;
        [SerializeField] private EditViewParameterDataTable editViewParameterDataTable;

        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text roleText;
        [SerializeField] private TMP_Text introduceText;
        [SerializeField] private TMP_Text HPText;
        [SerializeField] private TMP_Text SPText;
        [SerializeField] private TMP_Text attackText;
        [SerializeField] private TMP_Text defenceText;
        [SerializeField] private TMP_Text magicDefenceText;
        [SerializeField] private TMP_Text speedText;
        [SerializeField] private TMP_Text moveRangeText;
        [SerializeField] private TMP_Text attackRangeText;
        [SerializeField] private TMP_Text skillExplainText;
        [SerializeField] private TMP_Text attackTypeText;

        [SerializeField] private Slider HPSlider;
        [SerializeField] private Slider SPSlider;
        [SerializeField] private Slider attackSlider;
        [SerializeField] private Slider defenceSlider;
        [SerializeField] private Slider magicDefenceSlider;
        [SerializeField] private Slider speedSlider;
        [SerializeField] private Slider moveRangeSlider;
        [SerializeField] private Slider attackRangeSlider;

        private int maxHP;
        private int maxSP;
        private int maxAttack;
        private int maxDefence;
        private int maxMagicDefence;
        private int maxSpeed;
        private int maxMoveRange;
        private int maxAttackRange;

        private CharacterType viewingCharacter;

        private void Start()
        {
            var paramList = stageCharacterData.CharacterList.Select(x => x.Param);
            maxHP = paramList.Select(x => x.Hp).Max();
            maxSP = paramList.Select(x => x.Sp).Max();
            maxAttack = paramList.Select(x => x.AttackPower).Max();
            maxDefence = paramList.Select(x => x.DefensePower).Max();
            maxMagicDefence = paramList.Select(x => x.MagicDefensePower).Max();
            maxSpeed = paramList.Select(x => x.Speed).Max();
            maxMoveRange = paramList.Select(x => x.MovableRange).Max();
            maxAttackRange = paramList.Select(x => x.AttackRange).Max();
        }

        public void ViewParameter(CharacterType characterType, Vector3 viewPosition)
        {
            parameterPanel.SetActive(true);
            SetPanelPosition(viewPosition);

            if (viewingCharacter != characterType)
            {
                var parameter = stageCharacterData.GetCharacterParameter(characterType);
                var data = editViewParameterDataTable.GetViewData(characterType);

                ViewParameterText(data, parameter);
                ViewParameterSlider(parameter);
            }

            viewingCharacter = characterType;
        }

        private void ViewParameterText(ViewData data, CharacterParam param)
        {
            nameText.text = $"{data.Name}";
            roleText.text = $"{GetRoleName(data.Role)}";
            introduceText.text = $"{data.Introduce}";

            HPText.text = $"HP:{param.Hp}";
            SPText.text = $"SP:{param.Sp}";
            attackText.text = $"攻撃力:{param.AttackPower}";
            defenceText.text = $"防御力:{param.DefensePower}";
            magicDefenceText.text = $"魔法防御力:{param.MagicDefensePower}";
            speedText.text = $"スピード:{param.Speed}";
            moveRangeText.text = $"移動範囲:{param.MovableRange}";
            attackRangeText.text = $"攻撃範囲:{param.AttackRange}";
            if (param.AttackType == AttackType.Physical)
            {
                attackTypeText.text = "$攻撃タイプ:物理";
            }
            else if (param.AttackType == AttackType.Magic)
            {
                attackTypeText.text = "$攻撃タイプ:魔法";
            }
            

            skillExplainText.text = data.SkillExplain;
        }

        private void ViewParameterSlider(CharacterParam param)
        {
            HPSlider.value = (float)param.Hp / maxHP;
            SPSlider.value = (float)param.Sp / maxSP;
            attackSlider.value = (float)param.AttackPower / maxAttack;
            defenceSlider.value = (float)param.DefensePower / maxDefence;
            magicDefenceSlider.value = (float)param.MagicDefensePower / maxMagicDefence;
            speedSlider.value = (float)param.Speed / maxSpeed;
            moveRangeSlider.value = (float)param.MovableRange / maxMoveRange;
            attackRangeSlider.value = (float)param.AttackRange / maxAttackRange;
        }

        private void SetPanelPosition(Vector3 position)
        {
            position.y = Mathf.Clamp(position.y, 0f, 450f);

            var pos = mainCamera.WorldToViewportPoint(position);
            if (pos.x>=50f)
            {
                parameterPanelTransform.pivot = new Vector2(1, 0);
            }
            else
            {
                parameterPanelTransform.pivot = new Vector2(0, 0);
            }

            parameterPanel.transform.position = position;
        }

        public void HideParameter()
        {
            parameterPanel.SetActive(false);
        }

        public string GetRoleName(RoleType role)
            => role switch
            {
                RoleType.Attacker => "アタッカー",
                RoleType.SubAttacker => "準アタッカー",
                RoleType.AllRounder => "万能型",
                RoleType.Tank => "タンク",
                RoleType.Healer => "ヒーラー",
                _ => throw new ArgumentOutOfRangeException(),
            };
    }
}
