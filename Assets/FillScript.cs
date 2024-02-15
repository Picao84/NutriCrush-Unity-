using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Timeline;
using UnityEngine;

public class FillScript : MonoBehaviour
{
    GameObject parent;
    public int MaxAmount = 100;
    int currentAmount = 0;
    bool animate;
    float newRatio;
    float currentRatio = 0;


    // Start is called before the first frame update
    void Start()
    {
        parent = transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (animate && currentRatio < newRatio)
        {
            currentRatio = currentRatio + 0.01f;
            var beforeScaling = GetComponent<Renderer>().bounds.size.y;
            this.transform.localScale = new Vector3(this.transform.localScale.x, currentRatio, this.transform.localScale.z);
            var afterScaling = GetComponent<Renderer>().bounds.size.y;
            this.transform.Translate(new Vector3(0, (float)Math.Round(afterScaling - beforeScaling,3), 0));
        }
    } 

    public void AddAmount(int amount)
    {
        if (currentRatio >= parent.GetComponent<Renderer>().bounds.size.y)
            return;

        currentAmount += amount;
        newRatio = currentAmount * parent.GetComponent<Renderer>().bounds.size.y / MaxAmount;
        animate = true;
    }
}
