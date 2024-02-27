using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor.Timeline;
using UnityEngine;

public class FillScript : MonoBehaviour
{
    public float MaxAmount = 100;
    public float currentAmount = 0;
    bool animate;
    float newRatio;
    float currentRatio = 0;
    public bool simulate;
    public Vector3 initialPosition;
    public Vector3 initialScale;
    public GameObject hole;

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;
        initialScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (!simulate)
        {
            if (animate && currentRatio < newRatio)
            {
                currentRatio = currentRatio + 0.01f;
                var beforeScaling = GetComponent<Renderer>().bounds.size.y;
                this.transform.localScale = new Vector3(this.transform.localScale.x, currentRatio, this.transform.localScale.z);
                var afterScaling = GetComponent<Renderer>().bounds.size.y;
                this.transform.Translate(new Vector3(0, (float)Math.Round(afterScaling - beforeScaling, 3), 0));
            }
        }
    } 

    public bool AddAmount(float amount)
    {
        if (currentAmount >= MaxAmount)
        {
            hole.GetComponent<HoleCollider>().Close("Full");
            return false;
        }

        if (currentAmount + amount <= MaxAmount)
        {
            currentAmount += amount;
        }
        else
        {
            hole.GetComponent<HoleCollider>().Close("Over");
            return false;
        }

        if (currentAmount / MaxAmount < 0.95)
        {
            newRatio = currentAmount / MaxAmount;
        }
        else
        {
            newRatio = 0.95f;
        }

        animate = true;
        return true;
    }

    public bool Simulate(float amount)
    {
        if (currentAmount >= MaxAmount)
        {
            hole.GetComponent<HoleCollider>().Close("Full");
            return false;
        }
          

        if (currentAmount + amount <= MaxAmount)
        {
            currentAmount += amount;
        }
        else
        {
            hole.GetComponent<HoleCollider>().Close("Over");
            return false;
        }
          

        if(currentAmount / MaxAmount < 0.98)
        {
            currentRatio = currentAmount / MaxAmount;
        }
        else
        {
            currentRatio = 0.98f;
        }

        var beforeScaling = GetComponent<Renderer>().bounds.size.y;
        this.transform.localScale = new Vector3(this.transform.localScale.x, currentRatio, this.transform.localScale.z);
        var afterScaling = GetComponent<Renderer>().bounds.size.y;
        this.transform.Translate(new Vector3(0, (float)Math.Round(afterScaling - beforeScaling, 3), 0));

        return true;
    }

    public void Reset()
    {
        currentRatio = 0;
        newRatio = 0;
        hole.GetComponent<HoleCollider>().Open();
            currentAmount = 0;
            this.transform.localScale = initialScale;
            this.transform.position = initialPosition;
    }
}
