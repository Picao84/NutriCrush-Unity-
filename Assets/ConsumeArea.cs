using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumeArea : MonoBehaviour
{
    public NutritionTypeEnum NutritionType;


    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpriteRenderer>().material.color = Constants.NutritionTypeColors[NutritionType];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
       Destroy(collision.transform.parent.gameObject);
       GetComponent<PolygonCollider2D>().isTrigger = false;
    }
}
