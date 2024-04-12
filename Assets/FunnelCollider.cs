using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunnelCollider : MonoBehaviour
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
        if (other.gameObject.GetComponent<Sphere>() != null! && !other.gameObject.GetComponent<Sphere>().isPicked && other.gameObject.GetComponent<Sphere>().canBeAbsorbed)
        {
            other.gameObject.GetComponent<Sphere>().ResumeRotation();
            other.gameObject.GetComponent<Rigidbody>().drag =8;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Sphere>() != null! && !other.gameObject.GetComponent<Sphere>().isPicked && other.gameObject.GetComponent<Sphere>().canBeAbsorbed)
        {
            other.gameObject.GetComponent<Sphere>().ResumeRotation();
            other.gameObject.GetComponent<Rigidbody>().drag = 8;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<Sphere>() != null)
        {
            other.gameObject.GetComponent<Sphere>().PauseRotation();
            other.gameObject.GetComponent<Rigidbody>().drag = 1;
        }
    }
}
