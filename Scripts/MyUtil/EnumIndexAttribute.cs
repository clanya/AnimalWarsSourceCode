using UnityEngine;
using System;

public class EnumIndexAttribute : PropertyAttribute
{
    public readonly string[] names;
    public EnumIndexAttribute(Type enumType)
    {
        //コンパイラがエラーを吐いてくれるが、一応自分でもEnumかチェック
        if (!enumType.IsEnum)
        {
            Debug.LogError("Not an enum");
        }
        names = Enum.GetNames(enumType);
    }
}