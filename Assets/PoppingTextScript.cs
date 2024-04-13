using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PoppingTextScript : MonoBehaviour
{
    TextMeshPro textBox;
    bool play;
    Vector3 basePosition;

    // Start is called before the first frame update
    void Start()
    {
        textBox = GetComponent<TextMeshPro>();
        textBox.alpha = 0.0f;
        basePosition = transform.position;
    }

    public void Play(string text, Vector3 initialPosition)
    {
        play = true;
        textBox.text = text;
        textBox.alpha = 1.0f;

        this.transform.position = basePosition; //this.transform.position = new Vector3(this.transform.position.x, initialPosition.y, this.transform.position.z);
        this.transform.Translate(new Vector3(0, initialPosition.y, 0),Space.Self);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(play)
        {
            this.transform.Translate(new Vector3(0, (float)0.05, 0), Space.Self);
            textBox.alpha -= 0.02f;
        }
    }
}
