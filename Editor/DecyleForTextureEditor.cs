namespace com.faith.Gameplay
{
    using UnityEngine;
    using UnityEditor;

    [CustomEditor(typeof(DecyleForTexture))]
    public class DecyleForTextureEditor : Editor
    {
        private DecyleForTexture Reference;

        private void OnEnable()
        {
            Reference = (DecyleForTexture)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            CustomGUI();

            DrawDefaultInspector();

            serializedObject.ApplyModifiedProperties();
        }

        private void CustomGUI()
        {

            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("ProcessPreloadedTexture"))
                {

                    Reference.ProcessPreloadedTextureForDecyle();

                    Undo.RecordObject(Reference.decyleTextureContainer, "DecyleTextureContainer changed by editor : " + System.DateTime.Now);
                    EditorUtility.SetDirty(Reference.decyleTextureContainer);
                }
            }
            EditorGUILayout.EndHorizontal();
        }



    }
}


