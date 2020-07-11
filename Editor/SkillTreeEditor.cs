namespace com.faith.Gameplay
{
    using UnityEngine;
    using UnityEditor;

    [CustomEditor(typeof(SkillTree))]
    public class SkillTreeEditor : Editor
    {

        SkillTree mPSTReference;

        public void OnEnable()
        {

            mPSTReference = (SkillTree)target;
        }

        public override void OnInspectorGUI()
        {

            serializedObject.Update();

            PlayerSkillInspectorGUI();

            DrawDefaultInspector();

            serializedObject.ApplyModifiedProperties();
        }

        private void PlayerSkillInspectorGUI()
        {

            if (mPSTReference.skill != null)
            {

                EditorGUILayout.BeginVertical();
                {

                    for (int skillTreeIndex = 0; skillTreeIndex < mPSTReference.skill.Length; skillTreeIndex++)
                    {

                        EditorGUILayout.BeginHorizontal();
                        {

                            EditorGUILayout.LabelField(
                                mPSTReference.skill[skillTreeIndex].skillName
                                + " ("
                                + mPSTReference.GetCurrentLevelOfSkill(skillTreeIndex).ToString()
                                + ") : "
                                + mPSTReference.GetCurrentStatOfSkill(skillTreeIndex).ToString("#.00")
                                + " : $"
                                + mPSTReference.GetUpgradeCostForNextLevel(skillTreeIndex).ToString("#.00")
                            );

                            if (GUILayout.Button("Upgrade"))
                            {

                                for (int i = 0; i < 5; i++)
                                    mPSTReference.UpgradeSkill(skillTreeIndex);
                            }

                            if (GUILayout.Button("Degrade"))
                            {

                                mPSTReference.DegradeSkill(skillTreeIndex);
                            }

                            if (GUILayout.Button("Reset"))
                            {

                                mPSTReference.ResetSkill(skillTreeIndex);
                            }

                        }
                        EditorGUILayout.EndHorizontal();
                    }

                }
                EditorGUILayout.EndVertical();
            }

            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(2.5f));
        }
    }

}

