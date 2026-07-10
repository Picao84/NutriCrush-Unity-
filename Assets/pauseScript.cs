using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Utils;

public class pauseScript : MonoBehaviour
{
    Button pauseButton;
    bool isPaused;
    public GameObject SceneLogic;

    // Start is called before the first frame update
    void Start()
    {
       
       

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private async void PauseButton_clicked()
    {
        if(!isPaused)
        {
            var image = Resources.Load<Texture2D>("pauseButtonPressed");
            pauseButton.style.backgroundImage = new StyleBackground(image);

            await AsyncTask.Await(100);

            image = Resources.Load<Texture2D>("playButtonUnpressed");
            pauseButton.style.backgroundImage = new StyleBackground(image);

            isPaused = true;

            SceneLogic.GetComponent<SceneLogic3D>().Pause();
              
        }
        else
        {
            var image = Resources.Load<Texture2D>("playButtonPressed");
            pauseButton.style.backgroundImage = new StyleBackground(image);

            await AsyncTask.Await(100);

            image = Resources.Load<Texture2D>("pauseButtonUnpressed");
            pauseButton.style.backgroundImage = new StyleBackground(image);

            isPaused = false;

            SceneLogic.GetComponent<SceneLogic3D>().ResumeGame();
        }
    }

    private void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        pauseButton = uiDocument.rootVisualElement.Q<Button>();
        pauseButton.clicked += PauseButton_clicked;
    }
}
