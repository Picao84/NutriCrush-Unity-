using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    private float _bubbleThrust;
    private float _bubbleThrustMin = 10f;
    private float _bubbleThrustMax = 20f;
    bool left = false;
    public event EventHandler BubbleDestroyedEvent;
    public bool Picked { get; set; } = false;
    public NutritionTypeEnum NutritionType { get; set; }

    Rigidbody2D rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
       
      
    }

    private void FixedUpdate()
    {
        _bubbleThrust = UnityEngine.Random.Range(_bubbleThrustMin, _bubbleThrustMax);

        if (left)
        {
            left = false;
            rigidBody.AddForce(transform.right * -_bubbleThrust, Picked ? ForceMode2D.Force : ForceMode2D.Impulse);
        }
        else
        {
            left = true;
            rigidBody.AddForce(transform.right * _bubbleThrust, Picked ? ForceMode2D.Force : ForceMode2D.Impulse);
        }
    }

    private void OnDestroy()
    {
        BubbleDestroyedEvent?.Invoke(this, EventArgs.Empty);
    }
}
