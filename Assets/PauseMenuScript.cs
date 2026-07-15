using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Utils;

public class PauseMenuScript : MonoBehaviour
{
    VisualElement settingsButton;
    public SceneLogic3D sceneLogic;
    VisualElement exitButton;
   

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

        settingsButton = root.Q<VisualElement>("settings");
        settingsButton.RegisterCallback<MouseDownEvent>(async (mouseDownEvent) => {

            var image = Resources.Load<Texture2D>("settingsButtonPressed");
            settingsButton.style.backgroundImage = new StyleBackground(image);

            await AsyncTask.Await(100);

            image = Resources.Load<Texture2D>("settingsButtonUnpressed");
            settingsButton.style.backgroundImage = new StyleBackground(image);

            sceneLogic.GetComponent<SceneLogic3D>().OpenSettings();
        
        });

        exitButton = root.Q<VisualElement>("exit");
        exitButton.RegisterCallback<MouseDownEvent>(async (mouseDownEvent) => {

            var image = Resources.Load<Texture2D>("exitButtonLongPressed");
            exitButton.style.backgroundImage = new StyleBackground(image);

            await AsyncTask.Await(100);

            image = Resources.Load<Texture2D>("exitButtonLongUnpressed");
            exitButton.style.backgroundImage = new StyleBackground(image);

            sceneLogic.GetComponent<SceneLogic3D>().BackToMenu();

        });
    }
}
