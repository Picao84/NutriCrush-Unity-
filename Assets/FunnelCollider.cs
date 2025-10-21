using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunnelCollider : MonoBehaviour
{
    public GameObject SceneLogic3D;
    SceneLogic3D SceneLogic;

    // Start is called before the first frame update
    void Start()
    {
        SceneLogic = SceneLogic3D.GetComponent<SceneLogic3D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (SceneLogic != null && !SceneLogic.pausedBalls)
        {
            if (other.gameObject.GetComponent<Sphere>() != null! && !other.gameObject.GetComponent<Sphere>().isPicked && other.gameObject.GetComponent<Sphere>().canBeAbsorbed)
            {
                other.gameObject.GetComponent<Sphere>().ResumeRotation();
                other.gameObject.GetComponent<Rigidbody>().drag = 5.5f;
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (SceneLogic != null && !SceneLogic.pausedBalls)
        {
            if (other.gameObject.GetComponent<Sphere>() != null! && !other.gameObject.GetComponent<Sphere>().isPicked && other.gameObject.GetComponent<Sphere>().canBeAbsorbed)
            {
                other.gameObject.GetComponent<Sphere>().ResumeRotation();
                other.gameObject.GetComponent<Rigidbody>().drag = 5.5f;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (SceneLogic != null && !SceneLogic.pausedBalls)
        {
            if (other.gameObject.GetComponent<Sphere>() != null)
            {
                other.gameObject.GetComponent<Sphere>().numberOfTimesItExitedFunnel++;
                other.gameObject.GetComponent<Sphere>().PauseRotation();
                other.gameObject.GetComponent<Rigidbody>().drag = 1;
            }
        }
    }
}
