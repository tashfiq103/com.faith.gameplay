namespace com.faith.Gameplay
{
    using UnityEngine;
    using UnityEditor;

    [CustomEditor(typeof(QuestManager))]
    public class QuestManagerEditor : Editor
    {
        private QuestManager Reference;

        private void OnEnable()
        {
            Reference = (QuestManager)target;

            if (QuestManager.Instance == null)
                QuestManager.Instance = Reference;
        }

        public override void OnInspectorGUI()
        {

            serializedObject.Update();

            CreateAndResetPanel();

            ShowQuestInfo();

            DrawDefaultInspector();

            serializedObject.ApplyModifiedProperties();
        }

        private void CreateAndResetPanel()
        {

            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Create Quest"))
                {

                    Reference.CreateQuest(0);
                }

                if (GUILayout.Button("Reset Quest"))
                {

                    Reference.ResetAllQuest();
                }
            }
            EditorGUILayout.EndHorizontal();


        }

        private void ShowQuestInfo()
        {

            if (Reference.ed_ShowQuest)
            {
                int t_NumberOfActiveQuest = Reference.GetNumberOfActiveQuest();

                for (int questIndex = 0; questIndex < t_NumberOfActiveQuest; questIndex++)
                {

                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField(
                        "Quest :> "
                        + Reference.quests[Reference.GetEditorQuestIndex(questIndex)].questID
                        + " :> "
                        + ("(" + Reference.GetSubQuestId(questIndex) + ") ")
                        + Reference.GetCurrentProgressionOfQuest(questIndex)
                        + "/"
                        + Reference.GetAchievementRequiredForQuest(questIndex)
                        + (Reference.IsQuestComplete(questIndex) ? " : Complete" : " : Incomplete"));

                        if (GUILayout.Button("Progress"))
                        {

                            Reference.UpdateQuestProgress(questIndex);
                        }
                    }
                    EditorGUILayout.EndHorizontal();



                    EditorGUI.indentLevel += 2;

                    int t_NumberOfSubQuest = Reference.GetNumberOfSubQuest(questIndex);

                    for (int subQuestIndex = 0; subQuestIndex < t_NumberOfSubQuest; subQuestIndex++)
                    {

                        EditorGUILayout.BeginHorizontal();
                        {

                            EditorGUILayout.LabelField(
                                "SubQuest :> "
                                + Reference.GetCurrentProgressionOfSubQuest(questIndex, subQuestIndex)
                                + "/"
                                + Reference.GetAchievementRequiredOfSubQuest(questIndex, subQuestIndex)
                                + " :> Award "
                                + Reference.GetRewardAmountForSubQuest(questIndex, subQuestIndex)
                                + (Reference.IsSubQuestComplete(questIndex, subQuestIndex) ? " : Complete" : " : Incomplete"));

                        }
                        EditorGUILayout.EndHorizontal();


                    }

                    EditorGUI.indentLevel -= 2;
                }
            }
        }
    }

}

