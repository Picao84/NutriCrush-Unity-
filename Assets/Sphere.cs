using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sphere : MonoBehaviour
{
    public bool isPicked = false;
    Vector3 originalScreenTargetPosition;
    public bool wasConsumed;
    Vector3 initialScale;
    public NutritionElementsEnum element;
    public float elementQuantity = 10;

    // Start is called before the first frame update
    void Start()
    {
        initialScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void SetColor(NutritionElementsEnum color)
    {
        this.element = color;
    }

    public void SetQuantity(float quantity)
    {
        elementQuantity = quantity;
    }

    public void PauseRotation()
    {
        transform.parent.gameObject.transform.parent.GetComponent<Funnel>().PauseRotation();
    }
    public void ResumeRotation()
    {
        transform.parent.gameObject.transform.parent.GetComponent<Funnel>().ResumeRotation();
    }


    public void ConsumeSphere(Vector3 holePosition)
    {
        if (!isPicked)
        {
            wasConsumed = true;
            transform.parent.gameObject.transform.parent.GetComponent<Funnel>().PauseRotation();
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().drag = 10;
            transform.position = holePosition;
        }
    }


    private void FixedUpdate()
    {
        //if (isPicked)
        //{   
        //    Cursor.visible = false;
        //    var screenToWorldPoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z));
        //    Vector3 mousePositionOffset = new Vector3(screenToWorldPoint.x, screenToWorldPoint.y, screenToWorldPoint.z) - originalScreenTargetPosition;
        //    GetComponent<Rigidbody>().velocity = new Vector3(mousePositionOffset.x / Time.deltaTime, mousePositionOffset.z / Time.deltaTime, mousePositionOffset.y / Time.deltaTime);
        //    originalScreenTargetPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z));  
        //}

        if(wasConsumed)
        {
            if (transform.localScale.x > Vector3.zero.x)
            {
                this.transform.localScale = this.transform.localScale - initialScale * 0.1f;
            }
            else
            {
                GameObject.FindGameObjectWithTag("SceneLogic").GetComponent<SceneLogic3D>().RemoveSphere(this);
                Destroy(this.transform.root.gameObject);
            }
        }
    }

    private void OnMouseDown()
    {
        //if (!isPicked && !wasConsumed)
        //{
        //    //transform.position = new Vector3(transform.position.x, InitialPosition.y, transform.position.z);
        //    Cursor.visible = false;
        //    transform.parent.GetComponent<Funnel>().PauseRotation();
        //    GetComponent<Rigidbody>().useGravity = false;
        //    isPicked = true;
        //    originalScreenTargetPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z));
        //}
    
    }

    private void OnMouseUp()
    {
        //Cursor.visible = true;
        //transform.parent.GetComponent<Funnel>().ResumeRotation();
        //GetComponent<Rigidbody>().useGravity = true;
        //isPicked = false;
    }
}
