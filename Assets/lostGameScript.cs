using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Utils;
using Button = UnityEngine.UIElements.Button;

public class lostGameScript : MonoBehaviour
{
    public GameObject SceneLogic;
    Button exitButton;
    Button retryButton;

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

        exitButton = root.Q<Button>("exit");
        retryButton = root.Q<Button>("retry");

        exitButton.clicked += ExitButton_clicked;
        retryButton.clicked += RetryButton_clicked;

    }

    private async void RetryButton_clicked()
    {
        var image = Resources.Load<Texture2D>("retryButtonPressed");
        retryButton.style.backgroundImage = new StyleBackground(image);

        await AsyncTask.Await(100);

        image = Resources.Load<Texture2D>("retryButtonUnpressed");
        retryButton.style.backgroundImage = new StyleBackground(image);

        SceneLogic.GetComponent<SceneLogic3D>().Reset();
    }

    private async void ExitButton_clicked()
    {
        var image = Resources.Load<Texture2D>("exitButtonPressed");
        exitButton.style.backgroundImage = new StyleBackground(image);

        await AsyncTask.Await(100);

        image = Resources.Load<Texture2D>("exitButtonUnpressed");
        exitButton.style.backgroundImage = new StyleBackground(image);

        SceneLogic.GetComponent<SceneLogic3D>().BackToMenu();
    }
}
