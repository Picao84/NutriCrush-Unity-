
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using Utils;

public class Funnel : MonoBehaviour
{
    public int DEFAULT_SPEED = 2;
    bool rotating = true;
    Vector3 m_EulerAngleVelocity;
    Rigidbody m_Rigidbody;
    public GameObject NutritionalElementRotatingSphere;
    public GameObject SceneLogic3D;
    Dictionary<NutritionElementsEnum, Shader> ColorTextures = new Dictionary<NutritionElementsEnum, Shader>();
    Dictionary<NutritionElementsEnum, Shader> ColorGhostTextures = new Dictionary<NutritionElementsEnum, Shader>();
    public Shader FatMaterial;
    public Shader FatGhostMaterial;
    public Shader SaturatesMaterial;
    public Shader SaltMaterial;
    public Shader SaltGhostMaterial;
    public Shader SaturatesGhostMaterial;
    public Shader SugarMaterial;
    public Shader SugarGhostMaterial;
    public Shader OutlineGhostMaterial;
    public Shader OuterOutlineGhostMaterial;
    public GameObject SoundEffects;
    bool isSpeedUp;
    GameObject blades;

    // Start is called before the first frame update
    void Start()
    {
        m_EulerAngleVelocity = new Vector3(0, 0, 100);
        m_Rigidbody = GetComponent<Rigidbody>();
        ColorTextures.Add(NutritionElementsEnum.Fat, FatMaterial);
        ColorTextures.Add(NutritionElementsEnum.Saturates, SaturatesMaterial);
        ColorTextures.Add(NutritionElementsEnum.Salt, SaltMaterial);
        ColorTextures.Add(NutritionElementsEnum.Sugar, SugarMaterial);

        ColorGhostTextures.Add(NutritionElementsEnum.Fat, FatGhostMaterial);
        ColorGhostTextures.Add(NutritionElementsEnum.Saturates, SaturatesGhostMaterial);
        ColorGhostTextures.Add(NutritionElementsEnum.Salt, SaltGhostMaterial);
        ColorGhostTextures.Add(NutritionElementsEnum.Sugar, SugarGhostMaterial);

        blades = transform.GetChild(4).gameObject;
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
            transform.Rotate(0, isSpeedUp ? DEFAULT_SPEED * 1.5f : DEFAULT_SPEED, 0);
            blades?.transform.Rotate(0, 0, isSpeedUp ? -DEFAULT_SPEED * 15f : -DEFAULT_SPEED * 10);
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

    public void SpeedUp()
    {
        isSpeedUp = true;
    }

    public void ResetSpeed()
    {
        isSpeedUp = false;
    }

 
    public async void CreateNutritionBubbles(Vector3 initialBubblePosition, Food food, Dictionary<NutritionElementsEnum, float> leftOnBars = null, bool isGhost = false)
    {
        foreach(KeyValuePair<NutritionElementsEnum, float> element in food.NutritionElements)
        {
            if (element.Value == 0)
                continue;

            SoundEffects.GetComponent<SoundEffects>().PlaySphere();
           
            var bubble = Instantiate(NutritionalElementRotatingSphere, new Vector3(0,0,0), Quaternion.identity);
            Sphere sphere = bubble.transform.GetComponentInChildren<Sphere>();
            sphere.gameObject.transform.position = initialBubblePosition;
            sphere.IsGhost = isGhost;
            
           
            sphere.SetElement(element.Key);
            sphere.SetQuantity(element.Value);
            sphere.soundEffects = SoundEffects.GetComponent<SoundEffects>();

            if (isGhost)
            {
              
               
                sphere.gameObject.GetComponent<MeshRenderer>().materials[0].shader = OutlineGhostMaterial;
                sphere.gameObject.GetComponent<MeshRenderer>().materials[1].shader = OuterOutlineGhostMaterial;
                sphere.gameObject.GetComponent<MeshRenderer>().materials[2].shader = ColorGhostTextures[element.Key];
                SceneLogic3D.GetComponent<SceneLogic3D>().AddGhostSphere(bubble.transform.GetComponentInChildren<Sphere>());
            }
            else
            {
                if (leftOnBars[element.Key] < element.Value * SceneLogic3D.GetComponent<SceneLogic3D>().CurrentLevel.Multiplier)
                {
                    sphere.cannotBeAbsorbed = true;
                }
                SceneLogic3D.GetComponent<SceneLogic3D>().AddSphere(bubble.transform.GetComponentInChildren<Sphere>());
                sphere.gameObject.GetComponent<MeshRenderer>().materials[0].shader = ColorTextures[element.Key];
            }

            if (isSpeedUp)
            {
                bubble.GetComponentInChildren<Funnel>().SpeedUp();
            }


            await AsyncTask.Await(500);
            
        }

    }

}
