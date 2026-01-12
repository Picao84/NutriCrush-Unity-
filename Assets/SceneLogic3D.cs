using Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.Rendering;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.LowLevel;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Utils;
using StateMachine = Assets.StateMachine;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class SceneLogic3D : MonoBehaviour
{
    Rigidbody selectedRigidBody;
    Vector3 originalScreenTargetPosition;
    GameObject[] foodBubbles = new GameObject[6];
    ObservableCollection<Sphere> Spheres = new ObservableCollection<Sphere>();
    GameObject transparentPlane;
    bool hostDelayed;
    Camera gameCamera;
    public GameObject status;
    public GameObject FoodNameText;
    public GameObject CaloriesText;
    public GameObject FatAmountText;
    public GameObject SaturatesAmountText;
    public GameObject SaltAmountText;
    public GameObject SugarAmountText;
    public GameObject EffectsText;
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
    public GameObject CaloriesLevel;
    public GameObject SickBar;
    public GameObject SickBarPotential;
    public GameObject CaloriesSickArea;
    bool selectedHover;
    public GameObject CurrentLevelPanel;
    GameObject selectedFoodOver;
    Vector3 selectedFoodOverOriginalScale;
    Color32 sickColor = new Color32(144, 163, 78, 255);
    public Canvas canvas;
    public GameObject MainPanel;
    public GameObject LostPanel;
    public GameObject EditFoodPanel;
    public GameObject LevelCompletePanel;
    public GameObject LevelSelectionPanel;
    public GameObject SkipAndShuffle;
    public GameObject PlayNextLevelButton;
    public GameObject Reward;
    public GameObject Star1;
    public GameObject Star2;
    public GameObject Star3;
    bool caloriesFull = false;
    public AudioSource SoundEffects;
    public AudioSource Music;
    public GameObject foodImage;
    public GameObject rewardQuantity;
    Vector3 foodImageOriginalPosition;
    public GameObject Host;
    TimeSpan TimeLeft = TimeSpan.FromMinutes(3);
    public GameObject TimeText;
    public GameObject GameOverText;
    bool gameOver;
    string gameOverText;
    public GameObject foodChoices;
    public GameObject Flawless;
    public GameObject SickImage;
    public GameObject StarveImage;
    public GameObject Tutorial;
    public StateMachine state = StateMachine.Tutorial;
    public bool pausedBalls = false;
    bool canSelectFood = true;
    bool absorbedWellDone;
    bool downTheVortexMessage;
    bool tutorialFoodSelected;
    int tutorialNumberofRoundsPlayed;
    bool pausedForTimer;
    bool timeRunning;
    public Level CurrentLevel { get; private set; }
    TextMeshPro caloriesLevel;
    public GameObject VisualFunnel;
    public Dictionary<Sphere, int> Touches = new Dictionary<Sphere, int>();
    bool anyDownTheVortex = false;
    Coroutine Timer;
    public List<Food> CurrentShuffledDeck = new List<Food>();
    bool checkForTutorialToggle = true;
    public DataService dataService;
    public Dictionary<FoodEffects, int> ActiveEffects = new Dictionary<FoodEffects, int>();
    FoodEffects? CurrentAppliedEffect = null;
    bool firstGameSet;

    public Camera GetCamera()
    {
        return gameCamera;
    }
    private void TestDatabaseUpdate()
    {
        //Constants.PlayerData.PlayerFood.Add(new PlayerFood() { FoodId = 10, FoodTotal = 2, FoodOnDeck = 2 });
        //dataService.AddPlayerFood(new PlayerFood() { FoodId = 10, FoodTotal = 2, FoodOnDeck = 2 });

    }
    // Start is called before the first frame update
    void Start()
    {


#if !UNITY_WEBGL

        // Calculate the target width based on the screen width and 16:9 aspect ratio
        int targetHeight = Screen.width * 16 / 9;


        // Set the game's resolution to match the target width and height
        Screen.SetResolution(Screen.width, targetHeight, true);

#endif

        gameCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        foodBubbles[0] = GameObject.Find("FoodOne");
        foodBubbles[1] = GameObject.Find("FoodTwo");
        foodBubbles[2] = GameObject.Find("FoodThree");
        foodBubbles[3] = GameObject.Find("FoodFour");
        foodBubbles[4] = GameObject.Find("FoodFive");
        foodBubbles[5] = GameObject.Find("FoodSix");
        transparentPlane = GameObject.FindGameObjectWithTag("TransparentPlane");
        Spheres.CollectionChanged += Spheres_CollectionChanged;
        foodImageOriginalPosition = foodImage.transform.position;

        SickBar.GetComponent<SickFill>().SickBarFilled += SceneLogic3D_SickBarFilled;
        CaloriesBar.GetComponent<CaloriesFill>().CaloriesBarFilled += SceneLogic3D_CaloriesBarFilled;

        FoodNameText.GetComponent<TextMeshPro>().OnPreRenderText += SceneLogic3D_OnPreRenderText;
        foodChoices.SetActive(false);
        EnhancedTouchSupport.Enable();

        caloriesLevel = CaloriesLevel.GetComponent<TextMeshPro>();

        dataService = new DataService("existing.db");

        var foodDatabase = dataService.GetFoods();
        Constants.FoodsDatabase = foodDatabase.ToList();

        var playerfoodDatabase = dataService.GetPlayerFood();
        Constants.PlayerData.PlayerFood = playerfoodDatabase.ToList();
        Constants.PlayerData.InitialiseFoodDeck();

        var levelDataBase = dataService.GetLevels();
        Constants.Levels = levelDataBase.ToList();

        var sectionsDatabase = dataService.GetSections();
        Constants.Sections = sectionsDatabase.ToList();

        //TestDatabaseUpdate();
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
        Host.GetComponent<Host>().Hide();
        gameOver = true;
        gameOverText = text;
    }

    public void PlayNextLevel()
    {
        PlayLevel(Constants.Levels[CurrentLevel.Id]);
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
        VisualFunnel.GetComponent<Funnel>().ResetSpeed();

        SkipAndShuffle.GetComponent<SkipShuffle>().Reset();

        if (CurrentLevel.Id == 4 && !Constants.Levels[4].Unlocked)
        {
            Tutorial.GetComponent<TutorialScript>().ShowWithTextGroup(new List<string> { "Congratulations on reaching the Toddler tier!", "As a reward, you can now skip meals and refresh food!" }, 3);
            pausedBalls = true;
            hostDelayed = true;
        }

        if (CurrentLevel.Id == 7 && !Constants.Levels[7].Unlocked)
        {
            Tutorial.GetComponent<TutorialScript>().ShowWithTextGroup(new List<string> { "Congratulations on reaching the Child tier!", "Food now expires after some time and desintegrates..." }, 3);
            pausedBalls = true;
            hostDelayed = true;
        }

        if (!hostDelayed)
        {
            Host.GetComponent<Host>().Show();
        }
        gameOver = false;
        Music.Play();
        TimeLeft = TimeSpan.FromSeconds(CurrentLevel.Time);
        caloriesFull = false;
        ShuffleDeck();
        StartupFood();

        if(CurrentLevel.ChangeFood == 1)
        {
            SkipAndShuffle.SetActive(true);
        }
        else
        {
            SkipAndShuffle.SetActive(false);
        }
      
        foodChoices.SetActive(true);
        foreach (GameObject foodBubble in foodBubbles)
        {
            foodBubble.GetComponent<FoodBubble>().Show();
        }
     
        CurrentFat.GetComponent<FillScript>().Reset(!firstGameSet);
        CurrentSaturates.GetComponent<FillScript>().Reset(!firstGameSet);
        CurrentSalt.GetComponent<FillScript>().Reset(!firstGameSet);
        CurrentSugar.GetComponent<FillScript>().Reset(!firstGameSet);
        CaloriesBar.GetComponent<CaloriesFill>().Reset(!firstGameSet);
        SickBar.GetComponent<SickFill>().Reset(!firstGameSet);

        if (!firstGameSet)
        {
            firstGameSet = true;
        }

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
        ActiveEffects.Clear();
    }

    private GradesEnum CalculateGrade()
    {
        var fatScript = CurrentFat.GetComponent<FillScript>();
        var fatRatio = fatScript.currentAmount / fatScript.MaxAmount;
        var saturatesScript = CurrentSaturates.GetComponent<FillScript>();
        var saturatesRatio = saturatesScript.currentAmount / saturatesScript.MaxAmount;
        var saltScript = CurrentSalt.GetComponent<FillScript>();
        var saltRatio = saltScript.currentAmount / saltScript.MaxAmount;
        var sugarScript = CurrentSugar.GetComponent<FillScript>();
        var sugarRatio = sugarScript.currentAmount / sugarScript.MaxAmount;

        var average = (fatRatio + saturatesRatio + saltRatio + sugarRatio) / 4;

        GradesEnum result = average switch
        {
            > 0.90f => GradesEnum.A,
            > 0.75f => GradesEnum.B,
            _ => GradesEnum.C,
        };

        return result;

    }

    public void ContinueTutorial()
    {
        foreach (var sphere in Spheres)
        {
            if (!sphere.wasConsumed && !sphere.isPicked)
            {
                sphere.GetComponent<Rigidbody>().useGravity = true;
                sphere.GetComponent<Sphere>().ResumeRotation();
            }
        }

        pausedBalls = false;

        if (hostDelayed)
        {
            Host.GetComponent<Host>().Show();
            hostDelayed = false;
        }

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
            StartupFood();
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
            StartupFood();
            canSelectFood = false;
        }

        if (step == 5)
        {
            canSelectFood = true;
        }

        if (step == 6)
        {
            pausedBalls = true;
            foreach (var sphere in Spheres)
            {
                sphere.gameObject.GetComponent<Rigidbody>().useGravity = false;
                sphere.PauseRotation();
            }
            VisualFunnel.GetComponent<Funnel>().PauseRotation();
        }

        if (step == 9)
        {
            foreach (var sphere in Spheres)
            {
                if (!sphere.wasConsumed && !sphere.isPicked)
                {
                    sphere.gameObject.GetComponent<Rigidbody>().useGravity = true;
                    sphere.ResumeRotation();
                }
            }
            pausedBalls = false;
            VisualFunnel.GetComponent<Funnel>().ResumeRotation();
        }
    }

    private void ShuffleDeck()
    {
        CurrentShuffledDeck.Clear();

        var playerDeck = Constants.PlayerData.FoodDeck.ToList();

        while (playerDeck.Count > 0)
        {
            var nextFood = playerDeck[UnityEngine.Random.Range(0, playerDeck.Count)];
            CurrentShuffledDeck.Add(nextFood);
            playerDeck.Remove(nextFood);
        }

    }

    public void PlayLevel(Level level)
    {
        CaloriesSickArea.SetActive(true);
        TimeText = GameObject.FindGameObjectWithTag("Time");
        TimeText.GetComponent<TextMeshPro>().text = $"{(int)TimeLeft.TotalMinutes}:{TimeLeft.Seconds:00}";


        CurrentLevel = level;
        caloriesLevel.text = $"0/{CurrentLevel.CaloriesObjective}";
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
        state = StateMachine.NormalPlay;
        checkForTutorialToggle = false;
        LevelSelectionPanel.SetActive(false);
        CurrentLevelPanel.SetActive(true);
        CurrentLevelPanel.GetComponent<CurrentLevelPanelScript>().SetCurrentLevel(CurrentLevel);

        SickBar.GetComponent<SickFill>().MaxAmount = (level.MaxFat + level.MaxSaturates + level.MaxSalt + level.MaxSaturates) / 2;
        SickBarPotential.GetComponent<SickFill>().MaxAmount = (CurrentLevel.MaxFat + CurrentLevel.MaxSaturates + CurrentLevel.MaxSalt + CurrentLevel.MaxSaturates) / 2;


        Reset();
    }

    public async void StartGame()
    {
        state = GameObject.Find("Toggle").GetComponent<Toggle>().isOn ? StateMachine.Tutorial : StateMachine.NormalPlay;

        if (Constants.Levels.Count(x => x.Unlocked) == 1 || state == StateMachine.Tutorial)
        {
            gameOver = false;
            LostPanel.SetActive(false);
           
            CaloriesSickArea.SetActive(true);

            TimeText.GetComponent<TextMeshPro>().text = $"{(int)TimeLeft.TotalMinutes}:{TimeLeft.Seconds:00}";
            
            ShuffleDeck();

            /*if (checkForTutorialToggle)
            {
                state = GameObject.Find("Toggle").GetComponent<Toggle>().isOn ? StateMachine.Tutorial : StateMachine.NormalPlay;
            }*/

            SkipAndShuffle.SetActive(false);
            CurrentLevel = Constants.Levels[0];


            TimeLeft = TimeSpan.FromSeconds(CurrentLevel.Time);
            caloriesLevel.text = $"0/{CurrentLevel.CaloriesObjective}";
            CurrentLevelPanel.SetActive(true);
            CurrentLevelPanel.GetComponent<CurrentLevelPanelScript>().SetCurrentLevel(CurrentLevel);

            CurrentFat.GetComponent<FillScript>().Reset(!firstGameSet);
            CurrentSaturates.GetComponent<FillScript>().Reset(!firstGameSet);
            CurrentSalt.GetComponent<FillScript>().Reset(!firstGameSet);
            CurrentSugar.GetComponent<FillScript>().Reset(!firstGameSet);
            CaloriesBar.GetComponent<CaloriesFill>().Reset(!firstGameSet);
            SickBar.GetComponent<SickFill>().Reset(!firstGameSet);

            firstGameSet = true;

            CurrentFat.GetComponent<FillScript>().MaxAmount = CurrentLevel.MaxFat;

            CurrentSaturates.GetComponent<FillScript>().MaxAmount = CurrentLevel.MaxSaturates;
          
            CurrentSalt.GetComponent<FillScript>().MaxAmount = CurrentLevel.MaxSalt;
           
            CurrentSugar.GetComponent<FillScript>().MaxAmount = CurrentLevel.MaxSugar;
       
            CaloriesBar.GetComponent<CaloriesFill>().MaxAmount = CurrentLevel.CaloriesObjective;
          
            PotentialFat.GetComponent<FillScript>().MaxAmount = CurrentLevel.MaxFat;
            PotentialSaturates.GetComponent<FillScript>().MaxAmount = CurrentLevel.MaxSaturates;
            PotentialSalt.GetComponent<FillScript>().MaxAmount = CurrentLevel.MaxSalt;
            PotentialSugar.GetComponent<FillScript>().MaxAmount = CurrentLevel.MaxSugar;
            PotentialCalories.GetComponent<CaloriesFill>().MaxAmount = CurrentLevel.CaloriesObjective;

            SickBar.GetComponent<SickFill>().MaxAmount = (CurrentLevel.MaxFat + CurrentLevel.MaxSaturates + CurrentLevel.MaxSalt + CurrentLevel.MaxSaturates) / 2;
            SickBarPotential.GetComponent<SickFill>().MaxAmount = (CurrentLevel.MaxFat + CurrentLevel.MaxSaturates + CurrentLevel.MaxSalt + CurrentLevel.MaxSaturates) / 2;

            canvas.enabled = false;
            LevelSelectionPanel.SetActive(false);
            MainPanel.SetActive(false);
            LostPanel.SetActive(false);

            if (state == StateMachine.NormalPlay)
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

                if (CurrentLevel.ChangeFood == 1)
                {
                    SkipAndShuffle.SetActive(true);
                }
                foodChoices.SetActive(true);
                StartupFood();
                foreach (GameObject foodBubble in foodBubbles)
                {
                    foodBubble.GetComponent<FoodBubble>().Show();

                    await AsyncTask.Await(100);
                }
            }
            else
            {
                Tutorial.GetComponent<TutorialScript>().Show();
            }
        }
        else
        {
            LostPanel.SetActive(false);
            transparentPlane.SetActive(true);
            transparentPlane.GetComponent<TransparentPlane>().Show();
            MainPanel.SetActive(false);
            LevelSelectionPanel.SetActive(true);
        }
    }

    private void ReduceEffects()
    {
        if (ActiveEffects.Any())
        {
            if(!ActiveEffects.Any(x => x.Key == FoodEffects.SpeedUpGame))
            {
                VisualFunnel.GetComponent<Funnel>().ResetSpeed();
            }

            var newActiveEffects = new Dictionary<FoodEffects, int>();

            foreach (var effect in ActiveEffects)
            {
                if (CurrentAppliedEffect != null && CurrentAppliedEffect == effect.Key)
                {
                    newActiveEffects.Add(effect.Key, effect.Value);
                    CurrentAppliedEffect = null;
                }
                else
                {
                    if (effect.Value > 1)
                    {
                        newActiveEffects.Add(effect.Key, effect.Value - 1);
                    }

                    switch (effect.Key)
                    {
                        case FoodEffects.AccelerateSugar:
                        case FoodEffects.SlowDownSugar:

                            CurrentSugar.GetComponent<FillScript>().TurnPassed();
                            PotentialSugar.GetComponent<FillScript>().TurnPassed();

                            break;

                        case FoodEffects.AccelerateFat:
                        case FoodEffects.SlowDownFat:

                            CurrentFat.GetComponent<FillScript>().TurnPassed();
                            PotentialFat.GetComponent<FillScript>().TurnPassed();

                            break;

                        case FoodEffects.AccelerateSaturates:
                        case FoodEffects.SlowDownSaturates:

                            CurrentSaturates.GetComponent<FillScript>().TurnPassed();
                            PotentialSaturates.GetComponent<FillScript>().TurnPassed();

                            break;

                        case FoodEffects.AccelerateSalt:
                        case FoodEffects.SlowDownSalt:

                            CurrentSalt.GetComponent<FillScript>().TurnPassed();
                            PotentialSalt.GetComponent<FillScript>().TurnPassed();

                            break;

                      

                    }
                }
            }

            

            ActiveEffects = newActiveEffects;
        }
    }

    private async void Spheres_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (((ObservableCollection<Sphere>)sender).Count == 0)
        {
            if (state == StateMachine.Tutorial)
            {
                tutorialNumberofRoundsPlayed++;

                if (tutorialNumberofRoundsPlayed >= 3 && CaloriesBar.GetComponent<CaloriesFill>().currentAmount > CaloriesBar.GetComponent<CaloriesFill>().MaxAmount * 0.25)
                {
                    Tutorial.GetComponent<TutorialScript>().ShowWithTextGroup(new List<string> { "Doing good, so let's enabled the timer! Fill the calories bar before the time runs out!" });
                    pausedForTimer = true;
                    state = StateMachine.NormalPlay;
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
                        foreach (GameObject foodBubble in foodBubbles.Where(x => x.GetComponent<FoodBubble>().Food != null))
                        {                          
                            foodBubble.GetComponent<FoodBubble>().Show(true);
                        }

                        ReduceEffects();

                        await AsyncTask.Await(100);

                        foodBubbles.First(x => x.GetComponent<FoodBubble>().Food == null).GetComponent<FoodBubble>().Show();
                     
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
                    foreach (GameObject foodBubble in foodBubbles.Where(x => x.GetComponent<FoodBubble>().Food != null))
                    {
                        foodBubble.GetComponent<FoodBubble>().Show(true);
                    }

                    ReduceEffects();

                    await AsyncTask.Await(100);

                  


                    if (CurrentLevel.ChangeFood == 1)
                    {
                        SkipAndShuffle.SetActive(true);
                    }

                    GetNextFood();

                    foreach (GameObject foodBubble in foodBubbles)
                    {
                        foodBubble.GetComponent<FoodBubble>().Show();

                        await AsyncTask.Await(100);
                    }


                    SkipAndShuffle.GetComponent<SkipShuffle>().ReduceCooldown();
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
      
        List<GameObject> stars = new List<GameObject>
        {
            Star3,
            Star2,
            Star1
        };

        var fullstar = Resources.Load<Texture2D>("fullstar");
        var emptystar = Resources.Load<Texture2D>("emptystar");

        for (int i = 3; i > 0; i--)
        {
            if ((int)grade <= i)
            {
                stars[i - 1].GetComponent<Image>().sprite = Sprite.Create(fullstar, new Rect(0, 0, fullstar.width, fullstar.height), new Vector2(0.5f, 0.5f));
            }
            else
            {
                stars[i - 1].GetComponent<Image>().sprite = Sprite.Create(emptystar, new Rect(0, 0, emptystar.width, emptystar.height), new Vector2(0.5f, 0.5f));
            }

        }


        if (CurrentLevel != null)
        {
            var reward = CurrentLevel.Rewards[grade];
            var food = Constants.FoodsDatabase.FirstOrDefault(x => x.Id == reward.FoodId);

            var image = Resources.Load<Texture2D>(food.FileName);
            Reward.GetComponent<Image>().sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), new Vector2(0.5f, 0.5f));
            rewardQuantity.GetComponent<TextMeshProUGUI>().text = $"x{reward.FoodQuantity.ToString()}";

            if (!Constants.PlayerData.PlayerFood.Any(x => x.FoodId == reward.FoodId))
            {
                Constants.PlayerData.PlayerFood.Add(new PlayerFood() { FoodId = reward.FoodId, FoodTotal = reward.FoodQuantity });
                dataService.AddPlayerFood(new PlayerFood() { FoodId = reward.FoodId, FoodTotal = reward.FoodQuantity });
            }
            else
            {
                Constants.PlayerData.PlayerFood.First(x => x.FoodId == reward.FoodId).FoodTotal += reward.FoodQuantity;
            }

            dataService.StorePlayerFood(Constants.PlayerData.PlayerFood);
            
            dataService.StoreUnlockedLevel(CurrentLevel.Id + 1);

            if(CurrentLevel.Id % 3 == 0)
            {
                var section = (CurrentLevel.Id / 3);
                var sectionUnlocked = Constants.Sections[section].FoodToUnlock.All(x => Constants.PlayerData.PlayerFood.Any(z => z.FoodId == x.FoodId));

                if (!sectionUnlocked)
                {
                    PlayNextLevelButton.GetComponent<Button>().enabled = false;
                    
                }
            }

           

            var levelDataBase = dataService.GetLevels();
            Constants.Levels = levelDataBase.ToList();

            /*if (CurrentLevel.Id % 3 == 0 && Constants.Sections[(CurrentLevel.Id + 3) / 3].FoodToUnlock.Count == 0 || Constants.Sections[(CurrentLevel.Id + 3) / 3].FoodToUnlock.All(x => Constants.PlayerData.PlayerFood.Any(z => z.FoodId == x.FoodId)))
            {
                dataService.StoreUnlockedSection((CurrentLevel.Id + 3) / 3);
            }*/

            var sectionsDatabase = dataService.GetSections();
            Constants.Sections = sectionsDatabase.ToList();

            for (int index = 0; index < reward.FoodQuantity; index++)
            {
                if (Constants.PlayerData.FoodDeck.Count < Constants.MAX_DECK_SIZE)
                {
                    Constants.PlayerData.FoodDeck.Add(food.Clone());
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
                        sphere.gameObject.GetComponent<Rigidbody>().useGravity = true;
                        //sphere.ResumeRotation();
                    }
                }
            }
        

        if (gameOver && canvas.enabled == false)
        {
            transparentPlane.SetActive(true);
            transparentPlane.GetComponent<TransparentPlane>().Show();
            status.SetActive(false);
            StopCoroutine(Timer);
            foreach (var sphere in Spheres)
            {
                sphere.GetComponent<Rigidbody>().useGravity = false;
                GameObject.Destroy(sphere.transform.root.gameObject);
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

        if (TimeText != null)
        {
            TimeText.GetComponent<TextMeshPro>().text = $"{(int)TimeLeft.TotalMinutes}:{TimeLeft.Seconds:00}";
        }

        if (gameCamera != null && !canvas.enabled)
        {

            if (!pausedBalls)
            {

                var finger = Touch.fingers[0];



                if (finger.isActive)
                {
                    if (finger.currentTouch.valid)
                    {
                        if (selectedRigidBody == null)
                        {
                            var ray = gameCamera.ScreenPointToRay(finger.currentTouch.screenPosition);
                            var allHits = Physics.SphereCastAll(ray, 1);

                            if (allHits.Any(x => x.collider.transform.gameObject.GetComponent<Sphere>() != null && !x.collider.transform.gameObject.GetComponent<Sphere>().IsGhost && !x.collider.transform.gameObject.GetComponent<Sphere>().wasConsumed))
                            {

                                var sphere = allHits.First(x => x.collider.transform.gameObject.GetComponent<Sphere>() != null && !x.collider.transform.gameObject.GetComponent<Sphere>().IsGhost).collider.gameObject.GetComponent<Sphere>();

                                //Touches[sphere.GetComponent<Sphere>()]++;

                                sphere.PauseRotation();

                                sphere.gameObject.GetComponent<Rigidbody>().useGravity = false;

                                sphere.SetPicked();



                                selectedRigidBody = sphere.gameObject.GetComponent<Rigidbody>();
                            }


                            //var screenToWorldPoint = gameCamera.ScreenToWorldPoint(new Vector3(item.screenPosition.x, item.screenPosition.y, gameCamera.nearClipPlane));
                            //var startScreenPositionToWorldPoint = gameCamera.ScreenToWorldPoint(new Vector3(item.startScreenPosition.x, item.startScreenPosition.y, gameCamera.nearClipPlane));

                            //Vector3 positionOffset = screenToWorldPoint - startScreenPositionToWorldPoint;
                            //selectedRigidBody.velocity = new Vector3(psitionOffset.x / Time.deltaTime * 40, psitionOffset.y / Time.deltaTime * 40, psitionOffset.z / Time.deltaTime * 40);
                            //originalScreenTargetPosition = gameCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, gameCamera.nearClipPlane));

                            //sphere.GetComponent<Sphere>().transform.position = new Vector3(sphere.GetComponent<Sphere>().transform.position.x, 1, sphere.GetComponent<Sphere>().transform.position.z);
                            //sphere.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(positionOffset.x / Time.deltaTime * 40, positionOffset.y / Time.deltaTime * 40, positionOffset.z / Time.deltaTime * 40);


                            //originalScreenTargetPosition = gameCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, gameCamera.nearClipPlane));
                            //return sphere.GetComponent<Rigidbody>();
                        }
                    }
                }
                else
                {
                    if (selectedRigidBody != null && selectedRigidBody.GetComponent<Sphere>() != null)
                    {
                        //Cursor.visible = true;
                        var sphere = selectedRigidBody.GetComponent<Sphere>();
                        sphere.isPicked = false;
                        selectedRigidBody.useGravity = true;
                        selectedRigidBody = null;
                    }
                }




                //}


                //}

                //}






                if (Pointer.current.press.wasPressedThisFrame)
                {
                    GetFoodPicked();
                }

            }


        }
    }

    private void FixedUpdate()
    {
        if (selectedRigidBody != null && !pausedBalls)
        {
            var finger = Touch.fingers[0];
            var currentTouch = finger.currentTouch;

            if (finger.currentTouch.valid)
            {

                var distance = Vector2.Distance(finger.currentTouch.screenPosition, finger.currentTouch.startScreenPosition);

                var speed = distance / Time.deltaTime;

                var currentTouchToWorldPoint = gameCamera.ScreenToWorldPoint(new Vector3(currentTouch.screenPosition.x, currentTouch.screenPosition.y, gameCamera.nearClipPlane));
                var lastTouchToWorldPoint = gameCamera.ScreenToWorldPoint(new Vector3(currentTouch.startScreenPosition.x, currentTouch.startScreenPosition.y, gameCamera.nearClipPlane));
                var positionOffset = currentTouchToWorldPoint - lastTouchToWorldPoint;

                selectedRigidBody.velocity = new Vector3(positionOffset.x / Time.deltaTime * (speed / 15000), positionOffset.y / Time.deltaTime * (speed / 15000), positionOffset.z / Time.deltaTime * (speed / 15000));
                //selectedRigidBody.velocity = new Vector3(positionOffset.x / Time.deltaTime, positionOffset.y / Time.deltaTime, positionOffset.z / Time.deltaTime);

            }

            /*var screenToWorldPoint = gameCamera.ScreenToWorldPoint(new Vector3(Pointer.current.position.value.x, Pointer.current.position.value.y, gameCamera.nearClipPlane));
            Vector3 mousePositionOffset = new Vector3(screenToWorldPoint.x, screenToWorldPoint.y, screenToWorldPoint.z) - originalScreenTargetPosition;
            selectedRigidBody.velocity = new Vector3(mousePositionOffset.x / Time.deltaTime * 40, mousePositionOffset.y / Time.deltaTime * 40, mousePositionOffset.z / Time.deltaTime * 40);
            originalScreenTargetPosition = gameCamera.ScreenToWorldPoint(new Vector3(Pointer.current.position.value.x, Pointer.current.position.value.y, gameCamera.nearClipPlane));*/
        }
    }

    private void GetNextFood()
    {
        if (CurrentShuffledDeck.Count >= 6)
        {
            foreach (GameObject foodBubble in foodBubbles.Where(x => x.GetComponent<FoodBubble>().Food == null))
            {
                var nextFood = CurrentShuffledDeck.First();
                CurrentShuffledDeck.RemoveAt(0);

                foodBubble.GetComponent<FoodBubble>().SetFood(nextFood, CurrentLevel.FoodExpires == 1);
            }

        }
        else
        {
            ShuffleDeck();
            foreach (GameObject foodBubble in foodBubbles.Where(x => x.GetComponent<FoodBubble>().Food == null))
            {
                var nextFood = CurrentShuffledDeck.First();
                CurrentShuffledDeck.RemoveAt(0);

                foodBubble.GetComponent<FoodBubble>().SetFood(nextFood, CurrentLevel.FoodExpires == 1);
            }

        }


}

    private void StartupFood()
    {
       
            foreach (GameObject foodBubble in foodBubbles)
            {
                var nextFood = CurrentShuffledDeck.First();
                CurrentShuffledDeck.RemoveAt(0);

                foodBubble.GetComponent<FoodBubble>().SetFood(nextFood, CurrentLevel.FoodExpires == 1);
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


private async void GetFoodPicked()
{
    var ray = gameCamera.ScreenPointToRay(Pointer.current.position.value);

    var allHits = Physics.RaycastAll(ray);

   
        if(state == StateMachine.Tutorial)
        {
            if(allHits.Any(x => x.collider.transform.gameObject.name == "SpeechBallon"))
            {
                Tutorial.GetComponent<TutorialScript>().SkipPart();
            }
        }

        if (canSelectFood || state == StateMachine.NormalPlay)
        {

            if (allHits.Any(x => x.collider.transform.gameObject.GetComponent<FoodBubble>() != null))
            {
                var food = allHits.First(x => x.collider.transform.gameObject.GetComponent<FoodBubble>() != null).collider.transform.gameObject.GetComponent<FoodBubble>();
                if (food.Food != null)
                {
                    if (selectedFoodOver == null || selectedFoodOver != food.gameObject)
                    {
                        if (selectedFoodOver != null)
                        {
                            selectedFoodOver.transform.localScale = selectedFoodOverOriginalScale;
                        }
                        foodImage.transform.position = foodImageOriginalPosition;

                        PotentialFat.GetComponent<FillScript>().Reset(false);
                        ResetTextStyle(FatAmountText.GetComponent<TextMeshPro>());
                        PotentialSaturates.GetComponent<FillScript>().Reset(false);
                        ResetTextStyle(SaturatesAmountText.GetComponent<TextMeshPro>());
                        PotentialSugar.GetComponent<FillScript>().Reset(false);
                        ResetTextStyle(SugarAmountText.GetComponent<TextMeshPro>());
                        PotentialSalt.GetComponent<FillScript>().Reset(false);
                        ResetTextStyle(SaltAmountText.GetComponent<TextMeshPro>());
                        PotentialCalories.GetComponent<CaloriesFill>().Reset();
                        SickBarPotential.GetComponent<SickFill>().Reset();
                        status.SetActive(false);


                        selectedFoodOver = food.gameObject;


                        selectedFoodOverOriginalScale = selectedFoodOver.transform.localScale;
                        selectedFoodOver.transform.localScale = new Vector3(selectedFoodOverOriginalScale.x * 1.2f, selectedFoodOverOriginalScale.y * 1.2f, selectedFoodOverOriginalScale.z * 1.2f);


                        selectedHover = true;


                        FoodNameText.GetComponent<TextMeshPro>().text = food.Food.Name;

                        if (!string.IsNullOrEmpty(food.Food.FileName))
                        {
                            var image = Resources.Load<Texture2D>(food.Food.FileName);
                            foodImage.GetComponent<SpriteRenderer>().sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), new Vector2(0.5f, 0.5f));
                        }
                        else
                        {
                            foodImage.GetComponent<SpriteRenderer>().sprite = null;
                        }

                        var canAbsorbFat = PotentialFat.GetComponent<FillScript>().Simulate(CurrentFat.GetComponent<FillScript>().currentAmount + food.Food.NutritionElements[NutritionElementsEnum.Fat] * CurrentLevel.Multiplier);
                        if (!canAbsorbFat)
                        {
                            MakeTextGreenAndBold(FatAmountText.GetComponent<TextMeshPro>());
                            if (SickBarPotential.GetComponent<SickFill>().currentAmount == 0)
                            {
                                SickBarPotential.GetComponent<SickFill>().Simulate(SickBar.GetComponent<SickFill>().currentAmount + food.Food.NutritionElements[NutritionElementsEnum.Fat] * CurrentLevel.Multiplier);
                            }
                            else
                            {
                                SickBarPotential.GetComponent<SickFill>().Simulate(food.Food.NutritionElements[NutritionElementsEnum.Fat] * CurrentLevel.Multiplier);
                            }

                        }

                        var canAbsorbSaturates = PotentialSaturates.GetComponent<FillScript>().Simulate(CurrentSaturates.GetComponent<FillScript>().currentAmount + food.Food.NutritionElements[NutritionElementsEnum.Saturates] * CurrentLevel.Multiplier);
                        if (!canAbsorbSaturates)
                        {

                            MakeTextGreenAndBold(SaturatesAmountText.GetComponent<TextMeshPro>());

                            if (SickBarPotential.GetComponent<SickFill>().currentAmount == 0)
                            {
                                SickBarPotential.GetComponent<SickFill>().Simulate(SickBar.GetComponent<SickFill>().currentAmount + food.Food.NutritionElements[NutritionElementsEnum.Saturates] * CurrentLevel.Multiplier);
                            }
                            else
                            {
                                SickBarPotential.GetComponent<SickFill>().Simulate(food.Food.NutritionElements[NutritionElementsEnum.Saturates] * CurrentLevel.Multiplier);
                            }
                        }

                        var canAbsorbSalt = PotentialSalt.GetComponent<FillScript>().Simulate(CurrentSalt.GetComponent<FillScript>().currentAmount + food.Food.NutritionElements[NutritionElementsEnum.Salt] * CurrentLevel.Multiplier);
                        if (!canAbsorbSalt)
                        {
                            MakeTextGreenAndBold(SaltAmountText.GetComponent<TextMeshPro>());


                            if (SickBarPotential.GetComponent<SickFill>().currentAmount == 0)
                            {
                                SickBarPotential.GetComponent<SickFill>().Simulate(SickBar.GetComponent<SickFill>().currentAmount + food.Food.NutritionElements[NutritionElementsEnum.Salt] * CurrentLevel.Multiplier);
                            }
                            else
                            {
                                SickBarPotential.GetComponent<SickFill>().Simulate(food.Food.NutritionElements[NutritionElementsEnum.Salt] * CurrentLevel.Multiplier);
                            }
                        }

                        var canAbsorbSugar = PotentialSugar.GetComponent<FillScript>().Simulate(CurrentSugar.GetComponent<FillScript>().currentAmount + food.Food.NutritionElements[NutritionElementsEnum.Sugar] * CurrentLevel.Multiplier);
                        if (!canAbsorbSugar)
                        {
                            MakeTextGreenAndBold(SugarAmountText.GetComponent<TextMeshPro>());
                            if (SickBarPotential.GetComponent<SickFill>().currentAmount == 0)
                            {
                                SickBarPotential.GetComponent<SickFill>().Simulate(SickBar.GetComponent<SickFill>().currentAmount + food.Food.NutritionElements[NutritionElementsEnum.Sugar] * CurrentLevel.Multiplier);
                            }
                            else
                            {
                                SickBarPotential.GetComponent<SickFill>().Simulate(food.Food.NutritionElements[NutritionElementsEnum.Sugar] * CurrentLevel.Multiplier);
                            }
                        }
                        PotentialCalories.GetComponent<CaloriesFill>().Simulate(CaloriesBar.GetComponent<CaloriesFill>().currentAmount + food.Food.Calories * CurrentLevel.Multiplier);


                        CaloriesText.GetComponent<TextMeshPro>().text = $"{Math.Round(food.Food.Calories * CurrentLevel.Multiplier, 2)}";
                        FatAmountText.GetComponent<TextMeshPro>().text = $"{Math.Round(food.Food.NutritionElements[NutritionElementsEnum.Fat] * CurrentLevel.Multiplier, 2).ToString()}g";
                        SaturatesAmountText.GetComponent<TextMeshPro>().text = $"{(Math.Round(food.Food.NutritionElements[NutritionElementsEnum.Saturates] * CurrentLevel.Multiplier, 2)).ToString()}g";
                        SaltAmountText.GetComponent<TextMeshPro>().text = $"{Math.Round(food.Food.NutritionElements[NutritionElementsEnum.Salt] * CurrentLevel.Multiplier, 2).ToString()}g";
                        SugarAmountText.GetComponent<TextMeshPro>().text = $"{Math.Round(food.Food.NutritionElements[NutritionElementsEnum.Sugar] * CurrentLevel.Multiplier, 2).ToString()}g";

                        if (food.Food.Effect != null)
                        {
                            EffectsText.GetComponent<TextMeshPro>().text = food.Food.Effect.Description;
                        }
                        else
                        {
                            EffectsText.GetComponent<TextMeshPro>().text = string.Empty;
                        }

                        var currentColor = EffectsText.GetComponent<TextMeshPro>().color;

                        if (CurrentLevel.DoubleHalfAbsorption == 1 || CurrentLevel.SpeedUpSlowDown == 1)
                        {
                            EffectsText.GetComponent<TextMeshPro>().color = new Color(currentColor.r, currentColor.g, currentColor.b, 1f);
                        }
                        else
                        {
                            EffectsText.GetComponent<TextMeshPro>().color = new Color(currentColor.r, currentColor.g, currentColor.b, 0.1f);
                        }

                        status.SetActive(true);

                    }
                    else
                    {
                        if (selectedFoodOver != null && selectedFoodOver == food.gameObject)
                        {
                            Flawless.GetComponent<FlawlessScript>().Hide();
                            SoundEffects.GetComponent<SoundEffects>().PlayBubble();
                            CaloriesBar.GetComponent<CaloriesFill>().AddAmount(food.Food.Calories * CurrentLevel.Multiplier);
                            caloriesLevel.text = $"{CaloriesBar.GetComponent<CaloriesFill>().currentAmount}/{CurrentLevel.CaloriesObjective}";
                            ApplyFoodEffect(food);
                            food.FoodChosen();
                          

                            SkipAndShuffle.SetActive(false);


                            transparentPlane.GetComponent<TransparentPlane>().Hide();
                            status.SetActive(false);
                            PotentialFat.GetComponent<FillScript>().Reset(false);
                            ResetTextStyle(FatAmountText.GetComponent<TextMeshPro>());
                            PotentialSaturates.GetComponent<FillScript>().Reset(false);
                            ResetTextStyle(SaturatesAmountText.GetComponent<TextMeshPro>());
                            PotentialSugar.GetComponent<FillScript>().Reset(false);
                            ResetTextStyle(SugarAmountText.GetComponent<TextMeshPro>());
                            PotentialSalt.GetComponent<FillScript>().Reset(false);
                            ResetTextStyle(SaltAmountText.GetComponent<TextMeshPro>());
                            PotentialCalories.GetComponent<CaloriesFill>().Reset();
                            SickBarPotential.GetComponent<SickFill>().Reset();
                            Host.GetComponent<Host>().Hide();


                            if (CurrentLevel.FoodExpires == 1)
                            {
                                foreach (GameObject foodBubble in foodBubbles.Where(x => x != selectedFoodOver))
                                {
                                    if (foodBubble.GetComponent<FoodBubble>().Food != null)
                                    {
                                        if (foodBubble.GetComponent<FoodBubble>().expiresIn == 1)
                                        {
                                            foodBubble.GetComponent<FoodBubble>().FoodSpoiled();
                                        }
                                        else
                                        {
                                            foodBubble.GetComponent<FoodBubble>().ReduceExpiration();
                                        }
                                    }
                                }
                            }

                            selectedFoodOver = null;

                            if (state == StateMachine.Tutorial && !tutorialFoodSelected)
                            {
                                Tutorial.GetComponent<TutorialScript>().ResumeTutorial();
                                tutorialFoodSelected = true;
                            }
                        }
                    }
                }

            }
            else
            {
                if (SkipAndShuffle.GetComponent<SkipShuffle>().CanSKip)
                {

                    if (allHits.Any(x => x.collider.transform.gameObject.name == "SkipAndShuffle"))
                    {
                        SkipAndShuffle.GetComponent<SkipShuffle>().Deactivate();
                        SkipAndShuffle.SetActive(false);
                       

                        foreach (GameObject foodBubble in foodBubbles)
                        {
                            foodBubble.GetComponent<FoodBubble>().FoodSpoiled(false);

                        }

                        //SickBar.GetComponent<SickFill>().AddAmount(SickBar.GetComponent<SickFill>().MaxAmount * 0.05f);

                        await AsyncTask.Await(500);

                        GetNextFood();

                        foreach (GameObject foodBubble in foodBubbles)
                        {
                            foodBubble.GetComponent<FoodBubble>().Show();

                            await AsyncTask.Await(100);
                        }

                        SkipAndShuffle.SetActive(true);


                    }

                    if (selectedFoodOver != null)
                    {
                        selectedFoodOver.transform.localScale = selectedFoodOverOriginalScale;
                        selectedFoodOver = null;
                    }

                    if (selectedHover)
                    {
                        selectedHover = false;
                        foodImage.transform.position = foodImageOriginalPosition;
                        PotentialFat.GetComponent<FillScript>().Reset(false);
                        ResetTextStyle(FatAmountText.GetComponent<TextMeshPro>());
                        PotentialSaturates.GetComponent<FillScript>().Reset(false);
                        ResetTextStyle(SaturatesAmountText.GetComponent<TextMeshPro>());
                        PotentialSugar.GetComponent<FillScript>().Reset(false);
                        ResetTextStyle(SugarAmountText.GetComponent<TextMeshPro>());
                        PotentialSalt.GetComponent<FillScript>().Reset(false);
                        ResetTextStyle(SaltAmountText.GetComponent<TextMeshPro>());
                        PotentialCalories.GetComponent<CaloriesFill>().Reset();
                        SickBarPotential.GetComponent<SickFill>().Reset();
                        status.SetActive(false);
                    }
                }
            }
        }
    }

    public void AddSphere(Sphere sphere)
    {
        Spheres.Add(sphere);
        Touches.Add(sphere, 0);
    }

    private void ApplyFoodEffect(FoodBubble food)
    {
        if(CurrentLevel.DoubleHalfAbsorption == 1) {

            if (food.Food.Effect != null)
            {
                switch ((FoodEffects)food.Food.EffectId)
                {
                    case FoodEffects.AccelerateSugar:

                        if (!ActiveEffects.ContainsKey(FoodEffects.AccelerateSugar) && !ActiveEffects.ContainsKey(FoodEffects.SlowDownSugar))
                        {
                            CurrentAppliedEffect = FoodEffects.AccelerateSugar;
                            ActiveEffects.Add(FoodEffects.AccelerateSugar, food.Food.EffectAmount);
                            CurrentSugar.GetComponent<FillScript>().SetEffect(2, food.Food.EffectAmount);
                            PotentialSugar.GetComponent<FillScript>().SetEffect(2, food.Food.EffectAmount);
                        }

                        break;

                    case FoodEffects.SlowDownSugar:

                        if (!ActiveEffects.ContainsKey(FoodEffects.SlowDownSugar) && !ActiveEffects.ContainsKey(FoodEffects.AccelerateSugar))
                        {
                            CurrentAppliedEffect = FoodEffects.SlowDownSugar;
                            ActiveEffects.Add(FoodEffects.SlowDownSugar, food.Food.EffectAmount);
                            CurrentSugar.GetComponent<FillScript>().SetEffect(0.5f, food.Food.EffectAmount);
                            PotentialSugar.GetComponent<FillScript>().SetEffect(0.5f, food.Food.EffectAmount);
                        }

                        break;

                    case FoodEffects.AccelerateFat:

                        if (!ActiveEffects.ContainsKey(FoodEffects.AccelerateFat) && !ActiveEffects.ContainsKey(FoodEffects.SlowDownFat))
                        {
                            CurrentAppliedEffect = FoodEffects.AccelerateFat;
                            ActiveEffects.Add(FoodEffects.AccelerateFat, food.Food.EffectAmount);
                            CurrentFat.GetComponent<FillScript>().SetEffect(2, food.Food.EffectAmount);
                            PotentialFat.GetComponent<FillScript>().SetEffect(2, food.Food.EffectAmount);
                        }

                        break;

                    case FoodEffects.SlowDownFat:

                        if (!ActiveEffects.ContainsKey(FoodEffects.SlowDownFat) && !ActiveEffects.ContainsKey(FoodEffects.AccelerateFat))
                        {
                            CurrentAppliedEffect = FoodEffects.SlowDownFat;
                            ActiveEffects.Add(FoodEffects.SlowDownFat, food.Food.EffectAmount);
                            CurrentFat.GetComponent<FillScript>().SetEffect(0.5f, food.Food.EffectAmount);
                            PotentialFat.GetComponent<FillScript>().SetEffect(0.5f, food.Food.EffectAmount);
                        }

                        break;

                    case FoodEffects.AccelerateSaturates:

                        if (!ActiveEffects.ContainsKey(FoodEffects.AccelerateSaturates) && !ActiveEffects.ContainsKey(FoodEffects.SlowDownSaturates))
                        {
                            CurrentAppliedEffect = FoodEffects.AccelerateSaturates;

                            ActiveEffects.Add(FoodEffects.AccelerateSaturates, food.Food.EffectAmount);
                            CurrentSaturates.GetComponent<FillScript>().SetEffect(2, food.Food.EffectAmount);
                            PotentialSaturates.GetComponent<FillScript>().SetEffect(2, food.Food.EffectAmount);
                        }

                        break;

                    case FoodEffects.SlowDownSaturates:

                        if (!ActiveEffects.ContainsKey(FoodEffects.SlowDownSaturates) && !ActiveEffects.ContainsKey(FoodEffects.AccelerateSaturates))
                        {
                            CurrentAppliedEffect = FoodEffects.SlowDownSaturates;
                            ActiveEffects.Add(FoodEffects.SlowDownSaturates, food.Food.EffectAmount);
                            CurrentSaturates.GetComponent<FillScript>().SetEffect(0.5f, food.Food.EffectAmount);
                            PotentialSaturates.GetComponent<FillScript>().SetEffect(0.5f, food.Food.EffectAmount);
                        }

                        break;

                    case FoodEffects.AccelerateSalt:

                        if (!ActiveEffects.ContainsKey(FoodEffects.AccelerateSalt) && !ActiveEffects.ContainsKey(FoodEffects.SlowDownSalt))
                        {
                            CurrentAppliedEffect = FoodEffects.AccelerateSalt;
                            ActiveEffects.Add(FoodEffects.AccelerateSalt, food.Food.EffectAmount);
                            CurrentSalt.GetComponent<FillScript>().SetEffect(2, food.Food.EffectAmount);
                            PotentialSalt.GetComponent<FillScript>().SetEffect(2, food.Food.EffectAmount);
                        }

                        break;

                    case FoodEffects.SlowDownSalt:

                        if (!ActiveEffects.ContainsKey(FoodEffects.SlowDownSalt) && !ActiveEffects.ContainsKey(FoodEffects.AccelerateSalt))
                        {
                            CurrentAppliedEffect = FoodEffects.SlowDownSalt;
                            ActiveEffects.Add(FoodEffects.SlowDownSalt, food.Food.EffectAmount);
                            CurrentSalt.GetComponent<FillScript>().SetEffect(0.5f, food.Food.EffectAmount);
                            PotentialSalt.GetComponent<FillScript>().SetEffect(0.5f, food.Food.EffectAmount);
                        }

                        break;

                   

                    default:

                        break;
                }


            }
        }

        if (CurrentLevel.SpeedUpSlowDown == 1)
        {
            if (food.Food.Effect != null)
            {

                switch ((FoodEffects)food.Food.EffectId)
                {
            
                 case FoodEffects.SpeedUpGame:

                    if (!ActiveEffects.ContainsKey(FoodEffects.SpeedUpGame))
                    {
                        CurrentAppliedEffect = FoodEffects.SpeedUpGame;
                        ActiveEffects.Add(FoodEffects.SpeedUpGame, food.Food.EffectAmount);
                        VisualFunnel.GetComponent<Funnel>().SpeedUp();
                    }

                    break;
                }
            }
        
        }
    }

    public void RemoveSphere(Sphere sphere, bool absorbed)
    {

        if (absorbed && state == StateMachine.Tutorial && !absorbedWellDone && Spheres.Count > 1)
        {
            Tutorial.GetComponent<TutorialScript>().ShowWithTextGroup(new List<string> { "Well done! You've got it, that's it! Keep it up!" });
            absorbedWellDone = true;
            pausedBalls = true;
            VisualFunnel.GetComponent<Funnel>().PauseRotation();
        }

        if (!absorbed && state == StateMachine.Tutorial && !downTheVortexMessage && Spheres.Count > 1)
        {
            Tutorial.GetComponent<TutorialScript>().ShowWithTextGroup(new List<string> { "Balls down the vortex increase the sick bar and lead to Game Over!" });
            downTheVortexMessage = true;
            pausedBalls = true;
            VisualFunnel.GetComponent<Funnel>().PauseRotation();
        }

        if(!absorbed)
        {
            this.anyDownTheVortex = true;
        }
        else
        {
            Touches[sphere.GetComponent<Sphere>()] = sphere.GetComponent<Sphere>().numberOfTimesItExitedFunnel;
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
