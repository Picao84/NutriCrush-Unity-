using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SickFill : MonoBehaviour
{
    public float MaxAmount = 100;
    public float currentAmount = 100;
    bool animate;
    float newRatio = 1;
    float currentRatio = 1;
    public bool simulate;
    public Vector3 initialPosition;
    public Vector3 initialScale;
    public bool gameOver;
    SpriteRenderer smilie;
    TextMeshPro sickLevel;

    public event EventHandler SickBarFilled;

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;
        initialScale = transform.localScale;
        smilie = transform.parent.Find("SickIcon").GetComponent<SpriteRenderer>();
        sickLevel = transform.parent.GetComponentInChildren<TextMeshPro>();
        sickLevel.text = $"{currentRatio * 100}%";
    }

    // Update is called once per frame
    void Update()
    {
        if (!simulate)
        {
            if (currentRatio > newRatio)
            {
                currentRatio = currentRatio - 0.01f;
                var beforeScaling = GetComponent<Renderer>().bounds.min.x;
                this.transform.localScale = new Vector3(currentRatio, this.transform.localScale.y, this.transform.localScale.z);
                var afterScaling = GetComponent<Renderer>().bounds.min.x;
                this.transform.Translate(new Vector3((float)Math.Round(afterScaling - beforeScaling, 3),0,0));

                if (currentRatio > 0)
                {
                    sickLevel.text = $"{Math.Round(currentRatio * 100, 0)}%";
                }
            }
            else
            {
                if(currentRatio <= 0f && !gameOver)
                {
                    sickLevel.text = $"0%";
                    SickBarFilled?.Invoke(this, EventArgs.Empty);
                    gameOver = true;
                }
            }
        }
    }

    public void RemoveAmount(float amount)
    {
        if (currentAmount <= 0)
            return;

        if (currentAmount - amount > 0)
        {
            currentAmount -= amount;
        }
        else
        {
            currentAmount = 0;
        }

        if (currentAmount / MaxAmount > 0)
        {
            newRatio = currentAmount / MaxAmount;
        }
        else
        {
            newRatio = 0f;
        }

        if (newRatio <= 0.2f)
        {
            var health20 = Resources.Load<Texture2D>("health_20");
            smilie.sprite = Sprite.Create(health20, new Rect(0, 0, health20.width, health20.height), new Vector2(0.5f, 0.5f));

            return;
        }

        if (newRatio <= 0.4f)
        {
            var health40 = Resources.Load<Texture2D>("health_40");
            smilie.sprite = Sprite.Create(health40, new Rect(0, 0, health40.width, health40.height), new Vector2(0.5f, 0.5f));
            return;
        }

        if (newRatio <= 0.6f)
        {
            var health60 = Resources.Load<Texture2D>("health_60");
            smilie.sprite = Sprite.Create(health60, new Rect(0, 0, health60.width, health60.height), new Vector2(0.5f, 0.5f));

            return;
        }

        if (newRatio <= 0.8f)
        {
            var health80 = Resources.Load<Texture2D>("health_80");
            smilie.sprite = Sprite.Create(health80, new Rect(0, 0, health80.width, health80.height), new Vector2(0.5f, 0.5f));

            return;
        }
        //newRatio = currentAmount / MaxAmount;
        //animate = true;
    }

    public void Simulate(float amount)
    {
        if (currentAmount <= 0)
            return;

        if (currentAmount - amount > 0)
        {
            currentAmount -= amount;
        }
        else
        {
            currentAmount = 0;
        }

        if (currentAmount / MaxAmount > 0)
        {
            currentRatio = currentAmount / MaxAmount;
        }
        else
        {
            currentRatio = 0f;
        }

        var beforeScaling = GetComponent<Renderer>().bounds.min.x;
        this.transform.localScale = new Vector3(currentRatio, this.transform.localScale.y, this.transform.localScale.z);
        var afterScaling = GetComponent<Renderer>().bounds.min.x;
        this.transform.Translate(new Vector3((float)Math.Round(afterScaling - beforeScaling, 3), 0, 0));
    }

    public void Reset(bool firstReset = false)
    {
        gameOver = false;
        currentRatio = 1;
        newRatio = 1;
        currentAmount = MaxAmount;

        if (!simulate)
        {
            var health100 = Resources.Load<Texture2D>("health_100");
            smilie.sprite = Sprite.Create(health100, new Rect(0, 0, health100.width, health100.height), new Vector2(0.5f, 0.5f));
        }

        if (!firstReset)
        {
            this.transform.localScale = initialScale;
            this.transform.position = initialPosition;
        }

        if (!simulate)
        {
            sickLevel.text = $"{currentRatio*100}%";
        }
    }
}
