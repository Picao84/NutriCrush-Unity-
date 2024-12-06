
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using Utils;

public class Funnel : MonoBehaviour
{
    public int Speed = 100;
    bool rotating = true;
    Vector3 m_EulerAngleVelocity;
    Rigidbody m_Rigidbody;
    public GameObject NutritionalElementRotatingSphere;
    public GameObject SceneLogic3D;
    Dictionary<NutritionElementsEnum, Texture> ColorTextures = new Dictionary<NutritionElementsEnum, Texture>();
    public Texture RedMaterial;
    public Texture GreenMaterial;
    public Texture OrangeMaterial;
    public Texture PurpleMaterial;
    public GameObject SoundEffects;

    // Start is called before the first frame update
    void Start()
    {
        m_EulerAngleVelocity = new Vector3(0, 0, 100);
        m_Rigidbody = GetComponent<Rigidbody>();
        ColorTextures.Add(NutritionElementsEnum.Fat, RedMaterial);
        ColorTextures.Add(NutritionElementsEnum.Saturates, GreenMaterial);
        ColorTextures.Add(NutritionElementsEnum.Salt, OrangeMaterial);
        ColorTextures.Add(NutritionElementsEnum.Sugar, PurpleMaterial);
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    private void FixedUpdate()
    {
         if (rotating)
        {
            //Quaternion deltaRotation = Quaternion.Euler(m_EulerAngleVelocity * Time.fixedDeltaTime);
            //m_Rigidbody.MoveRotation(m_Rigidbody.rotation * deltaRotation);
            transform.Rotate(0, Speed, 0);
        }
    }

    public void PauseRotation()
    {
        rotating = false;
    }

    public void ResumeRotation()
    {
        rotating = true;
    }

    public async void CreateNutritionBubbles(Vector3 initialBubblePosition, Food food)
    {
        foreach(KeyValuePair<NutritionElementsEnum, float> element in food.NutritionElements)
        {
            if (element.Value == 0)
                continue;

            SoundEffects.GetComponent<SoundEffects>().PlaySphere();
           
            var bubble = Instantiate(NutritionalElementRotatingSphere, new Vector3(0,0,0), Quaternion.identity);
            Sphere sphere = bubble.transform.GetComponentInChildren<Sphere>();
            sphere.gameObject.transform.position = initialBubblePosition;
            sphere.SetColor(element.Key);
            sphere.SetQuantity(element.Value);
            sphere.soundEffects = SoundEffects.GetComponent<SoundEffects>();
            sphere.gameObject.GetComponent<MeshRenderer>().material.mainTexture = ColorTextures[element.Key];
            SceneLogic3D.GetComponent<SceneLogic3D>().AddSphere(bubble.transform.GetComponentInChildren<Sphere>());

            await AsyncTask.Await(500);
            
        }

    }

}
