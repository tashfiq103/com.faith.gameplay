namespace com.faith.gameplay {
    using UnityEngine;

    public enum DialougeTag {
        Player,
        NPC,
        Random
    }

    [System.Serializable]
    public struct DialougeScript {
        public DialougeTag dialougeTag;
        public Sprite expression;
        public Color speechColor;
        public string speech;
    }

    [CreateAssetMenu (fileName = "Dialouge", menuName = "CustomScriptableObject/Dialouge")]
    public class DialougePack : ScriptableObject {
        #region Custom Variables

        [System.Serializable]
        public struct Dialouge {
            public string dialougeName;
            public DialougeScript[] dialougeScripts;
        }

        #endregion

        #region Configuretion

        #endregion

        #region Public Variables

        public Dialouge[] dialouges;

        #endregion

        #region Public Callback

        public bool IsValidDialouge (int t_DialougeIndex) {
            if (t_DialougeIndex >= 0 && t_DialougeIndex < dialouges.Length)
                return true;

            Debug.LogError ("Invalid DialougeIndex : " + t_DialougeIndex + ", Must be with in [0," + dialouges.Length + ")");
            return false;
        }

        public bool IsValidDialougeScript (int t_DialougeIndex, int t_DialougeScriptIndex) {

            if (IsValidDialouge (t_DialougeIndex) && (t_DialougeScriptIndex >= 0 && t_DialougeScriptIndex < dialouges[t_DialougeIndex].dialougeScripts.Length))
                return true;

            Debug.LogError ("Invalid DialougeScriptIndex : " + t_DialougeScriptIndex + ", Must be with in [0," + dialouges[t_DialougeIndex].dialougeScripts.Length + ")");
            return false;
        }

        public int GetNumberOfDialougeScript (int t_DialougeIndex) {

            if (IsValidDialouge (t_DialougeIndex))
                return dialouges[t_DialougeIndex].dialougeScripts.Length;

            return 0;
        }

        public DialougeTag GetDialougeTag (int t_DialougeIndex, int t_DialougeScriptIndex) {

            if (IsValidDialougeScript (t_DialougeIndex, t_DialougeScriptIndex))
                return dialouges[t_DialougeIndex].dialougeScripts[t_DialougeScriptIndex].dialougeTag;

            return DialougeTag.Random;
        }

        public Sprite GetDialougeExpression (int t_DialougeIndex, int t_DialougeScriptIndex) {

            if (IsValidDialougeScript (t_DialougeIndex, t_DialougeScriptIndex))
                return dialouges[t_DialougeIndex].dialougeScripts[t_DialougeScriptIndex].expression;

            return null;
        }

        public Color GetDialougeSpeechColor (int t_DialougeIndex, int t_DialougeScriptIndex) {

            if (IsValidDialougeScript (t_DialougeIndex, t_DialougeScriptIndex))
                return dialouges[t_DialougeIndex].dialougeScripts[t_DialougeScriptIndex].speechColor == new Color () ? Color.black : dialouges[t_DialougeIndex].dialougeScripts[t_DialougeScriptIndex].speechColor;

            return Color.black;
        }

        public string GetDialougeSpeech (int t_DialougeIndex, int t_DialougeScriptIndex) {

            if (IsValidDialougeScript (t_DialougeIndex, t_DialougeScriptIndex))
                return dialouges[t_DialougeIndex].dialougeScripts[t_DialougeScriptIndex].speech;

            return "";
        }

        #endregion
    }

}