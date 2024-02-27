using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SickFill : MonoBehaviour
{
    public float MaxAmount = 100;
    public float currentAmount = 0;
    bool animate;
    float newRatio;
    float currentRatio = 0;
    public bool simulate;
    public Vector3 initialPosition;
    public Vector3 initialScale;

    public event EventHandler SickBarFilled;

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
            if (currentRatio < newRatio)
            {
                currentRatio = currentRatio + 0.01f;
                var beforeScaling = GetComponent<Renderer>().bounds.min.x;
                this.transform.localScale = new Vector3(currentRatio, this.transform.localScale.y, this.transform.localScale.z);
                var afterScaling = GetComponent<Renderer>().bounds.min.x;
                this.transform.Translate(new Vector3((float)Math.Round(afterScaling - beforeScaling, 3),0,0));
            }
            else
            {
                if(currentRatio >= 0.98f)
                {
                    SickBarFilled?.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }

    public void AddAmount(float amount)
    {
        if (currentAmount >= MaxAmount)
            return;

        if (currentAmount + amount < MaxAmount)
        {
            currentAmount += amount;
        }
        else
        {
            currentAmount = MaxAmount;
        }

        if (currentAmount / MaxAmount < 1)
        {
            newRatio = currentAmount / MaxAmount;
        }
        else
        {
            newRatio = 0.95f;
        }

        newRatio = currentAmount / MaxAmount;
        animate = true;
    }

    public void Simulate(float amount)
    {
        if (currentAmount >= MaxAmount)
            return;

        if (currentAmount + amount < MaxAmount)
        {
            currentAmount += amount;
        }
        else
        {
            currentAmount = MaxAmount;         
        }

        if (currentAmount / MaxAmount < 1)
        {
            currentRatio = currentAmount / MaxAmount;
        }
        else
        {
            currentRatio = 0.98f;
        }

        var beforeScaling = GetComponent<Renderer>().bounds.min.x;
        this.transform.localScale = new Vector3(currentRatio, this.transform.localScale.y, this.transform.localScale.z);
        var afterScaling = GetComponent<Renderer>().bounds.min.x;
        this.transform.Translate(new Vector3((float)Math.Round(afterScaling - beforeScaling, 3), 0, 0));
    }

    public void Reset()
    {
        currentRatio = 0;
        newRatio = 0;
        currentAmount = 0;
        this.transform.localScale = initialScale;
        this.transform.position = initialPosition;
        
    }
}
