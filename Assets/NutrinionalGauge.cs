using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NutrinionalGauge : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var worldSize = Camera.main.ScreenToWorldPoint(Camera.main.transform.right);
        //this.transform.position = new Vector3(Camera.main.ScreenToWorldPoint(Camera.main.pixelRect.position).x, Camera.main.pixelRect.yMin, 1);
        this.transform.localScale = new Vector3(Math.Abs(worldSize.x * 2), 1.5f, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
