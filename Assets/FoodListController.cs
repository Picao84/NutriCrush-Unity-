using Assets;
using Assets.UI;
using System;

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.LowLevel;

using UnityEngine.UIElements;

public class FoodListController
{
    VisualTreeAsset listTemplate;

    ListView foodList;

    public SceneLogic3D sceneLogic;

    public event EventHandler<int> QuantityChanged;

    Label deckSizeText;
    Button updateDeck;
    Button cancel;
    bool fixedHeightSet;
    float oldHeight;
    VisualElement topBar;
    Label totalCalories;
    Label totalFat;
    Label totalSaturates;
    Label totalSalt;
    Label totalSugar;
    VisualElement totalFatBar;
    VisualElement totalSaturatesBar;
    VisualElement totalSaltBar;
    VisualElement totalSugarBar;
    float maxFat;
    float maxSaturates;
    float maxSalt;
    float maxSugar;

    List<Food> foodDeck = Constants.PlayerData.FoodDeck;
    List<FoodByQuantity> FoodByQuantity = new List<FoodByQuantity>();

    public void InitialiseFoodDeck(VisualElement root, VisualTreeAsset listTemplate, SceneLogic3D sceneLogic3D)
    {
        this.listTemplate = listTemplate;
        this.sceneLogic = sceneLogic3D;

        deckSizeText = root.Q<Label>("Size");
        updateDeck = root.Q<Button>("updateDeck");
        cancel = root.Q<Button>("cancel");

        totalCalories = root.Q<Label>("totalCalories");
        totalFat = root.Q<Label>("totalFat");
        totalSaturates = root.Q<Label>("totalSaturates");
        totalSalt = root.Q<Label>("totalSalt");
        totalSugar = root.Q<Label>("totalSugar");

        totalFatBar = root.Q<VisualElement>("totalFatBar");
        totalSaturatesBar = root.Q<VisualElement>("totalSaturatesBar");
        totalSaltBar = root.Q<VisualElement>("totalSaltBar");
        totalSugarBar = root.Q<VisualElement>("totalSugarBar");

        var currentHigherLevel = Constants.Levels.Where(x => x.Unlocked == true).ToList().OrderBy(x => x.Id).First();

        maxFat = currentHigherLevel.MaxFat;
        maxSaturates = currentHigherLevel.MaxSaturates;
        maxSalt = currentHigherLevel.MaxSalt;
        maxSugar = currentHigherLevel.MaxSugar;

        foodList = root.Q<ListView>("foodList");
        foodList.Q<ScrollView>().verticalScrollerVisibility = ScrollerVisibility.Hidden;
        foodList.Q<ScrollView>().mouseWheelScrollSize = 1000f;

        topBar = root.Q<VisualElement>("topBar");
        SetSafeArea();
        

        foreach (var food in Constants.PlayerData.PlayerFood)
        {
            FoodByQuantity.Add(new Assets.FoodByQuantity() { Food = Constants.FoodsDatabase.First(x => x.Id == food.FoodId), Quantity = food.FoodOnDeck });
        }

        UpdateBars();

        var foodNotUsedByUnlocked = Constants.FoodsDatabase.Where(x => !Constants.PlayerData.PlayerFood.Any(y => y.FoodId == x.Id) && Constants.PlayerData.PlayerFood.Any(z => z.FoodId == x.Id));
        foreach(var food in foodNotUsedByUnlocked)
        {
            FoodByQuantity.Add(new FoodByQuantity() { Food = food.Clone(), Quantity = 0 });
        }

        FoodByQuantity = FoodByQuantity.OrderBy(x => x.Food.Name).ToList();

        var foodsNotUsed = Constants.FoodsDatabase.Where(x => !foodDeck.Any(y => y.Name == x.Name) && !Constants.PlayerData.PlayerFood.Any(z => z.FoodId == x.Id)).ToList();
        foreach(var food in foodsNotUsed)
        {
            FoodByQuantity.Add(new FoodByQuantity() { Food = food.Clone(), Quantity = 0 });
        }

        deckSizeText.text = $"{FoodByQuantity.Sum(x => x.Quantity)} / {Constants.MAX_DECK_SIZE}";

        FillFoodList();

        updateDeck.RegisterCallback<MouseEnterEvent>((MouseOverEvent) => 
        {
            if (updateDeck.enabledSelf)
            {
                updateDeck.style.backgroundColor = new StyleColor(new Color32(235, 235, 235, 255));
            }
        
        });

        updateDeck.RegisterCallback<MouseLeaveEvent>((MouseOverEvent) =>
        {
            if (updateDeck.enabledSelf)
            {
                updateDeck.style.backgroundColor = new StyleColor(Color.white);
            }

        });

        cancel.RegisterCallback<MouseEnterEvent>((MouseOverEvent) =>
        {
            cancel.style.backgroundColor = new StyleColor(new Color32(235, 235, 235, 255));

        });

        cancel.RegisterCallback<MouseLeaveEvent>((MouseOverEvent) =>
        {
            cancel.style.backgroundColor = new StyleColor(Color.white);

        });

        updateDeck.clicked += UpdateDeck_clicked;
        cancel.clicked += Cancel_clicked;

    }

    private void SetSafeArea()
    {
        var safeArea = Screen.safeArea;

        // Calculate the target width based on the screen width and 16:9 aspect ratio
        int targetHeight = Screen.width * 16 / 9;
        int difference = Screen.height - (int)safeArea.height;


        if (Application.platform == RuntimePlatform.Android)
        {
            topBar.style.maxHeight = new StyleLength(new Length((difference * 1920 / Screen.height) / (DisplayMetricsAndroid.Density + 1), LengthUnit.Pixel));
            topBar.style.minHeight = new StyleLength(new Length((difference * 1920 / Screen.height) / (DisplayMetricsAndroid.Density + 1), LengthUnit.Pixel));
            topBar.style.height = new StyleLength(new Length((difference * 1920 / Screen.height) / (DisplayMetricsAndroid.Density + 1), LengthUnit.Pixel));
            topBar.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>("topBar"));
        }
        else
        {
            //TEST
            if (Screen.height >= 1920)
            {
                topBar.style.maxHeight = new StyleLength(new Length((difference * 1920 / Screen.height) / ((Screen.dpi / 160) + 1), LengthUnit.Pixel));
                topBar.style.minHeight = new StyleLength(new Length((difference * 1920 / Screen.height) / ((Screen.dpi / 160) + 1), LengthUnit.Pixel));
                topBar.style.height = new StyleLength(new Length((difference * 1920 / Screen.height) / ((Screen.dpi / 160) + 1), LengthUnit.Pixel));
                topBar.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>("topBar"));
            }
            else
            {
                topBar.style.maxHeight = new StyleLength(new Length((difference * 1920 / Screen.height) / ((Screen.dpi / 160) + 1), LengthUnit.Pixel));
                topBar.style.minHeight = new StyleLength(new Length((difference * 1920 / Screen.height) / ((Screen.dpi / 160) + 1), LengthUnit.Pixel));
                topBar.style.height = new StyleLength(new Length((difference * 1920 / Screen.height) / ((Screen.dpi / 160) + 1), LengthUnit.Pixel));
                topBar.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>("topBar"));
            }
        }
    }


    private void Cancel_clicked()
    {
        sceneLogic.BackToMenu();
    }

    private void UpdateDeck_clicked()
    {
       SaveNewDeck();

       sceneLogic.BackToMenu();
    }

    public float GetListItemHeight()
    {
        return foodList.fixedItemHeight;
    }

    void FillFoodList()
    {

        foodList.makeItem = () => {

            var newListItem = listTemplate.Instantiate();

            var newListItemLogic = new FoodItemListController();

            newListItem.userData = newListItemLogic;

            newListItemLogic.SetVisualElements(newListItem, this);

            newListItem.style.width = new StyleLength(Length.Percent(85f));

            return newListItem;
        };

     

        foodList.bindItem = (item, index) => {

            (item.userData as FoodItemListController)?.SetFoodData(FoodByQuantity[index], FoodByQuantity.Sum(x => x.Quantity), sceneLogic.GetCamera());

            if (index == 0)
            {
                (item.userData as FoodItemListController)?.SetFirst();
            }
            else
            {
                (item.userData as FoodItemListController)?.RemoveFirst();
            }

        };

        foodList.itemsSource = FoodByQuantity;
    }

    public void SetFixedItemHeight(float newHeight)
    {
        if(newHeight != oldHeight)
        foodList.fixedItemHeight = newHeight;
    }

    private void UpdateBars()
    {
        var foodSelected = FoodByQuantity.Where(x => x.Quantity > 0).ToList();
        int calories = 0;
        float fat = 0;
        float saturates = 0;
        float salt = 0;
        float sugar = 0;

        foreach (FoodByQuantity food in foodSelected)
        {
            calories += food.Food.Calories * food.Quantity;
            fat += food.Food.NutritionElements[NutritionElementsEnum.Fat] * food.Quantity;
            saturates += food.Food.NutritionElements[NutritionElementsEnum.Saturates] * food.Quantity;
            salt += food.Food.NutritionElements[NutritionElementsEnum.Salt] * food.Quantity;
            sugar += food.Food.NutritionElements[NutritionElementsEnum.Sugar] * food.Quantity;
        }

        totalCalories.text = calories.ToString();
        totalFat.text = Math.Round(fat, 1).ToString();
        totalSaturates.text = Math.Round(saturates, 1).ToString();
        totalSalt.text = Math.Round(salt, 1).ToString();
        totalSugar.text = Math.Round(sugar, 1).ToString();

        float fatBarMax = maxFat * 6;
        float saturatesBarMax = maxSaturates * 6;
        float saltBarMax = maxSalt * 6;
        float sugarBarMax = maxSugar * 6;

        totalFatBar.style.width = new Length(fat / fatBarMax * 100 > 99 ? 99 : fat / fatBarMax * 100, LengthUnit.Percent);
        totalSaturatesBar.style.width = new Length(saturates / saturatesBarMax * 99 > 100 ? 99 : saturates / saturatesBarMax * 100, LengthUnit.Percent);
        totalSaltBar.style.width = new Length(salt / saltBarMax * 100 > 99 ? 99 : salt / saltBarMax * 100, LengthUnit.Percent);
        totalSugarBar.style.width = new Length(sugar / sugarBarMax * 100 > 99 ? 99 : sugar / sugarBarMax * 100, LengthUnit.Percent);
    }
  
    public void RefreshDeckSize()
    {
        var deckSize = FoodByQuantity.Sum(x => x.Quantity);

        deckSizeText.text = $"{deckSize} / {Constants.MAX_DECK_SIZE}";
        totalCalories.text = FoodByQuantity.Where(x => x.Quantity > 0).Sum(y => y.Food.Calories).ToString();
        QuantityChanged?.Invoke(this, FoodByQuantity.Sum(x => x.Quantity));

       UpdateBars();

        if (deckSize < Constants.MIN_DECK_SIZE)
        {
            updateDeck.SetEnabled(false);
        }
        else
        {
            updateDeck.SetEnabled(true);
        }
    }

    public void SaveNewDeck()
    {
        List<Food> newFoodDeck = new List<Food>();

        foreach(var foodQuantity in FoodByQuantity)
        {
            for(int index = 0; index < foodQuantity.Quantity;  index++) 
            { 
                newFoodDeck.Add(foodQuantity.Food.Clone());
            }
        }

        foreach (Food food in newFoodDeck)
        {
            if (Constants.PlayerData.PlayerFood.Any(x => x.FoodId == food.Id))
            {
                Constants.PlayerData.PlayerFood.First(x => x.FoodId == food.Id).FoodOnDeck = newFoodDeck.Count(x => x.Id == food.Id);
            }
        }

        foreach (PlayerFood playerFood in Constants.PlayerData.PlayerFood.Where(x => !newFoodDeck.Any(y => y.Id == x.FoodId)))
        {
            playerFood.FoodOnDeck = 0;
        }

        sceneLogic.dataService.StorePlayerFood(Constants.PlayerData.PlayerFood);

        Constants.PlayerData.InitialiseFoodDeck();

    }
}
