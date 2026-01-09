using Assets;
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
    

    List<Food> foodDeck = Constants.PlayerData.FoodDeck;
    List<FoodByQuantity> FoodByQuantity = new List<FoodByQuantity>();

    public void InitialiseFoodDeck(VisualElement root, VisualTreeAsset listTemplate, SceneLogic3D sceneLogic3D)
    {
        this.listTemplate = listTemplate;
        this.sceneLogic = sceneLogic3D;

        deckSizeText = root.Q<Label>("Size");
        updateDeck = root.Q<Button>("updateDeck");
        cancel = root.Q<Button>("cancel");

      

        foodList = root.Q<ListView>("foodList");
        foodList.Q<ScrollView>().verticalScrollerVisibility = ScrollerVisibility.Hidden;
        foodList.Q<ScrollView>().mouseWheelScrollSize = 1000f;
        

        foreach (var food in Constants.PlayerData.PlayerFood)
        {
            FoodByQuantity.Add(new Assets.FoodByQuantity() { Food = Constants.FoodsDatabase.First(x => x.Id == food.FoodId), Quantity = food.FoodOnDeck });
        }


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
  
    public void RefreshDeckSize()
    {
        var deckSize = FoodByQuantity.Sum(x => x.Quantity);

        deckSizeText.text = $"{deckSize} / {Constants.MAX_DECK_SIZE}";
        QuantityChanged?.Invoke(this, FoodByQuantity.Sum(x => x.Quantity));

        if(deckSize < Constants.MIN_DECK_SIZE)
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
