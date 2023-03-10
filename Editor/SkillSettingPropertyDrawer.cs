using System;
using Game.Character.Skills;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SkillSetting))]
public sealed class SkillSettingPropertyDrawer : PropertyDrawer
{
    #region PropertyStrings
    private const string SkillString = "Skill";
    private const string AttackTypeString = "attackSkillType";
    private const string BuffForStatusTypeListString = "buffForStatusTypeList";
    private const string AmountString = "amount";
    private const string TargetType = "targetType";
    private const string TargetRangeString = "targetRange";
    private const string ApplicableTurnString = "applicableTurn";
    private const string CostString = "cost";
    #endregion

    private readonly Property propertyData = new();
    private int singleLineHeightCount = 0;
    private float LineHeight => EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

    private class Property
    {
        public SerializedProperty skill;
        public SerializedProperty attackSkillType;
        public SerializedProperty buffForStatusTypeList;
        public SerializedProperty amount;
        public SerializedProperty targetType;
        public SerializedProperty targetRange;
        public SerializedProperty applicableTurn;
        public SerializedProperty cost;
        
        public ReorderableList buffForStatusTypeReorderableList;
    }
    
    private void Initialize(SerializedProperty property)
    {
        propertyData.skill = property.FindPropertyRelative(SkillString);
        propertyData.attackSkillType = property.FindPropertyRelative(AttackTypeString);
        propertyData.buffForStatusTypeList = property.FindPropertyRelative(BuffForStatusTypeListString);
        propertyData.amount = property.FindPropertyRelative(AmountString);
        propertyData.targetType = property.FindPropertyRelative(TargetType);
        propertyData.targetRange = property.FindPropertyRelative(TargetRangeString);
        propertyData.applicableTurn = property.FindPropertyRelative(ApplicableTurnString);
        propertyData.cost = property.FindPropertyRelative(CostString);

        var reorderableList = new ReorderableList(property.serializedObject, propertyData.buffForStatusTypeList);
        propertyData.buffForStatusTypeReorderableList = reorderableList;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Initialize(property);
    
        using (new EditorGUI.PropertyScope(position,label,property))
        {
            var pos = position;
            pos.height = EditorGUIUtility.singleLineHeight;
            EditorGUIUtility.labelWidth = position.width / 2;

            EditorGUI.BeginChangeCheck();
            string amountString = "default";
            switch ((Skill)propertyData.skill.intValue)
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
                    //buffForStatusTypeList, applicableTurn?????????
                    var reorderableList = propertyData.buffForStatusTypeReorderableList;
                    reorderableList.draggable = false;
                    reorderableList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "?????????????????????????????????????????????");
                    reorderableList.elementHeightCallback = index => LineHeight;
                    reorderableList.drawElementCallback = (rect, index, active, focused) =>
                    {
                        var elementProperty = propertyData.buffForStatusTypeList.GetArrayElementAtIndex(index);
                        EditorGUI.PropertyField(rect, elementProperty, new GUIContent($"???????????????????????? {index}"));
                    };
                    reorderableList.DoList(pos);
                    pos.y += reorderableList.GetHeight();
                    
                    EditorGUI.PropertyField(pos,propertyData.applicableTurn,new GUIContent("????????????????????????????????????"));
                    OneLineDownHeight(ref pos);
                    amountString = "?????????????????????";
                    break;
                //Attack
                case Skill.Skill_02:
                case Skill.Skill_03:
                case Skill.Skill_08:
                case Skill.Skill_09:
                    //attackSkillType?????????
                    EditorGUI.PropertyField(pos, propertyData.attackSkillType,new GUIContent("????????????"));
                    OneLineDownHeight(ref pos);
                    amountString = "?????????";
                    break;
                //Heal
                case Skill.Skill_04:
                case Skill.Skill_05:
                    amountString = "?????????";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            //??????????????? : amount,targetType,cost

            EditorGUI.PropertyField(pos, propertyData.targetType, new GUIContent("?????????????????????"));
            OneLineDownHeight(ref pos);
            
            //targetRange???targetType??? OneFriend or OneEnemy??????????????????
            switch ((TargetType)propertyData.targetType.intValue)
            {
                case Game.Character.Skills.TargetType.Self:
                case Game.Character.Skills.TargetType.AllFriends:
                case Game.Character.Skills.TargetType.AllEnemies:
                    break;
                case Game.Character.Skills.TargetType.OneFriend:
                case Game.Character.Skills.TargetType.OneEnemy:
                    EditorGUI.PropertyField(pos, propertyData.targetRange, new GUIContent("????????????"));
                    OneLineDownHeight(ref pos);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            EditorGUI.PropertyField(pos, propertyData.amount,new GUIContent(amountString));
            OneLineDownHeight(ref pos);
            EditorGUI.PropertyField(pos, propertyData.cost,new GUIContent("SP?????????"));
            OneLineDownHeight(ref pos);
            

            EditorGUI.EndChangeCheck();
        }
    }
    
    private void OneLineDownHeight(ref Rect position)
    {
        position.y += LineHeight;
        singleLineHeightCount += 1;
    }
    
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        Initialize(property);
        var height = base.GetPropertyHeight(property, label) + 
                     EditorGUIUtility.singleLineHeight * singleLineHeightCount +
                     propertyData.buffForStatusTypeReorderableList.GetHeight();
        return  height;
    }
}
#endif