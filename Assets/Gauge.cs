using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gauge : MonoBehaviour
{
    public NutritionTypeEnum Type;

    public int MaxQuantity;
    int currentQuantity;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1, 0, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ConsumeArea(int quantity)
    {
        currentQuantity = currentQuantity + quantity;
        float currentValueInChart = (float)currentQuantity / (float)MaxQuantity;
        GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1, currentValueInChart, 1);
    }
}
