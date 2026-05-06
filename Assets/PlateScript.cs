using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateScript : MonoBehaviour
{
    bool animate;
    bool disappear;
    float minScale = 0f;
    float currentScale = 1f;
    float maxScale = 1.2f;
    bool wentToMax;
    bool wentToMin;
    Vector3 originalScale;
    bool activateCombo;
    bool deactivateCombo;
    float comboScale = 1.6f;
    float comboMaxScale = 1.8f;
    float comboMinScale = 0.8f;

    // Start is called before the first frame update
    void Start()
    {
        originalScale = transform.localScale;
    }

    public void Reset()
    {
        currentScale = 1f;
        wentToMax = false;
        wentToMin = false;
        activateCombo = false;
        deactivateCombo = false;
        animate = false;
        disappear = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (animate)
        {
            if (activateCombo)
            {
                if(currentScale < comboMaxScale && !wentToMax)
                {
                    currentScale += 0.2f;
                    this.transform.localScale = new Vector3(originalScale.x * currentScale, originalScale.y * currentScale, originalScale.y * currentScale);

                    if(currentScale >= comboMaxScale)
                    {
                        wentToMax = true;
                    }
                }
                else
                {
                    if(currentScale > comboScale)
                    {
                        currentScale -= 0.1f;
                        this.transform.localScale = new Vector3(originalScale.x * currentScale, originalScale.y * currentScale, originalScale.y * currentScale);

                        if (Mathf.Approximately(currentScale, comboScale))
                        {
                            activateCombo = false;
                            animate = false;
                            wentToMax = false;
                        }
                    }
                }
            }
            else if(deactivateCombo)
            {
                if (currentScale > comboMinScale && !wentToMin)
                {
                    currentScale -= 0.1f;
                    this.transform.localScale = new Vector3(originalScale.x * currentScale, originalScale.y * currentScale, originalScale.y * currentScale);

                    if (currentScale <= comboMinScale)
                    {
                        wentToMin = true;
                    }
                }
                else
                {
                    if (currentScale < 1)
                    {
                        currentScale += 0.1f;
                        this.transform.localScale = new Vector3(originalScale.x * currentScale, originalScale.y * currentScale, originalScale.y * currentScale);

                        if(Mathf.Approximately(currentScale, 1))
                        {
                            animate = false;
                            deactivateCombo = false;
                            wentToMin = false;
                        }
                    }
                }
                
            }
            else
            {
                if (disappear && currentScale > minScale)
                {
                    currentScale -= 0.1f;
                    this.transform.localScale = new Vector3(originalScale.x * currentScale, originalScale.y * currentScale, originalScale.y * currentScale);

                    if (Mathf.Approximately(currentScale, 0))
                    {
                        animate = false;
                        wentToMax = false;
                    }
                }

                if (!disappear && !wentToMax)
                {
                    currentScale += 0.2f;
                    this.transform.localScale = new Vector3(originalScale.x * currentScale, originalScale.y * currentScale, originalScale.y * currentScale);

                    if (currentScale > maxScale)
                    {
                        wentToMax = true;
                    }
                }

                if (!disappear && currentScale > 1 && wentToMax)
                {
                    currentScale -= 0.1f;
                    this.transform.localScale = new Vector3(originalScale.x * currentScale, originalScale.y * currentScale, originalScale.y * currentScale);

                    if (Mathf.Approximately(currentScale, 1))
                    {
                        animate = false;
                        wentToMax = false;
                    }
                }
            }
        }
    }

    public void Disappear()
    {
        animate = true;
        disappear = true;
    }

    public void Appear()
    {
        animate = true;
        disappear = false;
        wentToMax = false;
    }

    public void ActivateCombo()
    {
        activateCombo = true;
        deactivateCombo = false;
        animate = true;
    }

    public void DeActivateCombo()
    {
        activateCombo = false;
        deactivateCombo = true;
        animate = true;
    }
}
