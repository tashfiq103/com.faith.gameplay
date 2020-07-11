﻿namespace com.faith.Gameplay
{
    using UnityEngine;

    using System.Collections.Generic;

    [CreateAssetMenu(fileName = "DecyleTextyreContainer", menuName = "Info Container/Create DecyleTextyreContainer")]
    public class DecyleTexturesContainer : ScriptableObject
    {
        public List<DecyleTextureInfo> decyleTextureInfo;

        public void AddDecyleTextureInfo(Texture2D t_OriginalTexture, DecyleTextureInfo t_DecyleTextureInfo, bool IsOverrideAllow = false)
        {

            bool t_IsDuplicate = false;
            int t_NumberOfStoredTextureInfoOfDecyle = decyleTextureInfo.Count;

            for (int i = 0; i < t_NumberOfStoredTextureInfoOfDecyle; i++)
            {

                if (decyleTextureInfo[i].texture2D == t_OriginalTexture)
                {

                    if (IsOverrideAllow)
                    {
                        decyleTextureInfo[i] = t_DecyleTextureInfo;
                    }

                    t_IsDuplicate = true;
                    break;
                }
            }

            if (!t_IsDuplicate)
            {

                decyleTextureInfo.Add(t_DecyleTextureInfo);
            }
        }
    }
}

