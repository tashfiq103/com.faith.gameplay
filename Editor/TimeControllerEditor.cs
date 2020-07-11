namespace com.faith.gameplay
{
    using UnityEngine;
    using UnityEditor;

    [CustomEditor(typeof(TimeController))]
    public class TimeControllerEditor : Editor
    {
        private TimeController Reference;

        private void OnEnable()
        {
            Reference = (TimeController)target;
        }

        public override void OnInspectorGUI()
        {

            if (Reference.showDebugPanel)
            {

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("TimeScale :" + Time.timeScale);
                    EditorGUILayout.LabelField("FixedDeltaTime :" + Time.fixedDeltaTime);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();

            }


            base.OnInspectorGUI();
        }
    }
}

