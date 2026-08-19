using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EffectTextScript : MonoBehaviour
{
    // Start is called before the first frame update
    TextMeshPro Text;
    Color32[] letterColors = new Color32[4];
    bool play;
    bool disappear;
    int currentAlpha = 255;

    void Start()
    {
        Text = GetComponent<TextMeshPro>();
        letterColors[0] = Constants.ParticleGradients[NutritionElementsEnum.Fat].colorMin;
        letterColors[1] = Constants.ParticleGradients[NutritionElementsEnum.Saturates].colorMin;
        letterColors[2] = Constants.ParticleGradients[NutritionElementsEnum.Salt].colorMin;
        letterColors[3] = Constants.ParticleGradients[NutritionElementsEnum.Sugar].colorMin;
    }

    public void SetText(string text)
    {
        Text.text = text;
        play = true;
        disappear = false;
        currentAlpha = 255;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {

        if (play)
        {
            if (Text.fontSize < 8)
            {
                Text.fontSize++;
            }
            else
            {
                play = false;
                disappear = true;
            }
        }

        if (!disappear)
        {
            Text.ForceMeshUpdate();

            TMP_TextInfo textInfo = Text.textInfo;

            int colorIndex = 0;

            int charCount = textInfo.characterCount;
            for (int i = 0; i < charCount; ++i)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

                int index = charInfo.vertexIndex;

                for (int j = 0; j < 4; ++j)
                {
                    Text.textInfo.meshInfo[charInfo.materialReferenceIndex].colors32[index + j] = letterColors[colorIndex];
                }

                if (colorIndex < letterColors.Length - 1)
                {
                    colorIndex++;
                }
                else
                {
                    colorIndex = 0;
                }
            }

            textInfo.meshInfo[0].mesh.vertices = textInfo.meshInfo[0].vertices;
            Text.UpdateVertexData();
        }
        else
        {
            if (currentAlpha > 0)
            {
                Text.ForceMeshUpdate();

                TMP_TextInfo textInfo = Text.textInfo;

                int colorIndex = 0;

                int charCount = textInfo.characterCount;
                for (int i = 0; i < charCount; ++i)
                {
                    TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

                    int index = charInfo.vertexIndex;
                    var color = letterColors[colorIndex];
                    Color32 colorToApply = new Color32(color.r, color.g, color.b, (byte)currentAlpha);

                    for (int j = 0; j < 4; ++j)
                    {
                        Text.textInfo.meshInfo[charInfo.materialReferenceIndex].colors32[index + j] = colorToApply;
                    }

                    if (colorIndex < letterColors.Length - 1)
                    {
                        colorIndex++;
                    }
                    else
                    {
                        colorIndex = 0;
                    }
                }

                currentAlpha -= 6;
            

                if (currentAlpha < 0)
                {
                    currentAlpha = 0;
                }

                textInfo.meshInfo[0].mesh.vertices = textInfo.meshInfo[0].vertices;
                Text.UpdateVertexData();
            }
            else
            {
                Text.fontSize = 0;
             
            }
        }

    }
}
