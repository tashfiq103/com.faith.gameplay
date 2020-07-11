namespace com.faith.gameplay
{
    using UnityEngine;
    using UnityEditor;

    [CustomEditor(typeof(TutorialManager))]
    public class TutorialManagerEditor : Editor
    {

        public TutorialManager Reference;

        void OnEnable()
        {

            Reference = (TutorialManager)target;
        }

        public override void OnInspectorGUI()
        {

            serializedObject.Update();

            if (GUILayout.Button("Reset Tutorial"))
            {

                Reference.ResetAllTutorial();
            }

            DrawDefaultInspector();

            serializedObject.ApplyModifiedProperties();
        }
    }
}


