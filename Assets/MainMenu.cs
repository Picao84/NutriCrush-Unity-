using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Utils;

public class MainMenu : MonoBehaviour
{
    public GameObject SceneLogic;
    float originalOpacity = 0.0f;
    public bool canNavigate = false;
    VisualElement playButton;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        
        var root = GetComponent<UIDocument>().rootVisualElement;
        root.style.opacity = new StyleFloat(0f);

        if (originalOpacity < 1.0f)
        {
            canNavigate = false;

            root.schedule.Execute(() =>
            {

                originalOpacity += 0.1f;
                root.style.opacity = new StyleFloat(originalOpacity);
            }).Every(1).Until(() =>
            {

                if (root.style.opacity.value >= 1f)
                {
                    canNavigate = true;
                    return true;
                }

                return false;

            });
        }

        playButton = root.Q<VisualElement>("play");
        playButton.RegisterCallback<PointerDownEvent>(async (PointerDownEvent) => {

            if (!canNavigate)
                return;

            canNavigate = false;

            var image = Resources.Load<Texture2D>("mainMenuPlayPressed");
            playButton.style.backgroundImage = new StyleBackground(image);

            await AsyncTask.Await(100);

            image = Resources.Load<Texture2D>("mainMenuPlayUnpressed");
            playButton.style.backgroundImage = new StyleBackground(image);

            root.schedule.Execute(() => {

                originalOpacity -= 0.1f;
                root.style.opacity = new StyleFloat(originalOpacity);
            }).Every(1).Until(() => {

                if (root.style.opacity.value <= 0f)
                {
                    SceneLogic.GetComponent<SceneLogic3D>().StartGame(false);

                    return true;
                }

                return false;

            });

            canNavigate = true;

        });

        var foodDeck = root.Q<VisualElement>("foodDeck");
        foodDeck.RegisterCallback<PointerDownEvent>(async (PointerDownEvent) => {

            if (!canNavigate)
                return;

            canNavigate = false;

            var image = Resources.Load<Texture2D>("editFoodDeckPressed");
            foodDeck.style.backgroundImage = new StyleBackground(image);

            await AsyncTask.Await(100);

            image = Resources.Load<Texture2D>("editFoodDeckUnpressed");
            foodDeck.style.backgroundImage = new StyleBackground(image);


            root.schedule.Execute(() => {

                originalOpacity -= 0.1f;
                root.style.opacity = new StyleFloat(originalOpacity);
            }).Every(1).Until(() => {

                if (root.style.opacity.value <= 0f)
                {
                    SceneLogic.GetComponent<SceneLogic3D>().EditFoodDeck();
                    return true;
                }

                return false;

            });

            canNavigate = true;

        });


        var howToPlay = root.Q<VisualElement>("howToPlay");
        howToPlay.RegisterCallback<PointerDownEvent>(async (PointerDownEvent) => {

            if (!canNavigate)
                return;

            canNavigate = false;

            var image = Resources.Load<Texture2D>("howToPlayPressed");
            howToPlay.style.backgroundImage = new StyleBackground(image);

            await AsyncTask.Await(100);

            image = Resources.Load<Texture2D>("howToPlayUnpressed");
            howToPlay.style.backgroundImage = new StyleBackground(image);

            root.schedule.Execute(() => {

                originalOpacity -= 0.1f;
                root.style.opacity = new StyleFloat(originalOpacity);
            }).Every(1).Until(() => {

                if (root.style.opacity.value <= 0f)
                {

                    SceneLogic.GetComponent<SceneLogic3D>().StartGame(true);
                    return true;
                }

                return false;

            });

            canNavigate = true;


        });


        var settings = root.Q<VisualElement>("settings");
        settings.RegisterCallback<PointerDownEvent>(async (PointerDownEvent) => {

            if (!canNavigate)
                return;

            canNavigate = false;

            var image = Resources.Load<Texture2D>("settingsButtonPressed");
            settings.style.backgroundImage = new StyleBackground(image);

            await AsyncTask.Await(100);

            image = Resources.Load<Texture2D>("settingsButtonUnpressed");
            settings.style.backgroundImage = new StyleBackground(image);

            SceneLogic.GetComponent<SceneLogic3D>().OpenSettings();

            canNavigate = true;

        });

    }

   
}
