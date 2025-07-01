using Assets;
using System;

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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

    List<Food> foodDeck = PlayerData.FoodDeck;
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

        var foodByName = foodDeck.GroupBy(x => x.Name);

        foreach(var food in foodByName)
        {
            FoodByQuantity.Add(new Assets.FoodByQuantity() { Food = food.First(), Quantity = food.Count() });
        }


        var foodNotUsedByUnlocked = Constants.FoodsDatabase.Where(x => !foodDeck.Any(y => y.Name == x.Name) && PlayerData.PlayerGlobalFoodItems.ContainsKey(x.Id));
        foreach(var food in foodNotUsedByUnlocked)
        {
            FoodByQuantity.Add(new FoodByQuantity() { Food = food.Clone(), Quantity = 0 });
        }

        FoodByQuantity = FoodByQuantity.OrderBy(x => x.Food.Name).ToList();

        var foodsNotUsed = Constants.FoodsDatabase.Where(x => !foodDeck.Any(y => y.Name == x.Name) && !PlayerData.PlayerGlobalFoodItems.ContainsKey(x.Id)).ToList();
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

    void FillFoodList()
    {
        foodList.makeItem = () => {

            var newListItem = listTemplate.Instantiate();

            var newListItemLogic = new FoodItemListController();

            newListItem.userData = newListItemLogic;

            newListItemLogic.SetVisualElements(newListItem, this);
        
            return newListItem;
        };

        foodList.bindItem = (item, index) => {

            (item.userData as FoodItemListController)?.SetFoodData(FoodByQuantity[index], FoodByQuantity.Sum(x => x.Quantity));
        
        };

        foodList.fixedItemHeight = 35;
        
        foodList.itemsSource = FoodByQuantity;
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

        PlayerData.UpdateFoodDeck(newFoodDeck);

    }
}
