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
        if (SceneLogic != null && !SceneLogic.pausedBalls && other.gameObject.GetComponent<Sphere>() != null)
        {
            var sphere = other.gameObject.GetComponent<Sphere>();

            if (!other.gameObject.GetComponent<Sphere>().cannotBeAbsorbed)
            {
                if (!sphere.isPicked)
                {
                    sphere.gameObject.GetComponent<Sphere>().ResumeRotation();
                    sphere.gameObject.GetComponent<Rigidbody>().drag = SceneLogic.gamePlayState == Assets.GameplayState.Single ? 5.5f : 12f;
                }
            }
            else
            {
                if (!sphere.GetComponent<Sphere>().isPicked)
                {
                    sphere.gameObject.GetComponent<Sphere>().ResumeRotation();
                    sphere.gameObject.GetComponent<Rigidbody>().drag = 3;
                }
            }
        }

    }


    private void OnTriggerEnter(Collider other)
    {
        if (SceneLogic != null && !SceneLogic.pausedBalls && other.gameObject.GetComponent<Sphere>() != null)
        {
            var sphere = other.gameObject.GetComponent<Sphere>();

            if (other.gameObject.GetComponent<Sphere>().cannotBeAbsorbed)
            {
                if (!sphere.isPicked)
                {
                    sphere.gameObject.GetComponent<Sphere>().ResumeRotation();
                    sphere.gameObject.GetComponent<Rigidbody>().drag = SceneLogic.gamePlayState == Assets.GameplayState.Single ? 5.5f : 12f;
                }
            }
            else
            {
                if (!sphere.GetComponent<Sphere>().isPicked)
                {
                    sphere.gameObject.GetComponent<Sphere>().ResumeRotation();
                    sphere.gameObject.GetComponent<Rigidbody>().drag = 3;
                }
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
