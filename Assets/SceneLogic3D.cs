using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

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
    public GameObject CaloriesText;
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
    public GameObject CaloriesBar;
    public GameObject PotentialCalories;
    public GameObject SickBar;
    public GameObject SickParPotential;
    bool selectedHover;
    GameObject selectedFoodOver;
    Vector3 selectedFoodOverOriginalScale;
    Color32 sickColor = new Color32(144, 163, 78,255);
    public Canvas canvas;
    public GameObject MainPanel;
    public GameObject LostPanel;
    public GameObject LevelCompletePanel;
    public GameObject GradeText;
    bool caloriesFull = false;
    public AudioSource SoundEffects;
    public AudioSource Music;
    public GameObject foodImage;
    Vector3 foodImageOriginalPosition;
    public GameObject Host;
    TimeSpan TimeLeft = TimeSpan.FromMinutes(3);
    GameObject timeText;
    System.Timers.Timer timer = new System.Timers.Timer(1000);
    public GameObject GameOverText;
    bool gameOver;
    string gameOverText;
    public GameObject foodChoices;
    public GameObject Flawless;
    public GameObject SickImage;

    public Dictionary<Sphere, int> Touches = new Dictionary<Sphere, int>();
    bool anyDownTheVortex = false;

    // Start is called before the first frame update
    void Start()
    {
        timer.Elapsed += Timer_Elapsed;
        gameCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        foodBubbles = GameObject.FindGameObjectsWithTag("FoodBubble");
        transparentPlane = GameObject.FindGameObjectWithTag("TransparentPlane");
        Spheres.CollectionChanged += Spheres_CollectionChanged;
        foodImageOriginalPosition = foodImage.transform.position;
        timeText = GameObject.FindGameObjectWithTag("Time");
        timeText.GetComponent<TextMeshPro>().text = $"{(int)TimeLeft.TotalMinutes}:{TimeLeft.Seconds:00}";
      
        SickBar.GetComponent<SickFill>().SickBarFilled += SceneLogic3D_SickBarFilled;
        CaloriesBar.GetComponent<CaloriesFill>().CaloriesBarFilled += SceneLogic3D_CaloriesBarFilled;

        FoodNameText.GetComponent<TextMeshPro>().OnPreRenderText += SceneLogic3D_OnPreRenderText;
        foodChoices.SetActive(false);
    }

    private void Timer_Elapsed(object sender, ElapsedEventArgs e)
    {
        TimeLeft = TimeLeft - TimeSpan.FromSeconds(1);

        if(TimeLeft.TotalSeconds == 0)
        {
            timer.Stop();
            GameOver("You starved!");
        }
    }

    private void SceneLogic3D_OnPreRenderText(TMP_TextInfo obj)
    {
        var textBounds = obj.textComponent.textBounds;
        foodImage.transform.localPosition = new Vector3(textBounds.min.x - 0.15f, foodImage.transform.localPosition.y, foodImage.transform.localPosition.z);
    }

    private void SceneLogic3D_CaloriesBarFilled(object sender, System.EventArgs e)
    {
        caloriesFull = true;
    }

    //public void OnDestroy()
    //{
    //    CaloriesBar.GetComponent<CaloriesFill>().CaloriesBarFilled -= SceneLogic3D_CaloriesBarFilled;
    //    SickBar.GetComponent<SickFill>().SickBarFilled -= SceneLogic3D_SickBarFilled;
    //    FoodNameText.GetComponent<TextMeshPro>().OnPreRenderText -= SceneLogic3D_OnPreRenderText;
    //}

    private void SceneLogic3D_SickBarFilled(object sender, System.EventArgs e)
    {
        GameOver("You got sick!");
        SickImage.SetActive(true);
    }

    private void GameOver(string text)
    {
        gameOver = true;
        gameOverText = text;
    }

    public void Reset()
    {
        Touches.Clear();
        Host.GetComponent<Host>().Show();
        gameOver = false;
        Music.Play();
        TimeLeft = TimeSpan.FromMinutes(3);
        caloriesFull = false;
        RandomiseFood();
        foreach (GameObject foodBubble in foodBubbles)
        {
            foodBubble.GetComponent<FoodBubble>().Show();
        }
        CaloriesBar.GetComponent<CaloriesFill>().Reset();
        SickBar.GetComponent<SickFill>().Reset();
        CurrentFat.GetComponent<FillScript>().Reset();
        CurrentSaturates.GetComponent<FillScript>().Reset();
        CurrentSalt.GetComponent<FillScript>().Reset();
        CurrentSugar.GetComponent<FillScript>().Reset();
        canvas.enabled = false;
        timer.Start();
    }

    private string CalculateGrade()
    {
        var caloriesScript = CaloriesBar.GetComponent<CaloriesFill>();
        var caloriesRatio = caloriesScript.currentAmount / caloriesScript.MaxAmount;
        var fatScript = CurrentFat.GetComponent<FillScript>();
        var fatRatio = fatScript.currentAmount / fatScript.MaxAmount;
        var saturatesScript = CurrentSaturates.GetComponent<FillScript>();
        var saturatesRatio = saturatesScript.currentAmount / saturatesScript.MaxAmount;
        var saltScript = CurrentSalt.GetComponent<FillScript>();
        var saltRatio = saltScript.currentAmount / saltScript.MaxAmount;
        var sugarScript = CurrentSugar.GetComponent<FillScript>();
        var sugarRatio = sugarScript.currentAmount / sugarScript.MaxAmount;

        var average = (caloriesRatio + fatRatio + saturatesRatio + saltRatio + sugarRatio) / 5;

        string result = average switch
        {
            > 0.95f => "A",
            > 0.85f => "B",
            > 0.75f => "C",
            > 0.65f => "D",
            > 0.50f => "E",
            > 0.40f => "F",
            > 0.25f => "G",
            _ => "H",
        };

        return result;

    }

    public void StartGame()
    {
        foodChoices.SetActive(true);
        SickBar.GetComponent<SickFill>().MaxAmount =
          (CurrentFat.GetComponent<FillScript>().MaxAmount +
          CurrentSaturates.GetComponent<FillScript>().MaxAmount +
          CurrentSalt.GetComponent<FillScript>().MaxAmount +
          CurrentSugar.GetComponent<FillScript>().MaxAmount) * 0.5f;
        SickParPotential.GetComponent<SickFill>().MaxAmount =
           (CurrentFat.GetComponent<FillScript>().MaxAmount +
            CurrentSaturates.GetComponent<FillScript>().MaxAmount +
            CurrentSalt.GetComponent<FillScript>().MaxAmount +
            CurrentSugar.GetComponent<FillScript>().MaxAmount) * 0.5f;
        RandomiseFood();
        canvas.enabled = false;
        Host.GetComponent<Host>().Show();
        timer.Start();
    }

    private void Spheres_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if(((ObservableCollection<Sphere>)sender).Count == 0) 
        {
            if (!caloriesFull && !gameOver)
            {
                if(Touches.All(x => x.Value == 1) && !anyDownTheVortex)
                {
                    Flawless.GetComponent<FlawlessScript>().Play();
                    TimeLeft += TimeSpan.FromSeconds(5);
                }

                anyDownTheVortex = false;
                Touches.Clear();

                transparentPlane.SetActive(true);
                transparentPlane.GetComponent<TransparentPlane>().Show();
                foreach (GameObject foodBubble in foodBubbles)
                {
                    foodBubble.GetComponent<FoodBubble>().Show();
                }
                RandomiseFood();
                Host.GetComponent<Host>().Show();
            }
            else
            {
                if (!gameOver)
                {
                    FinishLevel();
                }
            }
        }
    }

    private void FinishLevel()
    {
        timer.Stop();
        Music.Pause();
        SoundEffects.GetComponent<SoundEffects>().PlayWin();
        canvas.enabled = true;
        MainPanel.SetActive(false);
        LostPanel.SetActive(false);
        LevelCompletePanel.SetActive(true);
        SoundEffects.GetComponent<SoundEffects>().PlayWin();
        GradeText.GetComponent<TextMeshProUGUI>().text = CalculateGrade();
    }

    private void MakeTextGreenAndBold(TextMeshPro textMeshPro)
    {
        textMeshPro.color = sickColor;
        textMeshPro.fontStyle = FontStyles.Bold;
    }

    private void ResetTextStyle(TextMeshPro textMeshPro)
    {
        textMeshPro.color = Color.black;
        textMeshPro.fontStyle = FontStyles.Normal;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOver && canvas.enabled == false)
        {
            transparentPlane.SetActive(true);
            transparentPlane.GetComponent<TransparentPlane>().Show();
            status.SetActive(false);
            timer.Stop();
            foreach(var sphere in Spheres)
            {
                sphere.GetComponent<Rigidbody>().useGravity = false;
                GameObject.Destroy(sphere.gameObject);
            }

            Spheres.Clear();

            Cursor.visible = true;
            GameOverText.GetComponent<TextMeshProUGUI>().text = gameOverText;
            Music.Pause();
            Host.GetComponent<Host>().Hide();
            SoundEffects.GetComponent<SoundEffects>().PlayGameOver();
            canvas.enabled = true;
            MainPanel.SetActive(false);
            LostPanel.SetActive(true);
            LevelCompletePanel.SetActive(false);
        }

        timeText.GetComponent<TextMeshPro>().text = $"{(int)TimeLeft.TotalMinutes}:{TimeLeft.Seconds:00}";

        if (gameCamera != null && !canvas.enabled)
        {
            if (Input.GetMouseButtonDown(0))
            {
                selectedRigidBody = GetRigidbodyFromMouseClick();

                if(selectedRigidBody != null && selectedRigidBody.GetComponent<Sphere>() != null) 
                {
                    Cursor.visible = false;
                    var sphere = selectedRigidBody.GetComponent<Sphere>();
                    sphere.PauseRotation();
                    sphere.SetPicked();
                   
                    //selectedRigidBody.useGravity = false;             
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
                        selectedFoodOver = foodBubble;
                        selectedFoodOverOriginalScale = foodBubble.transform.localScale;
                        selectedFoodOver.transform.localScale = new Vector3(selectedFoodOverOriginalScale.x * 1.2f, selectedFoodOverOriginalScale.y * 1.2f, selectedFoodOverOriginalScale.z * 1.2f);

                        selectedHover = true;
 
                        var food = foodBubble.GetComponent<FoodBubble>().Food;
                        FoodNameText.GetComponent<TextMeshPro>().text = food.Name;

                        if (!string.IsNullOrEmpty(food.FileName))
                        {
                            var image = Resources.Load<Texture2D>(food.FileName);
                            foodImage.GetComponent<SpriteRenderer>().sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), new Vector2(0.5f, 0.5f));
                        }
                        else
                        {
                            foodImage.GetComponent<SpriteRenderer>().sprite = null;
                        }

                        CaloriesText.GetComponent<TextMeshPro>().text = $"{food.Calories}";
                        FatAmountText.GetComponent<TextMeshPro>().text = $"{food.NutritionElements[NutritionElementsEnum.Fat].ToString()}g";
                        var canAbsorbFat = PotentialFat.GetComponent<FillScript>().Simulate(CurrentFat.GetComponent<FillScript>().currentAmount + food.NutritionElements[NutritionElementsEnum.Fat]);
                        if (!canAbsorbFat)
                        {
                            MakeTextGreenAndBold(FatAmountText.GetComponent<TextMeshPro>());
                            SickParPotential.GetComponent<SickFill>().Simulate(SickBar.GetComponent<SickFill>().currentAmount + food.NutritionElements[NutritionElementsEnum.Fat]);
                        }
                        
                        var canAbsorbSaturates = PotentialSaturates.GetComponent<FillScript>().Simulate(CurrentSaturates.GetComponent<FillScript>().currentAmount + food.NutritionElements[NutritionElementsEnum.Saturates]);
                        if (!canAbsorbSaturates)
                        {
                            MakeTextGreenAndBold(SaturatesAmountText.GetComponent<TextMeshPro>());
                            SickParPotential.GetComponent<SickFill>().Simulate(SickBar.GetComponent<SickFill>().currentAmount + food.NutritionElements[NutritionElementsEnum.Saturates]);
                        }

                        var canAbsorbSalt = PotentialSalt.GetComponent<FillScript>().Simulate(CurrentSalt.GetComponent<FillScript>().currentAmount + food.NutritionElements[NutritionElementsEnum.Salt]);
                        if (!canAbsorbSalt)
                        {
                            MakeTextGreenAndBold(SaltAmountText.GetComponent<TextMeshPro>());
                            SickParPotential.GetComponent<SickFill>().Simulate(SickBar.GetComponent<SickFill>().currentAmount + food.NutritionElements[NutritionElementsEnum.Salt]);
                        }

                        var canAbsorbSugar = PotentialSugar.GetComponent<FillScript>().Simulate(CurrentSugar.GetComponent<FillScript>().currentAmount + food.NutritionElements[NutritionElementsEnum.Sugar]);
                        if (!canAbsorbSugar)
                        {
                            MakeTextGreenAndBold(SugarAmountText.GetComponent<TextMeshPro>());
                            SickParPotential.GetComponent<SickFill>().Simulate(SickBar.GetComponent<SickFill>().currentAmount + food.NutritionElements[NutritionElementsEnum.Sugar]);
                        }


                        PotentialCalories.GetComponent<CaloriesFill>().Simulate(CaloriesBar.GetComponent<CaloriesFill>().currentAmount + food.Calories);
                        SaturatesAmountText.GetComponent<TextMeshPro>().text = $"{food.NutritionElements[NutritionElementsEnum.Saturates].ToString()}g";
                        SaltAmountText.GetComponent<TextMeshPro>().text = $"{food.NutritionElements[NutritionElementsEnum.Salt].ToString()}g";
                        SugarAmountText.GetComponent<TextMeshPro>().text = $"{food.NutritionElements[NutritionElementsEnum.Sugar].ToString()}g";

                        status.SetActive(true);
                    }
                }
                else
                {
                    if(selectedFoodOver != null)
                    {
                        selectedFoodOver.transform.localScale = selectedFoodOverOriginalScale;
                        selectedFoodOver = null;
                    }

                    if (selectedHover)
                    {
                        selectedHover = false;
                        foodImage.transform.position = foodImageOriginalPosition;
                        PotentialFat.GetComponent<FillScript>().Reset();
                        ResetTextStyle(FatAmountText.GetComponent<TextMeshPro>());
                        PotentialSaturates.GetComponent<FillScript>().Reset();
                        ResetTextStyle(SaturatesAmountText.GetComponent<TextMeshPro>());
                        PotentialSugar.GetComponent<FillScript>().Reset();
                        ResetTextStyle(SugarAmountText.GetComponent<TextMeshPro>());
                        PotentialSalt.GetComponent<FillScript>().Reset();
                        ResetTextStyle(SaltAmountText.GetComponent<TextMeshPro>());
                        PotentialCalories.GetComponent<CaloriesFill>().Reset();
                        SickParPotential.GetComponent<SickFill>().Reset();
                        status.SetActive(false);
                    }
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
            selectedRigidBody.velocity = new Vector3(mousePositionOffset.x / Time.deltaTime * 40, mousePositionOffset.y / Time.deltaTime * 40, mousePositionOffset.z / Time.deltaTime * 40);
            originalScreenTargetPosition = gameCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, gameCamera.nearClipPlane)); 
        }
    }

    private void RandomiseFood()
    {
        foreach(GameObject foodBubble in foodBubbles)
        {
            //if (foodBubble.GetComponent<FoodBubble>().Food == null)
            //{
                foodBubble.GetComponent<FoodBubble>().SetFood(Constants3D.Foods[UnityEngine.Random.Range(0, Constants3D.Foods.Count)]);
            //}
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
            var sphere = allHits.First(x => x.collider.transform.gameObject.GetComponent<Sphere>() != null).collider;

            Touches[sphere.GetComponent<Sphere>()]++;
            
            originalScreenTargetPosition = gameCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, gameCamera.nearClipPlane));
            return sphere.GetComponent<Rigidbody>();
        }

        if (allHits.Any(x => x.collider.transform.gameObject.GetComponent<FoodBubble>() != null))
        {
            var food = allHits.First(x => x.collider.transform.gameObject.GetComponent<FoodBubble>() != null).collider.transform.gameObject.GetComponent<FoodBubble>();
            if (food.Food != null)
            {
                Flawless.GetComponent<FlawlessScript>().Hide();
                SoundEffects.GetComponent<SoundEffects>().PlayBubble();
                CaloriesBar.GetComponent<CaloriesFill>().AddAmount(food.Food.Calories);
                food.FoodChosen();
                transparentPlane.GetComponent<TransparentPlane>().Hide();
                status.SetActive(false);
                Host.GetComponent<Host>().Hide();
                food.Food = null;
            }

        }

        return null;
    }

    public void AddSphere(Sphere sphere)
    {
        Spheres.Add(sphere);
        Touches.Add(sphere, 0);
    }

    public void RemoveSphere(Sphere sphere, bool absorbed)
    {
        if (!anyDownTheVortex) 
        {
            this.anyDownTheVortex = !absorbed;
        }

        if(selectedRigidBody == sphere.GetComponent<Rigidbody>())
        {
            selectedRigidBody = null;
            Cursor.visible = true;
        }
        Spheres.Remove(sphere);
    }

}
