namespace com.faith.gameplay
{
    using UnityEngine;
    using UnityEditor;

    [CustomEditor(typeof(LevelManager))]
    public class LevelManagerEditor : Editor
    {
        private LevelManager Reference;

        private void OnEnable()
        {
            Reference = (LevelManager)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();
            CustomGUI();
            EditorGUILayout.Space();

            DrawDefaultInspector();

            serializedObject.ApplyModifiedProperties();
        }

        #region Configuretion

        private void CustomGUI()
        {

            int t_CurrentLevel = Reference.GetCurrentLevel();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("CurrentLevel (" + t_CurrentLevel + ")");
                if (GUILayout.Button("Increase Level"))
                {
                    Reference.IncreaseLevel();
                }
                if (GUILayout.Button("Reset Level"))
                {
                    Reference.ResetLevel();
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            EditorGUILayout.BeginHorizontal();
            {

                Reference.overridePreviousData = EditorGUILayout.Toggle("OverridePreviousData", Reference.overridePreviousData);

                if (GUILayout.Button("Generate Level Info"))
                {

                    int t_NumberOfLevel = Reference.maxLevel;
                    int index = 0;
                    if (Reference.overridePreviousData)
                    {
                        Reference.levelInfoContainer.ResetInfos();
                        index = 0;

                    }
                    else
                    {
                        if (Reference.levelInfoContainer.levelInfos == null)
                            Reference.levelInfoContainer.ResetInfos();

                        index = Reference.levelInfoContainer.levelInfos.Count;
                    }


                    for (; index < t_NumberOfLevel; index++)
                    {


                        Reference.levelInfoContainer.GenerateLevelInfo(
                                Reference.GetNumberOfColorGun(index),
                                Reference.GetDurationOfEachRound(index)
                            );
                    }

                    Undo.RecordObject(Reference.levelInfoContainer, "LevelInfoContainer changed by editor : " + System.DateTime.Now);
                    EditorUtility.SetDirty(Reference.levelInfoContainer);
                }
            }
            EditorGUILayout.EndHorizontal();


        }

        #endregion




    }

}
