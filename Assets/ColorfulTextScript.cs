using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ColorfulTextScript : MonoBehaviour
{
    TextMeshProUGUI Text;
    Color32[] letterColors = new Color32[4];

    // Start is called before the first frame update
    void Start()
    {
        Text = GetComponent<TextMeshProUGUI>();
        letterColors[0] = new Color32(217, 28, 28, 255);
        letterColors[1] = new Color32(79, 121, 79, 255);
        letterColors[2] = new Color32(211, 211, 29, 255);
        letterColors[3] = new Color32(218, 43, 177, 255);
    }

    // Update is called once per frame
    void Update()
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
