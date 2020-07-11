using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (GameManager))]
public class GameManagerEditor : Editor {

	GameManager Reference;

	private void OnEnable () {

		Reference = (GameManager) target;

		if(GameManager.Instance == null)
			GameManager.Instance = Reference;

	}

	public override void OnInspectorGUI () {

		serializedObject.Update ();

		EditorGUILayout.BeginHorizontal (); {

			EditorGUILayout.LabelField("Currency : $" + Reference.GetInGameCurrency().ToString("#.00"));
			if(GUILayout.Button("Increase")){

				Reference.UpdateInGameCurrency(1000000000);
			}
			if(GUILayout.Button("Reset")){

				Reference.ResetInGameCurrency();
			}
		}EditorGUILayout.EndHorizontal();

		//----------
		EditorGUILayout.BeginHorizontal (); {
			
			if(Reference.IsGamePaused()){
				//if : GamePaused
				if (GUILayout.Button ("Game Resume")) {

					Reference.ResumeGame();
				}
			}else{

				if (GUILayout.Button ("Game Pause")) {

					Reference.PauseGame();
				}
			}

		}
		EditorGUILayout.EndHorizontal ();

		DrawDefaultInspector ();

		serializedObject.ApplyModifiedProperties ();
	}
}