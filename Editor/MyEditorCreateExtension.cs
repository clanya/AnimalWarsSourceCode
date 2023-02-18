using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEditor;

public sealed class MyEditorCreateExtension
{
    private const string MenuItemMaterialCreationString = "Assets/Create/Material in Materials(Folder)";
    private const string MenuItemHLSLCreationStr = "Assets/Create/Shader/Shader(.hlsl)";
    
    /// <summary>
    /// 選択中のShader(ShaderGraph)のマテリアルを"Materials"フォルダに作成する
    /// </summary>
    [MenuItem(MenuItemMaterialCreationString, priority = 301)]
    private static void CreateMaterialInMaterialsFolder()
    {
        var selectedObject = Selection.objects[0];
        
        if (selectedObject is Shader selectedShader)
        {
            var material = new Material(selectedShader);
            var fileName = Path.GetFileName(selectedShader.name);
            ProjectWindowUtil.CreateAsset(material,@$"Assets/Materials/{fileName}.mat");
            return;
        }

        Debug.Log("Please Select Shader or ShaderGraph");
    }

    /// <summary>
    /// Shaderが選択中の時しかMenuItemを有効にしない
    /// </summary>
    [MenuItem(MenuItemMaterialCreationString,validate = true)]
    private static bool CanShowValidation()
    {
        var selectedObject = Selection.objects[0];
        return selectedObject is Shader;
    }
    
    /// <summary>
    /// 現在ActiveになっているProjectWindowにHLSLファイルを作成する
    /// </summary>
    [MenuItem(MenuItemHLSLCreationStr)]
    private static void CreateHlslFile()
    {
        var path = GetActiveFolderPath();
        File.WriteAllText(path + "/New hlsl.hlsl", "", System.Text.Encoding.UTF8);
        AssetDatabase.Refresh();
    }
    
    /// <summary>
    /// ProjectWindowでActiveになっているフォルダのパスを取得する
    /// </summary>
    private static string GetActiveFolderPath()
    {
        const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
        var asm = Assembly.Load("UnityEditor.dll"); //指定した名前でアセンブリを読み込む
        var typeProjectBrowser = asm.GetType("UnityEditor.ProjectBrowser"); //指定した名前のTypeオブジェクトを、アセンブリインスタンスから取得
        var projectBrowserWindow = EditorWindow.GetWindow(typeProjectBrowser);  //現在画面上にある ProjectBrowserタイプの最初に見つけたEditorWindowを返す
        //Type.GetMethod(string,BindingFlags)でstring名のメソッドを取得  返り値はMethodInfo型(メソッドを表すオブジェクト)
        //MethodInfo.Invokeで実行
        return (string)typeProjectBrowser.GetMethod("GetActiveFolderPath", flags)?.Invoke(projectBrowserWindow, null); 
    }
    //https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/ProjectBrowser.cs#:~:text=//%20Return%20the,%7D
    //https://qiita.com/r-ngtm/items/13d609cbd6a30e39f83a
}