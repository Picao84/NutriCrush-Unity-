using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;


public class FunnelBottom : MonoBehaviour
{
    
    public GameObject SceneLogic3D;
    public GameObject SickBar;
    public GameObject Particles;

    // Start is called before the first frame update
    void Start()
    {
        Particles = transform.parent.GetChild(2).gameObject;
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
                SickBar.GetComponent<SickFill>().RemoveAmount(other.transform.gameObject.GetComponent<Sphere>().elementQuantity * SceneLogic3D.GetComponent<SceneLogic3D>().CurrentLevel.Multiplier);
            }

            var element = other.transform.gameObject.GetComponent<Sphere>().element;

            ParticleSystem particleSystem = new ParticleSystem();

            switch (element)
            {
                case NutritionElementsEnum.Fat:

                    particleSystem = Particles.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>();

                    break;

                case NutritionElementsEnum.Saturates:

                    particleSystem = Particles.transform.GetChild(1).gameObject.GetComponent<ParticleSystem>();

                    break;

                case NutritionElementsEnum.Salt:

                    particleSystem = Particles.transform.GetChild(2).gameObject.GetComponent<ParticleSystem>();

                    break;

                case NutritionElementsEnum.Sugar:

                    particleSystem = Particles.transform.GetChild(3).gameObject.GetComponent<ParticleSystem>();

                    break;
            }

            var main = particleSystem.main;
            main.startColor = Constants.ParticleGradients[other.transform.gameObject.GetComponent<Sphere>().element];
            particleSystem.Emit(50);

            other.transform.gameObject.GetComponent<Sphere>().ConsumeSphere(transform.position, false);
           
        }
    }

}
