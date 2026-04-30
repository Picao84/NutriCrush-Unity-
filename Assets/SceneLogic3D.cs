using Assets;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;

using Utils;
using StateMachine = Assets.StateMachine;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class SceneLogic3D : MonoBehaviour
{
    Rigidbody selectedRigidBody;
    Vector3 originalScreenTargetPosition;
    GameObject[] foodBubbles = new GameObject[6];
    public ObservableCollection<Sphere> Spheres = new ObservableCollection<Sphere>();
    GameObject transparentPlane;
    Vector3 lastFingerPosition;
    TimerType timerType = TimerType.CountingUp;
    double firstFingerPositionTime;
    float lastSpeed;
    Vector3 startFingerPosition;
    public bool hostDelayed;
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
    
    public GameObject SickBar;
    public GameObject SickBarPotential;
    public GameObject CaloriesSickArea;
    public GameObject Options;
    public GameObject Rewards;
    public GameObject Plate;
    bool selectedHover;
    public GameObject CurrentLevelPanel;
    public GameplayState gamePlayState { get; private set; } = GameplayState.Single;
    GameObject selectedFoodOver;
    //Vector3 selectedFoodOverOriginalScale;
    Vector3 selectedFoodOverOriginalPosition;
    List<Vector3> comboFoodsOriginalScale = new List<Vector3>();
    Color32 sickColor = new Color32(144, 163, 78, 255);
    public Canvas canvas;
    public GameObject MainPanel;
    public GameObject LostPanel;
    public GameObject EditFoodPanel;
    public GameObject TopPanel;
    public GameObject LevelCompletePanel;
    public GameObject LevelSelectionPanel;
    public GameObject PausePanel;
    public GameObject SkipAndShuffle;
    public GameObject EnableCombo;

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
    public List<GameObject> foodsInCombo = new List<GameObject> ();
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
    bool gamePaused;
    StateMachine previousState;
    bool tutorialFoodSelected;
    int tutorialNumberofRoundsPlayed;
    bool pausedForTimer;
    bool timeRunning;
    bool canChoose = true;
    bool transparentPanelWasActive = true;
    public Level CurrentLevel { get; private set; }

    public GameObject VisualFunnel;
    public Dictionary<Sphere, int> Touches = new Dictionary<Sphere, int>();
    bool anyDownTheVortex = false;
    Coroutine Timer;
    Coroutine FrozenTimer;
    public List<Food> CurrentShuffledDeck = new List<Food>();
    bool checkForTutorialToggle = true;
    public DataService dataService;
    public Dictionary<FoodEffects, int> ActiveEffects = new Dictionary<FoodEffects, int>();
    FoodEffects? CurrentAppliedEffect = null;
    bool firstGameSet;
    public List<TutorialMessages> messagesShown;
    bool TimerWasRunning;
    bool canCheckIfPaused;

    public Camera GetCamera()
    {
        return gameCamera;
    }
    private void TestDatabaseUpdate()
    {
        Constants.PlayerData.PlayerFood.Add(new PlayerFood() { FoodId = 10, FoodTotal = 2, FoodOnDeck = 2 });
        dataService.AddPlayerFood(new PlayerFood() { FoodId = 10, FoodTotal = 2, FoodOnDeck = 2 });

    }

    private async void StartupAnalytics()
    {
        await Analytics.InitializeAnalytics(Debug.isDebugBuild);
    }

   

    // Start is called before the first frame update
    void Start()
    {
        StartupAnalytics();

#if !UNITY_WEBGL


        var safeArea = Screen.safeArea;
        
        // Calculate the target width based on the screen width and 16:9 aspect ratio
        int targetHeight = Screen.width * 16 / 9;
        int difference = Screen.height - (int) safeArea.height;
        
        // Set the game's resolution to match the target width and height
        Screen.SetResolution(Screen.width, targetHeight + difference, true);

        if (Application.platform == RuntimePlatform.Android)
        {
            TopPanel.GetComponent<TopPanel>().SetSafeAreaHeight((difference * 1920 / Screen.height) / (DisplayMetricsAndroid.Density + 1));
        }
        else
        {
            //TEST
            if (Screen.height >= 1920)
            {
                TopPanel.GetComponent<TopPanel>().SetSafeAreaHeight((difference * 1920 / Screen.height) / ((Screen.dpi / 160) + 1));
            }
            else
            {
                TopPanel.GetComponent<TopPanel>().SetSafeAreaHeight((difference * 1920 / Screen.height) / ((Screen.dpi / 160)+1));
            }
        }

        TopPanel.SetActive(true);   

#endif

        gameCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        gameCamera.pixelRect = safeArea;

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

        //FoodNameText.GetComponent<TextMeshPro>().OnPreRenderText += SceneLogic3D_OnPreRenderText;
        foodChoices.SetActive(false);
        EnhancedTouchSupport.Enable();

      

        dataService = new DataService("existing.db");

        var foodDatabase = dataService.GetFoods();
        Constants.FoodsDatabase = foodDatabase;

        var playerfoodDatabase = dataService.GetPlayerFood();
        Constants.PlayerData.PlayerFood = playerfoodDatabase.ToList();
        Constants.PlayerData.InitialiseFoodDeck();

        var levelDataBase = dataService.GetLevels();
        Constants.Levels = levelDataBase;

        var sectionsDatabase = dataService.GetSections();
        Constants.Sections = sectionsDatabase;

        messagesShown = dataService.GetTutorialMessages();

        PlayNextLevelButton.GetComponent<Button>().interactable = false;
        

        //TestDatabaseUpdate();
    }



    private void SceneLogic3D_OnPreRenderText(TMP_TextInfo obj)
    {
        var textBounds = obj.textComponent.textBounds;
        foodImage.transform.localPosition = new Vector3(textBounds.min.x - 0.05f, foodImage.transform.localPosition.y, foodImage.transform.localPosition.z);
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
        Options.SetActive(false);

        Analytics.LogEvent(new GameOverEvent { Level = CurrentLevel.Id, Reason = text });
    }

    public void PlayNextLevel()
    {
        PlayLevel(Constants.Levels[CurrentLevel.Id]);
    }

    public void Reset()
    {
        canChoose = true;
        gamePaused = false;
        pausedBalls = false;
       
        foodsInCombo.Clear();
        gamePlayState = GameplayState.Single;

      
        var plateslots = Plate.GetComponentsInChildren<PlateSlotScript>();
        foreach (var slot in plateslots)
        {
            slot.Reset();
        }

        var image = Resources.Load<Texture2D>("combo_closed");
        EnableCombo.GetComponent<SpriteRenderer>().sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), new Vector2(0.5f, 0.5f));

        foreach (GameObject foodBubble in foodBubbles)
        {
            foodBubble.GetComponent<FoodBubble>().GoBackToOriginalPosition();
        }

        Plate.GetComponent<PlateScript>().Appear();
        Plate.GetComponent<PlateScript>().DeActivateCombo();
        Plate.transform.GetChild(0).gameObject.SetActive(true);
        Plate.transform.GetChild(1).gameObject.SetActive(false);
        //Plate.SetActive(true);

        LevelSelectionPanel.SetActive(false);
        LevelCompletePanel.SetActive(false);
        EditFoodPanel.SetActive(false);
        LostPanel.SetActive(false);
        StarveImage.SetActive(false);
        SickImage.SetActive(false);
        Touches.Clear();
        VisualFunnel.GetComponent<Funnel>().ResetSpeed();
        VisualFunnel.GetComponent<Funnel>().ResumeRotation();

        SkipAndShuffle.GetComponent<SkipShuffle>().Reset();

        if (CurrentLevel.Id == 4 && messagesShown.First(x => x.Id == (int) TutorialMessagesEnum.ToddlerTier + 1).Showed == 0)
        {
            Tutorial.GetComponent<TutorialScript>().ShowWithTextGroup(Constants.TutorialMessages[TutorialMessagesEnum.ToddlerTier], 3);

            messagesShown.First(x => x.Id == (int)TutorialMessagesEnum.ToddlerTier + 1).Showed = 1;
            dataService.UpdateTutorialMessages(messagesShown);

            pausedBalls = true;
            hostDelayed = true;
            pausedForTimer = true;
        }

        if (CurrentLevel.Id == 7 && messagesShown.First(x => x.Id == (int)TutorialMessagesEnum.ChildTier + 1).Showed == 0)
        {
            Tutorial.GetComponent<TutorialScript>().ShowWithTextGroup(Constants.TutorialMessages[TutorialMessagesEnum.ChildTier], 3);

            messagesShown.First(x => x.Id == (int)TutorialMessagesEnum.ChildTier + 1).Showed = 1;
            dataService.UpdateTutorialMessages(messagesShown);

            pausedBalls = true;
            hostDelayed = true;
            pausedForTimer = true;
        }

        if (!hostDelayed)
        {
            Host.GetComponent<Host>().Show();
        }
        gameOver = false;
        Music.Play();

        if (timerType == TimerType.CountingDown)
        {
            TimeLeft = TimeSpan.FromSeconds(CurrentLevel.Time);
        }
        else
        {
            TimeLeft = TimeSpan.FromSeconds(0);
        }

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

        EnableCombo.SetActive(true);

        foodChoices.SetActive(true);
        foreach (GameObject foodBubble in foodBubbles)
        {
            foodBubble.GetComponent<FoodBubble>().Show();
        }
     
        CurrentFat.GetComponent<FillScript>().Reset(firstReset:!firstGameSet);
        CurrentSaturates.GetComponent<FillScript>().Reset(firstReset:!firstGameSet);
        CurrentSalt.GetComponent<FillScript>().Reset(firstReset:!firstGameSet);
        CurrentSugar.GetComponent<FillScript>().Reset(firstReset:!firstGameSet);
        CaloriesBar.GetComponent<CaloriesFill>().Reset(firstReset:!firstGameSet);
        SickBar.GetComponent<SickFill>().Reset(!firstGameSet);

        if (!firstGameSet)
        {
            firstGameSet = true;
        }

        canvas.enabled = false;

        if (!pausedBalls)
        {
            Timer = StartCoroutine(CustomTimer.Timer(1, () =>
            {
                if (timerType == TimerType.CountingDown)
                {
                    TimeLeft = TimeLeft - TimeSpan.FromSeconds(1);
                }
                else
                {
                    TimeLeft = TimeLeft + TimeSpan.FromSeconds(1);
                }

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
        ActiveEffects.Clear();

        Plate.transform.GetChild(0).gameObject.SetActive(true);
        Plate.transform.GetChild(1).gameObject.SetActive(false);

        Options.SetActive(true);
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

        double timerRatio = 1;

        if(timerType == TimerType.CountingUp)
        {
            var objective = CurrentLevel.Time;
            var achieved = TimeLeft.TotalSeconds;

            timerRatio = objective / achieved;
    
        }
        else
        {
            var objective = CurrentLevel.Time;
            var achieved = TimeLeft.TotalSeconds;
            timerRatio = (objective - achieved) / achieved;
        }

        var average = (fatRatio + saturatesRatio + saltRatio + sugarRatio + (timerRatio / 2) ) / 5;
         
        GradesEnum result = average switch
        {
            > 0.9f => GradesEnum.A, 
            > 0.5f => GradesEnum.B,
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

        canChoose = true;

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

            transparentPanelWasActive = true;
            //transparentPlane.SetActive(true);
            transparentPlane.GetComponent<TransparentPlane>().Show();
            foreach (GameObject foodBubble in foodBubbles)
            {
                foodBubble.GetComponent<FoodBubble>().Show();
            }
            StartupFood();
            Host.GetComponent<Host>().Show();
            Timer = StartCoroutine(CustomTimer.Timer(1, () => {

                if (timerType == TimerType.CountingDown)
                {
                    TimeLeft = TimeLeft - TimeSpan.FromSeconds(1);
                }
                else
                {
                    TimeLeft = TimeLeft + TimeSpan.FromSeconds(1);
                }

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
            foreach (GameObject foodBubble in foodBubbles.Where(x => x.GetComponent<FoodBubble>().Food != null))
            {
                foodBubble.GetComponent<FoodBubble>().Show(true);
            }
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

            Options.SetActive(true);

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
        Analytics.LogEvent(new StartedLevelEvent { Level = level.Id });

        CurrentLevel = level;
       
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
        //CurrentLevelPanel.SetActive(true);
        //CurrentLevelPanel.GetComponent<CurrentLevelPanelScript>().SetCurrentLevel(CurrentLevel);

        SickBar.GetComponent<SickFill>().MaxAmount = (level.MaxFat + level.MaxSaturates + level.MaxSalt + level.MaxSaturates) / 2;
        SickBarPotential.GetComponent<SickFill>().MaxAmount = (CurrentLevel.MaxFat + CurrentLevel.MaxSaturates + CurrentLevel.MaxSalt + CurrentLevel.MaxSaturates) / 2;


        Reset();
    }

    public async void StartGame(bool isTutorial)
    {
        if (Constants.Levels.Count(x => x.Unlocked) == 1 || isTutorial)
        {
            Plate.transform.GetChild(0).gameObject.SetActive(true);
            Plate.transform.GetChild(1).gameObject.SetActive(false);

            state = StateMachine.Tutorial;
            Tutorial.GetComponent<TutorialScript>().ResetTutorial();
            tutorialFoodSelected = false;
            foodChoices.SetActive(false);
            //Flawless.GetComponent<FlawlessScript>().Hide();
            
            for(int i = 0; i < 4; i++)
            {
                messagesShown[i].Showed = 0;
            }

            Analytics.LogEvent(new StartedLevelEvent { Level = 1 });

            canChoose = true;
            gameOver = false;
            LostPanel.SetActive(false);
            gamePaused = false;
            VisualFunnel.GetComponent<Funnel>().ResumeRotation();

            CaloriesSickArea.SetActive(true);

            TimeText.GetComponent<TextMeshPro>().text = $"{(int)TimeLeft.TotalMinutes}:{TimeLeft.Seconds:00}";
            
            ShuffleDeck();

            pausedBalls = false;
           

            SkipAndShuffle.SetActive(false);
            CurrentLevel = Constants.Levels[0];

            if (timerType == TimerType.CountingDown)
            {
                TimeLeft = TimeSpan.FromSeconds(CurrentLevel.Time);
            }

         
            //CurrentLevelPanel.SetActive(true);
            //CurrentLevelPanel.GetComponent<CurrentLevelPanelScript>().SetCurrentLevel(CurrentLevel);

            CurrentFat.GetComponent<FillScript>().Reset(firstReset: !firstGameSet);
            CurrentSaturates.GetComponent<FillScript>().Reset(firstReset: !firstGameSet);
            CurrentSalt.GetComponent<FillScript>().Reset(firstReset: !firstGameSet);
            CurrentSugar.GetComponent<FillScript>().Reset(firstReset: !firstGameSet);
            CaloriesBar.GetComponent<CaloriesFill>().Reset(firstReset: !firstGameSet);
            SickBar.GetComponent<SickFill>().Reset(firstReset: !firstGameSet);

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
                Options.SetActive(true);

                Host.GetComponent<Host>().Show();
                timeRunning = true;
    
                Timer = StartCoroutine(CustomTimer.Timer(1, () =>
                {

                    if (timerType == TimerType.CountingDown)
                    {
                        TimeLeft = TimeLeft - TimeSpan.FromSeconds(1);
                    }
                    else
                    {
                        TimeLeft = TimeLeft + TimeSpan.FromSeconds(1);
                    }

                    if (TimeLeft.TotalSeconds == 0)
                    {
                        timeRunning = false;
                        StopCoroutine(Timer);
                       
                        GameOver("You starved!");
                        StarveImage.SetActive(true);
                    }

                }));

                if (CurrentLevel.ChangeFood == 1)
                {
                    SkipAndShuffle.SetActive(true);
                }

                EnableCombo.SetActive(true) ;

                foodChoices.SetActive(true);
                StartupFood();
                foreach (GameObject foodBubble in foodBubbles)
                {
                    foodBubble.GetComponent<FoodBubble>().Show();

                    await AsyncTask.Await(100);
                }

              
                gamePaused = false;
            }
            else
            {
                Tutorial.GetComponent<TutorialScript>().Show();
            }
        }
        else
        {
            state = StateMachine.NormalPlay;
            LostPanel.SetActive(false);

            transparentPanelWasActive = true;
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
                //gamePlayState = GameplayState.Single;

                tutorialNumberofRoundsPlayed++;

                if (tutorialNumberofRoundsPlayed >= 3 && CaloriesBar.GetComponent<CaloriesFill>().currentAmount > CaloriesBar.GetComponent<CaloriesFill>().MaxAmount * 0.25)
                {
                    Tutorial.GetComponent<TutorialScript>().ShowWithTextGroup(new List<string> { "Doing good, so let's enable the timer! Fill the calories bar before the time runs out!" });
                    pausedForTimer = true;
                    state = StateMachine.NormalPlay;
                }
                else
                {
                    if (!caloriesFull && !gameOver)
                    {
                        Plate.GetComponent<PlateScript>().Appear();
                        if (gamePlayState == GameplayState.Combo)
                        {
                            Plate.GetComponent<PlateScript>().ActivateCombo();
                        }


                        var plateslots = Plate.GetComponentsInChildren<PlateSlotScript>();
                        foreach (var slot in plateslots)
                        {
                            slot.Reset();
                        }

                        if (Touches.All(x => x.Value == 1) && !anyDownTheVortex)
                        {
                            Flawless.GetComponent<FlawlessScript>().Play();

                            if (timerType == TimerType.CountingDown)
                            {
                                TimeLeft += TimeSpan.FromSeconds(5);
                            }
                            else
                            {
                                TimeLeft -= TimeSpan.FromSeconds(5);
                            }

                            if(messagesShown.First(x => x.Id == (int) TutorialMessagesEnum.Flawless + 1).Showed == 0)
                            {
                                Tutorial.GetComponent<TutorialScript>().ShowWithTextGroup(Constants.TutorialMessages[TutorialMessagesEnum.Flawless], 3);

                                messagesShown.First(x => x.Id == (int)TutorialMessagesEnum.Flawless + 1).Showed = 1;
                                dataService.UpdateTutorialMessages(messagesShown);

                                pausedBalls = true;
                                hostDelayed = true;
                            }
                        }

                        anyDownTheVortex = false;
                        Touches.Clear();

                        transparentPanelWasActive = true;
        
                        transparentPlane.GetComponent<TransparentPlane>().Show();
                        foreach (GameObject foodBubble in foodBubbles.Where(x => x.GetComponent<FoodBubble>().Food != null))
                        {                          
                            foodBubble.GetComponent<FoodBubble>().Show(true);
                        }

                        ReduceEffects();

                        if (!hostDelayed)
                        {
                            Host.GetComponent<Host>().Show();
                        }

                        await AsyncTask.Await(100);

                        foodBubbles.First(x => x.GetComponent<FoodBubble>().Food == null).GetComponent<FoodBubble>().Show();
                     
                        GetNextFood();
                      
                       

                        canChoose = true;
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
                //gamePlayState = GameplayState.Single;

                if (!caloriesFull && !gameOver)
                {
                    Plate.GetComponent<PlateScript>().Appear();
                    if(gamePlayState == GameplayState.Combo)
                    {
                        Plate.GetComponent<PlateScript>().ActivateCombo();
                    }


                    var plateslots = Plate.GetComponentsInChildren<PlateSlotScript>();
                    foreach (var slot in plateslots)
                    {
                        slot.Reset();
                    }


                    if (Touches.All(x => x.Value == 1) && !anyDownTheVortex)
                    {
                        Flawless.GetComponent<FlawlessScript>().Play();


                        if (timerType == TimerType.CountingDown)
                        {
                            TimeLeft += TimeSpan.FromSeconds(5);
                        }
                        else
                        {
                            TimeLeft -= TimeSpan.FromSeconds(5);
                        }

                        if (messagesShown.First(x => x.Id == (int)TutorialMessagesEnum.Flawless + 1).Showed == 0)
                        {
                            Tutorial.GetComponent<TutorialScript>().ShowWithTextGroup(Constants.TutorialMessages[TutorialMessagesEnum.Flawless], 3);

                            messagesShown.First(x => x.Id == (int)TutorialMessagesEnum.Flawless + 1).Showed = 1;
                            dataService.UpdateTutorialMessages(messagesShown);

                            pausedBalls = true;
                            hostDelayed = true;
                        }
                    }

                    anyDownTheVortex = false;
                    Touches.Clear();

                    transparentPanelWasActive = true;
          
                    transparentPlane.GetComponent<TransparentPlane>().Show();
                    foreach (GameObject foodBubble in foodBubbles.Where(x => x.GetComponent<FoodBubble>().Food != null))
                    {
                        foodBubble.GetComponent<FoodBubble>().Show(true);
                    }

                    ReduceEffects();

                    if (CurrentLevel.ChangeFood == 1)
                    {
                        SkipAndShuffle.SetActive(true);
                    }

                    EnableCombo.SetActive(true);

                    GetNextFood();

                    if (!hostDelayed)
                    {
                        Host.GetComponent<Host>().Show();
                    }


                    foreach (GameObject foodBubble in foodBubbles)
                    {
                        foodBubble.GetComponent<FoodBubble>().Show();

                        await AsyncTask.Await(100);
                    }


                    SkipAndShuffle.GetComponent<SkipShuffle>().ReduceCooldown();
                   

                    canChoose = true;
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
        transparentPanelWasActive = true;

        transparentPlane.GetComponent<TransparentPlane>().Show();
      
        SoundEffects.GetComponent<SoundEffects>().PlayWin();
        var grade = CalculateGrade();

        Analytics.LogEvent(new FinishedLevelEvent { Level = CurrentLevel.Id, Grade = grade.ToString() });

        Options.SetActive(false);

        /*List<GameObject> stars = new List<GameObject>
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

        }*/


        if (CurrentLevel != null)
        {
            //PlayNextLevelButton.GetComponent<Button>().interactable = true;
           

            Dictionary<string, int> rewards = new Dictionary<string, int>();

            for (int i = 3; i >= (int) grade; i--)
            {

                var reward = CurrentLevel.Rewards[(GradesEnum)i];
                var food = Constants.FoodsDatabase.FirstOrDefault(x => x.Id == reward.FoodId);

                rewards.Add(food.FileName, reward.FoodQuantity);

                if (!Constants.PlayerData.PlayerFood.Any(x => x.FoodId == reward.FoodId))
                {
                    Constants.PlayerData.PlayerFood.Add(new PlayerFood() { FoodId = reward.FoodId, FoodTotal = reward.FoodQuantity, FoodOnDeck = Constants.PlayerData.FoodDeck.Count + reward.FoodQuantity < Constants.MAX_DECK_SIZE ? reward.FoodQuantity : 0 });
                    dataService.AddPlayerFood(new PlayerFood() { FoodId = reward.FoodId, FoodTotal = reward.FoodQuantity, FoodOnDeck = Constants.PlayerData.FoodDeck.Count + reward.FoodQuantity < Constants.MAX_DECK_SIZE ? reward.FoodQuantity : 0 });
                }
                else
                {
                    Constants.PlayerData.PlayerFood.First(x => x.FoodId == reward.FoodId).FoodTotal += reward.FoodQuantity;

                    if (Constants.PlayerData.FoodDeck.Count + reward.FoodQuantity < Constants.MAX_DECK_SIZE)
                    {
                        Constants.PlayerData.PlayerFood.First(x => x.FoodId == reward.FoodId).FoodOnDeck += reward.FoodQuantity;
                    }

                    dataService.StorePlayerFood(Constants.PlayerData.PlayerFood);
                }

            }

            LevelCompletePanel.GetComponent<LevelCompleteScript>().SetFinishedLevelData(CurrentLevel.Id, grade, rewards);
            LevelCompletePanel.SetActive(true);

            //Rewards.GetComponent<RewardsScript>().SetRewards(rewards);

            Constants.PlayerData.InitialiseFoodDeck();

            dataService.StoreUnlockedLevel(CurrentLevel.Id + 1);

            /*if(CurrentLevel.Id % 3 == 0)
            {
                var section = (CurrentLevel.Id / 3);
                var sectionUnlocked = Constants.Sections[section].FoodToUnlock.All(x => Constants.PlayerData.PlayerFood.Any(z => z.FoodId == x.FoodId));

                if (!sectionUnlocked)
                {
                    PlayNextLevelButton.GetComponent<Button>().interactable = false;
                  
                }
                else
                {
                    PlayNextLevelButton.GetComponent<Button>().interactable = true;
                  
                }
            }*/


            var levelDataBase = dataService.GetLevels();
            Constants.Levels = levelDataBase.ToList();


            var sectionsDatabase = dataService.GetSections();
            Constants.Sections = sectionsDatabase.ToList();

          
        }

    }

    public Vector3 GetWorldPositionOnPlane(Vector3 screenPosition, float y)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        Plane xy = new Plane(Vector3.up, new Vector3(0, y, 0));
        float distance;
        xy.Raycast(ray, out distance);
        return ray.GetPoint(distance);
    }

    private void MakeTextGreenAndBold(TextMeshPro textMeshPro)
    {
        textMeshPro.color = sickColor;
        //textMeshPro.fontStyle = FontStyles.Bold;
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
                  
                    }
                }
            }
        

        if (gameOver && canvas.enabled == false)
        {
            transparentPanelWasActive = true;
          
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
            transparentPanelWasActive = true;
    
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
                            var allHits = Physics.SphereCastAll(ray, 0.3f);
                            
                            if (allHits.Any(x => x.collider.transform.gameObject.GetComponent<Sphere>() != null && !x.collider.transform.gameObject.GetComponent<Sphere>().IsGhost && !x.collider.transform.gameObject.GetComponent<Sphere>().wasConsumed))
                            {

                                var sphere = allHits.First(x => x.collider.transform.gameObject.GetComponent<Sphere>() != null && !x.collider.transform.gameObject.GetComponent<Sphere>().IsGhost).collider.gameObject.GetComponent<Sphere>();

                                sphere.PauseRotation();

                                sphere.gameObject.GetComponent<Rigidbody>().useGravity = false;

                                firstFingerPositionTime = Time.time;
                                lastSpeed = 0;

                                lastFingerPosition = finger.currentTouch.screenPosition;
                                sphere.SetPicked(GetWorldPositionOnPlane(lastFingerPosition, sphere.initialPosition.y));

                                selectedRigidBody = sphere.gameObject.GetComponent<Rigidbody>();
                            }


                        }
                    }
                }
                else
                {
                    if (selectedRigidBody != null && selectedRigidBody.GetComponent<Sphere>() != null)
                    {

                        var sphere = selectedRigidBody.GetComponent<Sphere>();
                        sphere.isPicked = false;
                        selectedRigidBody.useGravity = true;
                        selectedRigidBody = null;
                    }
                }

                if (Pointer.current.press.wasPressedThisFrame)
                {
                    GetFoodPicked();
                }

            }
            else
            {
                var finger = Touch.fingers[0];

                if (finger.isActive)
                {
                    if (finger.currentTouch.valid)
                    {

                        if (gamePaused && canCheckIfPaused)
                        {
                          
                            var ray = gameCamera.ScreenPointToRay(Pointer.current.position.value);

                            var allHits = Physics.RaycastAll(ray);

                            if (allHits.Any(x => x.collider.transform.gameObject.name == "Options"))
                            {
                                transparentPanelWasActive = false;
                               
                                transparentPlane.GetComponent<TransparentPlane>().Hide();

                                canvas.enabled = false;
                                MainPanel.SetActive(false);
                                LevelCompletePanel.SetActive(false);
                                LostPanel.SetActive(false);
                                PausePanel.SetActive(false);


                                canCheckIfPaused = false;
                                gamePaused = false;
                                pausedBalls = false;
                                VisualFunnel.GetComponent<Funnel>().ResumeRotation();

                                if (TimerWasRunning)
                                {
                                    Timer = StartCoroutine(CustomTimer.Timer(1, () =>
                                    {

                                        if (timerType == TimerType.CountingDown)
                                        {
                                            TimeLeft = TimeLeft - TimeSpan.FromSeconds(1);
                                        }
                                        else
                                        {
                                            TimeLeft = TimeLeft + TimeSpan.FromSeconds(1);
                                        }

                                        if (TimeLeft.TotalSeconds == 0)
                                        {
                                            timeRunning = false;

                                            StopCoroutine(Timer);

                                            GameOver("You starved!");
                                            StarveImage.SetActive(true);
                                        }

                                    }));
                                }
                            }
                        }
                    }
                }
            }


        }
    }

    private void FixedUpdate()
    {
        if (selectedRigidBody != null && !pausedBalls)
        {
            var finger = Touch.fingers[0];
            var currentTouch = finger.touchHistory.First();
           
            if (finger.currentTouch.valid)
            {
                var mode = finger.currentTouch.phase;
                var distance = Vector2.Distance(finger.currentTouch.screenPosition, lastFingerPosition);
                var speed = (float)(distance / Time.fixedDeltaTime);
                var currentTouchToWorldPoint = GetWorldPositionOnPlane(currentTouch.screenPosition, selectedRigidBody.GetComponent<Sphere>().initialPosition.y);

                if (mode == UnityEngine.InputSystem.TouchPhase.Moved) {

                    if (speed > 0)
                    {
                        selectedRigidBody.drag = 0;
                        selectedRigidBody.MovePosition(currentTouchToWorldPoint);
           
                        if(lastSpeed == 0)
                        {
                            startFingerPosition = currentTouch.screenPosition;
                            firstFingerPositionTime = currentTouch.time;
                        }
                    }
                   
                    lastSpeed = speed;
                }
                else
                {
                    if (mode == UnityEngine.InputSystem.TouchPhase.Ended)
                    {
                        var starFingerPositionToWorldPoint = GetWorldPositionOnPlane(startFingerPosition, selectedRigidBody.GetComponent<Sphere>().initialPosition.y);
                        var difference = currentTouchToWorldPoint - starFingerPositionToWorldPoint;
                        var differenceSpeed = (float)(difference.magnitude / (currentTouch.time - firstFingerPositionTime));

                        selectedRigidBody.AddForce(difference * differenceSpeed, ForceMode.VelocityChange);
                    }
                }

                lastFingerPosition = finger.currentTouch.screenPosition;

            }
        }
        else
        {
            if (selectedFoodOver != null)
            {
                var finger = Touch.fingers[0];
                var currentTouch = finger.touchHistory.First();
                var distance = Vector2.Distance(finger.currentTouch.screenPosition, lastFingerPosition);
                var speed = (float)(distance / Time.fixedDeltaTime);

                if (finger.currentTouch.valid)
                {
                    var mode = finger.currentTouch.phase;
                    var rigidBody = selectedFoodOver.GetComponent<Rigidbody>();

                    if (mode == UnityEngine.InputSystem.TouchPhase.Moved)
                    {
                        if (speed > 0)
                        {
                            var currentTouchToWorldPoint = GetWorldPositionOnPlane(currentTouch.screenPosition, selectedFoodOverOriginalPosition.y);
                            rigidBody.MovePosition(currentTouchToWorldPoint);
                        }
                    }
                    else
                    {
                        if(mode == UnityEngine.InputSystem.TouchPhase.Ended)
                        {
                            var foodOnBubble = selectedFoodOver.GetComponent<FoodBubble>();
                            status.SetActive(false);

                            if (!foodOnBubble.OnPlate)
                            {
                                selectedFoodOver.GetComponent<FoodBubble>().GoBackToOriginalPosition();

                                if (gamePlayState == GameplayState.Single)
                                {
                                    
                                    selectedFoodOver = null;
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
                                else
                                {
                                    if (foodsInCombo.Contains(selectedFoodOver))
                                    {
                                        foodsInCombo.Remove(selectedFoodOver);
                                    }
                                    selectedFoodOver = null;
                                    selectedHover = false;
                                    UpdateBarSimulation(true);
                                }
                            }
                            else
                            {
                                if (gamePlayState == GameplayState.Single)
                                {
                                    FoodWasChoosed(foodOnBubble);
                                }
                                else
                                {
                                    selectedFoodOver.transform.position = foodOnBubble.platePosition;
                                    foodOnBubble.ResetScale();

                                    if (!foodsInCombo.Contains(selectedFoodOver))
                                    {
                                        foodsInCombo.Add(selectedFoodOver);
                                    }
                                    
                                    selectedFoodOver = null;
                                    UpdateBarSimulation(true);

                                    if(foodsInCombo.Count == 3)
                                    {
                                        ComboSelected();
                                    }
                                }
                            }
                        }
                    }

                    lastFingerPosition = finger.currentTouch.screenPosition;
                }
            }
        }
    }

    private void FoodWasChoosed(FoodBubble food)
    {
        Plate.GetComponent<PlateScript>().Disappear();
        //Plate.SetActive(false);
       

        canChoose = false;

        Dictionary<NutritionElementsEnum, float> leftOnBars = new Dictionary<NutritionElementsEnum, float>()
                                {
                                    { NutritionElementsEnum.Fat, CurrentFat.GetComponent<FillScript>().MaxAmount - CurrentFat.GetComponent<FillScript>().currentAmount },
                                    { NutritionElementsEnum.Saturates, CurrentSaturates.GetComponent<FillScript>().MaxAmount - CurrentSaturates.GetComponent<FillScript>().currentAmount },
                                    { NutritionElementsEnum.Salt, CurrentSalt.GetComponent<FillScript>().MaxAmount - CurrentSalt.GetComponent<FillScript>().currentAmount },
                                    { NutritionElementsEnum.Sugar, CurrentSugar.GetComponent<FillScript>().MaxAmount - CurrentSugar.GetComponent<FillScript>().currentAmount },
                                };


        //Flawless.GetComponent<FlawlessScript>().Hide();
        SoundEffects.GetComponent<SoundEffects>().PlayBubble();
        CaloriesBar.GetComponent<CaloriesFill>().AddAmount(food.Food.Calories * CurrentLevel.Multiplier);
       
        ApplyFoodEffect(food);
        food.FoodChosen(leftOnBars);

        var otherFoodBubbles = foodBubbles.Where(x => x != food.gameObject).ToList();
        foreach (GameObject foodBubble in otherFoodBubbles)
        {
            foodBubble.GetComponent<FoodBubble>().disappear = true;
        }

        SkipAndShuffle.SetActive(false);
        EnableCombo.SetActive(false);


        transparentPanelWasActive = false;

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

    public async void ComboSelected()
    {
        //Flawless.GetComponent<FlawlessScript>().Hide();

        /*float fats = 0;
        float saturates = 0;
        float salt = 0;
        float sugar = 0;*/

        //Dictionary<NutritionElementsEnum, float> comboNutritionElements = new Dictionary<NutritionElementsEnum, float>();

        Plate.GetComponent<PlateScript>().Disappear();

        var otherFoodBubbles = foodBubbles.Where(x => !foodsInCombo.Any(y => y == x)).ToList();
        foreach (GameObject foodBubble in otherFoodBubbles)
        {
            foodBubble.GetComponent<FoodBubble>().disappear = true;
        }

        Dictionary<NutritionElementsEnum, float> leftOnBars = new Dictionary<NutritionElementsEnum, float>()
        { 
            { NutritionElementsEnum.Fat, CurrentFat.GetComponent<FillScript>().MaxAmount - CurrentFat.GetComponent<FillScript>().currentAmount },
            { NutritionElementsEnum.Saturates, CurrentSaturates.GetComponent<FillScript>().MaxAmount - CurrentSaturates.GetComponent<FillScript>().currentAmount },
            { NutritionElementsEnum.Salt, CurrentSalt.GetComponent<FillScript>().MaxAmount - CurrentSalt.GetComponent<FillScript>().currentAmount },
            { NutritionElementsEnum.Sugar, CurrentSugar.GetComponent<FillScript>().MaxAmount - CurrentSugar.GetComponent<FillScript>().currentAmount },
        };
        

        for (int i=0; i < foodsInCombo.Count; i++)
        {
            foodsInCombo[i].GetComponent<FoodBubble>().FoodChosen(leftOnBars);
          
            SoundEffects.GetComponent<SoundEffects>().PlayBubble();
            CaloriesBar.GetComponent<CaloriesFill>().AddAmount(foodsInCombo[i].GetComponent<FoodBubble>().Food.Calories * CurrentLevel.Multiplier);
          
            ApplyFoodEffect(foodsInCombo[i].GetComponent<FoodBubble>());

            /*fats += foodsInCombo[i].GetComponent<FoodBubble>().Food.NutritionElements[NutritionElementsEnum.Fat];
            saturates += foodsInCombo[i].GetComponent<FoodBubble>().Food.NutritionElements[NutritionElementsEnum.Saturates];
            salt += foodsInCombo[i].GetComponent<FoodBubble>().Food.NutritionElements[NutritionElementsEnum.Salt];
            sugar += foodsInCombo[i].GetComponent<FoodBubble>().Food.NutritionElements[NutritionElementsEnum.Sugar];*/


            await AsyncTask.Await(250);

        }

        StopCoroutine(Timer);
        TimeText.GetComponent<TextMeshPro>().color = Color.blue;

        FrozenTimer = StartCoroutine(CustomTimer.Timer(5, () =>
        {
            TimeText.GetComponent<TextMeshPro>().color = Color.black;
            Timer = StartCoroutine(CustomTimer.Timer(1, () =>
            {

                if (timerType == TimerType.CountingDown)
                {
                    TimeLeft = TimeLeft - TimeSpan.FromSeconds(1);
                }
                else
                {
                    TimeLeft = TimeLeft + TimeSpan.FromSeconds(1);
                }

                if (TimeLeft.TotalSeconds == 0)
                {
                    timeRunning = false;
                    StopCoroutine(Timer);

                    GameOver("You starved!");
                    StarveImage.SetActive(true);
                }
            }));

        }, true));

        /*Unity.Mathematics.Random random = new Unity.Mathematics.Random();
        var randomLocation = random.NextInt(foodsInCombo.Count - 1);

        VisualFunnel.GetComponent<Funnel>().CreateComboNutritionBubbles(foodsInCombo[randomLocation].transform.position,
            new Dictionary<NutritionElementsEnum, float> {
                {  NutritionElementsEnum.Fat, fats },
                  {  NutritionElementsEnum.Saturates, saturates },
                   {  NutritionElementsEnum.Salt, salt },
                    {  NutritionElementsEnum.Sugar, sugar },

            });*/


        /*for (int i = 0; i < foodsInCombo.Count; i++)
        {
            foodsInCombo[i].transform.localScale = comboFoodsOriginalScale[i];
        }*/

        //comboFoodsOriginalScale.Clear();
        foodsInCombo.Clear();
       

        SkipAndShuffle.SetActive(false);
        EnableCombo.SetActive(false);
    

        transparentPanelWasActive = false;

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
    }

    private void UpdateBarSimulation(bool isCombo = false)
    {
       
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

        if (!isCombo)
        {
            var calculatedBaseSick = false;

            var currentFood = selectedFoodOver.GetComponent<FoodBubble>().Food;
           

            var canAbsorbFat = PotentialFat.GetComponent<FillScript>().Simulate(CurrentFat.GetComponent<FillScript>().currentAmount + currentFood.NutritionElements[NutritionElementsEnum.Fat] * CurrentLevel.Multiplier);
            if (!canAbsorbFat)
            {
                if (!calculatedBaseSick)
                {
                    SickBarPotential.GetComponent<SickFill>().Simulate(SickBar.GetComponent<SickFill>().MaxAmount - SickBar.GetComponent<SickFill>().currentAmount);
                    calculatedBaseSick = true;
                }

                MakeTextGreenAndBold(FatAmountText.GetComponent<TextMeshPro>());
                SickBarPotential.GetComponent<SickFill>().Simulate(currentFood.NutritionElements[NutritionElementsEnum.Fat] * CurrentLevel.Multiplier);
        
            }

            var canAbsorbSaturates = PotentialSaturates.GetComponent<FillScript>().Simulate(CurrentSaturates.GetComponent<FillScript>().currentAmount + currentFood.NutritionElements[NutritionElementsEnum.Saturates] * CurrentLevel.Multiplier);
            if (!canAbsorbSaturates)
            {

                if (!calculatedBaseSick)
                {
                    SickBarPotential.GetComponent<SickFill>().Simulate(SickBar.GetComponent<SickFill>().MaxAmount - SickBar.GetComponent<SickFill>().currentAmount);
                    calculatedBaseSick = true;
                }

                MakeTextGreenAndBold(SaturatesAmountText.GetComponent<TextMeshPro>());
                SickBarPotential.GetComponent<SickFill>().Simulate(currentFood.NutritionElements[NutritionElementsEnum.Saturates] * CurrentLevel.Multiplier);
               
            }

            var canAbsorbSalt = PotentialSalt.GetComponent<FillScript>().Simulate(CurrentSalt.GetComponent<FillScript>().currentAmount + currentFood.NutritionElements[NutritionElementsEnum.Salt] * CurrentLevel.Multiplier);
            if (!canAbsorbSalt)
            {
                if (!calculatedBaseSick)
                {
                    SickBarPotential.GetComponent<SickFill>().Simulate(SickBar.GetComponent<SickFill>().MaxAmount - SickBar.GetComponent<SickFill>().currentAmount);
                    calculatedBaseSick = true;
                }

                MakeTextGreenAndBold(SaltAmountText.GetComponent<TextMeshPro>());
                SickBarPotential.GetComponent<SickFill>().Simulate(currentFood.NutritionElements[NutritionElementsEnum.Salt] * CurrentLevel.Multiplier);
               
            }

            var canAbsorbSugar = PotentialSugar.GetComponent<FillScript>().Simulate(CurrentSugar.GetComponent<FillScript>().currentAmount + currentFood.NutritionElements[NutritionElementsEnum.Sugar] * CurrentLevel.Multiplier);
            if (!canAbsorbSugar)
            {
                if (!calculatedBaseSick)
                {
                    SickBarPotential.GetComponent<SickFill>().Simulate(SickBar.GetComponent<SickFill>().MaxAmount - SickBar.GetComponent<SickFill>().currentAmount);
                    calculatedBaseSick = true;
                }

                MakeTextGreenAndBold(SugarAmountText.GetComponent<TextMeshPro>());
                SickBarPotential.GetComponent<SickFill>().Simulate(currentFood.NutritionElements[NutritionElementsEnum.Sugar] * CurrentLevel.Multiplier);
                
            }
            PotentialCalories.GetComponent<CaloriesFill>().Simulate(CaloriesBar.GetComponent<CaloriesFill>().currentAmount + currentFood.Calories * CurrentLevel.Multiplier);
        }
        else
        {
            var calculatedBaseSick = false;

            PotentialFat.GetComponent<FillScript>().Simulate(CurrentFat.GetComponent<FillScript>().currentAmount);
            PotentialSaturates.GetComponent<FillScript>().Simulate(CurrentSaturates.GetComponent<FillScript>().currentAmount);
            PotentialSalt.GetComponent<FillScript>().Simulate(CurrentSalt.GetComponent<FillScript>().currentAmount);
            PotentialSugar.GetComponent<FillScript>().Simulate(CurrentSugar.GetComponent<FillScript>().currentAmount);
            //SickBarPotential.GetComponent<SickFill>().Simulate(SickBar.GetComponent<SickFill>().currentAmount);
            PotentialCalories.GetComponent<CaloriesFill>().Simulate(CaloriesBar.GetComponent<CaloriesFill>().currentAmount);

            var newFoodCombo = foodsInCombo.ToList();

            if (selectedFoodOver != null && !newFoodCombo.Contains(selectedFoodOver))
            {
                newFoodCombo.Add(selectedFoodOver);
            }

            foreach (GameObject foodInCombo in newFoodCombo)
            {
                var food = foodInCombo.GetComponent<FoodBubble>().Food;

                var canAbsorbFat = PotentialFat.GetComponent<FillScript>().Simulate(food.NutritionElements[NutritionElementsEnum.Fat] * CurrentLevel.Multiplier);
                if (!canAbsorbFat)
                {

                    if (!calculatedBaseSick)
                    {
                        SickBarPotential.GetComponent<SickFill>().Simulate(SickBar.GetComponent<SickFill>().MaxAmount - SickBar.GetComponent<SickFill>().currentAmount);
                        calculatedBaseSick = true;
                    }

                    MakeTextGreenAndBold(FatAmountText.GetComponent<TextMeshPro>());
                    SickBarPotential.GetComponent<SickFill>().Simulate(food.NutritionElements[NutritionElementsEnum.Fat] * CurrentLevel.Multiplier);
                }

                var canAbsorbSaturates = PotentialSaturates.GetComponent<FillScript>().Simulate(food.NutritionElements[NutritionElementsEnum.Saturates] * CurrentLevel.Multiplier);
                if (!canAbsorbSaturates)
                {

                    if (!calculatedBaseSick)
                    {
                        SickBarPotential.GetComponent<SickFill>().Simulate(SickBar.GetComponent<SickFill>().MaxAmount - SickBar.GetComponent<SickFill>().currentAmount);
                        calculatedBaseSick = true;
                    }

                    MakeTextGreenAndBold(SaturatesAmountText.GetComponent<TextMeshPro>());
                    SickBarPotential.GetComponent<SickFill>().Simulate(food.NutritionElements[NutritionElementsEnum.Saturates] * CurrentLevel.Multiplier);
                    
                }

                var canAbsorbSalt = PotentialSalt.GetComponent<FillScript>().Simulate(food.NutritionElements[NutritionElementsEnum.Salt] * CurrentLevel.Multiplier);
                if (!canAbsorbSalt)
                {

                    if (!calculatedBaseSick)
                    {
                        SickBarPotential.GetComponent<SickFill>().Simulate(SickBar.GetComponent<SickFill>().MaxAmount - SickBar.GetComponent<SickFill>().currentAmount);
                        calculatedBaseSick = true;
                    }

                    MakeTextGreenAndBold(SaltAmountText.GetComponent<TextMeshPro>());
                    SickBarPotential.GetComponent<SickFill>().Simulate(food.NutritionElements[NutritionElementsEnum.Salt] * CurrentLevel.Multiplier);
                    
                }

                var canAbsorbSugar = PotentialSugar.GetComponent<FillScript>().Simulate(food.NutritionElements[NutritionElementsEnum.Sugar] * CurrentLevel.Multiplier);
                if (!canAbsorbSugar)
                {

                    if (!calculatedBaseSick)
                    {
                        SickBarPotential.GetComponent<SickFill>().Simulate(SickBar.GetComponent<SickFill>().MaxAmount - SickBar.GetComponent<SickFill>().currentAmount);
                        calculatedBaseSick = true;
                    }

                    MakeTextGreenAndBold(SugarAmountText.GetComponent<TextMeshPro>());
                    SickBarPotential.GetComponent<SickFill>().Simulate(food.NutritionElements[NutritionElementsEnum.Sugar] * CurrentLevel.Multiplier);
                    
                }
                PotentialCalories.GetComponent<CaloriesFill>().Simulate(food.Calories * CurrentLevel.Multiplier);
            }
        }

    }


    private async void GetFoodPicked()
{

        var ray = gameCamera.ScreenPointToRay(Pointer.current.position.value);

        var allHits = Physics.RaycastAll(ray);

        if (allHits.Any(x => x.collider.transform.gameObject.name == "Options"))
        {
            if (!gamePaused)
            {
                if (Timer != null)
                {
                    TimerWasRunning = true;
                    StopCoroutine(Timer);
                }

                gamePaused = true;

                transparentPlane.GetComponent<TransparentPlane>().Show();

                canvas.enabled = true;
                MainPanel.SetActive(false);
                LevelCompletePanel.SetActive(false);
                LostPanel.SetActive(false);
                PausePanel.SetActive(true);


                StartCoroutine(CustomTimer.Timer(1, () =>
                {
                    canCheckIfPaused = true;

                }, true));

                pausedBalls = true;
                VisualFunnel.GetComponent<Funnel>().PauseRotation();

            }

        }


        if (!canChoose)
            return;

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
                //if (gamePlayState == GameplayState.Single)
                //{

                    var food = allHits.First(x => x.collider.transform.gameObject.GetComponent<FoodBubble>() != null).collider.transform.gameObject.GetComponent<FoodBubble>();
                    if (food.Food != null)
                    {
                        if (selectedFoodOver == null || selectedFoodOver != food.gameObject)
                        {
                           
                            foodImage.transform.position = foodImageOriginalPosition;

                         
                            status.SetActive(false);


                            selectedFoodOver = food.gameObject;


                          
                            selectedFoodOverOriginalPosition = selectedFoodOver.transform.position;
                            selectedFoodOver.transform.localScale = new Vector3(selectedFoodOver.transform.localScale.x * 1.2f, selectedFoodOver.transform.localScale.y * 1.2f, selectedFoodOver.transform.localScale.z * 1.2f);


                            selectedHover = true;


                            //FoodNameText.GetComponent<TextMeshPro>().text = food.Food.Name;

                            if (!string.IsNullOrEmpty(food.Food.FileName))
                            {
                                var image = Resources.Load<Texture2D>(food.Food.FileName);
                                foodImage.GetComponent<SpriteRenderer>().sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), new Vector2(0.5f, 0.5f));
                            }
                            else
                            {
                                foodImage.GetComponent<SpriteRenderer>().sprite = null;
                            }

                            UpdateBarSimulation(gamePlayState == GameplayState.Combo);

                            CaloriesText.GetComponent<TextMeshPro>().text = $"{Math.Round(food.Food.Calories * CurrentLevel.Multiplier, 2)} <b>kCal</b>";
                            FatAmountText.GetComponent<TextMeshPro>().text = $"{Math.Round(food.Food.NutritionElements[NutritionElementsEnum.Fat] * CurrentLevel.Multiplier, 2)}";
                            SaturatesAmountText.GetComponent<TextMeshPro>().text = $"{Math.Round(food.Food.NutritionElements[NutritionElementsEnum.Saturates] * CurrentLevel.Multiplier, 2)}";
                            SaltAmountText.GetComponent<TextMeshPro>().text = $"{Math.Round(food.Food.NutritionElements[NutritionElementsEnum.Salt] * CurrentLevel.Multiplier, 2)}";
                            SugarAmountText.GetComponent<TextMeshPro>().text = $"{Math.Round(food.Food.NutritionElements[NutritionElementsEnum.Sugar] * CurrentLevel.Multiplier, 2)}";

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
                            /*if (selectedFoodOver != null && selectedFoodOver == food.gameObject)
                            {
                                canChoose = false;

                                Dictionary<NutritionElementsEnum, float> leftOnBars = new Dictionary<NutritionElementsEnum, float>()
                                {
                                    { NutritionElementsEnum.Fat, CurrentFat.GetComponent<FillScript>().MaxAmount - CurrentFat.GetComponent<FillScript>().currentAmount },
                                    { NutritionElementsEnum.Saturates, CurrentSaturates.GetComponent<FillScript>().MaxAmount - CurrentSaturates.GetComponent<FillScript>().currentAmount },
                                    { NutritionElementsEnum.Salt, CurrentSalt.GetComponent<FillScript>().MaxAmount - CurrentSalt.GetComponent<FillScript>().currentAmount },
                                    { NutritionElementsEnum.Sugar, CurrentSugar.GetComponent<FillScript>().MaxAmount - CurrentSugar.GetComponent<FillScript>().currentAmount },
                                };


                                //Flawless.GetComponent<FlawlessScript>().Hide();
                                SoundEffects.GetComponent<SoundEffects>().PlayBubble();
                                CaloriesBar.GetComponent<CaloriesFill>().AddAmount(food.Food.Calories * CurrentLevel.Multiplier);
                                caloriesLevel.text = $"{CaloriesBar.GetComponent<CaloriesFill>().currentAmount}/{CurrentLevel.CaloriesObjective}";
                                ApplyFoodEffect(food);
                                food.FoodChosen(leftOnBars);

                                var otherFoodBubbles = foodBubbles.Where(x => x != food.gameObject).ToList();
                                foreach (GameObject foodBubble in otherFoodBubbles)
                                {
                                    foodBubble.GetComponent<FoodBubble>().disappear = true;
                                }

                                SkipAndShuffle.SetActive(false);
                                EnableCombo.SetActive(false);
                              

                                transparentPanelWasActive = false;

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
                            }*/
                        }
                    }
                //}
                /*else if(gamePlayState == GameplayState.Combo)
                {
                    var food = allHits.First(x => x.collider.transform.gameObject.GetComponent<FoodBubble>() != null).collider.transform.gameObject.GetComponent<FoodBubble>();

                    if (!foodsInCombo.Contains(food.gameObject))
                    {
                        if (foodsInCombo.Count < 3)
                        {
                            comboFoodsOriginalScale.Add(food.transform.localScale);
                            foodsInCombo.Add(food.gameObject);
                            food.transform.localScale = new Vector3(food.transform.localScale.x * 1.2f, food.transform.localScale.y * 1.2f, food.transform.localScale.z * 1.2f);

                            UpdateBarSimulation(true);
                        }
                    }
                    else
                    {
                        var index = foodsInCombo.IndexOf(food.gameObject);
                        food.transform.localScale = comboFoodsOriginalScale[index];
                        foodsInCombo.Remove(food.gameObject);

                      

                        UpdateBarSimulation(true);
                    }
                }*/

            }
            else
            {
                if (SkipAndShuffle.GetComponent<SkipShuffle>().CanSKip)
                {

                    if (allHits.Any(x => x.collider.transform.gameObject.name == "SkipAndShuffle"))
                    {

                        foodsInCombo.Clear();
                        comboFoodsOriginalScale.Clear();
                        SkipAndShuffle.GetComponent<SkipShuffle>().Deactivate();
                        SkipAndShuffle.SetActive(false);
                        EnableCombo.SetActive(false);
                      


                        foreach (GameObject foodBubble in foodBubbles)
                        {
                            foodBubble.GetComponent<FoodBubble>().FoodSpoiled(false);

                        }

                        await AsyncTask.Await(500);

                        GetNextFood();

                        foreach (GameObject foodBubble in foodBubbles)
                        {
                            foodBubble.GetComponent<FoodBubble>().Show();

                            await AsyncTask.Await(100);
                        }

                        SkipAndShuffle.SetActive(true);
                        EnableCombo.SetActive(true);


                    }
                }

                if (allHits.Any(x => x.collider.transform.gameObject.name == "EnableCombo"))
                {
                    if(gamePlayState == GameplayState.Single)
                    {
                        gamePlayState = GameplayState.Combo;
                        var image = Resources.Load<Texture2D>("combo_open");
                        EnableCombo.GetComponent<SpriteRenderer>().sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), new Vector2(0.5f, 0.5f));
                        Plate.GetComponent<PlateScript>().ActivateCombo();
                        Plate.transform.GetChild(0).gameObject.SetActive(false);
                        Plate.transform.GetChild(1).gameObject.SetActive(true);
                    }
 
                    else
                    {
                        gamePlayState = GameplayState.Single;
                        var image = Resources.Load<Texture2D>("combo_closed");
                        EnableCombo.GetComponent<SpriteRenderer>().sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), new Vector2(0.5f, 0.5f));
                        Plate.GetComponent<PlateScript>().DeActivateCombo();
                       
                        foreach (GameObject foodBubble in foodBubbles)
                        {
                            foodBubble.GetComponent<FoodBubble>().GoBackToOriginalPosition();
                        }
                        var plateslots = Plate.GetComponentsInChildren<PlateSlotScript>();
                        foreach (var slot in plateslots)
                        {
                            slot.Reset();
                        }
                        Plate.transform.GetChild(0).gameObject.SetActive(true);
                        Plate.transform.GetChild(1).gameObject.SetActive(false);
                        foodsInCombo.Clear();
                    }

                   
                }

                if (gamePlayState == GameplayState.Single)
                {


                    if (selectedFoodOver != null)
                    {
                      
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
                else
                {
                    if (allHits.Any(x => x.collider.transform.gameObject.name == "EatCombo"))
                    {
                        ComboSelected();

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

        if (absorbed && messagesShown.First(x => x.Id == (int) TutorialMessagesEnum.BallAbsorbed + 1).Showed == 0 && Spheres.Count > 1)
        {
            Tutorial.GetComponent<TutorialScript>().ShowWithTextGroup(Constants.TutorialMessages[TutorialMessagesEnum.BallAbsorbed]);
            messagesShown.First(x => x.Id == (int)TutorialMessagesEnum.BallAbsorbed + 1).Showed = 1;

            dataService.UpdateTutorialMessages(messagesShown);

            pausedBalls = true;
            VisualFunnel.GetComponent<Funnel>().PauseRotation();
        }

        if (!absorbed && messagesShown.First(x => x.Id == (int)TutorialMessagesEnum.BallDownVortex + 1).Showed == 0 && Spheres.Count > 1)
        {
            Tutorial.GetComponent<TutorialScript>().ShowWithTextGroup(Constants.TutorialMessages[TutorialMessagesEnum.BallDownVortex]);

            messagesShown.First(x => x.Id == (int)TutorialMessagesEnum.BallDownVortex + 1).Showed = 1;

            dataService.UpdateTutorialMessages(messagesShown);

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
        transparentPanelWasActive = true;
        //transparentPlane.SetActive(true);
        transparentPlane.GetComponent<TransparentPlane>().Show();
        MainPanel.SetActive(false);
        LevelSelectionPanel.SetActive(true);

    }

    public void EditFoodDeck()
    {
        transparentPanelWasActive = true;
        transparentPlane.GetComponent<TransparentPlane>().Show();
        MainPanel.SetActive(false);
        EditFoodPanel.SetActive(true);
    }

    public void CancelEditFoodDeck()
    {
        transparentPanelWasActive = true;
        transparentPlane.GetComponent<TransparentPlane>().Show();
        EditFoodPanel.SetActive(false);
        MainPanel.SetActive(true);
    }

    public void ResumeGame()
    {
        if(transparentPanelWasActive)
        {

            transparentPlane.GetComponent<TransparentPlane>().Show();
        }
        else
        {
            transparentPlane.GetComponent<TransparentPlane>().Hide();
        }

        canvas.enabled = false;
        MainPanel.SetActive(false);
        LevelCompletePanel.SetActive(false);
        LostPanel.SetActive(false);
        PausePanel.SetActive(false);


        canCheckIfPaused = false;
        gamePaused = false;
        pausedBalls = false;
        VisualFunnel.GetComponent<Funnel>().ResumeRotation();

        if (TimerWasRunning)
        {
            Timer = StartCoroutine(CustomTimer.Timer(1, () =>
            {

                if (timerType == TimerType.CountingDown)
                {
                    TimeLeft = TimeLeft - TimeSpan.FromSeconds(1);
                }
                else
                {
                    TimeLeft = TimeLeft + TimeSpan.FromSeconds(1);
                }

                if (TimeLeft.TotalSeconds == 0)
                {
                    timeRunning = false;

                    StopCoroutine(Timer);

                    GameOver("You starved!");
                    StarveImage.SetActive(true);
                }

            }));
        }
    }

    public void BackToMenu()
    {
        gameOver = true;

        foreach (var sphere in Spheres)
        {
            sphere.GetComponent<Rigidbody>().useGravity = false;
            GameObject.Destroy(sphere.transform.root.gameObject);
        }

        Spheres.Clear();

        Host.GetComponent<Host>().Hide();

        Options.SetActive(false);
        //CurrentLevelPanel.SetActive(false);
        checkForTutorialToggle = true;
        transparentPanelWasActive = true;
  
        transparentPlane.GetComponent<TransparentPlane>().Show();
        EditFoodPanel.SetActive(false);
        LevelSelectionPanel.SetActive(false);
        LevelCompletePanel.SetActive(false);
        LostPanel.SetActive(false);
        MainPanel.SetActive(true);
        PausePanel.SetActive(false);
      


    }



}
