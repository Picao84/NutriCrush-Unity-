using Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Utils;

public class LevelDetailScript : MonoBehaviour
{
    Label titleLevel;
    Label fattext;
    Label saturatesText;
    Label saltText;
    Label sugarText;
    Label caloriesText;
    VisualElement rewardsContainer;
    public VisualTreeAsset rewardTemplate;
    Button cancel;
    Button playLevel;
    GameObject deckPanel;
    SceneLogic3D sceneLogic;
    Level level;

    public void SetLevelAndDeckPanel(Level level, GameObject deckPanel, SceneLogic3D sceneLogic)
    {
        this.level = level;

        titleLevel.text = level.Name;
        fattext.text = level.MaxFat.ToString();
        saturatesText.text = level.MaxSaturates.ToString();
        saltText.text = level.MaxSalt.ToString();
        sugarText.text = level.MaxSugar.ToString();
        caloriesText.text = level.CaloriesObjective.ToString();

        this.deckPanel = deckPanel;
        this.sceneLogic = sceneLogic;

        var fullstar = Resources.Load<Texture2D>("fullstar");
        var emptystar = Resources.Load<Texture2D>("emptystar");



        foreach (var reward in level.Rewards.Reverse())
        {
            var rewardRow = rewardTemplate.Instantiate();

            List<VisualElement> stars = new List<VisualElement>
            {
                rewardRow.Q<VisualElement>("star1"),
                rewardRow.Q<VisualElement>("star2"),
                rewardRow.Q<VisualElement>("star3")
            };


            for (int i = 3; i > 0; i--)
            { 
                if((int)reward.Key <= i)
                {
                    stars[3 - i].style.backgroundImage = new StyleBackground(fullstar);
                }
                else
                {
                    stars[3 - i].style.backgroundImage = new StyleBackground(emptystar);
                }
               
            }

            var uiDocument = GetComponent<UIDocument>();

            List<VisualElement> currentStars = new List<VisualElement>
            {
                 uiDocument.rootVisualElement.Q<VisualElement>("currentRateStar3"),
                  uiDocument.rootVisualElement.Q<VisualElement>("currentRateStar2"),
                   uiDocument.rootVisualElement.Q<VisualElement>("currentRateStar1"),
            };

            var grade = level.MaxGrade;

            if (grade != 0)
            {
                for (int i = 3; i > 0; i--)
                {
                    if ((int)grade <= i)
                    {
                        currentStars[i - 1].style.backgroundImage = new StyleBackground(fullstar);
                    }
                    else
                    {
                        currentStars[i - 1].style.backgroundImage = new StyleBackground(emptystar);
                    }
                }
            }
            else
            {
                for (int i = 3; i > 0; i--)
                {
                    currentStars[i - 1].style.backgroundImage = new StyleBackground(emptystar);
                }
            }


            var food = Constants.FoodsDatabase.FirstOrDefault(x => x.Id == reward.Value.FoodId);

            rewardRow.Q<VisualElement>("rewardImage").style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>(food.FileName));

            rewardRow.Q<Label>("quantity").text = $"x{reward.Value.FoodQuantity}";

            rewardsContainer.Add(rewardRow);
        }
    }


    private void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        titleLevel = uiDocument.rootVisualElement.Q<Label>("levelText");
        fattext = uiDocument.rootVisualElement.Q<Label>("fatText");
        saturatesText = uiDocument.rootVisualElement.Q<Label>("saturatesText");
        saltText = uiDocument.rootVisualElement.Q<Label>("saltText");
        sugarText = uiDocument.rootVisualElement.Q<Label>("sugarText");
        rewardsContainer = uiDocument.rootVisualElement.Q<VisualElement>("rewardsContainer");
        caloriesText = uiDocument.rootVisualElement.Q<Label>("caloriesText");

        cancel = uiDocument.rootVisualElement.Q<Button>("cancel");

        cancel.clicked += Cancel_clicked;

        playLevel = uiDocument.rootVisualElement.Q<Button>("playLevel");

        playLevel.clicked += PlayLevel_clicked;

    }

    private async void PlayLevel_clicked()
    {
        var image = Resources.Load<Texture2D>("playLevelButtonPressed");
        playLevel.style.backgroundImage = new StyleBackground(image);

        await AsyncTask.Await(100);

        image = Resources.Load<Texture2D>("playLevelButtonUnpressed");
        playLevel.style.backgroundImage = new StyleBackground(image);


        deckPanel.GetComponent<LevelDeckScript>().originalOpacity = 0.0f;
        deckPanel.transform.parent.gameObject.SetActive(false);
        this.gameObject.SetActive(false);
        sceneLogic.PlayLevel(level);
    }

    private async void Cancel_clicked()
    {
        var image = Resources.Load<Texture2D>("pinkButtonPressed");
        cancel.style.backgroundImage = new StyleBackground(image);

        await AsyncTask.Await(100);

        image = Resources.Load<Texture2D>("pinkButtonUnpressed");
        cancel.style.backgroundImage = new StyleBackground(image);


        this.gameObject.SetActive(false);
        deckPanel.SetActive(true);
    }
}

