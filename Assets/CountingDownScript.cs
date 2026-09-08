using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class CountingDownScript : MonoBehaviour
{
    TextMeshPro Text;
    Color32[] letterColors = new Color32[4];
    bool play;
    bool disappear;
    int currentAlpha = 255; 

    // Start is called before the first frame update
    void Start()
    {
        Text = GetComponent<TextMeshPro>();
  
        letterColors[0] = Constants.ParticleGradients[NutritionElementsEnum.Fat].colorMin;
        letterColors[1] = Constants.ParticleGradients[NutritionElementsEnum.Saturates].colorMin;
        letterColors[2] = Constants.ParticleGradients[NutritionElementsEnum.Salt].colorMin;
        letterColors[3] = Constants.ParticleGradients[NutritionElementsEnum.Sugar].colorMin;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (play)
        {
            if (Text.fontSize < 15)
            {
                Text.fontSize++;
            }
            else
            {
                play = false;
                disappear = true;
            }
        }

        if (disappear)
        {
            if (currentAlpha > 0)
            {
                Text.color = new Color(Text.color.r, Text.color.g, Text.color.b, (byte)currentAlpha);
                currentAlpha -= 20;

                if (currentAlpha < 0)
                {
                    currentAlpha = 0;
                }
            }
            else
            {
                Text.fontSize = 0;
            }
        }

    }


    public void Play(Color32 color, string text)
    {
        Text.text = text;
        Text.color = color;
        play = true;
        disappear = false;
        currentAlpha = 255;

    }

    /*public void Hide()
    {
        if (Text.alpha == 1)
        {
            disappear = true;
        }
    }*/

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
