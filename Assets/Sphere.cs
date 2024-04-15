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
    public SoundEffects soundEffects;
    public Vector3 initialPosition;
    bool absorbed;
    public bool canBeAbsorbed;

    // Start is called before the first frame update
    void Start()
    {
        canBeAbsorbed = true;
        initialScale = transform.localScale;
        initialPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void SetPicked()
    {
        isPicked = true;
        this.transform.position = new Vector3(this.transform.position.x, 1, this.transform.position.z);

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


    public void ConsumeSphere(Vector3 holePosition, bool absorbed = true)
    {
        if (!isPicked)
        {
            this.absorbed = absorbed;

            if (absorbed)
            {
                soundEffects.PlayAbsorbing();
            }
            else
            {
                soundEffects.PlayDownTheVortex();
            }
            wasConsumed = true;
            transform.parent.gameObject.transform.parent.GetComponent<Funnel>().PauseRotation();
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().drag = 10;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            transform.position = holePosition;
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Sphere>() != null)
        {
            soundEffects.PlaySphere();
        }
    }

    private void FixedUpdate()
    {
        if(wasConsumed)
        {
            if (transform.localScale.x > Vector3.zero.x)
            {
                this.transform.localScale = this.transform.localScale - initialScale * 0.05f;
            }
            else
            {
                GameObject.FindGameObjectWithTag("SceneLogic").GetComponent<SceneLogic3D>().RemoveSphere(this, absorbed);
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
