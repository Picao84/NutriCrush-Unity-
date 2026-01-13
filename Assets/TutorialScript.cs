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
    bool step5done = true;
    bool step4done;
    bool step6done;
    bool step9done;
    bool finishedFirstPart;
    bool skipPart = false;
    bool canSkipPart = true;
    bool skipInitialTutorial;
    Vector3 InitialPosition;

    float customTimeToWait;

    public GameObject SpeechBalloon;
    GameObject Text;

    SpriteRenderer balloonSprite;

    List<string> TutorialText = new List<string>()
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

    public void ShowWithTextGroup(List<string> textGroupToShow, float timeToWait = 2)
    {
        text.text = string.Empty;
        customTextGroup = textGroupToShow;
        currentCustomTextStep = 0;
        skipInitialTutorial = true;
        customTimeToWait = timeToWait;
        Show();

    }

    List<string> customTextGroup = new List<string>();

    public void ResumeTutorial()
    {
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
        text.color = new Color32(0, 0, 0, 0);
        balloonSprite.color = new Color(1f, 1f, 1f, 0.0f);
        text.text = string.Empty;
        
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
                text.text = string.Empty;
                resetText = false;
            }

            if (skipPart)
            {
                skipPart = false;
                text.text = TutorialText[currentStep];
                doNextLetter = false;


                if (currentStep != 5)
                {
                    StartCoroutine(CustomTimer.Timer(2, () =>
                    {
                        resetText = true;

                        if (currentStep < TutorialText.Count - 1)
                        {
                            currentStep++;
                            doNextLetter = true;
                        }

                        canSkipPart = true;


                    }, true));

                }

                    if (currentStep == 5)
                    {

                        step5done = false;
                    }

                if (currentStep == 6 && !step6done && text.text.Length > TutorialText[currentStep].Length * 0.75)
                {
                    GameObject.FindGameObjectWithTag("SceneLogic").GetComponent<SceneLogic3D>().ContinueTutorial(currentStep);
                    step6done = true;
                    canSkipPart = true;
                }

            }
            else
            if (doNextLetter)
            {
                if (!string.IsNullOrEmpty(TutorialText[currentStep]) && text.text.Length < TutorialText[currentStep].Length && !skipInitialTutorial)
                {
                    canSkipPart = true;

                    text.text += TutorialText[currentStep][text.text.Length];

                    doNextLetter = false;

                    if (text.text.Length < TutorialText[currentStep].Length)
                    {
                        if (currentStep == 6 && !step6done && text.text.Length > TutorialText[currentStep].Length * 0.75)
                        {
                            GameObject.FindGameObjectWithTag("SceneLogic").GetComponent<SceneLogic3D>().ContinueTutorial(currentStep);
                            step6done = true;
                        }

                        StartCoroutine(CustomTimer.Timer(1 / 50000, () => {

                            doNextLetter = true;

                        }, true));

                    }
                    else
                    {
                        if (currentStep != 5)
                        {
                           StartCoroutine(CustomTimer.Timer(2, () => {


                               resetText = true;

                                if (currentStep < TutorialText.Count - 1)
                                {
                                    currentStep++;
                                    doNextLetter = true;
                                }
                   
                                

                            }, true));
                              
                        }

                        if (currentStep == 5)
                        {
                            step5done = false;
                        }

                    }
                }
                else
                {
                    if(customTextGroup.Count > currentCustomTextStep && !string.IsNullOrEmpty(customTextGroup[currentCustomTextStep]) && text.text.Length < customTextGroup[currentCustomTextStep].Length)
                    {
                        text.text += customTextGroup[currentCustomTextStep][text.text.Length];
                        doNextLetter = false;

                        if (text.text.Length < customTextGroup[currentCustomTextStep].Length)
                        {

                            StartCoroutine(CustomTimer.Timer(1 / 50000, () => {

                                doNextLetter = true;

                            }, true));

                        }
                        else
                        {
                            currentCustomTextStep++;

                            if(currentCustomTextStep < customTextGroup.Count)
                            {
                                StartCoroutine(CustomTimer.Timer(2, () => {

                                    resetText = true;
                                    doNextLetter = true;
   

                                }, true));
                            }
                            else
                            {
                                StartCoroutine(CustomTimer.Timer(customTimeToWait, () => {

                                    disappear = true;


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

        if(currentStep == 2 && !step2done)
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
        }

        if(currentStep == 4 && !step4done)
        {
            GameObject.FindGameObjectWithTag("SceneLogic").GetComponent<SceneLogic3D>().ContinueTutorial(currentStep);
            step4done = true;
        }

        if(currentStep == 5 && !step5done)
        {
            GameObject.FindGameObjectWithTag("SceneLogic").GetComponent<SceneLogic3D>().ContinueTutorial(currentStep);
            step5done = true;
        }

        if (currentStep == 9 && !step9done)
        {
            GameObject.FindGameObjectWithTag("SceneLogic").GetComponent<SceneLogic3D>().ContinueTutorial(currentStep);
            finishedFirstPart = true;
            Hide();
            step9done = true;
        }

        if (disappear)
        {
            Hide();
            GameObject.FindGameObjectWithTag("SceneLogic").GetComponent<SceneLogic3D>().ContinueTutorial();
            disappear = false;
        }

        if (isShowing)
        {
            if (transform.position.x <= ScreenPosition.x)
            {
                transform.position = new Vector3(transform.position.x + 0.02f, transform.position.y, transform.position.z);
            }
            else
            {
                isShowingBalloon = true;
                isShowingText = true;

                if (!finishedFirstPart)
                {
                    transform.position = ScreenPosition;
                }
                else
                {
                    transform.position = SecondScreenPosition;
                }
                isShowing = false;
                readyToUpdateText = true;
                StartCoroutine(CustomTimer.Timer(1 / 50000, () => {

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

            if (text.color.a < 1f)
            {
                text.color = new Color(0f, 0f, 0f, text.color.a + 0.1f);
            }
            else
            {
                isShowingText = false;
            }
        }

        if (isHidingText)
        {

            if (text.color.a > 0f)
            {
                text.color = new Color(0f, 0f, 0f, text.color.a - 0.1f);
            }
            else
            {
                isHidingText = false;
            }
        }


        if (isHiding)
        {

            if (transform.position.x >= InitialPosition.x)
            {
                transform.position = new Vector3(transform.position.x - 10 * Time.deltaTime, transform.position.y, transform.position.z);
            }
            else
            {
                isHiding = false;
            }
        }
    }

    public void Show()
    {
        isShowing = true;
    }

    public void Hide()
    {
        isHiding = true;
        isHidingBalloon = true;
        isHidingText = true;
    }
}
