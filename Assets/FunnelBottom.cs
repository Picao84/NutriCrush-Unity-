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
    SceneLogic3D SceneLogic;

    // Start is called before the first frame update
    void Start()
    {
        Particles = transform.parent.GetChild(2).gameObject;
        SceneLogic = SceneLogic3D.GetComponent<SceneLogic3D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.transform.gameObject.GetComponent<Sphere>() != null)
        {
            var sphere = other.transform.gameObject.GetComponent<Sphere>();
            float amountToAply = 1f;

            ParticleSystem particleSystem = new ParticleSystem();

            switch (sphere.element)
            {
                case NutritionElementsEnum.Fat:

                    particleSystem = Particles.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>();
                    amountToAply = SceneLogic.CurrentFat.GetComponent<FillScript>().amountToApply;

                    break;

                case NutritionElementsEnum.Saturates:

                    particleSystem = Particles.transform.GetChild(1).gameObject.GetComponent<ParticleSystem>();
                    amountToAply = SceneLogic.CurrentSaturates.GetComponent<FillScript>().amountToApply;

                    break;

                case NutritionElementsEnum.Salt:

                    particleSystem = Particles.transform.GetChild(2).gameObject.GetComponent<ParticleSystem>();
                    amountToAply = SceneLogic.CurrentSalt.GetComponent<FillScript>().amountToApply;

                    break;

                case NutritionElementsEnum.Sugar:

                    particleSystem = Particles.transform.GetChild(3).gameObject.GetComponent<ParticleSystem>();
                    amountToAply = SceneLogic.CurrentSugar.GetComponent<FillScript>().amountToApply;

                    break;
            }

            if (!sphere.IsGhost)
            {
                SickBar.GetComponent<SickFill>().RemoveAmount((sphere.elementQuantity * SceneLogic.CurrentLevel.Multiplier) * amountToAply);
            }

            var main = particleSystem.main;
            main.startColor = Constants.ParticleGradients[other.transform.gameObject.GetComponent<Sphere>().element];
            particleSystem.Emit(50);

            sphere.ConsumeSphere(transform.position, false);
           
        }
    }

}
