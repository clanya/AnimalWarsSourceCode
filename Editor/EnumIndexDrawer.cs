using UnityEngine;
using System.Linq;
using System;
#if UNITY_EDITOR
using UnityEditor;

/// <summary>
/// propertyがList or Arrayであることが前提 int.Parseの部分で例外がでる。
/// </summary>
[CustomPropertyDrawer(typeof(EnumIndexAttribute))]
public class EnumIndexDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //Type型のattributeをEnumIndexAttribute型にキャストし、namesを取得。成功すること前提なので、通常キャスト。
        var names = ((EnumIndexAttribute) attribute).names;
        
        try
        {
            //EnumIndexを使用しているListのパス(例:characterList.Array.data[0])を取得し、リストのインデックスを取得。
            var index = int.Parse(property.propertyPath.Split('[', ']').Last(c => !string.IsNullOrEmpty(c)));
            
            if (index < names.Length)
            {
                label.text = names[index];
            }

            EditorGUI.PropertyField(position, property, label, true);
        }
        catch(FormatException)
        {
            Debug.LogWarning("この属性はArrayやListに付けることを想定しています");
            EditorGUI.PropertyField(position, property);
        }
    }
    
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
}
#endif