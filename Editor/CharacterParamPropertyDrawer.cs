using System;
using Game.Character;
using Game.Character.Models;
using Game.Character.Skills;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(CharacterParam))]
public sealed class CharacterParamPropertyDrawer : PropertyDrawer
{
    #region PropertyStrings
    private const string CharacterTypeString = "type";
    private const string AttackTypeString = "attackType";
    private const string HpString = "hp";
    private const string SpString = "sp";
    private const string AttackPowerString = "attackPower";
    private const string DefensePowerString = "defensePower";
    private const string MagicDefensePowerString = "magicDefensePower";
    private const string SpeedString = "speed";
    private const string MovableRangeString = "movableRange";
    private const string AttackRangeString = "attackRange";
    private const string SkillTypeString = "skillType";
    private const string SkillString = "skill";
    private const string SkillSettingString = "skillSetting";
    #endregion
    
    private Property propertyData = new();
    private bool isExpandedParameter = true;
    private bool isExpandedSkillSettings = true;
    private class Property
    {
        public SerializedProperty characterType;
        public SerializedProperty attackType;
        public SerializedProperty hp;
        public SerializedProperty sp;
        public SerializedProperty attackPower;
        public SerializedProperty defensePower;
        public SerializedProperty magicDefensePower;
        public SerializedProperty speed;
        public SerializedProperty movableRange;
        public SerializedProperty attackRange;
        public SerializedProperty skillType;
        public SerializedProperty skill;
        public SerializedProperty skillSetting;
    }
    
    private int lineHeightCount = 0;
    private float LineHeight => EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

    private void Initialize(SerializedProperty property)
    {
        propertyData.characterType = property.FindPropertyRelative(CharacterTypeString);
        propertyData.attackType = property.FindPropertyRelative(AttackTypeString);
        propertyData.hp = property.FindPropertyRelative(HpString);
        propertyData.sp = property.FindPropertyRelative(SpString);
        propertyData.attackPower = property.FindPropertyRelative(AttackPowerString);
        propertyData.defensePower = property.FindPropertyRelative(DefensePowerString);
        propertyData.magicDefensePower = property.FindPropertyRelative(MagicDefensePowerString);
        propertyData.speed = property.FindPropertyRelative(SpeedString);
        propertyData.movableRange = property.FindPropertyRelative(MovableRangeString);
        propertyData.attackRange = property.FindPropertyRelative(AttackRangeString);
        propertyData.skillType = property.FindPropertyRelative(SkillTypeString);
        propertyData.skill = property.FindPropertyRelative(SkillString);
        propertyData.skillSetting = property.FindPropertyRelative(SkillSettingString);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        using (new EditorGUI.PropertyScope(position,label,property))
        {
            Initialize(property);
            var leftRect = position;
            var rightRect = position;
            leftRect.height = EditorGUIUtility.singleLineHeight;
            rightRect.height = EditorGUIUtility.singleLineHeight;
            
            EditorGUI.BeginChangeCheck();
            EditorGUIUtility.labelWidth = position.width / 2;
            // EditorGUI.PropertyField(leftRect, propertyData.characterType,new GUIContent("Character Type"));
            // OneLineDownHeight(ref leftRect,ref rightRect);

            isExpandedParameter = EditorGUI.Foldout(leftRect, isExpandedParameter, "パラメータ");
            OneLineDownHeight(ref leftRect,ref rightRect);
            
            if (isExpandedParameter)
            {
                EditorGUI.PropertyField(leftRect, propertyData.attackType);
                OneLineDownHeight(ref leftRect,ref rightRect);
            
                EditorGUIUtility.labelWidth = position.width / 4;
                var x = leftRect.width / 2;
                rightRect.x += x;
                leftRect.width /= 2;
                rightRect.width /= 2;
            
                EditorGUI.PropertyField(leftRect, propertyData.hp,new GUIContent("HP"));
                EditorGUI.PropertyField(rightRect, propertyData.sp, new GUIContent("SP"));
                OneLineDownHeight(ref leftRect,ref rightRect);
            
                EditorGUI.PropertyField(leftRect, propertyData.attackPower,new GUIContent("攻撃力"));
                EditorGUI.PropertyField(rightRect, propertyData.speed, new GUIContent("速さ"));
                OneLineDownHeight(ref leftRect,ref rightRect);
            
                EditorGUI.PropertyField(leftRect, propertyData.defensePower,new GUIContent("防御力"));
                EditorGUI.PropertyField(rightRect, propertyData.magicDefensePower, new GUIContent("魔法防御力"));
                OneLineDownHeight(ref leftRect,ref rightRect);
            
                EditorGUI.PropertyField(leftRect, propertyData.movableRange,new GUIContent("移動可能範囲"));
                EditorGUI.PropertyField(rightRect, propertyData.attackRange, new GUIContent("攻撃可能範囲"));
                OneLineDownHeight(ref leftRect,ref rightRect);
            }
            else
            {
                EditorGUIUtility.labelWidth = position.width / 4;
                var x = leftRect.width / 2;
                rightRect.x += x;
                leftRect.width /= 2;
                rightRect.width /= 2;
            }
            
            EditorGUIUtility.labelWidth = position.width / 2;
            leftRect.width *= 2;
            EditorGUI.PropertyField(leftRect,propertyData.skill);
            propertyData.skillSetting.FindPropertyRelative("Skill").enumValueIndex = propertyData.skill.enumValueIndex;
            OneLineDownHeight(ref leftRect,ref rightRect);

            isExpandedSkillSettings = EditorGUI.Foldout(leftRect, isExpandedSkillSettings, "スキル設定");
            OneLineDownHeight(ref leftRect,ref rightRect);
            
            if (isExpandedSkillSettings)
            {
                EditorGUI.PropertyField(leftRect, propertyData.skillSetting);
            }
            
            EditorGUI.EndChangeCheck();
        }
    }

    private void OneLineDownHeight(ref Rect left,ref Rect right)
    {
        left.y += LineHeight;
        right.y += LineHeight;
        lineHeightCount += 1;
    }
    
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var height = base.GetPropertyHeight(property, label);
        if (isExpandedParameter)
        {
            height += EditorGUIUtility.singleLineHeight * 8;
        }
        else
        {
            height += EditorGUIUtility.singleLineHeight * 3;
        }
        
        if (isExpandedSkillSettings)
        {
            var skill = property.FindPropertyRelative(SkillSettingString).FindPropertyRelative("Skill").intValue;
            var targetType = property.FindPropertyRelative(SkillSettingString).FindPropertyRelative("targetType").intValue;
            var arraySize = property.FindPropertyRelative(SkillSettingString)
                .FindPropertyRelative("buffForStatusTypeList")
                .arraySize;
            
            height += (LineHeight + EditorGUIUtility.standardVerticalSpacing) * GetSkillSettingsHeight(skill,targetType,arraySize-1);
        }
        return height;
    }

    private int GetSkillSettingsHeight(int skill,int targetType,int arraySize)
    {
        int heightCount = 0;
        switch ((Skill)skill)
        {
            //Test
            case Skill.Skill_00:
            //Buff,DeBuff
            case Skill.Skill_01:
            case Skill.Skill_06:
            case Skill.Skill_07:
            case Skill.Skill_10:
            case Skill.Skill_11:
            case Skill.Skill_12:
                arraySize = Math.Max(0, arraySize);
                heightCount += 7 + arraySize;
                break;
            //Attack
            case Skill.Skill_02:
            case Skill.Skill_03:
            case Skill.Skill_08:
            case Skill.Skill_09:
                heightCount += 4;
                break;
            //Heal
            case Skill.Skill_04:
            case Skill.Skill_05:
                heightCount += 3;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        switch ((TargetType)targetType)
        {
            case TargetType.Self:
            case TargetType.AllFriends:
            case TargetType.AllEnemies:
                break;
            case TargetType.OneFriend:
            case TargetType.OneEnemy:
                heightCount += 1;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        return heightCount;
    }
}
#endif
