using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sphere : MonoBehaviour
{
    Vector3 InitialPosition;
    public bool isPicked = false;
    Rigidbody rigidbody;
    Vector3 originalScreenTargetPosition;

    // Start is called before the first frame update
    void Start()
    {
        InitialPosition = transform.position;
        rigidbody = GetComponent<Rigidbody>();
        //Cursor.lockState = CursorLockMode.Confined;
    }

    // Update is called once per frame
    void Update()
    {
       
    }


    private void FixedUpdate()
    {
        if (isPicked)
        {   
            Cursor.visible = false;
            var screenToWorldPoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z));
            Vector3 mousePositionOffset = new Vector3(screenToWorldPoint.x, screenToWorldPoint.y, screenToWorldPoint.z) - originalScreenTargetPosition;
            rigidbody.velocity = new Vector3(mousePositionOffset.x / Time.deltaTime, mousePositionOffset.z / Time.deltaTime, mousePositionOffset.y / Time.deltaTime);
            originalScreenTargetPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z));  
        } 
    }

    private void OnMouseDown()
    {
        if (!isPicked)
        {
            //transform.position = new Vector3(transform.position.x, InitialPosition.y, transform.position.z);
            Cursor.visible = false;
            transform.parent.GetComponent<Funnel>().PauseRotation();
            GetComponent<Rigidbody>().useGravity = false;
            isPicked = true;
            originalScreenTargetPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z));
        }
    
    }

    private void OnMouseUp()
    {
        Cursor.visible = true;
        transform.parent.GetComponent<Funnel>().ResumeRotation();
        GetComponent<Rigidbody>().useGravity = true;
        isPicked = false;
    }
}
