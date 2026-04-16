using Assets;
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
    FoodBubble foodBubble;


    // Start is called before the first frame update
    void Start()
    {
        originalScale = transform.localScale;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (animate)
        {
            if (grow && currentScale < maxScale)
            {
                currentScale += 0.1f;
                this.transform.localScale = new Vector3(originalScale.x * currentScale, originalScale.y * currentScale, originalScale.y * currentScale);

                if(currentScale == maxScale)
                {
                    animate = false;
                }
            }

            if (!grow && currentScale > 1)
            {
                currentScale -= 0.1f;
                this.transform.localScale = new Vector3(originalScale.x * currentScale, originalScale.y * currentScale, originalScale.y * currentScale);

                if (currentScale == 1)
                {
                    animate = false;
                }
            }

        }
    }

    public void Reset()
    {
        transform.localScale = originalScale;
        currentScale = 1f;
        animate = false;
        grow = false;
        foodBubble = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<FoodBubble>() != null && foodBubble == null && !other.GetComponent<FoodBubble>().OnPlate)
        {
            animate = true;
            foodBubble = other.GetComponent<FoodBubble>();
            grow = true;
            other.GetComponent<FoodBubble>().OnPlate = true;
            other.GetComponent<FoodBubble>().platePosition = this.transform.position;
            //other.transform.position = new Vector3(this.transform.position.x, other.transform.position.y, this.transform.position.z);
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<FoodBubble>() != null && other.GetComponent<FoodBubble>() == foodBubble)
        {
            foodBubble = null;
            grow = false;
            other.GetComponent<FoodBubble>().OnPlate = false;
            other.GetComponent<FoodBubble>().platePosition = Vector3.zero;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<FoodBubble>() != null && foodBubble == null && !other.GetComponent<FoodBubble>().OnPlate && !other.GetComponent<FoodBubble>().chosen)
        {
            animate = true;
            foodBubble = other.GetComponent<FoodBubble>();
            grow = true;
            other.GetComponent<FoodBubble>().OnPlate = true;
            other.GetComponent<FoodBubble>().platePosition = this.transform.position;
            //other.transform.position = new Vector3(this.transform.position.x, other.transform.position.y, this.transform.position.z);
        }
    }

}
