namespace com.faith.gameplay
{
    using UnityEngine;

    using System.Collections.Generic;

    #region Custom Variables (Global)

    [System.Serializable]
    public class PixelInfo
    {
        public float colorTransparency;
        public float colorSaturation;
        public Vector2Int pixelPosition;

        public PixelInfo(float colorTransparency, float colorSaturation, Vector2Int pixelPosition)
        {

            this.colorTransparency = colorTransparency;
            this.colorSaturation = colorSaturation;
            this.pixelPosition = pixelPosition;
        }
    }

    [System.Serializable]
    public class PixelGroupInfo
    {
        public int numberOfPixelsInThisGroup;
        public Vector2Int pixelGroupPosition;
        public List<PixelInfo> pixelInfo;

        public void CountTheNumberOfPixelInThisGroup()
        {
            numberOfPixelsInThisGroup = pixelInfo.Count;
        }
    }

    [System.Serializable]
    public class DecyleTextureInfo
    {
        public Texture2D texture2D;
        public PixelGroupInfo[] pixelGroupInfo;

        public int numberOfVerticalPixelGroup;
        public int numberOfHorizontalPixelGroup;
        public int numberOfPixelGroupInfo;

        public DecyleTextureInfo(Texture2D t_Texture2D)
        {

            texture2D = t_Texture2D;
        }

        public void CretatePixelGroup(
            int t_NumberOfVerticalPixelGroup,
            int t_NumberOfHorizontalPixelGroup,
            int t_NumberOfPixelsPerVerticalPixelGroup,
            int t_NumberOfPixelsPerHorizontalPixelGroup)
        {

            numberOfVerticalPixelGroup = t_NumberOfVerticalPixelGroup;
            numberOfHorizontalPixelGroup = t_NumberOfHorizontalPixelGroup;
            numberOfPixelGroupInfo = t_NumberOfVerticalPixelGroup * t_NumberOfHorizontalPixelGroup;
            pixelGroupInfo = new PixelGroupInfo[numberOfPixelGroupInfo];

            int t_AbsoluteIndex;


            for (int i = 0; i < t_NumberOfHorizontalPixelGroup; i++)
            {

                for (int j = 0; j < t_NumberOfVerticalPixelGroup; j++)
                {

                    t_AbsoluteIndex = i + (j * t_NumberOfHorizontalPixelGroup);

                    pixelGroupInfo[t_AbsoluteIndex] = new PixelGroupInfo();
                    pixelGroupInfo[t_AbsoluteIndex].pixelGroupPosition = new Vector2Int(
                        (i * t_NumberOfPixelsPerHorizontalPixelGroup) + (t_NumberOfPixelsPerHorizontalPixelGroup / 2),
                        (j * t_NumberOfPixelsPerVerticalPixelGroup) + (t_NumberOfPixelsPerVerticalPixelGroup / 2));
                    //Debug.Log("PixelGroupPosition : " + pixelGroupInfo[t_AbsoluteIndex].pixelGroupPosition);
                    pixelGroupInfo[t_AbsoluteIndex].pixelInfo = new List<PixelInfo>();
                }
            }
        }
    }

    #endregion

    public class DecyleForTexture : MonoBehaviour
    {
        #region Custom Variables (Local)

        public enum DecyleShape
        {
            circle,
            square
        };

        #endregion

        #region Public Variables

        [Header("Configuretion  :   Editor")]
        public bool IsOverrideAllow = false;
        public DecyleTexturesContainer decyleTextureContainer;

        [Space(5.0f)]
        public DecyleShape defaultDecyleShape = DecyleShape.square;
        [Range(0f, 1f)]
        public float defaultPixelGroupArea = 0.05f;
        [Range(1f, 3f)]
        public float defaultPixelSpreadArea = 1f;
        [Range(0f, 1f)]
        public float defaultPixelDensityOnSpreadingArea = 0.05f;
        [Range(0.5f, 1f)]
        public float colorSaturation = 0.5f;

        [Space(5.0f)]
        public Texture2D[] decyleTextures;

        #endregion

        #region Private Variables

        private DecyleShape m_DecyleShape;

        private float m_PixelGroupArea;
        private float m_PixelSpreadArea;
        private float m_PixelDensityOnSpreadingArea;

        #endregion

        #region Mono Behaviour

        #endregion

        #region Configuretion

        private void DecyleProcessor(Texture2D t_Texture, bool t_StoreToDatabase = true)
        {
            //Considering

            //m_PixelGroupArea = 0.1f

            //t_Texture                                 = 16x16px
            //t_Height                                  = 16
            //t_Width                                   = 16;

            //t_NumberOfVerticalPixelGroup              = Mathf.CeilToInt(16 * 0.1) = Mathf.CeilTonInt(1.6) = 2
            //t_NumberOfHorizontalPixelGroup            = Mathf.CeilToInt(16 * 0.1) = Mathf.CeilTonInt(1.6) = 2
            //t_TotalNumberOfPixelGroup                 = 2 + 2 = 4

            //t_NumberOfPixelsPerVerticalPixelGroup     = 16 / 2 = 8
            //t_NumberOfPixelsPerHorizontalPixelGroup   = 16 / 2 = 8


            int t_Height = t_Texture.height;
            int t_Width = t_Texture.width;

            int t_NumberOfVerticalPixelGroup = Mathf.CeilToInt(t_Height / (t_Height * m_PixelGroupArea * 1f));
            int t_NumberOfHorizontalPixelGroup = Mathf.CeilToInt(t_Width / (t_Width * m_PixelGroupArea * 1f));

            int t_TotalNumberOfPixelGroup = t_NumberOfVerticalPixelGroup * t_NumberOfHorizontalPixelGroup;

            int t_NumberOfPixelsPerVerticalPixelGroup = Mathf.CeilToInt(t_Height / t_NumberOfVerticalPixelGroup);
            int t_NumberOfPixelsPerHorizontalPixelGroup = Mathf.CeilToInt(t_Width / t_NumberOfHorizontalPixelGroup);

            Debug.Log("(Width, Height) : (" + t_Width + "," + t_Height + ")");
            Debug.Log("(Horizontal, Vertical) : (" + t_NumberOfHorizontalPixelGroup + "," + t_NumberOfVerticalPixelGroup + ")");
            Debug.Log("(PixelOnHorizontal, PixelOnVertical) : (" + t_NumberOfPixelsPerHorizontalPixelGroup + "," + t_NumberOfPixelsPerVerticalPixelGroup + ")");

            float t_RadiusOfCirclePixel = Mathf.Sqrt(Mathf.Pow(t_NumberOfPixelsPerHorizontalPixelGroup / 2f, 2) + Mathf.Pow(t_NumberOfPixelsPerVerticalPixelGroup / 2f, 2));
            float t_ExtendedRadiusOfCirclePixel = t_RadiusOfCirclePixel * defaultPixelSpreadArea;
            float t_CurrentDistance;
            float t_ValueMiltiplier;

            int t_PixelGroupPositionX;
            int t_PixelGroupPositionY;
            int t_PixelGroupIndex;

            DecyleTextureInfo t_NewDecyleTexture = new DecyleTextureInfo(t_Texture);
            t_NewDecyleTexture.CretatePixelGroup(
                t_NumberOfVerticalPixelGroup,
                t_NumberOfHorizontalPixelGroup,
                t_NumberOfPixelsPerVerticalPixelGroup,
                t_NumberOfPixelsPerHorizontalPixelGroup);

            for (int i = 0; i < t_Width; i++)
            {

                for (int j = 0; j < t_Height; j++)
                {

                    switch (m_DecyleShape)
                    {
                        case DecyleShape.circle:

                            for (int k = 0; k < t_NewDecyleTexture.numberOfPixelGroupInfo; k++)
                            {

                                Vector2Int t_PixelGroupPosition = t_NewDecyleTexture.pixelGroupInfo[k].pixelGroupPosition;
                                Vector2Int t_PixelPosition = new Vector2Int(i, j);

                                t_CurrentDistance = Vector2Int.Distance(t_PixelGroupPosition, t_PixelPosition);

                                if (t_CurrentDistance <= t_RadiusOfCirclePixel)
                                {
                                    t_ValueMiltiplier = t_CurrentDistance / t_RadiusOfCirclePixel;

                                    t_NewDecyleTexture.pixelGroupInfo[k].pixelInfo.Add(new PixelInfo(
                                            1f,
                                            t_ValueMiltiplier * 0.5f,
                                            new Vector2Int(i, j)
                                        ));
                                }
                                else if ((Random.Range(0f, 1f) <= defaultPixelDensityOnSpreadingArea) && (t_CurrentDistance <= t_ExtendedRadiusOfCirclePixel))
                                {

                                    t_ValueMiltiplier = t_CurrentDistance / t_ExtendedRadiusOfCirclePixel;

                                    t_NewDecyleTexture.pixelGroupInfo[k].pixelInfo.Add(new PixelInfo(
                                            1f,
                                            t_ValueMiltiplier,
                                            new Vector2Int(i, j)
                                        ));
                                }
                            }

                            break;
                        case DecyleShape.square:

                            t_PixelGroupPositionX = i / (t_NumberOfPixelsPerHorizontalPixelGroup * 1);
                            t_PixelGroupPositionY = j / (t_NumberOfPixelsPerVerticalPixelGroup * 1);

                            t_PixelGroupIndex = t_PixelGroupPositionX + (t_PixelGroupPositionY * t_NumberOfHorizontalPixelGroup);

                            t_NewDecyleTexture.pixelGroupInfo[t_PixelGroupIndex].pixelInfo.Add(new PixelInfo(
                                            1f,
                                            1f,
                                            new Vector2Int(i, j)
                                        ));
                            break;
                    }


                }
            }

            for (int pixelGroupIndex = 0; pixelGroupIndex < t_TotalNumberOfPixelGroup; pixelGroupIndex++)
            {

                t_NewDecyleTexture.pixelGroupInfo[pixelGroupIndex].CountTheNumberOfPixelInThisGroup();
            }

            decyleTextureContainer.AddDecyleTextureInfo(t_Texture, t_NewDecyleTexture, IsOverrideAllow);
        }

        private Texture2D DecyleTexture(int t_TextureIndex, Texture2D t_CurrentTexture, Vector2 t_DecylePosition, Color t_ColorOfDecyle)
        {

            DecyleTextureInfo t_CurrentDecyleTextureInfo = decyleTextureContainer.decyleTextureInfo[t_TextureIndex];

            int t_PixelInfoX = (int)(t_CurrentDecyleTextureInfo.numberOfHorizontalPixelGroup * t_DecylePosition.x);
            int t_PixelInfoY = (int)(t_CurrentDecyleTextureInfo.numberOfVerticalPixelGroup * t_DecylePosition.y);
            int t_PixelInfo = t_PixelInfoX + (t_PixelInfoY * t_CurrentDecyleTextureInfo.numberOfHorizontalPixelGroup);

            PixelGroupInfo t_SelectedPixelGroupInfo = t_CurrentDecyleTextureInfo.pixelGroupInfo[t_PixelInfo];

            int t_NumberOfPixelToBeModified = t_SelectedPixelGroupInfo.numberOfPixelsInThisGroup;

            Color t_ColorStartPointOfDecyle = new Color(
                    t_ColorOfDecyle.r + ((1f - t_ColorOfDecyle.r) * colorSaturation),
                    t_ColorOfDecyle.g + ((1f - t_ColorOfDecyle.g) * colorSaturation),
                    t_ColorOfDecyle.b + ((1f - t_ColorOfDecyle.b) * colorSaturation),
                    t_ColorOfDecyle.a
                );

            for (int i = 0; i < t_NumberOfPixelToBeModified; i++)
            {

                t_CurrentTexture.SetPixel(
                    t_SelectedPixelGroupInfo.pixelInfo[i].pixelPosition.x,
                    t_SelectedPixelGroupInfo.pixelInfo[i].pixelPosition.y,
                    Color.Lerp(t_ColorOfDecyle, t_ColorStartPointOfDecyle, t_SelectedPixelGroupInfo.pixelInfo[i].colorSaturation));
            }

            t_CurrentTexture.Apply();

            return t_CurrentTexture;
        }

        #endregion

        #region Public Callback

#if UNITY_EDITOR

        public void ProcessPreloadedTextureForDecyle()
        {

            int t_NumberOfPreloadedTexture = decyleTextures.Length;
            for (int i = 0; i < t_NumberOfPreloadedTexture; i++)
            {

                ProcessTextureForDecyle(decyleTextures[i], true);
            }
        }

#endif

        public void ProcessTextureForDecyle(Texture2D t_Texture2D, bool t_StoreToDatabase = true, float t_PixelGroupArea = -1, float t_PixelSpreadArea = -1, float t_PixelDensityOnSpreadingArea = 1)
        {

            ProcessTextureForDecyle(t_Texture2D, defaultDecyleShape, t_StoreToDatabase, t_PixelGroupArea, t_PixelSpreadArea, t_PixelDensityOnSpreadingArea);
        }

        public void ProcessTextureForDecyle(Texture2D t_Texture2D, DecyleShape t_DecyleShape = DecyleShape.square, bool t_StoreToDatabase = true, float t_PixelGroupArea = -1, float t_PixelSpreadArea = -1, float t_PixelDensityOnSpreadingArea = 1)
        {

            m_DecyleShape = t_DecyleShape;

            if (t_PixelGroupArea == -1)
                m_PixelGroupArea = defaultPixelGroupArea;
            else
                m_PixelGroupArea = t_PixelGroupArea;

            if (t_PixelSpreadArea == -1)
                m_PixelSpreadArea = defaultPixelSpreadArea;
            else
                m_PixelSpreadArea = t_PixelSpreadArea;

            if (t_PixelDensityOnSpreadingArea == -1)
                m_PixelDensityOnSpreadingArea = defaultPixelDensityOnSpreadingArea;
            else
                m_PixelDensityOnSpreadingArea = t_PixelDensityOnSpreadingArea;


            DecyleProcessor(t_Texture2D, t_StoreToDatabase);
        }

        public Texture2D GetDecyleTexture(int t_TextureIndex, Texture2D t_CurrentTexture, Vector2 t_DecylePosition, Color t_ColorOfDecyle)
        {
            return DecyleTexture(t_TextureIndex, t_CurrentTexture, t_DecylePosition, t_ColorOfDecyle);
        }

        #endregion
    }
}


