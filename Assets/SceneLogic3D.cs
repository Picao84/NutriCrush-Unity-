using Assets;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
    Rigidbody sphereToAddForce;
    Vector3 forceToAddToSphere;
    Rigidbody selectedRigidBody;
    Vector3 originalScreenTargetPosition;
    GameObject[] foodBubbles = new GameObject[6];
    public ObservableCollection<Sphere> Spheres = new ObservableCollection<Sphere>();
    public ObservableCollection<Sphere> GhostSpheres = new ObservableCollection<Sphere>();
    GameObject transparentPlane;
    Vector2 lastFingerPosition;
    double lastFingerTime;
    TimerType timerType = TimerType.CountingUp;
    double firstFingerPositionTime;
    float lastSpeed;
    Vector2 startFingerPosition;
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
    public GameObject BottomPanel;
    public GameObject SettingsPanel;
    public GameObject CountingDown;
    public GameObject SickBar;
    public GameObject SickBarPotential;
    public GameObject CaloriesSickArea;
    public GameObject Options;
    public GameObject Rewards;
    public GameObject Plate;
    public GameObject TutorialHand;
    bool finishedMainTutorial = false;
    public GameObject PauseButtonCanvas;

    bool selectedHover;
    public GameObject CurrentLevelText;
    public GameplayState gamePlayState { get; private set; } = GameplayState.Single;
    GameObject selectedFoodOver;
    //Vector3 selectedFoodOverOriginalScale;
    Vector3 selectedFoodOverOriginalPosition;
    List<Vector3> comboFoodsOriginalScale = new List<Vector3>();
    Color32 sickColor = Color.red;
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
    public GameObject Fridge;
        

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
    bool pausedForTimerWithoutFood;
    bool showFoodAfterTutorial;
    bool timeRunning;
    bool canChoose = true;
    bool transparentPanelWasActive = true;
    public bool ballsPausedOnTutorial { get; private set; }
    public bool ballsPausedOnCombo { get; private set; }
    bool tutorialBallsAreIn = false;
    public Level CurrentLevel { get; private set; }

    public GameObject VisualFunnel;
    public Dictionary<Sphere, int> Touches = new Dictionary<Sphere, int>();
    bool anyDownTheVortex = false;
    Coroutine Timer;
    Coroutine FrozenTimer;
    Coroutine FrozenTimerCounter;
    int frozenCount = 5;
    int frozenColor = 0;
    public List<Food> CurrentShuffledDeck = new List<Food>();
    bool checkForTutorialToggle = true;
    public DataService dataService;

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
        //int difference = Screen.height - (int) safeArea.height;
        
        // Set the game's resolution to match the target width and height
        //Screen.SetResolution(Screen.width, targetHeight, true);


        TopPanel.SetActive(true);
        
#endif

        gameCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        var sensorSize = gameCamera.sensorSize;
        var aspectRatio = (float) Screen.currentResolution.width / (float) Screen.currentResolution.height;
        gameCamera.sensorSize = new Vector2(sensorSize.y * aspectRatio, sensorSize.y);
       
        

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

        var playerAbilities = dataService.GetPlayerAbilities().ToList();

        foreach( var ability in playerAbilities)
        {
            Constants.PlayerData.PlayerAbilities.Add((PlayerAbility)ability.Id, ability.Unlocked);
        }

        Constants.PlayerData.InitialiseFoodDeck();

        var levelDataBase = dataService.GetLevels();
        Constants.Levels = levelDataBase;

        var sectionsDatabase = dataService.GetSections();
        Constants.Sections = sectionsDatabase;

        messagesShown = dataService.GetTutorialMessages();

        PlayNextLevelButton.GetComponent<Button>().interactable = false;
        PauseButtonCanvas.SetActive(false);

        Constants.UserSettings = dataService.GetUserSettings();

        Music.volume = Constants.UserSettings.MusicLevel / 100f;
        SoundEffects.GetComponent<SoundEffects>().SetVolume(Constants.UserSettings.SoundLevel / 100f);
        //TestDatabaseUpdate();

        Constants.FoodBallTextures.Add(NutritionElementsEnum.Fat, Resources.Load<Texture2D>("fatBall"));
        Constants.FoodBallTextures.Add(NutritionElementsEnum.Saturates, Resources.Load<Texture2D>("saturatesBall"));
        Constants.FoodBallTextures.Add(NutritionElementsEnum.Salt, Resources.Load<Texture2D>("saltBall"));
        Constants.FoodBallTextures.Add(NutritionElementsEnum.Sugar, Resources.Load<Texture2D>("sugarBall"));

        Constants.FoodBubbleMaterials.Add(NutritionElementsEnum.Fat, Resources.Load<Material>("Material/BubbleFat"));
        Constants.FoodBubbleMaterials.Add(NutritionElementsEnum.Saturates, Resources.Load<Material>("Material/BubbleSaturates"));
        Constants.FoodBubbleMaterials.Add(NutritionElementsEnum.Salt, Resources.Load<Material>("Material/BubbleSalt"));
        Constants.FoodBubbleMaterials.Add(NutritionElementsEnum.Sugar, Resources.Load<Material>("Material/BubbleSugar"));


    }

    public void UpdateUserSettings()
    {
        Music.volume = Constants.UserSettings.MusicLevel / 100f;
        SoundEffects.GetComponent<SoundEffects>().SetVolume(Constants.UserSettings.SoundLevel / 100f);
    }

    public void SaveUserSettings()
    {
        dataService.UpdateUserSettings(Constants.UserSettings);

        Music.volume = Constants.UserSettings.MusicLevel / 100f;
        SoundEffects.GetComponent<SoundEffects>().SetVolume(Constants.UserSettings.SoundLevel / 100f);
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
        //SickImage.SetActive(true);
    }

    private void GameOver(string text)
    {
        Host.GetComponent<Host>().Hide();
        gameOver = true;
        gameOverText = text;
        //Options.SetActive(false);
        PauseButtonCanvas.SetActive(false);

        if (FrozenTimer != null)
        {
            StopCoroutine(FrozenTimer);
        }

        if (FrozenTimerCounter != null)
        {
            StopCoroutine(FrozenTimerCounter);
            frozenCount = 5;
            frozenColor = 0;
        }

        Analytics.LogEvent(new GameOverEvent { Level = CurrentLevel.Id, Reason = text });
    }

    public void PlayNextLevel()
    {
        PlayLevel(Constants.Levels[CurrentLevel.Id]);
    }

    private void UnlockPlayerAbility(PlayerAbility ability)
    {
        Constants.PlayerData.PlayerAbilities[ability] = 1;
        dataService.UpdatePlayerAbility(new PlayerAbilities { Id = (int)ability, Unlocked = 1 });
    }

    public void Reset()
    {
        canChoose = true;
        gamePaused = false;
        pausedBalls = false;

        foodsInCombo.Clear();
        gamePlayState = GameplayState.Single;

        ballsPausedOnTutorial = false;
        ballsPausedOnCombo = false;
        //SetHoleCapsuleCollider(false);

        if (FrozenTimer != null)
        {
            StopCoroutine(FrozenTimer);
        }

        if (FrozenTimerCounter != null)
        {
            StopCoroutine(FrozenTimerCounter);
            frozenCount = 5;
            frozenColor = 0;
        }

        var image = Resources.Load<Texture2D>("combo_closed");
        EnableCombo.GetComponent<SpriteRenderer>().sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), new Vector2(0.5f, 0.5f));

        foreach (GameObject foodBubble in foodBubbles)
        {
            foodBubble.GetComponent<FoodBubble>().GoBackToOriginalPosition(false);
        }

       

        Plate.GetComponent<PlateScript>().Reset();
        Plate.GetComponent<PlateScript>().Appear();
        Plate.GetComponent<PlateScript>().DeActivateCombo();
        Plate.transform.GetChild(0).gameObject.SetActive(true);
        Plate.transform.GetChild(1).gameObject.SetActive(true);


        var plateslots = Plate.GetComponentsInChildren<PlateSlotScript>();
        foreach (var slot in plateslots)
        {
            slot.Reset();
        }

        Plate.transform.GetChild(0).gameObject.SetActive(true);
        Plate.transform.GetChild(1).gameObject.SetActive(false);

        //Plate.SetActive(true);

        LevelSelectionPanel.SetActive(false);
        LevelCompletePanel.SetActive(false);
        EditFoodPanel.SetActive(false);
        BottomPanel.SetActive(false);
        LostPanel.SetActive(false);
        StarveImage.SetActive(false);
        SickImage.SetActive(false);
        Touches.Clear();
        VisualFunnel.GetComponent<Funnel>().ResetSpeed();
        VisualFunnel.GetComponent<Funnel>().ResumeRotation();

        SkipAndShuffle.GetComponent<SkipShuffle>().Reset();

        timerType = (TimerType)CurrentLevel.TimeCountingUp;

        var showPauseButton = true;

        if (CurrentLevel.AbilityUnlocked == (int)PlayerAbility.SkipAndShuffle && messagesShown.First(x => x.Id == (int)TutorialMessagesEnum.ToddlerTier).Showed == 0)
        {
            UnlockPlayerAbility(PlayerAbility.SkipAndShuffle);

            Tutorial.SetActive(true);
            PauseButtonCanvas.SetActive(false);
            showPauseButton = false;
            Tutorial.GetComponent<TutorialScript>().ShowWithTextGroup(Constants.TutorialMessages[TutorialMessagesEnum.ToddlerTier], 3);

            messagesShown.First(x => x.Id == (int)TutorialMessagesEnum.ToddlerTier).Showed = 1;
            dataService.UpdateTutorialMessages(messagesShown);

            pausedBalls = true;
            hostDelayed = true;
            pausedForTimerWithoutFood = true;

            if (Timer != null)
            {
                StopCoroutine(Timer);
            }
        }

        if (CurrentLevel.FoodExpires == 1 && messagesShown.First(x => x.Id == (int)TutorialMessagesEnum.ChildTier).Showed == 0)
        {
            Tutorial.SetActive(true);
            PauseButtonCanvas.SetActive(false);
            showPauseButton = false;
            Tutorial.GetComponent<TutorialScript>().ShowWithTextGroup(Constants.TutorialMessages[TutorialMessagesEnum.ChildTier], 3);

            messagesShown.First(x => x.Id == (int)TutorialMessagesEnum.ChildTier).Showed = 1;
            dataService.UpdateTutorialMessages(messagesShown);

            pausedBalls = true;
            hostDelayed = true;
            pausedForTimerWithoutFood = true;

            if (Timer != null)
            {
                StopCoroutine(Timer);
            }
        }

        if (CurrentLevel.AbilityUnlocked == (int)PlayerAbility.FoodEffects && messagesShown.First(x => x.Id == (int)TutorialMessagesEnum.TeenTier).Showed == 0)
        {
            UnlockPlayerAbility(PlayerAbility.FoodEffects);

            Tutorial.SetActive(true);
            PauseButtonCanvas.SetActive(false);
            showPauseButton = false;
            Tutorial.GetComponent<TutorialScript>().ShowWithTextGroup(Constants.TutorialMessages[TutorialMessagesEnum.TeenTier], 3);

            messagesShown.First(x => x.Id == (int)TutorialMessagesEnum.TeenTier).Showed = 1;
            dataService.UpdateTutorialMessages(messagesShown);

            pausedBalls = true;
            hostDelayed = true;
            pausedForTimerWithoutFood = true;

            if (Timer != null)
            {
                StopCoroutine(Timer);
            }
        }

        if (CurrentLevel.AbilityUnlocked == (int)PlayerAbility.Combo && messagesShown.First(x => x.Id == (int)TutorialMessagesEnum.YoungAdultTier).Showed == 0)
        {
            UnlockPlayerAbility(PlayerAbility.Combo);

            Tutorial.SetActive(true);
            PauseButtonCanvas.SetActive(false);
            showPauseButton = false;
            Tutorial.GetComponent<TutorialScript>().ShowWithTextGroup(Constants.TutorialMessages[TutorialMessagesEnum.YoungAdultTier], 3);

            messagesShown.First(x => x.Id == (int)TutorialMessagesEnum.YoungAdultTier).Showed = 1;
            dataService.UpdateTutorialMessages(messagesShown);

            pausedBalls = true;
            hostDelayed = true;
            pausedForTimerWithoutFood = true;

            if (Timer != null)
            {
                StopCoroutine(Timer);
            }
        }

        if (CurrentLevel.TimeCountingUp == (int)TimerType.CountingDown && messagesShown.First(x => x.Id == (int)TutorialMessagesEnum.AdultTier).Showed == 0)
        {
            Tutorial.SetActive(true);
            PauseButtonCanvas.SetActive(false);
            showPauseButton = false;
            Tutorial.GetComponent<TutorialScript>().ShowWithTextGroup(Constants.TutorialMessages[TutorialMessagesEnum.AdultTier], 3);

            messagesShown.First(x => x.Id == (int)TutorialMessagesEnum.AdultTier).Showed = 1;
            dataService.UpdateTutorialMessages(messagesShown);

            pausedBalls = true;
            hostDelayed = true;
            pausedForTimerWithoutFood = true;

            if (Timer != null)
            {
                StopCoroutine(Timer);
            }
        }

        if (CurrentLevel.EnergyUsage == 1 && messagesShown.First(x => x.Id == (int)TutorialMessagesEnum.MiddleAgedTier).Showed == 0)
        {
            Tutorial.SetActive(true);
            PauseButtonCanvas.SetActive(false);
            showPauseButton = false;
            Tutorial.GetComponent<TutorialScript>().ShowWithTextGroup(Constants.TutorialMessages[TutorialMessagesEnum.MiddleAgedTier], 3);

            messagesShown.First(x => x.Id == (int)TutorialMessagesEnum.MiddleAgedTier).Showed = 1;
            dataService.UpdateTutorialMessages(messagesShown);

            pausedBalls = true;
            hostDelayed = true;
            pausedForTimerWithoutFood = true;

            if (Timer != null)
            {
                StopCoroutine(Timer);
            }
        }

        if (CurrentLevel.AbilityUnlocked == (int)PlayerAbility.Fridge && messagesShown.First(x => x.Id == (int)TutorialMessagesEnum.RetiredTier).Showed == 0)
        {
            UnlockPlayerAbility(PlayerAbility.Fridge);

            Tutorial.SetActive(true);
            showPauseButton = false;
            PauseButtonCanvas.SetActive(false);
            Tutorial.GetComponent<TutorialScript>().ShowWithTextGroup(Constants.TutorialMessages[TutorialMessagesEnum.RetiredTier], 3);

            messagesShown.First(x => x.Id == (int)TutorialMessagesEnum.RetiredTier).Showed = 1;
            dataService.UpdateTutorialMessages(messagesShown);

            pausedBalls = true;
            hostDelayed = true;
            pausedForTimerWithoutFood = true;

            if (Timer != null)
            {
                StopCoroutine(Timer);
            }
        }

        if (CurrentLevel.RandomEffects == 1 && messagesShown.First(x => x.Id == (int)TutorialMessagesEnum.SeniorTier).Showed == 0)
        {
            Tutorial.SetActive(true);
            showPauseButton = false;
            PauseButtonCanvas.SetActive(false);
            Tutorial.GetComponent<TutorialScript>().ShowWithTextGroup(Constants.TutorialMessages[TutorialMessagesEnum.SeniorTier], 3);

            messagesShown.First(x => x.Id == (int)TutorialMessagesEnum.SeniorTier).Showed = 1;
            dataService.UpdateTutorialMessages(messagesShown);

            pausedBalls = true;
            hostDelayed = true;
            pausedForTimerWithoutFood = true;

            if (Timer != null)
            {
                StopCoroutine(Timer);
            }
        }

        if (CurrentLevel.SharedHealth == 1 && messagesShown.First(x => x.Id == (int)TutorialMessagesEnum.ElderTier).Showed == 0)
        {
            Tutorial.SetActive(true);
            showPauseButton = false;
            PauseButtonCanvas.SetActive(false);
            Tutorial.GetComponent<TutorialScript>().ShowWithTextGroup(Constants.TutorialMessages[TutorialMessagesEnum.ElderTier], 3);

            messagesShown.First(x => x.Id == (int)TutorialMessagesEnum.ElderTier).Showed = 1;
            dataService.UpdateTutorialMessages(messagesShown);

            pausedBalls = true;
            hostDelayed = true;
            pausedForTimerWithoutFood = true;

            if (Timer != null)
            {
                StopCoroutine(Timer);
            }
        }

        /*if (!hostDelayed)
        {
            Host.GetComponent<Host>().Show();
        }*/
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

        if (!pausedForTimerWithoutFood)
        {
            ShuffleDeck();
            StartupFood();

            if (Constants.PlayerData.PlayerAbilities[PlayerAbility.SkipAndShuffle] == 1)
            {
                SkipAndShuffle.SetActive(true);
                SkipAndShuffle.GetComponent<SkipShuffle>().Appear();
            }
            else
            {
                SkipAndShuffle.SetActive(false);
                SkipAndShuffle.GetComponent<SkipShuffle>().Disappear();
            }

            if (Constants.PlayerData.PlayerAbilities[PlayerAbility.Combo] == 1)
            {
                EnableCombo.SetActive(true);
            }
            else
            {
                EnableCombo.SetActive(false);
            }

            if (Constants.PlayerData.PlayerAbilities[PlayerAbility.Fridge] == 1)
            {
                Fridge.SetActive(true);
            }
            else
            {
                Fridge.SetActive(false);
            }

            if (Constants.PlayerData.PlayerAbilities[PlayerAbility.FoodEffects] == 1)
            {
                foreach (GameObject foodBubble in foodBubbles)
                {
                    foodBubble.GetComponent<FoodBubble>().EnableFoodEffects();
                }
            }

            foodChoices.SetActive(true);
            foreach (GameObject foodBubble in foodBubbles)
            {
                foodBubble.GetComponent<FoodBubble>().Show();
            }

        }
     
        CurrentFat.GetComponent<FillScript>().Reset(firstReset:!firstGameSet, fullReset: true);
        CurrentSaturates.GetComponent<FillScript>().Reset(firstReset:!firstGameSet, fullReset: true);
        CurrentSalt.GetComponent<FillScript>().Reset(firstReset:!firstGameSet, fullReset: true);
        CurrentSugar.GetComponent<FillScript>().Reset(firstReset:!firstGameSet, fullReset: true);
        CaloriesBar.GetComponent<CaloriesFill>().Reset(firstReset:!firstGameSet);
        SickBar.GetComponent<SickFill>().Reset(!firstGameSet);

        if (!firstGameSet)
        {
            firstGameSet = true;
        }

        canvas.enabled = false;

        if (!pausedBalls && !pausedForTimerWithoutFood)
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



            Plate.transform.GetChild(0).gameObject.SetActive(true);
            Plate.transform.GetChild(1).gameObject.SetActive(false);

            //Options.SetActive(true);
            if (showPauseButton)
            {
                PauseButtonCanvas.SetActive(true);
            }

        }
        //LEVEL COMPLETE TEST
        /*LevelCompletePanel.GetComponent<LevelCompleteScript>().SetFinishedLevelData(CurrentLevel.Id, GradesEnum.B, new Dictionary<string, int>
            {
                { "Avocado", 1 },
                { "Banana", 1},

            },
            new Dictionary<NutritionElementsEnum, int> {
                { NutritionElementsEnum.Fat, 85 },
                 { NutritionElementsEnum.Saturates, 40 },
                   { NutritionElementsEnum.Salt, 90 },
                    { NutritionElementsEnum.Sugar, 75 },
            }, 80, new TimeSpan(0, 0, 4, 00, 0)
        );
        LevelCompletePanel.SetActive(true);
        return;*/
    }

    private Tuple<GradesEnum, Dictionary<NutritionElementsEnum, int>, int> CalculateGrade()
    {
        var fatScript = CurrentFat.GetComponent<FillScript>();
        var fatRatio = fatScript.currentAmount / fatScript.MaxAmount;
        var saturatesScript = CurrentSaturates.GetComponent<FillScript>();
        var saturatesRatio = saturatesScript.currentAmount / saturatesScript.MaxAmount;
        var saltScript = CurrentSalt.GetComponent<FillScript>();
        var saltRatio = saltScript.currentAmount / saltScript.MaxAmount;
        var sugarScript = CurrentSugar.GetComponent<FillScript>();
        var sugarRatio = sugarScript.currentAmount / sugarScript.MaxAmount;
        float timeRatioForScore = 0;

        var percentages = new Dictionary<NutritionElementsEnum, int>
        {
            { NutritionElementsEnum.Fat, (int) (fatRatio * 100) },
             { NutritionElementsEnum.Saturates, (int) (saturatesRatio * 100) },
              { NutritionElementsEnum.Salt, (int) (saltRatio * 100) },
               { NutritionElementsEnum.Sugar, (int) (sugarRatio * 100) },
        };

        double timerRatio = 1;

        var objective = CurrentLevel.Time;
        var achieved = TimeLeft.TotalSeconds;

        if (timerType == TimerType.CountingUp)
        {
            timerRatio = objective / achieved;
        }
        else
        {
            timerRatio = (objective - achieved) / achieved;
        }

        timeRatioForScore = (float) ((objective - achieved) / achieved);

        if(timeRatioForScore > 1)
        {
            timeRatioForScore = 1;
        }

        var average = (fatRatio + saturatesRatio + saltRatio + sugarRatio + timeRatioForScore) / 5;
         
        GradesEnum result = average switch
        {
            > 0.9f => GradesEnum.A, 
            > 0.5f => GradesEnum.B,
            _ => GradesEnum.C,
        };

        return new Tuple<GradesEnum, Dictionary<NutritionElementsEnum, int>, int>(result, percentages, (int)(timerRatio * 100));

    }

    public void ContinueTutorial()
    {
        Tutorial.SetActive(false);

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
        PauseButtonCanvas.SetActive(true);
        /*if (hostDelayed)
        {
            Host.GetComponent<Host>().Show();
            hostDelayed = false;
        }*/

        VisualFunnel.GetComponent<Funnel>().ResumeRotation();

        if (pausedForTimerWithoutFood)
        {
            pausedForTimerWithoutFood = false;
            anyDownTheVortex = false;
            Touches.Clear();

            //transparentPanelWasActive = true;
            //transparentPlane.SetActive(true);
            //transparentPlane.GetComponent<TransparentPlane>().Show();

            Plate.GetComponent<PlateScript>().Appear();
            Plate.transform.GetChild(0).GetComponent<PlateSlotScript>().Reset();
            Plate.transform.GetChild(0).gameObject.SetActive(true);

            //Host.GetComponent<Host>().Show();
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

            ShuffleDeck();
            StartupFood();

            if (Constants.PlayerData.PlayerAbilities[PlayerAbility.SkipAndShuffle] == 1)
            {
                SkipAndShuffle.SetActive(true);
                SkipAndShuffle.GetComponent<SkipShuffle>().Appear();
            }
            else
            {
                SkipAndShuffle.SetActive(false);
                SkipAndShuffle.GetComponent<SkipShuffle>().Disappear();
            }

            if (Constants.PlayerData.PlayerAbilities[PlayerAbility.Combo] == 1)
            {
                EnableCombo.SetActive(true);
            }
            else
            {
                EnableCombo.SetActive(false);
            }

            if (Constants.PlayerData.PlayerAbilities[PlayerAbility.Fridge] == 1)
            {
                Fridge.SetActive(true);
            }
            else
            {
                Fridge.SetActive(false);
            }

            if (Constants.PlayerData.PlayerAbilities[PlayerAbility.FoodEffects] == 1)
            {
                foreach (GameObject foodBubble in foodBubbles)
                {
                    foodBubble.GetComponent<FoodBubble>().EnableFoodEffects();
                }
            }

            foodChoices.SetActive(true);
            foreach (GameObject foodBubble in foodBubbles)
            {
                foodBubble.GetComponent<FoodBubble>().Show();
            }
        }
        else
        {
            if (showFoodAfterTutorial)
            {
                GetNextFood();

                showFoodAfterTutorial = false;
                Plate.GetComponent<PlateScript>().Appear();
                Plate.transform.GetChild(0).GetComponent<PlateSlotScript>().Reset();
                Plate.transform.GetChild(0).gameObject.SetActive(true);

                foodChoices.SetActive(true);
                foreach (GameObject foodBubble in foodBubbles)
                {
                    foodBubble.GetComponent<FoodBubble>().Show();
                }

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
        }
    }

    public void EnableFoodSelection()
    {
        canSelectFood = true;
        TutorialHand.SetActive(true);
        TutorialHand.GetComponent<TutorialHandScript>().SetPath(foodBubbles.First(x => x.name == "FoodFour").transform.position, Plate.transform.position);
           
    }

    public void ReEnableTutorialPathHand(Sphere sphere)
    {
        Vector3 end = Vector3.zero;

        switch (sphere.element)
        {
            case NutritionElementsEnum.Fat:

                var fatPotPosition = GameObject.Find("Red").transform.GetChild(0).transform.position;
                var aboveFatPotPosition = new Vector3(fatPotPosition.x - 0.2f, fatPotPosition.y, fatPotPosition.z + 1);

                end = aboveFatPotPosition - new Vector3(0, 0, 0.1f);

                break;

            case NutritionElementsEnum.Saturates:

                var saturatesPotPosition = GameObject.Find("Green").transform.GetChild(0).transform.position;

                var aboveSaturatesPotPosition = new Vector3(saturatesPotPosition.x, saturatesPotPosition.y, saturatesPotPosition.z + 1);

                end = aboveSaturatesPotPosition - new Vector3(0, 0, 0.1f);

                break;

            case NutritionElementsEnum.Salt:

                var saltPotPosition = GameObject.Find("Orange").transform.GetChild(0).transform.position;

                var aboveSaltPotPosition = new Vector3(saltPotPosition.x, saltPotPosition.y, saltPotPosition.z + 1);

                end = aboveSaltPotPosition - new Vector3(0, 0, 0.1f);

                break;

            case NutritionElementsEnum.Sugar:

                var sugarPotPosition = GameObject.Find("Purple").transform.GetChild(0).transform.position;

                var aboveSugarPotPosition = new Vector3(sugarPotPosition.x, sugarPotPosition.y, sugarPotPosition.z + 1);

                end = aboveSugarPotPosition - new Vector3(0, 0, 0.1f);

                break;
        }

        TutorialHand.GetComponent<TutorialHandScript>().SetPath(sphere.transform.position, end, true);
        TutorialHand.SetActive(true);
    }

    public void ContinueTutorial(int step)
    {
        if (step == 2)
        {    
            foodChoices.SetActive(true);
            Plate.GetComponent<PlateScript>().Appear();

            StartupFood();
            foreach (GameObject foodBubble in foodBubbles.Where(x => x.GetComponent<FoodBubble>().Food != null))
            {
                foodBubble.GetComponent<FoodBubble>().Show(true);
            }

        }

       
        if (step == 3)
        {
            pausedBalls = true;
            foreach (var sphere in Spheres)
            {
                sphere.gameObject.GetComponent<Rigidbody>().useGravity = false;
                sphere.PauseRotation();
            }
            VisualFunnel.GetComponent<Funnel>().PauseRotation();

           
        }

        if(step == 4)
        {
            TutorialHand.SetActive(true);

            var nextBall = Spheres[0];
            Vector3 end = Vector3.zero;

            switch (nextBall.GetComponent<Sphere>().element)
            {
                case NutritionElementsEnum.Fat:

                    var fatPotPosition = GameObject.Find("Red").transform.GetChild(0).transform.position;
                    var aboveFatPotPosition = new Vector3(fatPotPosition.x - 0.2f, fatPotPosition.y, fatPotPosition.z + 1);

                    end = aboveFatPotPosition - new Vector3(0, 0, 0.1f);

                    break;

                case NutritionElementsEnum.Saturates:

                    var saturatesPotPosition = GameObject.Find("Green").transform.GetChild(0).transform.position;

                    var aboveSaturatesPotPosition = new Vector3(saturatesPotPosition.x, saturatesPotPosition.y, saturatesPotPosition.z + 1);

                    end = aboveSaturatesPotPosition - new Vector3(0, 0, 0.1f);

                    break;

                case NutritionElementsEnum.Salt:

                    var saltPotPosition = GameObject.Find("Orange").transform.GetChild(0).transform.position;

                    var aboveSaltPotPosition = new Vector3(saltPotPosition.x, saltPotPosition.y, saltPotPosition.z + 1);

                    end = aboveSaltPotPosition - new Vector3(0, 0, 0.1f);

                    break;

                case NutritionElementsEnum.Sugar:

                    var sugarPotPosition = GameObject.Find("Purple").transform.GetChild(0).transform.position;

                    var aboveSugarPotPosition = new Vector3(sugarPotPosition.x, sugarPotPosition.y, sugarPotPosition.z + 1);

                    end = aboveSugarPotPosition - new Vector3(0, 0, 0.1f);

                    break;
            }

            TutorialHand.GetComponent<TutorialHandScript>().SetPath(nextBall.transform.position, end, true);
            tutorialBallsAreIn = true;

            ballsPausedOnTutorial = true;
        }

        if (step == 5)
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

            //Options.SetActive(true);
            PauseButtonCanvas.SetActive(true);

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

        CurrentLevelText.GetComponent<TextMeshPro>().text = $"lvl {level.Id.ToString()}"; 

        SickBar.GetComponent<SickFill>().MaxAmount = (level.MaxFat + level.MaxSaturates + level.MaxSalt + level.MaxSugar) / 2;
        SickBarPotential.GetComponent<SickFill>().MaxAmount = (CurrentLevel.MaxFat + CurrentLevel.MaxSaturates + CurrentLevel.MaxSalt + CurrentLevel.MaxSugar) / 2;

        Reset();
    }

    private void SetHoleCapsuleCollider(bool isTutorial = false)
    {
        if (isTutorial)
        {
            GameObject.Find("Red").GetComponentInChildren<CapsuleCollider>().height = 5;
            GameObject.Find("Green").GetComponentInChildren<CapsuleCollider>().height = 5;
            GameObject.Find("Orange").GetComponentInChildren<CapsuleCollider>().height = 5;
            GameObject.Find("Purple").GetComponentInChildren<CapsuleCollider>().height = 5;

            GameObject.Find("Red").GetComponentInChildren<CapsuleCollider>().radius = 0.5f;
            GameObject.Find("Green").GetComponentInChildren<CapsuleCollider>().radius = 0.5f;
            GameObject.Find("Orange").GetComponentInChildren<CapsuleCollider>().radius = 0.5f;
            GameObject.Find("Purple").GetComponentInChildren<CapsuleCollider>().radius = 0.5f;
        }
        else
        {
            GameObject.Find("Red").GetComponentInChildren<CapsuleCollider>().height = 0.5f;
            GameObject.Find("Green").GetComponentInChildren<CapsuleCollider>().height = 0.5f;
            GameObject.Find("Orange").GetComponentInChildren<CapsuleCollider>().height = 0.5f;
            GameObject.Find("Purple").GetComponentInChildren<CapsuleCollider>().height = 0.5f;

            GameObject.Find("Red").GetComponentInChildren<CapsuleCollider>().radius = 0.2f;
            GameObject.Find("Green").GetComponentInChildren<CapsuleCollider>().radius = 0.2f;
            GameObject.Find("Orange").GetComponentInChildren<CapsuleCollider>().radius = 0.2f;
            GameObject.Find("Purple").GetComponentInChildren<CapsuleCollider>().radius = 0.2f;
        }

    }

    public async void StartGame(bool isTutorial)
    {
        if (Constants.Levels.Count(x => x.Unlocked) == 1 || isTutorial)
        {
            Tutorial.SetActive(true);
            Plate.GetComponent<PlateScript>().Reset();
            Plate.GetComponent<PlateScript>().DeActivateCombo();
            Plate.transform.GetChild(0).gameObject.SetActive(true);
            Plate.transform.GetChild(1).gameObject.SetActive(true);

            //SetHoleCapsuleCollider(true);
            finishedMainTutorial = false;
            tutorialBallsAreIn = false;
            canSelectFood = false;

            ballsPausedOnTutorial = false;
            ballsPausedOnCombo = false;
            gamePlayState = GameplayState.Single;

            var plateslots = Plate.GetComponentsInChildren<PlateSlotScript>();
            foreach (var slot in plateslots)
            {
                slot.Reset();
            }

            Plate.transform.GetChild(1).gameObject.SetActive(false);

            state = StateMachine.Tutorial;
            TutorialHand.GetComponent<TutorialHandScript>().Reset();
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

          
            
            ShuffleDeck();

            pausedBalls = false;
           

            SkipAndShuffle.SetActive(false);
            SkipAndShuffle.GetComponent<SkipShuffle>().Disappear();
            CurrentLevel = Constants.Levels[0];

            TimeLeft = TimeSpan.FromSeconds(0);
            TimeText.GetComponent<TextMeshPro>().text = $"{(int)TimeLeft.TotalMinutes}:{TimeLeft.Seconds:00}";
            CurrentLevelText.GetComponent<TextMeshPro>().text = $"lvl {CurrentLevel.Id.ToString()}";

            if (Constants.PlayerData.PlayerAbilities[PlayerAbility.SkipAndShuffle] == 1)
            {
                SkipAndShuffle.SetActive(true);
                SkipAndShuffle.GetComponent<SkipShuffle>().Appear();
            }
            else
            {
                SkipAndShuffle.SetActive(false);
                SkipAndShuffle.GetComponent<SkipShuffle>().Disappear();
            }

            if (Constants.PlayerData.PlayerAbilities[PlayerAbility.Combo] == 1)
            {
                EnableCombo.SetActive(true);
            }
            else
            {
                EnableCombo.SetActive(false);
            }

            if (Constants.PlayerData.PlayerAbilities[PlayerAbility.Fridge] == 1)
            {
                Fridge.SetActive(true);
            }
            else
            {
                Fridge.SetActive(false);
            }

            if (Constants.PlayerData.PlayerAbilities[PlayerAbility.FoodEffects] == 1)
            {
                foreach (GameObject foodBubble in foodBubbles)
                {
                    foodBubble.GetComponent<FoodBubble>().EnableFoodEffects();
                }
            }

            //CurrentLevelPanel.SetActive(true);
            //CurrentLevelPanel.GetComponent<CurrentLevelPanelScript>().SetCurrentLevel(CurrentLevel);

           
           

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

            SickBar.GetComponent<SickFill>().MaxAmount = (CurrentLevel.MaxFat + CurrentLevel.MaxSaturates + CurrentLevel.MaxSalt + CurrentLevel.MaxSugar) / 2;
            SickBarPotential.GetComponent<SickFill>().MaxAmount = (CurrentLevel.MaxFat + CurrentLevel.MaxSaturates + CurrentLevel.MaxSalt + CurrentLevel.MaxSugar) / 2;
            SickBar.GetComponent<SickFill>().Reset(firstReset: !firstGameSet);

            canvas.enabled = false;
            LevelSelectionPanel.SetActive(false);
            MainPanel.SetActive(false);
            LostPanel.SetActive(false);
            BottomPanel.SetActive(false);

            CurrentFat.GetComponent<FillScript>().Reset(firstReset: !firstGameSet, fullReset: true);
            CurrentSaturates.GetComponent<FillScript>().Reset(firstReset: !firstGameSet, fullReset: true);
            CurrentSalt.GetComponent<FillScript>().Reset(firstReset: !firstGameSet, fullReset: true);
            CurrentSugar.GetComponent<FillScript>().Reset(firstReset: !firstGameSet, fullReset: true);
            CaloriesBar.GetComponent<CaloriesFill>().Reset(firstReset: !firstGameSet);

            Tutorial.GetComponent<TutorialScript>().Show();
            
        }
        else
        {
            //LEVEL COMPLETE TEST
            /*LevelCompletePanel.GetComponent<LevelCompleteScript>().SetFinishedLevelData(1, GradesEnum.B, new Dictionary<string, int>
                {
                    { "Avocado", 1 },
                    { "Banana", 1},

                },
                new Dictionary<NutritionElementsEnum, int> {
                    { NutritionElementsEnum.Fat, 85 },
                     { NutritionElementsEnum.Saturates, 40 },
                       { NutritionElementsEnum.Salt, 90 },
                        { NutritionElementsEnum.Sugar, 75 },
                }, 80, new TimeSpan(0, 0, 4, 00, 0)
            );
            LevelCompletePanel.SetActive(true);
            return;*/


            Tutorial.SetActive(false);
            state = StateMachine.NormalPlay;
            LostPanel.SetActive(false);

            transparentPanelWasActive = true;
            transparentPlane.GetComponent<TransparentPlane>().Show();
            MainPanel.SetActive(false);
            BottomPanel.SetActive(true);
            LevelSelectionPanel.SetActive(true);
        }
    }

    public void OpenSettings()
    {
        SettingsPanel.SetActive(true);
    }

    /*private void ReduceEffects()
    {
        if (ActiveEffects.Any())
        {
            if(!ActiveEffects.Any(x => x.Key == FoodEffects.SugarRush))
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
                        case FoodEffects.SugarFriendly:

                         

                            break;

                        case FoodEffects.AccelerateFat:
                        case FoodEffects.FatBurning:

                         

                            break;

                        case FoodEffects.AccelerateSaturates:
                        case FoodEffects.HeartHealthy:


                            break;

                        case FoodEffects.AccelerateSalt:
                        case FoodEffects.Hydration:

                          

                            break;

                      

                    }
                }
            }

            

            ActiveEffects = newActiveEffects;
        }
    }*/

    private async void Spheres_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (((ObservableCollection<Sphere>)sender).Count == 0)
        {
            ballsPausedOnTutorial = false;
            //SetHoleCapsuleCollider(false);

            CurrentFat.GetComponent<FillScript>().StopUsingEnergy();
            CurrentSaturates.GetComponent<FillScript>().StopUsingEnergy();
            CurrentSalt.GetComponent<FillScript>().StopUsingEnergy();
            CurrentSugar.GetComponent<FillScript>().StopUsingEnergy();

            if (state == StateMachine.Tutorial)
            {
                //gamePlayState = GameplayState.Single;

                if (!finishedMainTutorial)
                {
                    finishedMainTutorial = true;
                    PauseButtonCanvas.SetActive(false);
                    Tutorial.GetComponent<TutorialScript>().ShowWithTextGroup(new List<string> { "Paws-ing for applause! Let's turn on the blender. Don't let the balls get shredded!" });
                    pausedForTimerWithoutFood = false;
                    TutorialHand.GetComponent<TutorialHandScript>().Stop();
                    showFoodAfterTutorial = true;
                    if(Timer != null)
                    {
                        StopCoroutine(Timer);
                    }
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
                            Plate.transform.GetChild(0).gameObject.SetActive(false);
                            Plate.transform.GetChild(1).gameObject.SetActive(true);
                        }
                        else
                        {
                            Plate.transform.GetChild(0).gameObject.SetActive(true);
                            Plate.transform.GetChild(1).gameObject.SetActive(false);
                        }

                        ResetPots();

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
                                TimeLeft += TimeSpan.FromSeconds(3);
                            }
                            else
                            {
                                TimeLeft -= TimeSpan.FromSeconds(3);
                            }

                            /*if(messagesShown.First(x => x.Id == (int) TutorialMessagesEnum.Flawless + 1).Showed == 0)
                            {
                                Tutorial.GetComponent<TutorialScript>().ShowWithTextGroup(Constants.TutorialMessages[TutorialMessagesEnum.Flawless], 3);

                                messagesShown.First(x => x.Id == (int)TutorialMessagesEnum.Flawless + 1).Showed = 1;
                                dataService.UpdateTutorialMessages(messagesShown);

                                pausedBalls = true;
                                hostDelayed = true;
                            }*/
                        }

                        anyDownTheVortex = false;
                        Touches.Clear();

                        transparentPanelWasActive = true;
        
                        transparentPlane.GetComponent<TransparentPlane>().Show();
                        foreach (GameObject foodBubble in foodBubbles.Where(x => x.GetComponent<FoodBubble>().Food != null))
                        {                          
                            foodBubble.GetComponent<FoodBubble>().Show(true);
                        }

                        //ReduceEffects();

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

                    if (gamePlayState == GameplayState.Combo)
                    {
                        Plate.GetComponent<PlateScript>().ActivateCombo();
                        Plate.transform.GetChild(0).gameObject.SetActive(false);
                        Plate.transform.GetChild(1).gameObject.SetActive(true);
                    }
                    else
                    {
                        Plate.transform.GetChild(0).gameObject.SetActive(true);
                        Plate.transform.GetChild(1).gameObject.SetActive(false);
                    }

                    ResetPots();

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
                            TimeLeft += TimeSpan.FromSeconds(3);
                        }
                        else
                        {
                            TimeLeft -= TimeSpan.FromSeconds(3);
                        }

                        /*if (messagesShown.First(x => x.Id == (int)TutorialMessagesEnum.Flawless + 1).Showed == 0)
                        {
                            Tutorial.GetComponent<TutorialScript>().ShowWithTextGroup(Constants.TutorialMessages[TutorialMessagesEnum.Flawless], 3);

                            messagesShown.First(x => x.Id == (int)TutorialMessagesEnum.Flawless + 1).Showed = 1;
                            dataService.UpdateTutorialMessages(messagesShown);

                            pausedBalls = true;
                            hostDelayed = true;
                        }*/
                    }

                    anyDownTheVortex = false;
                    Touches.Clear();

                    transparentPanelWasActive = true;
          
                    transparentPlane.GetComponent<TransparentPlane>().Show();
                    foreach (GameObject foodBubble in foodBubbles.Where(x => x.GetComponent<FoodBubble>().Food != null))
                    {
                        foodBubble.GetComponent<FoodBubble>().Show(true);
                    }

                    //ReduceEffects();

                    if (Constants.PlayerData.PlayerAbilities[PlayerAbility.SkipAndShuffle] == 1)
                    {
                        //SkipAndShuffle.SetActive(true);
                        SkipAndShuffle.GetComponent<SkipShuffle>().Appear();
                    }

                    if (Constants.PlayerData.PlayerAbilities[PlayerAbility.Combo] == 1)
                    {
                        EnableCombo.SetActive(true);
                    }
                    else
                    {
                        EnableCombo.SetActive(false);
                    }

                    if (Constants.PlayerData.PlayerAbilities[PlayerAbility.Fridge] == 1)
                    {
                        Fridge.SetActive(true);
                    }
                    else
                    {
                        Fridge.SetActive(false);
                    }


                    if (Constants.PlayerData.PlayerAbilities[PlayerAbility.FoodEffects] == 1)
                    {
                        foreach (GameObject foodBubble in foodBubbles)
                        {
                            foodBubble.GetComponent<FoodBubble>().EnableFoodEffects();
                        }
                    }

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


                    //SkipAndShuffle.GetComponent<SkipShuffle>().ReduceCooldown();
                   

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
            if(state == StateMachine.Tutorial && tutorialBallsAreIn)
            {
                var nextBall = Spheres[0];
                Vector3 end = Vector3.zero;

                switch (nextBall.GetComponent<Sphere>().element)
                {
                    case NutritionElementsEnum.Fat:

                        var fatPotPosition = GameObject.Find("Red").transform.GetChild(0).transform.position;
                        var aboveFatPotPosition = new Vector3(fatPotPosition.x - 0.2f, fatPotPosition.y, fatPotPosition.z + 1);

                        end = aboveFatPotPosition - new Vector3(0, 0, 0.1f);

                        break;

                    case NutritionElementsEnum.Saturates:

                        var saturatesPotPosition = GameObject.Find("Green").transform.GetChild(0).transform.position;

                        var aboveSaturatesPotPosition = new Vector3(saturatesPotPosition.x, saturatesPotPosition.y, saturatesPotPosition.z + 1);

                        end = aboveSaturatesPotPosition - new Vector3(0, 0, 0.1f);

                        break;

                    case NutritionElementsEnum.Salt:

                        var saltPotPosition = GameObject.Find("Orange").transform.GetChild(0).transform.position;

                        var aboveSaltPotPosition = new Vector3(saltPotPosition.x, saltPotPosition.y, saltPotPosition.z + 1);

                        end = aboveSaltPotPosition - new Vector3(0, 0, 0.1f);

                        break;
                         
                    case NutritionElementsEnum.Sugar:

                        var sugarPotPosition = GameObject.Find("Purple").transform.GetChild(0).transform.position;

                        var aboveSugarPotPosition = new Vector3(sugarPotPosition.x, sugarPotPosition.y, sugarPotPosition.z + 1);

                        end = aboveSugarPotPosition - new Vector3(0, 0, 0.1f);

                        break;
                }

                TutorialHand.GetComponent<TutorialHandScript>().SetPath(nextBall.transform.position, end, true);
                TutorialHand.SetActive(true);
            }
        }
    }

    public void FinishLevel()
    {
        if (FrozenTimer != null)
        {
            StopCoroutine(FrozenTimer);
        }

        if (FrozenTimerCounter != null)
        {
            StopCoroutine(FrozenTimerCounter);
            frozenCount = 5;
            frozenColor = 0;
        }

        if (Timer != null)
        {
            StopCoroutine(Timer);
        }
        Music.Pause();
        SoundEffects.GetComponent<SoundEffects>().PlayWin();
        canvas.enabled = true;
        BottomPanel.SetActive(false);
        MainPanel.SetActive(false);
        LostPanel.SetActive(false);
        transparentPanelWasActive = true;
        Plate.GetComponent<PlateScript>().Disappear();

        transparentPlane.GetComponent<TransparentPlane>().Show();
      
        SoundEffects.GetComponent<SoundEffects>().PlayWin();
        var grade = CalculateGrade();

        Analytics.LogEvent(new FinishedLevelEvent { Level = CurrentLevel.Id, Grade = grade.ToString() });

        //Options.SetActive(false);
        PauseButtonCanvas.SetActive(false);

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
        
        for (int i = 0; i < GhostSpheres.Count; i++)
        {
            Destroy(GhostSpheres[i].gameObject);
        }

        GhostSpheres.Clear();

            if (CurrentLevel != null)
            {
                //PlayNextLevelButton.GetComponent<Button>().interactable = true;


                Dictionary<string, int> rewards = new Dictionary<string, int>();

                for (int i = 3; i >= (int)grade.Item1; i--)
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

                LevelCompletePanel.GetComponent<LevelCompleteScript>().SetFinishedLevelData(CurrentLevel.Id, grade.Item1, rewards, grade.Item2, grade.Item3, TimeLeft);
                LevelCompletePanel.SetActive(true);

                //Rewards.GetComponent<RewardsScript>().SetRewards(rewards);

                Constants.PlayerData.InitialiseFoodDeck();

                if (CurrentLevel.Id != Constants.Levels.Last().Id && !Constants.Levels.First(x => x.Id == CurrentLevel.Id + 1).Unlocked)
                {
                    Constants.Levels.First(x => x.Id == CurrentLevel.Id + 1).Unlocked = true;
                    dataService.StoreUnlockedLevel(CurrentLevel.Id + 1);
                }

                if (Constants.Levels.First(x => x.Id == CurrentLevel.Id).MaxGrade > (int)grade.Item1 || Constants.Levels.First(x => x.Id == CurrentLevel.Id).MaxGrade == 0)
                {
                    Constants.Levels.First(x => x.Id == CurrentLevel.Id).MaxGrade = (int)grade.Item1;
                    dataService.UpdateLevelMaxGrade(CurrentLevel.Id, (int)grade.Item1);
                }

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


                //var levelList = dataService.GetLevels();
                //Constants.Levels = levelList.ToList();


                //var sectionsDatabase = dataService.GetSections();
                //Constants.Sections = sectionsDatabase.ToList();
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

    private void MakeTextRedAndBold(TextMeshPro textMeshPro)
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
                if (!ballsPausedOnTutorial)
                {
                    for (int i = 0; i < Spheres.Count; i++)
                    {
                        Spheres[i].GetComponent<Rigidbody>().useGravity = false;
                        Spheres[i].GetComponent<Rigidbody>().isKinematic = true;
                        Spheres[i].GetComponent<Sphere>().PauseRotation();
                    }

                    for (int i = 0; i < GhostSpheres.Count; i++)
                    {
                        GhostSpheres[i].GetComponent<Rigidbody>().useGravity = false;
                        GhostSpheres[i].GetComponent<Sphere>().PauseRotation();
                    }
                }   
                else
                {
                    for (int i = 0; i < Spheres.Count; i++)
                    {
                      
                        Spheres[i].GetComponent<Rigidbody>().isKinematic = false;
                        Spheres[i].GetComponent<Sphere>().PauseRotation();
                    }

                    for (int i = 0; i < GhostSpheres.Count; i++)
                    {
                        GhostSpheres[i].GetComponent<Rigidbody>().useGravity = false;
                        GhostSpheres[i].GetComponent<Sphere>().PauseRotation();
                    }
                }
            }
            else
            {
                if (!ballsPausedOnCombo)
                {

                    for (int i = 0; i < Spheres.Count; i++)
                    {
                        if (!Spheres[i].wasConsumed && !Spheres[i].isPicked)
                        {
                            Spheres[i].isOnComboPause = false;
                            Spheres[i].gameObject.GetComponent<Rigidbody>().useGravity = true;
                            Spheres[i].GetComponent<Rigidbody>().isKinematic = false;

                        }
                    }

                    for (int i = 0; i < GhostSpheres.Count; i++)
                    {
                        GhostSpheres[i].GetComponent<Rigidbody>().useGravity = true;
                    }
            }
        }
        

        if (gameOver && canvas.enabled == false)
        {
            transparentPanelWasActive = true;
          
            transparentPlane.GetComponent<TransparentPlane>().Show();
            status.SetActive(false);
            StopCoroutine(Timer);
            for (int i = 0; i < Spheres.Count; i++)
            {
                Destroy(Spheres[i].gameObject);
            }

            for (int i = 0; i < GhostSpheres.Count; i++)
            {
                Destroy(GhostSpheres[i].gameObject);
            }

            Spheres.Clear();
            GhostSpheres.Clear();

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
            BottomPanel.SetActive(false);
        }

        if (TimeText != null)
        {
            TimeText.GetComponent<TextMeshPro>().text = $"{(int)TimeLeft.TotalMinutes}:{TimeLeft.Seconds:00}";
        }

        if (gameCamera != null && !canvas.enabled)
        {

            if (!pausedBalls || (pausedBalls && ballsPausedOnTutorial))
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

                                TutorialHand.GetComponent<TutorialHandScript>().Stop();

                                switch (sphere.element)
                                {
                                    case NutritionElementsEnum.Fat:

                                        var currentFat = CurrentFat.GetComponent<FillScript>();

                                        if (currentFat.currentAmount + ((sphere.elementQuantity * CurrentLevel.Multiplier) * currentFat.amountToApply)  > currentFat.MaxAmount)
                                        {
                                            currentFat.CloseLid();
                                        }
                                        else
                                        {
                                            currentFat.RemoveLid();
                                        }

                                        break;

                                    case NutritionElementsEnum.Saturates:


                                        var currentSaturates = CurrentSaturates.GetComponent<FillScript>();

                                        if (currentSaturates.currentAmount + ((sphere.elementQuantity * CurrentLevel.Multiplier) * currentSaturates.amountToApply) > currentSaturates.MaxAmount)
                                        {
                                            currentSaturates.CloseLid();
                                        }
                                        else
                                        {
                                            currentSaturates.RemoveLid();
                                        }

                                        break;

                                    case NutritionElementsEnum.Salt:

                                        var currentSalt = CurrentSalt.GetComponent<FillScript>();

                                        if (currentSalt.currentAmount + ((sphere.elementQuantity * CurrentLevel.Multiplier) * currentSalt.amountToApply) > currentSalt.MaxAmount)
                                        {
                                            currentSalt.CloseLid();
                                        }
                                        else
                                        {
                                            currentSalt.RemoveLid();
                                        }

                                        break;

                                    case NutritionElementsEnum.Sugar:

                                        var currentSugar = CurrentSugar.GetComponent<FillScript>();

                                        if (currentSugar.currentAmount + ((sphere.elementQuantity * CurrentLevel.Multiplier) * currentSugar.amountToApply) > currentSugar.MaxAmount)
                                        {
                                            currentSugar.CloseLid();
                                        }
                                        else
                                        {
                                            currentSugar.RemoveLid();
                                        }

                                        break;
                                }
                            }


                        }
                    }
                }
                else
                {
                    /*if (selectedRigidBody != null && selectedRigidBody.GetComponent<Sphere>() != null)
                    {

                        var sphere = selectedRigidBody.GetComponent<Sphere>();
                        sphere.isPicked = false;
                        selectedRigidBody.useGravity = true;
                        selectedRigidBody = null;
                    }*/
                }

                if (Pointer.current.press.wasPressedThisFrame)
                {
                    GetFoodPicked();
                }

            }

        }
        else
        {
            if(gameCamera != null && canvas.enabled)
            {
                if (Pointer.current.press.wasPressedThisFrame)
                {
                    GetFoodPicked();
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (canvas.isActiveAndEnabled)
            return;


        if(sphereToAddForce != null)
        {
            if (sphereToAddForce.isKinematic)
            {
                sphereToAddForce.AddForce(forceToAddToSphere, ForceMode.Impulse);
            }

            sphereToAddForce.isKinematic = false;
            sphereToAddForce = null;
            forceToAddToSphere = Vector3.zero;
        }

        if ((selectedRigidBody != null && !pausedBalls) || (selectedRigidBody != null && pausedBalls && ballsPausedOnTutorial))
        {
            var finger = Touch.fingers[0];
            var currentTouch = finger.touchHistory.First();
           
            if (finger.currentTouch.valid)
            {
                var mode = finger.currentTouch.phase;
                var distance = Vector2.Distance(finger.currentTouch.screenPosition, lastFingerPosition);
                var speed = (float)(distance / (finger.currentTouch.time - lastFingerTime));
                var currentTouchToWorldPoint = GetWorldPositionOnPlane(currentTouch.screenPosition, selectedRigidBody.GetComponent<Sphere>().initialPosition.y);

                if (mode == UnityEngine.InputSystem.TouchPhase.Moved) 
                {
                    if (Mathf.Abs(distance) > 0.000001)
                    {
                        selectedRigidBody.drag = 0;
                        selectedRigidBody.MovePosition(currentTouchToWorldPoint);
                        /*if(lastSpeed == 0)
                        {
                            startFingerPosition = currentTouch.screenPosition;
                            firstFingerPositionTime = currentTouch.time;
                        }*/
                    }
                    else
                    {
                        startFingerPosition = currentTouch.screenPosition;
                        firstFingerPositionTime = currentTouch.time;
                    }
                   
                    //lastSpeed = speed;
                    lastFingerTime = finger.currentTouch.time;
                }
                else
                {
                    if (mode == UnityEngine.InputSystem.TouchPhase.Ended)
                    {
                        var startFingerPositionToWorldPoint = GetWorldPositionOnPlane(startFingerPosition, selectedRigidBody.GetComponent<Sphere>().initialPosition.y);
                        //var lastFingerPositionToWorldPoint = GetWorldPositionOnPlane(lastFingerPosition, selectedRigidBody.GetComponent<Sphere>().initialPosition.y);
                        var difference = currentTouchToWorldPoint - startFingerPositionToWorldPoint;
                        var differenceSpeed = (float)(difference.magnitude / (currentTouch.time - firstFingerPositionTime));
                       
                        var sphere = selectedRigidBody.GetComponent<Sphere>();
                        sphere.SetUnpicked();

                      
                        sphereToAddForce = selectedRigidBody;
                        forceToAddToSphere = difference * differenceSpeed;

                        if (Touches.ContainsKey(sphere))
                        {
                            Touches[sphere]++;
                        }
 

                        selectedRigidBody = null;
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
                                if (!foodOnBubble.InFridge)
                                {
                                    if (foodOnBubble.canBeRemovedFromFridge)
                                    {
                                        selectedFoodOver.GetComponent<FoodBubble>().GoBackToOriginalPosition();
                                    }
                                    else
                                    {
                                        selectedFoodOver.GetComponent<FoodBubble>().GoBackToFridgePosition();
                                    }
                                   

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
                                        UpdateBarSimulation(true, foodWasChosen: false);
                                    }

                                }
                                else
                                {
                                    selectedFoodOver.transform.position = foodOnBubble.fridgePosition;
                                    foodOnBubble.ResetScale();

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
                                

                                    if(foodsInCombo.Count == 3)
                                    {
                                        ComboSelected();
                                    }
                                    else
                                    {
                                        UpdateBarSimulation(true, foodWasChosen: true);
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
        Plate.transform.GetChild(0).gameObject.SetActive(false);
        Plate.transform.GetChild(1).gameObject.SetActive(false);
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
        food.FoodChosen(leftOnBars, Constants.PlayerData.PlayerAbilities[PlayerAbility.FoodEffects] == 1);

       

        //SkipAndShuffle.SetActive(false);
        SkipAndShuffle.GetComponent<SkipShuffle>().Disappear();
        EnableCombo.SetActive(false);
        Fridge.SetActive(false);

        transparentPanelWasActive = false;

        transparentPlane.GetComponent<TransparentPlane>().Hide();
        status.SetActive(false);
        PotentialFat.GetComponent<FillScript>().Reset(false, foodWasChosen: true);
        ResetTextStyle(FatAmountText.GetComponent<TextMeshPro>());
        PotentialSaturates.GetComponent<FillScript>().Reset(false, foodWasChosen: true);
        ResetTextStyle(SaturatesAmountText.GetComponent<TextMeshPro>());
        PotentialSugar.GetComponent<FillScript>().Reset(false, foodWasChosen: true);
        ResetTextStyle(SugarAmountText.GetComponent<TextMeshPro>());
        PotentialSalt.GetComponent<FillScript>().Reset(false, foodWasChosen: true);
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

        var otherFoodBubbles = foodBubbles.Where(x => x != food.gameObject).ToList();
        foreach (GameObject foodBubble in otherFoodBubbles)
        {
            foodBubble.GetComponent<FoodBubble>().disappear = true;
        }

        selectedFoodOver = null;

        if (state == StateMachine.Tutorial && !tutorialFoodSelected)
        {
            Tutorial.GetComponent<TutorialScript>().ResumeTutorial();
            tutorialFoodSelected = true;
        }

        if(CurrentLevel.EnergyUsage == 1)
        {
            CurrentFat.GetComponent<FillScript>().UseEnergy();
            CurrentSaturates.GetComponent<FillScript>().UseEnergy();
            CurrentSalt.GetComponent<FillScript>().UseEnergy();
            CurrentSugar.GetComponent<FillScript>().UseEnergy();
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

                foodBubble.GetComponent<FoodBubble>().SetFood(nextFood, CurrentLevel);
            }

        }
        else
        {
            ShuffleDeck();
            foreach (GameObject foodBubble in foodBubbles.Where(x => x.GetComponent<FoodBubble>().Food == null))
            {
                var nextFood = CurrentShuffledDeck.First();
                CurrentShuffledDeck.RemoveAt(0);

                foodBubble.GetComponent<FoodBubble>().SetFood(nextFood, CurrentLevel);
            }

        }
}

    private void StartupFood()
    {
       
            foreach (GameObject foodBubble in foodBubbles)
            {
                var nextFood = CurrentShuffledDeck.First();
                CurrentShuffledDeck.RemoveAt(0);

                foodBubble.GetComponent<FoodBubble>().SetFood(nextFood, CurrentLevel);
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
        ballsPausedOnCombo = true;
        Plate.GetComponent<PlateScript>().Disappear();
        Plate.transform.GetChild(0).gameObject.SetActive(false);
        Plate.transform.GetChild(1).gameObject.SetActive(false);

        Dictionary<NutritionElementsEnum, float> leftOnBars = new Dictionary<NutritionElementsEnum, float>()
        { 
            { NutritionElementsEnum.Fat, CurrentFat.GetComponent<FillScript>().MaxAmount - CurrentFat.GetComponent<FillScript>().currentAmount },
            { NutritionElementsEnum.Saturates, CurrentSaturates.GetComponent<FillScript>().MaxAmount - CurrentSaturates.GetComponent<FillScript>().currentAmount },
            { NutritionElementsEnum.Salt, CurrentSalt.GetComponent<FillScript>().MaxAmount - CurrentSalt.GetComponent<FillScript>().currentAmount },
            { NutritionElementsEnum.Sugar, CurrentSugar.GetComponent<FillScript>().MaxAmount - CurrentSugar.GetComponent<FillScript>().currentAmount },
        };

        bool canFat = false;
        bool canSaturates = false;
        bool canSalt = false;
        bool canSugar = false;
       

        for (int i=0; i < foodsInCombo.Count; i++)
        {
            var food = foodsInCombo[i].GetComponent<FoodBubble>();

            food.FoodChosen(leftOnBars, Constants.PlayerData.PlayerAbilities[PlayerAbility.FoodEffects] == 1, isCombo : true);

            if (!canFat)
            {
                if (leftOnBars[NutritionElementsEnum.Fat] > food.Food.NutritionElements[NutritionElementsEnum.Fat] * CurrentLevel.Multiplier)
                {
                    canFat = true;
                    PotentialFat.GetComponent<FillScript>().RemoveLid();
                }
            }

            if (!canSaturates)
            {
                if (leftOnBars[NutritionElementsEnum.Saturates] > food.Food.NutritionElements[NutritionElementsEnum.Saturates] * CurrentLevel.Multiplier)
                {
                    canSaturates = true;
                    PotentialSaturates.GetComponent<FillScript>().RemoveLid();
                }
            }

            if (!canSalt)
            {
                if (leftOnBars[NutritionElementsEnum.Salt] > food.Food.NutritionElements[NutritionElementsEnum.Salt] * CurrentLevel.Multiplier)
                {
                    canSalt = true;
                    PotentialSalt.GetComponent<FillScript>().RemoveLid();
                }
            }

            if (!canSugar)
            {
                if (leftOnBars[NutritionElementsEnum.Sugar] > food.Food.NutritionElements[NutritionElementsEnum.Sugar] * CurrentLevel.Multiplier)
                {
                    canSugar = true;
                    PotentialSugar.GetComponent<FillScript>().RemoveLid();
                }
            }

            SoundEffects.GetComponent<SoundEffects>().PlayBubble();
            CaloriesBar.GetComponent<CaloriesFill>().AddAmount(foodsInCombo[i].GetComponent<FoodBubble>().Food.Calories * CurrentLevel.Multiplier);
          
            ApplyFoodEffect(foodsInCombo[i].GetComponent<FoodBubble>());

            /*fats += foodsInCombo[i].GetComponent<FoodBubble>().Food.NutritionElements[NutritionElementsEnum.Fat];
            saturates += foodsInCombo[i].GetComponent<FoodBubble>().Food.NutritionElements[NutritionElementsEnum.Saturates];
            salt += foodsInCombo[i].GetComponent<FoodBubble>().Food.NutritionElements[NutritionElementsEnum.Salt];
            sugar += foodsInCombo[i].GetComponent<FoodBubble>().Food.NutritionElements[NutritionElementsEnum.Sugar];*/


            await AsyncTask.Await(100);

        }

      


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
      


        //SkipAndShuffle.SetActive(false);
        SkipAndShuffle.GetComponent<SkipShuffle>().Disappear();
        EnableCombo.SetActive(false);
        Fridge.SetActive(false);

        transparentPanelWasActive = false;

        transparentPlane.GetComponent<TransparentPlane>().Hide();
        status.SetActive(false);
        PotentialFat.GetComponent<FillScript>().Reset(false, foodWasChosen: true);
        ResetTextStyle(FatAmountText.GetComponent<TextMeshPro>());
        PotentialSaturates.GetComponent<FillScript>().Reset(false, foodWasChosen: true);
        ResetTextStyle(SaturatesAmountText.GetComponent<TextMeshPro>());
        PotentialSugar.GetComponent<FillScript>().Reset(false, foodWasChosen: true);
        ResetTextStyle(SugarAmountText.GetComponent<TextMeshPro>());
        PotentialSalt.GetComponent<FillScript>().Reset(false, foodWasChosen: true);
        ResetTextStyle(SaltAmountText.GetComponent<TextMeshPro>());
        PotentialCalories.GetComponent<CaloriesFill>().Reset();
        SickBarPotential.GetComponent<SickFill>().Reset();
        Host.GetComponent<Host>().Hide();

        var otherFoodBubbles = foodBubbles.Where(x => !foodsInCombo.Any(y => y == x)).ToList();

        if (CurrentLevel.FoodExpires == 1)
        {
            foreach (GameObject foodBubble in otherFoodBubbles)
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

        foreach (GameObject foodBubble in otherFoodBubbles)
        {
            foodBubble.GetComponent<FoodBubble>().disappear = true;
        }

        foodsInCombo.Clear();

        await AsyncTask.Await(1500);

        VisualFunnel.GetComponent<Funnel>().PauseRotation();

        StopCoroutine(Timer);
        TimeText.GetComponent<TextMeshPro>().faceColor = new Color32(49, 30, 18, 140);
        TimeText.GetComponent<TextMeshPro>().outlineColor = new Color32(226, 214, 208, 140);

        CountingDown.GetComponent<CountingDownScript>().Play(Constants.ParticleGradients[(NutritionElementsEnum)frozenColor].colorMin, frozenCount.ToString());

        FrozenTimerCounter = StartCoroutine(CustomTimer.Timer(1, () => {

            if (frozenColor < Constants.ParticleGradients.Count - 1)
            {
                frozenColor++;
            }
            else
            {
                frozenColor = 0;
            }

            frozenCount--;
          

            if(frozenCount == 0)
            {
                StopCoroutine(FrozenTimerCounter);
                frozenCount = 5;
                frozenColor = 0;
                return;
            }

            CountingDown.GetComponent<CountingDownScript>().Play(Constants.ParticleGradients[(NutritionElementsEnum)frozenColor].colorMin, frozenCount.ToString());

        }));


        FrozenTimer = StartCoroutine(CustomTimer.Timer(5, () =>
        {
            ballsPausedOnCombo = false;
            VisualFunnel.GetComponent<Funnel>().ResumeRotation();
            TimeText.GetComponent<TextMeshPro>().faceColor = new Color32(49, 30, 18, 255);
            TimeText.GetComponent<TextMeshPro>().outlineColor = new Color32(226, 214, 208, 255);
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

            if (CurrentLevel.EnergyUsage == 1)
            {
                CurrentFat.GetComponent<FillScript>().UseEnergy();
                CurrentSaturates.GetComponent<FillScript>().UseEnergy();
                CurrentSalt.GetComponent<FillScript>().UseEnergy();
                CurrentSugar.GetComponent<FillScript>().UseEnergy();
            }

        }, true));
    }

    private void ResetPots()
    {
        PotentialFat.GetComponent<FillScript>().ResetPot();
        PotentialSaturates.GetComponent<FillScript>().ResetPot();
        PotentialSugar.GetComponent<FillScript>().ResetPot();
        PotentialSalt.GetComponent<FillScript>().ResetPot();
    }

    private void UpdateBarSimulation(bool isCombo = false, bool foodWasChosen = false)
    {
       
        PotentialFat.GetComponent<FillScript>().Reset(false, foodWasChosen: foodWasChosen, isCombo: isCombo);
        ResetTextStyle(FatAmountText.GetComponent<TextMeshPro>());
        PotentialSaturates.GetComponent<FillScript>().Reset(false, foodWasChosen: foodWasChosen, isCombo: isCombo);
        ResetTextStyle(SaturatesAmountText.GetComponent<TextMeshPro>());
        PotentialSugar.GetComponent<FillScript>().Reset(false, foodWasChosen: foodWasChosen, isCombo: isCombo);
        ResetTextStyle(SugarAmountText.GetComponent<TextMeshPro>());
        PotentialSalt.GetComponent<FillScript>().Reset(false, foodWasChosen: foodWasChosen, isCombo: isCombo);
        ResetTextStyle(SaltAmountText.GetComponent<TextMeshPro>());
        PotentialCalories.GetComponent<CaloriesFill>().Reset();
        SickBarPotential.GetComponent<SickFill>().Reset();

        if (!isCombo)
        {
            var calculatedBaseSick = false;

            var currentFood = selectedFoodOver.GetComponent<FoodBubble>().Food;
           
            var canAbsorbFat = PotentialFat.GetComponent<FillScript>().SimulateSingle(CurrentFat.GetComponent<FillScript>().currentAmount, currentFood.NutritionElements[NutritionElementsEnum.Fat] * CurrentLevel.Multiplier);
            if (!canAbsorbFat)
            {
                if (!calculatedBaseSick)
                {
                    SickBarPotential.GetComponent<SickFill>().Simulate(SickBar.GetComponent<SickFill>().MaxAmount - SickBar.GetComponent<SickFill>().currentAmount);
                    calculatedBaseSick = true;
                }

                MakeTextRedAndBold(FatAmountText.GetComponent<TextMeshPro>());
                SickBarPotential.GetComponent<SickFill>().Simulate((currentFood.NutritionElements[NutritionElementsEnum.Fat] * CurrentLevel.Multiplier) * PotentialFat.GetComponent<FillScript>().amountToApply);
        
            }

            var canAbsorbSaturates = PotentialSaturates.GetComponent<FillScript>().SimulateSingle(CurrentSaturates.GetComponent<FillScript>().currentAmount, currentFood.NutritionElements[NutritionElementsEnum.Saturates] * CurrentLevel.Multiplier);
            if (!canAbsorbSaturates)
            {

                if (!calculatedBaseSick)
                {
                    SickBarPotential.GetComponent<SickFill>().Simulate(SickBar.GetComponent<SickFill>().MaxAmount - SickBar.GetComponent<SickFill>().currentAmount);
                    calculatedBaseSick = true;
                }

                MakeTextRedAndBold(SaturatesAmountText.GetComponent<TextMeshPro>());
                SickBarPotential.GetComponent<SickFill>().Simulate((currentFood.NutritionElements[NutritionElementsEnum.Saturates] * CurrentLevel.Multiplier) * PotentialSaturates.GetComponent<FillScript>().amountToApply);
               
            }

            var canAbsorbSalt = PotentialSalt.GetComponent<FillScript>().SimulateSingle(CurrentSalt.GetComponent<FillScript>().currentAmount, currentFood.NutritionElements[NutritionElementsEnum.Salt] * CurrentLevel.Multiplier);
            if (!canAbsorbSalt)
            {
                if (!calculatedBaseSick)
                {
                    SickBarPotential.GetComponent<SickFill>().Simulate(SickBar.GetComponent<SickFill>().MaxAmount - SickBar.GetComponent<SickFill>().currentAmount);
                    calculatedBaseSick = true;
                }

                MakeTextRedAndBold(SaltAmountText.GetComponent<TextMeshPro>());
                SickBarPotential.GetComponent<SickFill>().Simulate((currentFood.NutritionElements[NutritionElementsEnum.Salt] * CurrentLevel.Multiplier) * PotentialSalt.GetComponent<FillScript>().amountToApply);
               
            }

            var canAbsorbSugar = PotentialSugar.GetComponent<FillScript>().SimulateSingle(CurrentSugar.GetComponent<FillScript>().currentAmount, currentFood.NutritionElements[NutritionElementsEnum.Sugar] * CurrentLevel.Multiplier);
            if (!canAbsorbSugar)
            {
                if (!calculatedBaseSick)
                {
                    SickBarPotential.GetComponent<SickFill>().Simulate(SickBar.GetComponent<SickFill>().MaxAmount - SickBar.GetComponent<SickFill>().currentAmount);
                    calculatedBaseSick = true;
                }

                MakeTextRedAndBold(SugarAmountText.GetComponent<TextMeshPro>());
                SickBarPotential.GetComponent<SickFill>().Simulate((currentFood.NutritionElements[NutritionElementsEnum.Sugar] * CurrentLevel.Multiplier) * PotentialSugar.GetComponent<FillScript>().amountToApply);
                
            }

            if (!calculatedBaseSick)
            {
                SickBarPotential.GetComponent<SickFill>().Simulate(SickBar.GetComponent<SickFill>().MaxAmount - SickBar.GetComponent<SickFill>().currentAmount);
            }

            if(currentFood.Effect != null && currentFood.Effect.Id == (int) FoodEffects.SuperFood)
            {
                SickBarPotential.GetComponent<SickFill>().SimulatePercentage(currentFood.EffectAmount);
            }

            PotentialCalories.GetComponent<CaloriesFill>().Simulate(CaloriesBar.GetComponent<CaloriesFill>().currentAmount + currentFood.Calories * CurrentLevel.Multiplier);
        }
        else
        {
            var calculatedBaseSick = false;

            PotentialFat.GetComponent<FillScript>().SimulateSingle(CurrentFat.GetComponent<FillScript>().currentAmount, 0);
            PotentialSaturates.GetComponent<FillScript>().SimulateSingle(CurrentSaturates.GetComponent<FillScript>().currentAmount, 0);
            PotentialSalt.GetComponent<FillScript>().SimulateSingle(CurrentSalt.GetComponent<FillScript>().currentAmount, 0);
            PotentialSugar.GetComponent<FillScript>().SimulateSingle(CurrentSugar.GetComponent<FillScript>().currentAmount, 0);
            //SickBarPotential.GetComponent<SickFill>().Simulate(SickBar.GetComponent<SickFill>().currentAmount);
            PotentialCalories.GetComponent<CaloriesFill>().Simulate(CaloriesBar.GetComponent<CaloriesFill>().currentAmount);

            var newFoodCombo = foodsInCombo.ToList();

            if (selectedFoodOver != null && !newFoodCombo.Contains(selectedFoodOver))
            {
                newFoodCombo.Add(selectedFoodOver);
            }

            bool canFat = true;
            bool canSaturates = true;
            bool canSalt = true;
            bool canSugar = true;

            foreach (GameObject foodInCombo in newFoodCombo)
            {
                var food = foodInCombo.GetComponent<FoodBubble>().Food;

                var canAbsorbFat = PotentialFat.GetComponent<FillScript>().SimulateCombo(food.NutritionElements[NutritionElementsEnum.Fat] * CurrentLevel.Multiplier);
                if (!canAbsorbFat)
                {
                    canFat = false;

                    if (!calculatedBaseSick)
                    {
                        SickBarPotential.GetComponent<SickFill>().Simulate(SickBar.GetComponent<SickFill>().MaxAmount - SickBar.GetComponent<SickFill>().currentAmount);
                        calculatedBaseSick = true;
                    }

                    MakeTextRedAndBold(FatAmountText.GetComponent<TextMeshPro>());
                    SickBarPotential.GetComponent<SickFill>().Simulate((food.NutritionElements[NutritionElementsEnum.Fat] * CurrentLevel.Multiplier) * CurrentFat.GetComponent<FillScript>().amountToApply);
                }

                var canAbsorbSaturates = PotentialSaturates.GetComponent<FillScript>().SimulateCombo(food.NutritionElements[NutritionElementsEnum.Saturates] * CurrentLevel.Multiplier);
                if (!canAbsorbSaturates)
                {
                    canSaturates = false;

                    if (!calculatedBaseSick)
                    {
                        SickBarPotential.GetComponent<SickFill>().Simulate(SickBar.GetComponent<SickFill>().MaxAmount - SickBar.GetComponent<SickFill>().currentAmount);
                        calculatedBaseSick = true;
                    }

                    MakeTextRedAndBold(SaturatesAmountText.GetComponent<TextMeshPro>());
                    SickBarPotential.GetComponent<SickFill>().Simulate((food.NutritionElements[NutritionElementsEnum.Saturates] * CurrentLevel.Multiplier) * CurrentSaturates.GetComponent<FillScript>().amountToApply);
                    
                }

                var canAbsorbSalt = PotentialSalt.GetComponent<FillScript>().SimulateCombo(food.NutritionElements[NutritionElementsEnum.Salt] * CurrentLevel.Multiplier);
                if (!canAbsorbSalt)
                {
                    canSalt = false;

                    if (!calculatedBaseSick)
                    {
                        SickBarPotential.GetComponent<SickFill>().Simulate(SickBar.GetComponent<SickFill>().MaxAmount - SickBar.GetComponent<SickFill>().currentAmount);
                        calculatedBaseSick = true;
                    }

                    MakeTextRedAndBold(SaltAmountText.GetComponent<TextMeshPro>());
                    SickBarPotential.GetComponent<SickFill>().Simulate((food.NutritionElements[NutritionElementsEnum.Salt] * CurrentLevel.Multiplier) * CurrentSalt.GetComponent<FillScript>().amountToApply);
                    
                }

                var canAbsorbSugar = PotentialSugar.GetComponent<FillScript>().SimulateCombo(food.NutritionElements[NutritionElementsEnum.Sugar] * CurrentLevel.Multiplier);
                if (!canAbsorbSugar)
                {
                    canSugar = false;

                    if (!calculatedBaseSick)
                    {
                        SickBarPotential.GetComponent<SickFill>().Simulate(SickBar.GetComponent<SickFill>().MaxAmount - SickBar.GetComponent<SickFill>().currentAmount);
                        calculatedBaseSick = true;
                    }

                    MakeTextRedAndBold(SugarAmountText.GetComponent<TextMeshPro>());
                    SickBarPotential.GetComponent<SickFill>().Simulate((food.NutritionElements[NutritionElementsEnum.Sugar] * CurrentLevel.Multiplier) * CurrentSugar.GetComponent<FillScript>().amountToApply);
                    
                }


                if (!calculatedBaseSick)
                {
                    SickBarPotential.GetComponent<SickFill>().Simulate(SickBar.GetComponent<SickFill>().MaxAmount - SickBar.GetComponent<SickFill>().currentAmount);
                }

                if (food.Effect != null && food.Effect.Id == (int)FoodEffects.SuperFood)
                {
                    SickBarPotential.GetComponent<SickFill>().SimulatePercentage(food.EffectAmount);
                }

                PotentialCalories.GetComponent<CaloriesFill>().Simulate(food.Calories * CurrentLevel.Multiplier);
            }

            if (canFat)
            {
                PotentialFat.GetComponent<FillScript>().RemoveLid();
            }

            if (canSaturates)
            {
                PotentialSaturates.GetComponent<FillScript>().RemoveLid();
            }

            if (canSalt)
            {
                PotentialSalt.GetComponent<FillScript>().RemoveLid();
            }

            if (canSugar)
            {
                PotentialSugar.GetComponent<FillScript>().RemoveLid();
            }
        }

    }

    public void Pause()
    {
        if (Timer != null)
        {
            TimerWasRunning = true;
            StopCoroutine(Timer);
        }

        gamePaused = true;

        //transparentPlane.GetComponent<TransparentPlane>().Show();

        canvas.enabled = true;
        MainPanel.SetActive(false);
        LevelCompletePanel.SetActive(false);
        LostPanel.SetActive(false);
        PausePanel.SetActive(true);
        BottomPanel.SetActive(false);

        pausedBalls = true;
        VisualFunnel.GetComponent<Funnel>().PauseRotation();
    }


    private async void GetFoodPicked()
{

        var ray = gameCamera.ScreenPointToRay(Pointer.current.position.value);

        var allHits = Physics.RaycastAll(ray);

        if (allHits.Any(x => x.collider.transform.gameObject.name == "Options"))
        {
            if (!gamePaused)
            {
                

                /*var image = Resources.Load<Texture2D>("pauseButtonPressed");
                Options.GetComponent<SpriteRenderer>().sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), new Vector2(0.5f, 0.5f));


                await AsyncTask.Await(100);

                image = Resources.Load<Texture2D>("playButtonUnpressed");
                Options.GetComponent<SpriteRenderer>().sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), new Vector2(0.5f, 0.5f));*/

                gamePaused = true;

                transparentPlane.GetComponent<TransparentPlane>().Show();

                canvas.enabled = true;
                MainPanel.SetActive(false);
                LevelCompletePanel.SetActive(false);
                LostPanel.SetActive(false);
                PausePanel.SetActive(true);
                BottomPanel.SetActive(false);

             

                pausedBalls = true;
                VisualFunnel.GetComponent<Funnel>().PauseRotation();
            }
            else
            {
                var image = Resources.Load<Texture2D>("playButtonPressed");
                Options.GetComponent<SpriteRenderer>().sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), new Vector2(0.5f, 0.5f));

                await AsyncTask.Await(100);

                image = Resources.Load<Texture2D>("pauseButtonUnpressed");
                Options.GetComponent<SpriteRenderer>().sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), new Vector2(0.5f, 0.5f));

                ResumeGame();
            }

        }


        if (!canChoose || gamePaused)
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

                
                  TutorialHand.GetComponent<TutorialHandScript>().Stop();
                

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

                                    if (image != null)
                                    {
                                        foodImage.GetComponent<SpriteRenderer>().sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), new Vector2(0.5f, 0.5f));
                                    }
                                    else
                                    {
                                        foodImage.GetComponent<SpriteRenderer>().sprite = null;
                                    }
                            }
                            else
                            {
                                foodImage.GetComponent<SpriteRenderer>().sprite = null;
                            }

                            UpdateBarSimulation(gamePlayState == GameplayState.Combo);

                            CaloriesText.GetComponent<TextMeshPro>().text = $"{Math.Round(food.Food.Calories * CurrentLevel.Multiplier, 2):0.00} <b>kCal</b>";
                            FatAmountText.GetComponent<TextMeshPro>().text = $"{Math.Round(food.Food.NutritionElements[NutritionElementsEnum.Fat] * CurrentLevel.Multiplier, 2):0.00}";
                            SaturatesAmountText.GetComponent<TextMeshPro>().text = $"{Math.Round(food.Food.NutritionElements[NutritionElementsEnum.Saturates] * CurrentLevel.Multiplier, 2):0.00}";
                            SaltAmountText.GetComponent<TextMeshPro>().text = $"{Math.Round(food.Food.NutritionElements[NutritionElementsEnum.Salt] * CurrentLevel.Multiplier, 2):0.00}";
                            SugarAmountText.GetComponent<TextMeshPro>().text = $"{Math.Round(food.Food.NutritionElements[NutritionElementsEnum.Sugar] * CurrentLevel.Multiplier, 2):0.00}";

                            if (food.Food.Effect != null)
                            {
                                EffectsText.GetComponent<TextMeshPro>().text = food.Food.Effect.Description;
                            }
                            else
                            {
                                EffectsText.GetComponent<TextMeshPro>().text = string.Empty;
                            }

                            var currentColor = EffectsText.GetComponent<TextMeshPro>().color;

                            /*if (CurrentLevel.DoubleHalfAbsorption == 1 || CurrentLevel.SpeedUpSlowDown == 1)
                            {
                                EffectsText.GetComponent<TextMeshPro>().color = new Color(currentColor.r, currentColor.g, currentColor.b, 1f);
                            }
                            else
                            {
                                EffectsText.GetComponent<TextMeshPro>().color = new Color(currentColor.r, currentColor.g, currentColor.b, 0.1f);
                            }*/

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
                if (SkipAndShuffle.GetComponent<SkipShuffle>().CanSKip && SkipAndShuffle.GetComponent<SkipShuffle>().isVisible)
                {
                  
                    if (allHits.Any(x => x.collider.transform.gameObject.name == "SkipAndShuffle"))
                    {
                        SkipAndShuffle.GetComponent<SkipShuffle>().Skipped();

                        var image = Resources.Load<Texture2D>("skipShufflePressed");
                        SkipAndShuffle.GetComponent<SpriteRenderer>().sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), new Vector2(0.5f, 0.5f));

                        await AsyncTask.Await(100);

                        image = Resources.Load<Texture2D>("skipShuffleUnpressed");
                        SkipAndShuffle.GetComponent<SpriteRenderer>().sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), new Vector2(0.5f, 0.5f));


                        foodsInCombo.Clear();
                        comboFoodsOriginalScale.Clear();
                       
                        
                        EnableCombo.SetActive(false);
                        Fridge.SetActive(false);


                        foreach (GameObject foodBubble in foodBubbles.Where(x => !x.GetComponent<FoodBubble>().InFridge))
                        {
                            foodBubble.GetComponent<FoodBubble>().FoodSpoiled(false);

                        }

                        await AsyncTask.Await(1000);

                        GetNextFood();

                        foreach (GameObject foodBubble in foodBubbles)
                        {
                            foodBubble.GetComponent<FoodBubble>().Show();

                            await AsyncTask.Await(100);
                        }

                        //SkipAndShuffle.SetActive(true);

                        //SkipAndShuffle.GetComponent<SkipShuffle>().isVisible = true;
                        //SkipAndShuffle.GetComponent<SpriteRenderer>().enabled = true;

                        if (Constants.PlayerData.PlayerAbilities[PlayerAbility.Combo] == 1)
                        {
                            EnableCombo.SetActive(true);
                        }

                        if (Constants.PlayerData.PlayerAbilities[PlayerAbility.Fridge] == 1)
                        {
                            Fridge.SetActive(true);
                        }
                      


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
                            if (!foodBubble.GetComponent<FoodBubble>().InFridge)
                            {
                                foodBubble.GetComponent<FoodBubble>().GoBackToOriginalPosition();
                            }
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

    public void AddGhostSphere(Sphere sphere)
    {
        GhostSpheres.Add(sphere);
    }


    private void ApplyFoodEffect(FoodBubble food)
    {
        if (Constants.PlayerData.PlayerAbilities[PlayerAbility.FoodEffects] == 1)
        { 
            if (food.Food.Effect != null)
            {
                switch ((FoodEffects)food.Food.EffectId)
                {
                    case FoodEffects.AccelerateSugar:

                        
                            CurrentSugar.GetComponent<FillScript>().SetEffect(2, food.Food.EffectAmount);
                            PotentialSugar.GetComponent<FillScript>().SetEffect(2, food.Food.EffectAmount);
                        

                        break;

                    case FoodEffects.SugarFriendly:

                            CurrentSugar.GetComponent<FillScript>().SetEffect(0.5f, food.Food.EffectAmount);
                            PotentialSugar.GetComponent<FillScript>().SetEffect(0.5f, food.Food.EffectAmount);
                        

                        break;

                    case FoodEffects.AccelerateFat:

                       
                            CurrentFat.GetComponent<FillScript>().SetEffect(2, food.Food.EffectAmount);
                            PotentialFat.GetComponent<FillScript>().SetEffect(2, food.Food.EffectAmount);
                        

                        break;

                    case FoodEffects.FatBurning:

                      
                            CurrentFat.GetComponent<FillScript>().SetEffect(0.5f, food.Food.EffectAmount);
                            PotentialFat.GetComponent<FillScript>().SetEffect(0.5f, food.Food.EffectAmount);
                        

                        break;

                    case FoodEffects.AccelerateSaturates:

                       
                            CurrentSaturates.GetComponent<FillScript>().SetEffect(2, food.Food.EffectAmount);
                            PotentialSaturates.GetComponent<FillScript>().SetEffect(2, food.Food.EffectAmount);
                        

                        break;

                    case FoodEffects.HeartHealthy:

                            CurrentSaturates.GetComponent<FillScript>().SetEffect(0.5f, food.Food.EffectAmount);
                            PotentialSaturates.GetComponent<FillScript>().SetEffect(0.5f, food.Food.EffectAmount);
                        

                        break;

                    case FoodEffects.AccelerateSalt:

                      
                            CurrentSalt.GetComponent<FillScript>().SetEffect(2, food.Food.EffectAmount);
                            PotentialSalt.GetComponent<FillScript>().SetEffect(2, food.Food.EffectAmount);
                        

                        break;

                    case FoodEffects.Hydration:

                       
                            CurrentSalt.GetComponent<FillScript>().SetEffect(0.5f, food.Food.EffectAmount);
                            PotentialSalt.GetComponent<FillScript>().SetEffect(0.5f, food.Food.EffectAmount);
                        

                        break;

                    case FoodEffects.SugarRush:

                      
                            VisualFunnel.GetComponent<Funnel>().SpeedUp(food.Food.EffectAmount);
                        

                        break;

                    case FoodEffects.SuperFood:

                            SickBar.GetComponent<SickFill>().AddPercentage(food.Food.EffectAmount);

                        break;

                    default:

                        break;
                }


            }
        }
    }
    public void RemoveGhostSphere(Sphere sphere)
    {
        GhostSpheres.Remove(sphere);
    }


    public void RemoveSphere(Sphere sphere, bool absorbed)
    {

        if (absorbed && messagesShown.First(x => x.Id == (int) TutorialMessagesEnum.BallAbsorbed).Showed == 0 && Spheres.Count > 1)
        {
            PauseButtonCanvas.SetActive(false);
            Tutorial.GetComponent<TutorialScript>().ShowWithTextGroup(Constants.TutorialMessages[TutorialMessagesEnum.BallAbsorbed], continueTutorial: false);
            messagesShown.First(x => x.Id == (int)TutorialMessagesEnum.BallAbsorbed).Showed = 1;

            dataService.UpdateTutorialMessages(messagesShown);
          
            pausedBalls = true;
            VisualFunnel.GetComponent<Funnel>().PauseRotation();
            pausedForTimerWithoutFood = false;
     
            if (Timer != null)
            {
                StopCoroutine(Timer);
            }
        }

        if (!absorbed && messagesShown.First(x => x.Id == (int)TutorialMessagesEnum.BallDownVortex).Showed == 0 && Spheres.Count > 1)
        {
            Tutorial.SetActive(true);
            PauseButtonCanvas.SetActive(false);
            Tutorial.GetComponent<TutorialScript>().ShowWithTextGroup(Constants.TutorialMessages[TutorialMessagesEnum.BallDownVortex]);

            messagesShown.First(x => x.Id == (int)TutorialMessagesEnum.BallDownVortex).Showed = 1;

            dataService.UpdateTutorialMessages(messagesShown);

            pausedBalls = true;
            VisualFunnel.GetComponent<Funnel>().PauseRotation();
            pausedForTimerWithoutFood = false;
            showFoodAfterTutorial = false;
            if (Timer != null)
            {
                StopCoroutine(Timer);
            }
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
        BottomPanel.SetActive(false);

       
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
       

        for (int i = 0; i < Spheres.Count; i++)
        {
            Destroy(Spheres[i].gameObject);
        }

        for (int i = 0; i < GhostSpheres.Count; i++)
        {
            Destroy(GhostSpheres[i].gameObject);
        }

        if (Timer != null)
        {
            StopCoroutine(Timer);
        }

        if(FrozenTimer != null)
        {
            StopCoroutine(FrozenTimer);
            frozenCount = 5;
            frozenColor = 0;
        }

        if (FrozenTimerCounter != null)
        {
            StopCoroutine(FrozenTimerCounter);
        }

        gameOver = true;

        Spheres.Clear();
        GhostSpheres.Clear();

        Host.GetComponent<Host>().Hide();

        //Options.SetActive(false);
        PauseButtonCanvas.SetActive(false);

        //CurrentLevelPanel.SetActive(false);
        checkForTutorialToggle = true;
        transparentPanelWasActive = true;
  
        //transparentPlane.GetComponent<TransparentPlane>().Show();
        EditFoodPanel.SetActive(false);
        LevelSelectionPanel.SetActive(false);
        LevelCompletePanel.SetActive(false);
        LostPanel.SetActive(false);
        BottomPanel.SetActive(true);
        PausePanel.SetActive(false);
        MainPanel.SetActive(true);
      
      



    }



}
