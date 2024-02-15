using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleCollider : MonoBehaviour
{
    public ColorEnum color;
    public GameObject SceneLogic3D;
    GameObject GaugeFill;

    // Start is called before the first frame update
    void Start()
    {
        GaugeFill = GameObject.FindWithTag(color.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject != null && other.gameObject.GetComponent<Sphere>() != null && !other.gameObject.GetComponent<Sphere>().wasConsumed && !other.gameObject.GetComponent<Sphere>().isPicked)
        {
            GaugeFill.GetComponent<FillScript>().AddAmount(10);
            SceneLogic3D.GetComponent<SceneLogic3D>().RemoveSphere(other.gameObject.GetComponent<Sphere>());
            other.gameObject.GetComponent<Sphere>().ConsumeSphere(this.transform.position);
        }
    }
}
