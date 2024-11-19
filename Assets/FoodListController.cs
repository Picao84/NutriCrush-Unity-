using Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class FoodListController
{
    VisualTreeAsset listTemplate;

    ListView foodList;

    public event EventHandler<int> QuantityChanged;

    TextMeshProUGUI deckSizeText;

    List<Food> foodDeck = PlayerData.FoodDeck;
    List<FoodByQuantity> FoodByQuantity = new List<FoodByQuantity>();

    public void InitialiseFoodDeck(VisualElement root, VisualTreeAsset listTemplate, TextMeshProUGUI deckSizeText)
    {
        this.listTemplate = listTemplate;
        this.deckSizeText = deckSizeText;

        foodList = root.Q<ListView>("foodList");
        foodList.Q<ScrollView>().verticalScrollerVisibility = ScrollerVisibility.Hidden;
        foodList.Q<ScrollView>().mouseWheelScrollSize = 1000f;

        var foodByName = foodDeck.GroupBy(x => x.Name);

        foreach(var food in foodByName)
        {
            FoodByQuantity.Add(new Assets.FoodByQuantity() { Food = food.First(), Quantity = food.Count() });
        }

        var foodsNotUsed = Constants.FoodsUnlockedByUser.Where(x => !foodDeck.Any(y => y.Name == x.Name)).ToList();
        foreach(var food in foodsNotUsed)
        {
            FoodByQuantity.Add(new FoodByQuantity() { Food = food.Clone(), Quantity = 0 });
        }

        FoodByQuantity = FoodByQuantity.OrderBy(x => x.Food.Name).ToList();

        deckSizeText.text = $"{FoodByQuantity.Sum(x => x.Quantity)} / {Constants.MAX_DECK_SIZE}";

        FillFoodList();
    }
    
    void FillFoodList()
    {
        foodList.makeItem = () => {

            var newListItem = listTemplate.Instantiate();

            var newListItemLogic = new FoodItemListController();

            newListItem.userData = newListItemLogic;

            newListItemLogic.SetVisualElements(newListItem, this, FoodByQuantity.Sum(x => x.Quantity));
        
            return newListItem;
        };

        foodList.bindItem = (item, index) => {

            (item.userData as FoodItemListController)?.SetFoodData(FoodByQuantity[index]);
        
        };

        foodList.fixedItemHeight = 60;

        foodList.itemsSource = FoodByQuantity;
    }

    public void RefreshDeckSize()
    {
        deckSizeText.text = $"{FoodByQuantity.Sum(x => x.Quantity)} / {Constants.MAX_DECK_SIZE}";
        QuantityChanged?.Invoke(this, FoodByQuantity.Sum(x => x.Quantity));
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
