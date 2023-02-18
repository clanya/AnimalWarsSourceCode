using System;
using Game.Character.Skills;
using UnityEngine;

namespace Game.Character.Models
{
    /// <summary>
    /// Note:プロパティ名はインスペクタ拡張のserializedObject.FindPropertyで文字列検索しているので、変更する際は注意すること.
    /// </summary>
    [Serializable]
    public class CharacterParam
    {
        #region SerializeField
        [SerializeField] private CharacterType type;
        [SerializeField] private AttackType attackType;
        [SerializeField] private int hp;
        [SerializeField] private int sp;
        [SerializeField] private int attackPower;
        [SerializeField] private int defensePower;
        [SerializeField] private int magicDefensePower;
        [SerializeField] private int speed;
        [SerializeField] private int movableRange;
        [SerializeField] private int attackRange;

        [Header("AI用に仮置き")]
        [SerializeField] private SkillType skillType;

        [SerializeField] private Skill skill;
        [SerializeField] private SkillSetting skillSetting;
        #endregion

        public CharacterType Type => type;
        public int Hp => hp;
        public int Sp => sp;
        public int AttackPower => attackPower;
        public int DefensePower => defensePower;
        public int MagicDefensePower => magicDefensePower;
        public int Speed => speed;
        public AttackType AttackType => attackType;
        public SkillType SkillType => skillType;
        public int MovableRange => movableRange;
        public Skill Skill => skill;
        public SkillSetting SkillSetting => skillSetting;
        public int AttackRange => attackRange;
    }
}
