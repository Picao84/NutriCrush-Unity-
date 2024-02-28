using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Unity.VisualScripting;
using UnityEngine;
using Timer = System.Timers.Timer;

public enum NutritionTypeEnum
{
    Fat,
    Salt,
    Sugar
}

public static class Constants
{
    public static readonly Dictionary<NutritionTypeEnum, Color> NutritionTypeColors = new Dictionary<NutritionTypeEnum, Color> 
    {
        { NutritionTypeEnum.Fat, Color.blue },
        { NutritionTypeEnum.Salt, Color.red },
        { NutritionTypeEnum.Sugar, Color.green },
    };

}

public class NutritionElementEnum : MonoBehaviour
{
    Rigidbody2D selectedRigidBody;
    public GameObject Circle;
    Timer Timer;
    bool createNewCircle = true;
    Vector2 originalScreenTargetPosition;
    bool unselectedSlowedDown;
    GameObject[] Gauges;

    // Start is called before the first frame update
    void Start()
    {
        Timer = new Timer(5000);
        Timer.Elapsed += Timer_Elapsed;
        Timer.Start();
        Gauges = GameObject.FindGameObjectsWithTag("Chart");
    }

    private void Timer_Elapsed(object sender, ElapsedEventArgs e)
    {
        createNewCircle = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Camera.main != null)
        {
            if (createNewCircle)
            {
                var worldPoint = Camera.main.ScreenToWorldPoint(new Vector2(Camera.main.pixelRect.width / 2, Camera.main.pixelRect.yMax * 0.2f));
                var newCircle = Instantiate(Circle, new Vector2(worldPoint.x, Math.Abs(worldPoint.y)), Quaternion.identity);

                var type = (NutritionTypeEnum)UnityEngine.Random.Range(0, 3);
                newCircle.transform.GetChild(0).GetComponent<SpriteRenderer>().material.color = Constants.NutritionTypeColors[type];
                var bubble = newCircle.transform.GetChild(0).AddComponent<Bubble>();
                bubble.NutritionType = type;
                bubble.BubbleDestroyedEvent += Bubble_BubbleDestroyedEvent;
               
                createNewCircle = false;
            }
            
            if(Input.GetMouseButtonDown(0))
            {
                selectedRigidBody = GetRigidbodyFromMouseClick();

                if (selectedRigidBody != null)
                {
                    var objects = GameObject.FindGameObjectsWithTag("Circle");
                    var pickedBubble = GameObject.FindGameObjectsWithTag("Circle").First(x => x.GetComponent<Rigidbody2D>() == selectedRigidBody).GetComponent<Bubble>();
                    pickedBubble.Picked = true;
                    GameObject.FindGameObjectsWithTag("Triangle").First(x => x.GetComponent<ConsumeArea>().NutritionType == pickedBubble.NutritionType)
                    .GetComponent<PolygonCollider2D>().isTrigger = true;
                    SlowDownUnselected();
                }
            }

            if(Input.GetMouseButtonUp(0) && selectedRigidBody != null)
            {
                var pickedBubble = GameObject.FindGameObjectsWithTag("Circle").First(x => x.GetComponent<Rigidbody2D>() == selectedRigidBody).GetComponent<Bubble>();
                GameObject.FindGameObjectsWithTag("Triangle").First(x => x.GetComponent<ConsumeArea>().NutritionType == pickedBubble.NutritionType)
                   .GetComponent<PolygonCollider2D>().isTrigger = false;
                GameObject.FindGameObjectsWithTag("Circle").First(x => x.GetComponent<Rigidbody2D>() == selectedRigidBody).GetComponent<Bubble>().Picked = false;
    
                selectedRigidBody.isKinematic = false;
                selectedRigidBody = null;
                ResetSpeed();                       
            }
        }
    }

    private void Bubble_BubbleDestroyedEvent(object sender, EventArgs e)
    {
        try
        {
            var bubble = (Bubble)sender;
            var gauge = Gauges.First(x => x.GetComponent<Gauge>().Type == bubble.NutritionType);
            gauge.GetComponent<Gauge>().ConsumeArea(1);

            bubble.BubbleDestroyedEvent -= Bubble_BubbleDestroyedEvent;
            ResetSpeed();
        }
        catch (Exception ex)
        {

        }
    }

    private void SlowDownUnselected()
    {
        if (unselectedSlowedDown)
            return;

            unselectedSlowedDown = true;

            var unselectedCircles = GameObject.FindGameObjectsWithTag("Circle").Where(x => x.GetComponent<Rigidbody2D>() != selectedRigidBody).ToList();

            foreach (var circle in unselectedCircles)
            {
                circle.GetComponent<Bubble>().Picked = true;
                circle.GetComponent<Rigidbody2D>().gravityScale = 0.1f;
            }
    }

    private void ResetSpeed()
    {
        foreach (var circle in GameObject.FindGameObjectsWithTag("Circle"))
        {
            circle.GetComponent<Bubble>().Picked = false;
            circle.GetComponent<Rigidbody2D>().gravityScale = 1f;
        }

        unselectedSlowedDown = false;
    }

    private void FixedUpdate()
    {
        if (selectedRigidBody != null)
        {
            var screenToWorldPoint = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
            Vector2 mousePositionOffset = new Vector2(screenToWorldPoint.x, screenToWorldPoint.y) - originalScreenTargetPosition;
            selectedRigidBody.velocity = new Vector2(mousePositionOffset.x / Time.deltaTime, mousePositionOffset.y / Time.deltaTime);
            originalScreenTargetPosition = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
        }
    }

    Rigidbody2D GetRigidbodyFromMouseClick()
    {
        RaycastHit2D rayHit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition));

        if(rayHit.collider != null)
        {
            if (rayHit.collider.gameObject.GetComponent<Rigidbody2D>())
            {
                originalScreenTargetPosition = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
                return rayHit.collider?.gameObject.GetComponent<Rigidbody2D>();
            }
        }
        return null;
    }
}
