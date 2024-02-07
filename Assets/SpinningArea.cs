using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningArea : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<Sphere>() != null && !other.gameObject.GetComponent<Sphere>().isPicked)
        {
            other.gameObject.GetComponent<Rigidbody>().drag = 1;
            other.gameObject.transform.parent.GetComponent<Funnel>().PauseRotation();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Sphere>() != null && other.gameObject.GetComponent<Sphere>().isPicked)
        {
            other.gameObject.GetComponent<Rigidbody>().drag = 1;
            other.gameObject.transform.parent.GetComponent<Funnel>().PauseRotation();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<Sphere>() != null && !other.gameObject.GetComponent<Sphere>().isPicked)
        {
            other.gameObject.GetComponent<Rigidbody>().drag = 10;
            other.gameObject.transform.parent.GetComponent<Funnel>().ResumeRotation();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
      
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }
}
