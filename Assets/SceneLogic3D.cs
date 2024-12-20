using Assets;
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
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    public GameObject SickBarPotential;
    bool selectedHover;
    public GameObject CurrentLevelPanel;
    GameObject selectedFoodOver;
    Vector3 selectedFoodOverOriginalScale;
    Color32 sickColor = new Color32(144, 163, 78,255);
    public Canvas canvas;
    public GameObject MainPanel;
    public GameObject LostPanel;
    public GameObject EditFoodPanel;
    public GameObject LevelCompletePanel;
    public GameObject LevelSelectionPanel;
    public GameObject GradeText;
    public GameObject Reward;
    bool caloriesFull = false;
    public AudioSource SoundEffects;
    public AudioSource Music;
    public GameObject foodImage;
    public GameObject rewardQuantity;
    Vector3 foodImageOriginalPosition;
    public GameObject Host;
    TimeSpan TimeLeft = TimeSpan.FromMinutes(3);
    GameObject timeText;
    public GameObject GameOverText;
    bool gameOver;
    string gameOverText;
    public GameObject foodChoices;
    public GameObject Flawless;
    public GameObject SickImage;
    public GameObject StarveImage;
    public GameObject Tutorial;
    public bool tutorialEnabled = true;
    bool pausedBalls;
    bool canSelectFood = true;
    bool absorbedWellDone;
    bool downTheVortexMessage;
    bool tutorialFoodSelected;
    int tutorialNumberofRoundsPlayed;
    bool pausedForTimer;
    bool timeRunning;
    bool showedBallsMessage;
    Level currentLevel;

    public GameObject VisualFunnel;
    public Dictionary<Sphere, int> Touches = new Dictionary<Sphere, int>();
    bool anyDownTheVortex = false;
    Coroutine Timer;
    public List<Food> CurrentShuffledDeck = new List<Food>();
    bool checkForTutorialToggle = true;

    // Start is called before the first frame update
    void Start()
    {
      

#if !UNITY_WEBGL

        // Calculate the target width based on the screen width and 16:9 aspect ratio
        int targetWidth = Screen.height * 9 / 16;

        // Set the game's resolution to match the target width and height
        Screen.SetResolution(targetWidth, Screen.height, true);

#endif
        PlayerData.InitialiseFoodDeck();

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

    public void PlayNextLevel()
    {
        PlayLevel(Constants.Levels[currentLevel.LevelID + 1]);
    }

    public void Reset()
    {
        LevelSelectionPanel.SetActive(false);
        LevelCompletePanel.SetActive(false);
        EditFoodPanel.SetActive(false);
        LostPanel.SetActive(false);
        StarveImage.SetActive(false);
        SickImage.SetActive(false);
        Touches.Clear();
        Host.GetComponent<Host>().Show();
        gameOver = false;
        Music.Play();
        TimeLeft = TimeSpan.FromMinutes(3);
        caloriesFull = false;
        GetNextFood();
        foreach (GameObject foodBubble in foodBubbles)
        {
            foodBubble.GetComponent<FoodBubble>().Show();
        }
        foodChoices.SetActive(true);
        CaloriesBar.GetComponent<CaloriesFill>().Reset();
        SickBar.GetComponent<SickFill>().Reset();
        CurrentFat.GetComponent<FillScript>().Reset();
        CurrentSaturates.GetComponent<FillScript>().Reset();
        CurrentSalt.GetComponent<FillScript>().Reset();
        CurrentSugar.GetComponent<FillScript>().Reset();
        canvas.enabled = false;
        Timer = StartCoroutine(CustomTimer.Timer(1, () => {

            TimeLeft = TimeLeft - TimeSpan.FromSeconds(1);

            if (TimeLeft.TotalSeconds == 0)
            {
                timeRunning = false;
                StopCoroutine(Timer);
                //timer.Stop();
                GameOver("You starved!");
                StarveImage.SetActive(true);
            }

        }));
    }

    private GradesEnum CalculateGrade()
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

        GradesEnum result = average switch
        {
            > 0.95f => GradesEnum.A,
            > 0.85f => GradesEnum.B,
            > 0.75f => GradesEnum.C,
            > 0.65f => GradesEnum.D,
            > 0.50f => GradesEnum.E,
            > 0.40f => GradesEnum.F,
            > 0.25f => GradesEnum.G,
            _ => GradesEnum.H,
        };

        return result;

    }

    public void ContinueTutorial()
    {
        pausedBalls = false;
        showedBallsMessage = true;

        VisualFunnel.GetComponent<Funnel>().ResumeRotation();

        if (pausedForTimer)
        {
            pausedForTimer = false;
            anyDownTheVortex = false;
            Touches.Clear();

            transparentPlane.SetActive(true);
            transparentPlane.GetComponent<TransparentPlane>().Show();
            foreach (GameObject foodBubble in foodBubbles)
            {
                foodBubble.GetComponent<FoodBubble>().Show();
            }
            GetNextFood();
            Host.GetComponent<Host>().Show();
            Timer = StartCoroutine(CustomTimer.Timer(1, () => {

                TimeLeft = TimeLeft - TimeSpan.FromSeconds(1);

                if (TimeLeft.TotalSeconds == 0)
                {
                    timeRunning = false;
                    StopCoroutine(Timer);
                    //timer.Stop();
                    GameOver("You starved!");
                    StarveImage.SetActive(true);
                }

            }));
        }
    }

    public void ContinueTutorial(int step)
    {
        if (step == 4)
        {
            foodChoices.SetActive(true);
            GetNextFood();
            canSelectFood = false;
        }

        if(step == 5)
        {
            canSelectFood = true;
        }

        if(step == 6)
        {

            pausedBalls = true;
            VisualFunnel.GetComponent<Funnel>().PauseRotation();
        }

        if(step == 9)
        {
            pausedBalls = false;
            VisualFunnel.GetComponent<Funnel>().ResumeRotation();
        }
    }

    private void ShuffleDeck()
    {
        var playerDeck = PlayerData.FoodDeck.ToList();

        while (playerDeck.Count > 0)
        {
            var nextFood = playerDeck[UnityEngine.Random.Range(0, playerDeck.Count)];
            CurrentShuffledDeck.Add(nextFood);
            playerDeck.Remove(nextFood);
        }

    }

    public void PlayLevel(Level level)
    {
        currentLevel = level;
        CurrentFat.GetComponent<FillScript>().MaxAmount = level.MaxFat;
        CurrentSaturates.GetComponent<FillScript>().MaxAmount = level.MaxSaturates;
        CurrentSalt.GetComponent<FillScript>().MaxAmount = level.MaxSalt;
        CurrentSugar.GetComponent<FillScript>().MaxAmount = level.MaxSugar;
        CaloriesBar.GetComponent<CaloriesFill>().MaxAmount = level.CaloriesObjective;
        PotentialFat.GetComponent<FillScript>().MaxAmount = level.MaxFat;
        PotentialSaturates.GetComponent<FillScript>().MaxAmount = level.MaxSaturates;
        PotentialSalt.GetComponent<FillScript>().MaxAmount = level.MaxSalt;
        PotentialSugar.GetComponent<FillScript>().MaxAmount = level.MaxSugar;
        PotentialCalories.GetComponent<CaloriesFill>().MaxAmount = level.CaloriesObjective;
        tutorialEnabled = false;
        checkForTutorialToggle = false;
        CurrentLevelPanel.SetActive(true);
        CurrentLevelPanel.GetComponent<CurrentLevelPanelScript>().SetCurrentLevel(currentLevel);
        Reset();
    }

    public void StartGame()
    {
        if (PlayerData.LevelsUnlocked.Count == 1)
        {
            ShuffleDeck();

            if (checkForTutorialToggle)
            {
                tutorialEnabled = GameObject.Find("Toggle").GetComponent<Toggle>().isOn;
            }

            currentLevel = Constants.Levels[0];
            CurrentLevelPanel.SetActive(true);
            CurrentLevelPanel.GetComponent<CurrentLevelPanelScript>().SetCurrentLevel(currentLevel);

            CurrentFat.GetComponent<FillScript>().MaxAmount = currentLevel.MaxFat;
            CurrentSaturates.GetComponent<FillScript>().MaxAmount = currentLevel.MaxSaturates;
            CurrentSalt.GetComponent<FillScript>().MaxAmount = currentLevel.MaxSalt;
            CurrentSugar.GetComponent<FillScript>().MaxAmount = currentLevel.MaxSugar;
            CaloriesBar.GetComponent<CaloriesFill>().MaxAmount = currentLevel.CaloriesObjective;
            PotentialFat.GetComponent<FillScript>().MaxAmount = currentLevel.MaxFat;
            PotentialSaturates.GetComponent<FillScript>().MaxAmount = currentLevel.MaxSaturates;
            PotentialSalt.GetComponent<FillScript>().MaxAmount = currentLevel.MaxSalt;
            PotentialSugar.GetComponent<FillScript>().MaxAmount = currentLevel.MaxSugar;
            PotentialCalories.GetComponent<CaloriesFill>().MaxAmount = currentLevel.CaloriesObjective;

            canvas.enabled = false;
            LevelSelectionPanel.SetActive(false);
            MainPanel.SetActive(false);

            if (!tutorialEnabled)
            {
                Host.GetComponent<Host>().Show();
                timeRunning = true;
                //timer.Start();
                Timer = StartCoroutine(CustomTimer.Timer(1, () =>
                {

                    TimeLeft = TimeLeft - TimeSpan.FromSeconds(1);

                    if (TimeLeft.TotalSeconds == 0)
                    {
                        timeRunning = false;
                        StopCoroutine(Timer);
                        //timer.Stop();
                        GameOver("You starved!");
                        StarveImage.SetActive(true);
                    }

                }));

                foodChoices.SetActive(true);
                GetNextFood();
            }
            else
            {
                Tutorial.GetComponent<TutorialScript>().Show();
            }
        }
        else
        {
            transparentPlane.SetActive(true);
            transparentPlane.GetComponent<TransparentPlane>().Show();
            MainPanel.SetActive(false);
            LevelSelectionPanel.SetActive(true);
        }
    }

    private void Spheres_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if(((ObservableCollection<Sphere>)sender).Count == 0) 
        {
            if (tutorialEnabled)
            {
                tutorialNumberofRoundsPlayed++;

                if(tutorialNumberofRoundsPlayed >= 3 && CaloriesBar.GetComponent<CaloriesFill>().currentAmount > CaloriesBar.GetComponent<CaloriesFill>().MaxAmount * 0.25)
                {
                    Tutorial.GetComponent<TutorialScript>().ShowWithText("Doing good, so let's enabled the timer! Fill the calories bar before the time runs out!");
                    pausedForTimer = true;
                    tutorialEnabled = false;
                }
                else
                {
                    if (!caloriesFull && !gameOver)
                    {
                        if (Touches.All(x => x.Value == 1) && !anyDownTheVortex)
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
                        GetNextFood();
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
            else
            {
                if (!caloriesFull && !gameOver)
                {
                    if (Touches.All(x => x.Value == 1) && !anyDownTheVortex)
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
                    GetNextFood();
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
    }

    private void FinishLevel()
    {
        StopCoroutine(Timer);
        Music.Pause();
        SoundEffects.GetComponent<SoundEffects>().PlayWin();
        canvas.enabled = true;
        MainPanel.SetActive(false);
        LostPanel.SetActive(false);
        transparentPlane.SetActive(true);
        transparentPlane.GetComponent<TransparentPlane>().Show();
        LevelCompletePanel.SetActive(true);
        SoundEffects.GetComponent<SoundEffects>().PlayWin();
        var grade = CalculateGrade();
        GradeText.GetComponent<TextMeshProUGUI>().text = grade.ToString();

        if (currentLevel != null) 
        {
            var reward = currentLevel.Rewards[grade];
            var food = Constants.FoodsDatabase.FirstOrDefault(x => x.Id == reward.FoodId);

            var image = Resources.Load<Texture2D>(food.FileName);
            Reward.GetComponent<Image>().sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), new Vector2(0.5f, 0.5f));
            rewardQuantity.GetComponent<TextMeshProUGUI>().text = $"x{reward.Quantity.ToString()}" ;

            if (!PlayerData.PlayerGlobalFoodItems.ContainsKey(reward.FoodId))
            {
                PlayerData.PlayerGlobalFoodItems.Add(reward.FoodId, reward.Quantity);
            }
            else
            {
                PlayerData.PlayerGlobalFoodItems[reward.FoodId] += reward.Quantity;
            }

            PlayerData.LevelsUnlocked.Add(currentLevel.LevelID + 1);

            for (int index = 0; index < reward.Quantity; index++)
            {
                if (PlayerData.FoodDeck.Count < Constants.MAX_DECK_SIZE)
                {
                    PlayerData.FoodDeck.Add(food.Clone());
                }
            }
        }
                
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

//#if !UNITY_WEBGL


//        // Calculate the target width based on the screen width and 16:9 aspect ratio
//        int targetWidth = Screen.height * 9 / 16;

//        // Set the game's resolution to match the target width and height
//        Screen.SetResolution(targetWidth, Screen.height, true);

//#endif

        if (tutorialEnabled !&& !showedBallsMessage)
        {
            if (pausedBalls)
            {
                foreach (var sphere in Spheres)
                {
                    sphere.GetComponent<Rigidbody>().useGravity = false;
                    sphere.GetComponent<Sphere>().PauseRotation();
                }
            }
            else
            {
                foreach (var sphere in Spheres)
                {
                    if (!sphere.wasConsumed && !sphere.isPicked)
                    {
                        sphere.GetComponent<Rigidbody>().useGravity = true;
                        sphere.GetComponent<Sphere>().ResumeRotation();
                    }
                }
            }
        }

        if (gameOver && canvas.enabled == false)
        {
            transparentPlane.SetActive(true);
            transparentPlane.GetComponent<TransparentPlane>().Show();
            status.SetActive(false);
            StopCoroutine(Timer);
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
            transparentPlane.SetActive(true);
            transparentPlane.GetComponent<TransparentPlane>().Show();
            LostPanel.SetActive(true);
            LevelCompletePanel.SetActive(false);
        }

        timeText.GetComponent<TextMeshPro>().text = $"{(int)TimeLeft.TotalMinutes}:{TimeLeft.Seconds:00}";

        if (gameCamera != null && !canvas.enabled)
        {
            if (Input.GetMouseButtonDown(0) && ((tutorialEnabled && !pausedBalls) || !tutorialEnabled))
            {
                selectedRigidBody = GetRigidbodyFromMouseClick();

                if(selectedRigidBody != null && selectedRigidBody.GetComponent<Sphere>() != null) 
                {
                    Cursor.visible = false;
                    var sphere = selectedRigidBody.GetComponent<Sphere>();
                    sphere.PauseRotation();
                    sphere.SetPicked();           
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
                            if (SickBarPotential.GetComponent<SickFill>().currentAmount == 0)
                            {
                                SickBarPotential.GetComponent<SickFill>().Simulate(SickBar.GetComponent<SickFill>().currentAmount + food.NutritionElements[NutritionElementsEnum.Fat]);
                            }
                            else
                            {
                                SickBarPotential.GetComponent<SickFill>().Simulate(food.NutritionElements[NutritionElementsEnum.Fat]);
                            }

                        }
                        
                        var canAbsorbSaturates = PotentialSaturates.GetComponent<FillScript>().Simulate(CurrentSaturates.GetComponent<FillScript>().currentAmount + food.NutritionElements[NutritionElementsEnum.Saturates]);
                        if (!canAbsorbSaturates)
                        {
                          
                            MakeTextGreenAndBold(SaturatesAmountText.GetComponent<TextMeshPro>());

                            if (SickBarPotential.GetComponent<SickFill>().currentAmount == 0)
                            {
                                SickBarPotential.GetComponent<SickFill>().Simulate(SickBar.GetComponent<SickFill>().currentAmount + food.NutritionElements[NutritionElementsEnum.Saturates]);
                            }
                            else
                            {
                                SickBarPotential.GetComponent<SickFill>().Simulate(food.NutritionElements[NutritionElementsEnum.Saturates]);
                            }
                        }

                        var canAbsorbSalt = PotentialSalt.GetComponent<FillScript>().Simulate(CurrentSalt.GetComponent<FillScript>().currentAmount + food.NutritionElements[NutritionElementsEnum.Salt]);
                        if (!canAbsorbSalt)
                        {
                            MakeTextGreenAndBold(SaltAmountText.GetComponent<TextMeshPro>());


                            if (SickBarPotential.GetComponent<SickFill>().currentAmount == 0)
                            {
                                SickBarPotential.GetComponent<SickFill>().Simulate(SickBar.GetComponent<SickFill>().currentAmount + food.NutritionElements[NutritionElementsEnum.Salt]);
                            }
                            else
                            {
                                SickBarPotential.GetComponent<SickFill>().Simulate(food.NutritionElements[NutritionElementsEnum.Salt]);
                            }
                        }

                        var canAbsorbSugar = PotentialSugar.GetComponent<FillScript>().Simulate(CurrentSugar.GetComponent<FillScript>().currentAmount + food.NutritionElements[NutritionElementsEnum.Sugar]);
                        if (!canAbsorbSugar)
                        {
                            MakeTextGreenAndBold(SugarAmountText.GetComponent<TextMeshPro>());
                            if (SickBarPotential.GetComponent<SickFill>().currentAmount == 0)
                            {
                                SickBarPotential.GetComponent<SickFill>().Simulate(SickBar.GetComponent<SickFill>().currentAmount + food.NutritionElements[NutritionElementsEnum.Sugar]);
                            }
                            else
                            {
                                SickBarPotential.GetComponent<SickFill>().Simulate(food.NutritionElements[NutritionElementsEnum.Sugar]);
                            }
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
                        SickBarPotential.GetComponent<SickFill>().Reset();
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

    private void GetNextFood()
    {
        if(CurrentShuffledDeck.Count >= 6)
        {
            foreach (GameObject foodBubble in foodBubbles)
            {
                var nextFood = CurrentShuffledDeck.First();
                CurrentShuffledDeck.RemoveAt(0);

                foodBubble.GetComponent<FoodBubble>().SetFood(nextFood);
            }
        }
        else
        {
            ShuffleDeck();
            foreach (GameObject foodBubble in foodBubbles)
            {
                var nextFood = CurrentShuffledDeck.First();
                CurrentShuffledDeck.RemoveAt(0);

                foodBubble.GetComponent<FoodBubble>().SetFood(nextFood);
            }
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

        if (canSelectFood || !tutorialEnabled)
        {
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

                    if(tutorialEnabled && !tutorialFoodSelected)
                    {
                        Tutorial.GetComponent<TutorialScript>().ResumeTutorial();
                        tutorialFoodSelected = true;
                    }
                }

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

        if (absorbed && tutorialEnabled && !absorbedWellDone && Spheres.Count > 1)
        {
            Tutorial.GetComponent<TutorialScript>().ShowWithText("Well done! You've got it, that's it! Keep it up!");
            absorbedWellDone = true;
            pausedBalls = true;
            VisualFunnel.GetComponent<Funnel>().PauseRotation();
        }

        if (!absorbed && tutorialEnabled && !downTheVortexMessage && Spheres.Count > 1)
        {
            Tutorial.GetComponent<TutorialScript>().ShowWithText("Balls down the vortex increase the sick bar and lead to Game Over!");
            downTheVortexMessage = true;
            pausedBalls = true;
            VisualFunnel.GetComponent<Funnel>().PauseRotation();
        }

        if(!absorbed)
        {
            this.anyDownTheVortex = true;
        }

        if(selectedRigidBody == sphere.GetComponent<Rigidbody>())
        {
            selectedRigidBody = null;
            Cursor.visible = true;
        }

        Spheres.Remove(sphere);

    }

    public void OpenLevelSelection()
    {
        transparentPlane.SetActive(true);
        transparentPlane.GetComponent<TransparentPlane>().Show();
        MainPanel.SetActive(false);
        LevelSelectionPanel.SetActive(true);
    }

    public void EditFoodDeck()
    {
        transparentPlane.SetActive(true);
        transparentPlane.GetComponent<TransparentPlane>().Show();
        MainPanel.SetActive(false);
        EditFoodPanel.SetActive(true);
    }

    public void CancelEditFoodDeck()
    {
        transparentPlane.SetActive(true);
        transparentPlane.GetComponent<TransparentPlane>().Show();
        EditFoodPanel.SetActive(false);
        MainPanel.SetActive(true);
    }

    public void BackToMenu()
    {
        CurrentLevelPanel.SetActive(false);
        checkForTutorialToggle = true;
        transparentPlane.SetActive(true);
        transparentPlane.GetComponent<TransparentPlane>().Show();
        EditFoodPanel.SetActive(false);
        LevelSelectionPanel.SetActive(false);
        LevelCompletePanel.SetActive(false);
        LostPanel.SetActive(false);
        MainPanel.SetActive(true);
    }



}
