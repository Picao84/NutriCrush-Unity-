using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateSlotScript : MonoBehaviour
{
    Vector3 originalScale;
    bool grow;
    float maxScale = 1.6f;
    float currentScale = 1f;
    bool animate;


    // Start is called before the first frame update
    void Start()
    {
        originalScale = transform.localScale;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Reset()
    {
        transform.localScale = originalScale;
        currentScale = 1f;
        animate = false;
        grow = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<FoodBubble>() != null)
        {
            animate = true;
            grow = true;
            other.GetComponent<FoodBubble>().OnPlate = true;
            //other.transform.position = new Vector3(this.transform.position.x, other.transform.position.y, this.transform.position.z);
        }
    }

    private void FixedUpdate()
    {
        if (animate)
        {
            if (grow && currentScale < maxScale)
            {
                currentScale += 0.1f;
                this.transform.localScale = new Vector3(originalScale.x * currentScale, originalScale.y * currentScale, originalScale.y * currentScale);
            }

            if (!grow && currentScale > 1)
            {
                currentScale -= 0.1f;
                this.transform.localScale = new Vector3(originalScale.x * currentScale, originalScale.y * currentScale, originalScale.y * currentScale);

                if(currentScale == 1)
                {
                    animate = false;
                }
            }

        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<FoodBubble>() != null)
        {
            grow = false;
            other.GetComponent<FoodBubble>().OnPlate = false;
        }
    }

}
