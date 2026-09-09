using Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Timers;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

public class TutorialScript : MonoBehaviour
{
    TextMeshPro text;
    int currentStep = 0;
    int currentCustomTextStep = 0;
    bool isRunning;

    Vector3 ScreenPosition = new Vector3(3.29f, 0.3377f, -5.8f);
    Vector3 SecondScreenPosition = new Vector3(3.29f, 0.3377f, -4.4f);

    bool isShowing;
    bool isHiding;

    bool isShowingBalloon;
    bool isHidingBalloon;
    bool isShowingText;
    bool isHidingText;
    bool readyToUpdateText;
    bool doNextLetter;
    bool resetText;
    bool step2done;
    bool step2FoodDone;
    bool step4done;
    bool step5done = false;
    bool step3done;
    bool finishedFirstPart;
    bool skipPart = false;
    bool canSkipPart = true;
    bool skipInitialTutorial;
    Vector3 InitialPosition;
    bool continueTutorial = false;
    bool routineInterrupted;
    Texture2D catMouthOpenImage;
    Texture2D catMouthClosedImage;

    float customTimeToWait;

    public GameObject SpeechBalloon;
    GameObject Text;

    Coroutine CatSpeech;
    Coroutine previousText;

    bool catMouthOpen;

    SpriteRenderer balloonSprite;

    /*List<string> TutorialText = new List<string>()
    {
        "Welcome to Nutri Mayhem! ..",
        "Your aim is to fill the Fat, Saturates, Salt and Sugar bars above...",
        "... by the time the Calories bar on the top left is filled..",
        "..without filling the sick bar. Each round is made of two phases.",
        "On the first phase you select food from the bubbles below.",
        "Go ahead, select one now!",
        "Calories are immediately consumed, but..",
        "..balls are spawned for fat, saturates, salt and sugar.",
        "Throw them in the matching coloured lane holes above!",
        "",
    };*/

    List<string> TutorialText = new List<string>()
    {
        "Meow therr! I'm chef Fantaine and I'm here to pla.. err help you make perrfect food!",
        "Perrfect food means you filling my bo.. er.. the calories bar above and the bar under each pot.",
        "Right meow, let's begin. Pick a food by paw-ing it into the plate in the middle!",
        "Delicious! We got the calories an.. uuh.. Paw-don me, I got distracted by these balls!",
        "Paw the balls into the corresponding boiling pots above to consume its nutrient!",
        ""
    };

    bool disappear;

    string customText;

    /*public void ShowWithText(string textToShow, float timeToWait = 2)
    {
        text.text = string.Empty;
        customText = textToShow;
        skipInitialTutorial = true;
        customTimeToWait = timeToWait;
        Show();
       
    }*/

    

    public void ShowWithTextGroup(List<string> textGroupToShow, float timeToWait = 2, bool continueTutorial = true)
    {
        text.text = string.Empty;
        customTextGroup = textGroupToShow;
        currentCustomTextStep = 0;
        skipInitialTutorial = true;
        customTimeToWait = timeToWait;
        this.continueTutorial = continueTutorial;
        if(previousText != null)
        {
            StopCoroutine(previousText);
        }
        readyToUpdateText = true;
        Show();

    }

    List<string> customTextGroup = new List<string>();

    public void ResumeTutorial()
    {
        balloonSprite.color = new Color(1f, 1f, 1f, 1f); ;
        text.color = new Color32(124, 94, 68, 255);

        resetText = true;
        currentStep++;
        doNextLetter = true;

    }

    // Start is called before the first frame update
    void Start()
    {
        InitialPosition = transform.position;
        SpeechBalloon = transform.GetChild(1).gameObject;

        Text = transform.GetChild(0).gameObject;
        balloonSprite = SpeechBalloon.GetComponent<SpriteRenderer>();

        text = Text.GetComponent<TextMeshPro>();
        text.color = new Color32(124, 94, 68, 255);
        balloonSprite.color = new Color(1f, 1f, 1f, 0.0f);
        text.text = string.Empty;

        catMouthOpenImage = Resources.Load<Texture2D>("catMouthOpen");
        catMouthClosedImage = Resources.Load<Texture2D>("catMouthClosed");

    }

    public void ResetTutorial()
    {
        transform.position = InitialPosition;
        text.text = string.Empty;
        currentStep = 0;
        step2done = false;
        step2FoodDone = false;
        step4done = false;
        step5done = false;
        step3done = false;
        finishedFirstPart = false;
        isShowingBalloon = false;
        isHidingBalloon = false;
        isShowingText = false;
        isHidingText = false;
        readyToUpdateText = false;
        doNextLetter = false;
        resetText = false;
        currentCustomTextStep = 0;
        skipPart = false;
        skipInitialTutorial = false;
        canSkipPart = true;

    }

    public void SkipPart()
    {
        if (canSkipPart)
        {
            skipPart = true;
            canSkipPart = false;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (readyToUpdateText)
        {
            if (resetText)
            {
                if (CatSpeech != null)
                {
                    StopCoroutine(CatSpeech);

                    balloonSprite.sprite = Sprite.Create(catMouthClosedImage, new Rect(0, 0, catMouthClosedImage.width, catMouthClosedImage.height), new Vector2(0.5f, 0.5f));
                    catMouthOpen = false;
                }
                CatSpeech = StartCoroutine(CustomTimer.Timer(0.1f, () =>
                    {

                        if (catMouthOpen)
                        {
                            balloonSprite.sprite = Sprite.Create(catMouthClosedImage, new Rect(0, 0, catMouthClosedImage.width, catMouthClosedImage.height), new Vector2(0.5f, 0.5f));
                            catMouthOpen = false;
                        }
                        else
                        {
                            balloonSprite.sprite = Sprite.Create(catMouthOpenImage, new Rect(0, 0, catMouthOpenImage.width, catMouthOpenImage.height), new Vector2(0.5f, 0.5f));

                            catMouthOpen = true;
                        }

                    }));
                

                text.text = string.Empty;
                resetText = false;
            }

            if (skipPart)
            {
                skipPart = false;
                text.text = TutorialText[currentStep];
                doNextLetter = false;

                if(text.text.Length == TutorialText[currentStep].Length)
                {
                    if (CatSpeech != null)
                    {
                        StopCoroutine(CatSpeech);

                        balloonSprite.sprite = Sprite.Create(catMouthClosedImage, new Rect(0, 0, catMouthClosedImage.width, catMouthClosedImage.height), new Vector2(0.5f, 0.5f));
                        catMouthOpen = false;
                    }
                }

                if (currentStep != 2)
                {
                    StartCoroutine(CustomTimer.Timer(1, () =>
                    {
                        resetText = true;

                        if (currentStep < TutorialText.Count - 1)
                        {
                            currentStep++;
                            doNextLetter = true;
                        }

                        if (currentStep < 2)
                        {
                            canSkipPart = true;
                        }


                    }, true));

                }

                if (currentStep == 2 && !step2done && text.text.Length == TutorialText[currentStep].Length)
                {
                    step2done = true;

                   
                    StartCoroutine(CustomTimer.Timer(1, () => {

                        GameObject.FindGameObjectWithTag("SceneLogic").GetComponent<SceneLogic3D>().EnableFoodSelection();
                        balloonSprite.color = new Color(1f, 1f, 1f, 0f); ;
                          text.color = new Color(124, 94, 68, 0f);

                    }, true));

                }



                if (currentStep == 3 && !step3done && text.text.Length > TutorialText[currentStep].Length * 0.25)
                {
                    GameObject.FindGameObjectWithTag("SceneLogic").GetComponent<SceneLogic3D>().ContinueTutorial(currentStep);
                    step3done = true;
                    //canSkipPart = true;
                    
                }

                if(currentStep == 4 && !step4done && text.text.Length == TutorialText[currentStep].Length)
                {
                    GameObject.FindGameObjectWithTag("SceneLogic").GetComponent<SceneLogic3D>().ContinueTutorial(currentStep);
                    finishedFirstPart = true;
                    step4done = true;

                    StartCoroutine(CustomTimer.Timer(1, () =>
                    {
                        Hide();

                    }, true));
                }

            }
            else
            if (doNextLetter)
            {
                if (!string.IsNullOrEmpty(TutorialText[currentStep]) && text.text.Length < TutorialText[currentStep].Length && !skipInitialTutorial)
                {
                   

                    text.text += TutorialText[currentStep][text.text.Length];

                    if (text.text.Length == TutorialText[currentStep].Length)
                    {
                        if (CatSpeech != null)
                        {
                            StopCoroutine(CatSpeech);

                            balloonSprite.sprite = Sprite.Create(catMouthClosedImage, new Rect(0, 0, catMouthClosedImage.width, catMouthClosedImage.height), new Vector2(0.5f, 0.5f));
                            catMouthOpen = false;
                        }
                    }


                    doNextLetter = false;

                    if (text.text.Length < TutorialText[currentStep].Length)
                    {
                        if (currentStep == 3 && !step3done && text.text.Length > TutorialText[currentStep].Length * 0.25)
                        {
                            GameObject.FindGameObjectWithTag("SceneLogic").GetComponent<SceneLogic3D>().ContinueTutorial(currentStep);
                            step3done = true;
                            //canSkipPart = true;
                        }

                        StartCoroutine(CustomTimer.Timer(1 / 100000, () => {

                            doNextLetter = true;

                        }, true));

                    }
                    else
                    {
                        if (currentStep != 2)
                        {
                           StartCoroutine(CustomTimer.Timer(1, () => {


                               resetText = true;

                                if (currentStep < TutorialText.Count - 1)
                                {
                                    currentStep++;
                                    doNextLetter = true;
                                }

                               if (currentStep < 2)
                               {
                                   canSkipPart = true;
                               }

                           }, true));
                              
                        }


                        if (currentStep == 2 && !step2done && text.text.Length == TutorialText[currentStep].Length)
                        {
                            step2done = true;

                          

                            StartCoroutine(CustomTimer.Timer(1, () => {

                                GameObject.FindGameObjectWithTag("SceneLogic").GetComponent<SceneLogic3D>().EnableFoodSelection();
                                balloonSprite.color = new Color(1f, 1f, 1f, 0f); ;
                                text.color = new Color(124, 94, 68, 0f);

                            }, true));

                        }

                        if (currentStep == 4 && !step4done && text.text.Length == TutorialText[currentStep].Length)
                        {
                            GameObject.FindGameObjectWithTag("SceneLogic").GetComponent<SceneLogic3D>().ContinueTutorial(currentStep);
                            finishedFirstPart = true;
                            step4done = true;

                            StartCoroutine(CustomTimer.Timer(1, () =>
                            {
                                Hide();

                            }, true));
                        }

                        /*if (currentStep == 2 && step2done)
                        {
                            currentStep++;
                        }*/

                    }
                }
                else
                {
                    if (customTextGroup.Count > currentCustomTextStep && !string.IsNullOrEmpty(customTextGroup[currentCustomTextStep]) && text.text.Length < customTextGroup[currentCustomTextStep].Length)
                    {
                        text.text += customTextGroup[currentCustomTextStep][text.text.Length];
                        doNextLetter = false;

                        if (text.text.Length < customTextGroup[currentCustomTextStep].Length)
                        {

                            StartCoroutine(CustomTimer.Timer(1 / 100000, () => {

                                doNextLetter = true;

                            }, true));

                        }
                        else
                        {
                            currentCustomTextStep++;

                            if(currentCustomTextStep < customTextGroup.Count)
                            {
                                StartCoroutine(CustomTimer.Timer(1, () => {

                                    resetText = true;
                                    doNextLetter = true;
   

                                }, true));
                            }
                            else
                            {

                                if (CatSpeech != null)
                                {
                                    StopCoroutine(CatSpeech);

                                    readyToUpdateText = false;

                                    balloonSprite.sprite = Sprite.Create(catMouthClosedImage, new Rect(0, 0, catMouthClosedImage.width, catMouthClosedImage.height), new Vector2(0.5f, 0.5f));
                                    catMouthOpen = false;
                                }


                                previousText = StartCoroutine(CustomTimer.Timer(customTimeToWait, () => {

                                    disappear = true;
                                     previousText = null;
                                      
                                }, true));
                            }

                        }
                      

                        /*if (!string.IsNullOrEmpty(customText))
                    {
                        if (text.text.Length < customText.Length) 
                        { 
                            text.text += customText[text.text.Length];

                            doNextLetter = false;

                            StartCoroutine(CustomTimer.Timer(1 / 50000, () => {

                               
                                doNextLetter = true;

                            }));

                        }
                        else
                        {
                            customText = string.Empty;
                            StartCoroutine(CustomTimer.Timer(customTimeToWait, () => {

                              

                                disappear = true;
                               
                                

                            }, true));
                        }*/
                    }
                }
            }
        }

        /*if(currentStep == 2 && !step2done)
        {
            readyToUpdateText = false;

            if (transform.position.z <= SecondScreenPosition.z)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 5 * Time.deltaTime);
            }
            else
            {
                step2done = true;
                transform.position = SecondScreenPosition;
                readyToUpdateText = true;
                resetText = true;
                doNextLetter = true;
              
            }
        }*/

        if(currentStep == 2 && !step2done)
        {
            if (!step2FoodDone)
            {
                GameObject.FindGameObjectWithTag("SceneLogic").GetComponent<SceneLogic3D>().ContinueTutorial(currentStep);
                step2FoodDone = true;
            }
        }


        /*if (currentStep == 5 && !step5done)
        {
            GameObject.FindGameObjectWithTag("SceneLogic").GetComponent<SceneLogic3D>().ContinueTutorial(currentStep);
            finishedFirstPart = true;
            Hide();
            step5done = true;
        }*/

        if (disappear)
        {
            Hide();
            if (continueTutorial)
            {
                GameObject.FindGameObjectWithTag("SceneLogic").GetComponent<SceneLogic3D>().ContinueTutorial();
            }
            continueTutorial = false;
            disappear = false;
        }

        if (isShowing)
        {
            /*if (transform.position.x <= ScreenPosition.x)
            {
                transform.position = new Vector3(transform.position.x + Math.Abs((ScreenPosition.x - InitialPosition.x) / 10), transform.position.y, transform.position.z);
            }
            else
            */{
                isShowingBalloon = true;
                isShowingText = true;

                /*if (!finishedFirstPart)
                {
                    transform.position = ScreenPosition;
                }
                else
                {
                    transform.position = SecondScreenPosition;
                }*/
                isShowing = false;
                readyToUpdateText = true;
                StartCoroutine(CustomTimer.Timer(1 / 100000, () => {

                    doNextLetter = true;
                }, true));

            }
        }

        if (isShowingBalloon)
        {

            if (balloonSprite.color.a < 1f)
            {
                balloonSprite.color = new Color(1f, 1f, 1f, balloonSprite.color.a + 0.1f);
            }
            else
            {
                isShowingBalloon = false;
            }
        }

        if (isHidingBalloon)
        {

            if (balloonSprite.color.a > 0f)
            {
                balloonSprite.color = new Color(1f, 1f, 1f, balloonSprite.color.a - 0.1f);
            }
            else
            {
                isHidingBalloon = false;
            }
        }

        if (isShowingText)
        {

            //if (text.color.a < 1f)
            //{
                text.color = new Color32(124, 94, 68, 255);
            //}
            //else
            //{
                isShowingText = false;
            //}
        }

        if (isHidingText)
        {

            //if (text.color.a > 0f)
            //{
                text.color = new Color(1, 1, 1, 0f);
            //}
            //else
            //{
                isHidingText = false;
            //}
        }


        /*if (isHiding)
        {

            if (transform.position.x >= InitialPosition.x)
            {
                transform.position = new Vector3(transform.position.x - Math.Abs((ScreenPosition.x - InitialPosition.x) / 10), transform.position.y, transform.position.z);
            }
            else
            {
                isHiding = false;
            }
        }*/
    }

    public void Show()
    {
        CatSpeech = StartCoroutine(CustomTimer.Timer(0.1f, () => {

            if (catMouthOpen)
            {
                balloonSprite.sprite = Sprite.Create(catMouthClosedImage, new Rect(0, 0, catMouthClosedImage.width, catMouthClosedImage.height), new Vector2(0.5f, 0.5f));
                catMouthOpen = false;
            }
            else
            {
                balloonSprite.sprite = Sprite.Create(catMouthOpenImage, new Rect(0, 0, catMouthOpenImage.width, catMouthOpenImage.height), new Vector2(0.5f, 0.5f));

                catMouthOpen = true;
            }
        
        }));

        isShowing = true;
        isHiding = false;
    }

    public void Hide()
    {
        if (CatSpeech != null)
        {
            StopCoroutine(CatSpeech);

            balloonSprite.sprite = Sprite.Create(catMouthClosedImage, new Rect(0, 0, catMouthClosedImage.width, catMouthClosedImage.height), new Vector2(0.5f, 0.5f));
            catMouthOpen = false;
        }
        readyToUpdateText = false;
        isShowing = false;
        isHiding = true;
        isHidingBalloon = true;
        isHidingText = true;
    }
}
