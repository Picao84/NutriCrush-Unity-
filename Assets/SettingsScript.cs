 using Assets.UI;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UIElements;
using Utils;

public class SettingsScript : MonoBehaviour
{
    const int DEFAULT_SOUND_VOLUME = 50;

    const int DEFAULT_MUSIC_VOLUME = 50;

    CustomSlider soundSlider;
    CustomSlider musicSlider;
    Button defaultButton;
    Button saveButton;
    public GameObject sceneLogic;
    public GameObject MainMenu;

    int previousMusicLevel;
    int previousSoundLevel;

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

        var close = root.Q<VisualElement>("close");

        close.RegisterCallback<MouseDownEvent>(async (mouseDownEvent) => {

            var image = Resources.Load<Texture2D>("exitRoundPressed");
            close.style.backgroundImage = new StyleBackground(image);

            await AsyncTask.Await(100);

            image = Resources.Load<Texture2D>("exitRoundUnpressed");
            close.style.backgroundImage = new StyleBackground(image);

            Constants.UserSettings.MusicLevel = previousMusicLevel;
            Constants.UserSettings.SoundLevel = previousSoundLevel;

            sceneLogic.GetComponent<SceneLogic3D>().UpdateUserSettings();

            this.gameObject.SetActive(false);

            await AsyncTask.Await(100);

            MainMenu.GetComponent<MainMenu>().canNavigate = true;
        });

        soundSlider = root.Q<CustomSlider>("soundSlider");
        soundSlider.highValue = 100;
        soundSlider.value = Constants.UserSettings.SoundLevel;
        previousSoundLevel = Constants.UserSettings.SoundLevel;
        soundSlider.RegisterValueChangedCallback((change) => {

            Constants.UserSettings.SoundLevel = (int) change.newValue;
            sceneLogic.GetComponent<SceneLogic3D>().UpdateUserSettings();
        });



        musicSlider = root.Q<CustomSlider>("musicSlider");
        musicSlider.highValue = 100;
        musicSlider.value = Constants.UserSettings.MusicLevel;
        previousMusicLevel = Constants.UserSettings.MusicLevel;
        musicSlider.RegisterValueChangedCallback((change) => {

            Constants.UserSettings.MusicLevel = (int) change.newValue;
            sceneLogic.GetComponent<SceneLogic3D>().UpdateUserSettings();

        });

        defaultButton = root.Q<Button>("default");
        defaultButton.clicked += DefaultButton_clicked;

        saveButton = root.Q<Button>("save");
        saveButton.clicked += SaveButton_clicked;
    }

    private async void SaveButton_clicked()
    {
        var image = Resources.Load<Texture2D>("saveButtonPressed");
        saveButton.style.backgroundImage = new StyleBackground(image);

        await AsyncTask.Await(100);

        image = Resources.Load<Texture2D>("saveButtonUnpressed");
        saveButton.style.backgroundImage = new StyleBackground(image);

        sceneLogic.GetComponent<SceneLogic3D>().SaveUserSettings();

        this.gameObject.SetActive(false);

        await AsyncTask.Await(100);

        MainMenu.GetComponent<MainMenu>().canNavigate = true;
       
    }

    private async void DefaultButton_clicked()
    {
        var image = Resources.Load<Texture2D>("defaultButtonPressed");
        defaultButton.style.backgroundImage = new StyleBackground(image);

        await AsyncTask.Await(100);

        image = Resources.Load<Texture2D>("defaultButtonUnpressed");
        defaultButton.style.backgroundImage = new StyleBackground(image);

        soundSlider.value = DEFAULT_SOUND_VOLUME;
        musicSlider.value = DEFAULT_MUSIC_VOLUME;

        sceneLogic.GetComponent<SceneLogic3D>().UpdateUserSettings();
    }
}
