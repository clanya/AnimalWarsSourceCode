using Game.BattleFlow;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(StageCharacterData))] 
    public sealed class CharacterDataEditor : Editor
    {
        private SerializedProperty characterList;
        private ReorderableList characterReorderableList;
        private const string CharacterListString = "characterList";
        
        public void OnEnable()
        {
            characterList = serializedObject.FindProperty(CharacterListString);
            var reorderableList = new ReorderableList(serializedObject, characterList);
            characterReorderableList = reorderableList;
            characterReorderableList.draggable = false;
            characterReorderableList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "キャラクターリスト");
            characterReorderableList.drawElementCallback = (rect, index, _, _) =>
            {
                EditorGUI.indentLevel++;
                var characterProperty = characterList.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect, characterProperty);
                EditorGUI.indentLevel--;
            };
            
            reorderableList.elementHeightCallback = index 
                => EditorGUI.GetPropertyHeight(characterList.GetArrayElementAtIndex(index));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            characterReorderableList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }
    }
