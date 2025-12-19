using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class FunnelBottom : MonoBehaviour
{
    ParticleSystem WasteParticleSystem;
    public GameObject SceneLogic3D;
    public GameObject SickBar;

    // Start is called before the first frame update
    void Start()
    {
        WasteParticleSystem = transform.parent.GetChild(2).GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.transform.gameObject.GetComponent<Sphere>() != null)
        {
            if (!other.transform.gameObject.GetComponent<Sphere>().IsGhost)
            {
                SickBar.GetComponent<SickFill>().AddAmount(other.transform.gameObject.GetComponent<Sphere>().elementQuantity * SceneLogic3D.GetComponent<SceneLogic3D>().CurrentLevel.Multiplier);
                WasteParticleSystem.Emit(20);
            }
            other.transform.gameObject.GetComponent<Sphere>().ConsumeSphere(transform.position, false);
           
        }
    }

}
