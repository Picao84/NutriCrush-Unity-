using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FridgeScript : MonoBehaviour
{
    SpriteRenderer fridgeDoor;
    SpriteRenderer fridgeSlot;
    FoodBubble foodBubble;
    bool hasFood;

    // Start is called before the first frame update
    void Start()
    {
        fridgeDoor = GetComponent<SpriteRenderer>();
        fridgeSlot = transform.GetChild(0).GetComponent<SpriteRenderer>();

        fridgeSlot.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<FoodBubble>() != null && !hasFood)
        {
            var image = Resources.Load<Texture2D>("fridgeOpen");
            fridgeDoor.sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), new Vector2(0.5f, 0.5f));
            fridgeSlot.enabled = true;

            other.GetComponent<FoodBubble>().InFridge = true;
            other.GetComponent<FoodBubble>().fridgePosition = fridgeSlot.transform.position;
            hasFood = true;
            foodBubble = other.gameObject.GetComponent<FoodBubble>();
        }   
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<FoodBubble>() != null && other.GetComponent<FoodBubble>() == foodBubble)
        {
            var image = Resources.Load<Texture2D>("fridgeClosed");
            fridgeDoor.sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), new Vector2(0.5f, 0.5f));
            fridgeSlot.enabled = false;

            other.GetComponent<FoodBubble>().InFridge = false;     
            hasFood = false;
            foodBubble = null;  
            
        }
    }
}
