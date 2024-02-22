using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class FunnelBottom : MonoBehaviour
{
    Gradient greenGradient;
    float currentLevel = 0;
    ParticleSystem WasteParticleSystem;
    public GameObject SceneLogic3D;
    public GameObject SickBar;

    // Start is called before the first frame update
    void Start()
    {
        greenGradient = new Gradient();
        var colors = new GradientColorKey[2];
        colors[0] = new GradientColorKey(Color.black, 0.0f);
        colors[1] = new GradientColorKey(new Color(135/255f,171/255f,8/255f), 0.5f);

        var alphas = new GradientAlphaKey[1];
        alphas[0] = new GradientAlphaKey(1.0f, 1.0f);

        greenGradient.SetKeys(colors,alphas);

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
            SickBar.GetComponent<SickFill>().AddAmount(other.transform.gameObject.GetComponent<Sphere>().elementQuantity);
            other.transform.gameObject.GetComponent<Sphere>().ConsumeSphere(transform.position);
            WasteParticleSystem.Emit(5);
        }
    }

}
