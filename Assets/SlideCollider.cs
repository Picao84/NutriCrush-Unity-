using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideCollider : MonoBehaviour
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
        if(other.gameObject.GetComponent<Sphere>() != null)
        {
            other.gameObject.GetComponent<Sphere>().PauseRotation();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<Sphere>() != null)
        {
            other.gameObject.GetComponent<Sphere>().ResumeRotation();
        }
    }
}
