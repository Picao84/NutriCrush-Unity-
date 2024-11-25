using Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

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

        foreach(var reward in level.Rewards)
        {
            var rewardRow = rewardTemplate.Instantiate();
            rewardRow.Q<Label>("rewardGrade").text = $"Grade {reward.Key}";

            var food = Constants.FoodsDatabase.FirstOrDefault(x => x.Id == reward.Value.FoodId);

            rewardRow.Q<VisualElement>("rewardImage").style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>(food.FileName));

            rewardRow.Q<Label>("quantity").text = $"x{reward.Value.Quantity}";

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

        cancel.RegisterCallback<MouseEnterEvent>((MouseOverEvent) =>
        {
            cancel.style.backgroundColor = new StyleColor(new Color32(235, 235, 235, 255));

        });

        cancel.RegisterCallback<MouseLeaveEvent>((MouseOverEvent) =>
        {
            cancel.style.backgroundColor = new StyleColor(Color.white);

        });

        cancel.clicked += Cancel_clicked;

        playLevel = uiDocument.rootVisualElement.Q<Button>("playLevel");

        playLevel.RegisterCallback<MouseEnterEvent>((MouseOverEvent) =>
        {
            playLevel.style.backgroundColor = new StyleColor(new Color32(235, 235, 235, 255));

        });

        playLevel.RegisterCallback<MouseLeaveEvent>((MouseOverEvent) =>
        {
            playLevel.style.backgroundColor = new StyleColor(Color.white);

        });

        playLevel.clicked += PlayLevel_clicked;


    }

    private void PlayLevel_clicked()
    {
        this.gameObject.SetActive(false);
        deckPanel.SetActive(false);
        sceneLogic.PlayLevel(level);
    }

    private void Cancel_clicked()
    {
        this.gameObject.SetActive(false);
        deckPanel.SetActive(true);
    }
}

