using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TMPro;
using UnityEngine;

public class SceneLogic3D : MonoBehaviour
{
    Rigidbody selectedRigidBody;
    Vector3 originalScreenTargetPosition;
    GameObject[] foodBubbles;
    ObservableCollection<Sphere> Spheres = new ObservableCollection<Sphere>();
    GameObject transparentPlane;
    Camera gameCamera;
    public GameObject status;
    public GameObject FoodNameText;
    public GameObject FatAmountText;
    public GameObject SaturatesAmountText;
    public GameObject SaltAmountText;
    public GameObject SugarAmountText;
    public GameObject CurrentFat;
    public GameObject PotentialFat;
    public GameObject CurrentSaturates;
    public GameObject PotentialSaturates;
    public GameObject CurrentSalt;
    public GameObject PotentialSalt;
    public GameObject CurrentSugar;
    public GameObject PotentialSugar;
    bool selectedHover;

    // Start is called before the first frame update
    void Start()
    {
        gameCamera = GameObject.FindGameObjectWithTag("Camera").GetComponent<Camera>();
        foodBubbles = GameObject.FindGameObjectsWithTag("FoodBubble");
        transparentPlane = GameObject.FindGameObjectWithTag("TransparentPlane");
        Spheres.CollectionChanged += Spheres_CollectionChanged;
        RandomiseFood();
    }

    private void Spheres_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if(((ObservableCollection<Sphere>)sender).Count == 0) 
        {
            transparentPlane.SetActive(true);
            transparentPlane.GetComponent<TransparentPlane>().Show();
            foreach (GameObject foodBubble in foodBubbles)
            {
                foodBubble.GetComponent<FoodBubble>().Show();
            }
            RandomiseFood();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameCamera != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                selectedRigidBody = GetRigidbodyFromMouseClick();

                if(selectedRigidBody != null && selectedRigidBody.GetComponent<Sphere>() != null) 
                {
                    Cursor.visible = false;
                    var sphere = selectedRigidBody.GetComponent<Sphere>();
                    selectedRigidBody.drag = 1;
                    sphere.PauseRotation();
                    sphere.isPicked = true;
                    selectedRigidBody.useGravity = false;
                    
                }
            }

            if (Input.GetMouseButtonUp(0) && selectedRigidBody != null)
            {
                if (selectedRigidBody != null && selectedRigidBody.GetComponent<Sphere>() != null)
                {
                    Cursor.visible = true;
                    var sphere = selectedRigidBody.GetComponent<Sphere>();
                    sphere.isPicked = false;
                    selectedRigidBody.useGravity = true;
                }

                selectedRigidBody = null;
            }

            if(selectedRigidBody == null)
            {
                
                var foodBubble = GetFoodBubbleFromMouseOver();

                if(foodBubble != null)
                {
                    if (!selectedHover)
                    {
                        selectedHover = true;

                        var food = foodBubble.GetComponent<FoodBubble>().Food;
                        FoodNameText.GetComponent<TextMeshPro>().text = food.Name;
                        FatAmountText.GetComponent<TextMeshPro>().text = $"{food.NutritionElements[NutritionElementsEnum.Fat].ToString()}g";
                        PotentialFat.GetComponent<FillScript>().Simulate(CurrentFat.GetComponent<FillScript>().currentAmount + food.NutritionElements[NutritionElementsEnum.Fat]);
                        PotentialSaturates.GetComponent<FillScript>().Simulate(CurrentSaturates.GetComponent<FillScript>().currentAmount + food.NutritionElements[NutritionElementsEnum.Saturates]);
                        PotentialSalt.GetComponent<FillScript>().Simulate(CurrentSalt.GetComponent<FillScript>().currentAmount + food.NutritionElements[NutritionElementsEnum.Salt]);
                        PotentialSugar.GetComponent<FillScript>().Simulate(CurrentSugar.GetComponent<FillScript>().currentAmount + food.NutritionElements[NutritionElementsEnum.Sugar]);
                        SaturatesAmountText.GetComponent<TextMeshPro>().text = $"{food.NutritionElements[NutritionElementsEnum.Saturates].ToString()}g";
                        SaltAmountText.GetComponent<TextMeshPro>().text = $"{food.NutritionElements[NutritionElementsEnum.Salt].ToString()}g";
                        SugarAmountText.GetComponent<TextMeshPro>().text = $"{food.NutritionElements[NutritionElementsEnum.Sugar].ToString()}g";

                        status.SetActive(true);
                    }
                }
                else
                {
                    selectedHover = false;
                    PotentialFat.GetComponent<FillScript>().Reset();
                    PotentialSaturates.GetComponent<FillScript>().Reset();
                    PotentialSugar.GetComponent<FillScript>().Reset();
                    PotentialSalt.GetComponent<FillScript>().Reset();
                    status.SetActive(false);
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if(selectedRigidBody != null) 
        {
            var screenToWorldPoint = gameCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, gameCamera.nearClipPlane));
            Vector3 mousePositionOffset = new Vector3(screenToWorldPoint.x, screenToWorldPoint.y, screenToWorldPoint.z) - originalScreenTargetPosition;
            selectedRigidBody.velocity = new Vector3(mousePositionOffset.x / Time.deltaTime * 100, mousePositionOffset.y / Time.deltaTime, mousePositionOffset.z / Time.deltaTime * 100);
            originalScreenTargetPosition = gameCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, gameCamera.nearClipPlane)); 
        }
    }

    private void RandomiseFood()
    {
        foreach(GameObject foodBubble in foodBubbles)
        {
            foodBubble.GetComponent<FoodBubble>().Food = Constants3D.Foods[Random.Range(0, Constants3D.Foods.Count)];
        }
    }

    private GameObject GetFoodBubbleFromMouseOver()
    {
        var ray = gameCamera.ScreenPointToRay(Input.mousePosition);

        var allHits = Physics.RaycastAll(ray);

        if (allHits.Any(x => x.collider.transform.gameObject.GetComponent<FoodBubble>() != null))
        {
            return allHits.First(x => x.collider.transform.gameObject.GetComponent<FoodBubble>() != null).collider.transform.gameObject;
        }

        return null;
    }

    private Rigidbody GetRigidbodyFromMouseClick()
    {
        var ray = gameCamera.ScreenPointToRay(Input.mousePosition);

        var allHits = Physics.RaycastAll(ray);

        if (allHits.Any(x => x.collider.transform.gameObject.GetComponent<Sphere>() != null))
        {
            originalScreenTargetPosition = gameCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, gameCamera.nearClipPlane));
            return allHits.First(x => x.collider.transform.gameObject.GetComponent<Sphere>() != null).collider.GetComponent<Rigidbody>();
        }

        if (allHits.Any(x => x.collider.transform.gameObject.GetComponent<FoodBubble>() != null))
        {
            GameObject.FindGameObjectWithTag("TransparentPlane").GetComponent<TransparentPlane>().Hide();
            allHits.First(x => x.collider.transform.gameObject.GetComponent<FoodBubble>() != null).collider.transform.gameObject.GetComponent<FoodBubble>().FoodChosen();
        }

        return null;
    }

    public void AddSphere(Sphere sphere)
    {
        Spheres.Add(sphere);
    }

    public void RemoveSphere(Sphere sphere)
    {
        Spheres.Remove(sphere);
    }


}
