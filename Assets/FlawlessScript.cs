using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class FlawlessScript : MonoBehaviour
{
    TextMeshPro Text;
    Color32[] letterColors = new Color32[4];
    bool play;
    bool disappear;
    TextMeshPro textDesc;

    // Start is called before the first frame update
    void Start()
    {
        Text = GetComponent<TextMeshPro>();
        textDesc = transform.GetChild(0).GetComponent<TextMeshPro>();
        letterColors[0] = new Color32(217, 28,28, 255);
        letterColors[1] = new Color32(79, 121, 79, 255);
        letterColors[2] = new Color32(211,211,29,255);
        letterColors[3] = new Color32(218,43,177,255);
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (play)
        {
            if (Text.fontSize < 17)
            {
                Text.fontSize++;

                if(textDesc.fontSize < 5)
                {
                    textDesc.fontSize++;
                }
            }
            else
            {
                play = false;
            }
        }

        if (disappear)
        {
            if (Text.alpha > 0)
            {
                Text.alpha = -0.01f;
            }
            else
            {
                Text.fontSize = 0;
                textDesc.fontSize = 0;
                Text.alpha = 1;
                disappear = false;
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

    }


    public void Play()
    {
        play = true;
    }

    public void Hide()
    {
        disappear = true;
    }

    private Color32 hexToColor(string hex)
    {
        hex = hex.Replace("0x", "");//in case the string is formatted 0xFFFFFF
        hex = hex.Replace("#", "");//in case the string is formatted #FFFFFF
        byte a = 255;//assume fully visible unless specified in hex
        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        //Only use alpha if the string has enough characters
        if (hex.Length == 8)
        {
            a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
        }
        return new Color32(r, g, b, a);
    }
}
