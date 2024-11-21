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

    public SceneLogic3D sceneLogic;

    public event EventHandler<int> QuantityChanged;

    Label deckSizeText;
    Button updateDeck;
    Button cancel;

    List<Food> foodDeck = PlayerData.FoodDeck;
    List<FoodByQuantityRow> FoodByQuantityInRow = new List<FoodByQuantityRow>();
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


        foreach (var food in foodByName)
        {
            FoodByQuantity.Add(new Assets.FoodByQuantity() { Food = food.First(), Quantity = food.Count() });
           
        }

        FoodByQuantity = FoodByQuantity.OrderBy(x => x.Food.Name).ToList();


        var foodsNotUsed = Constants.FoodsDatabase.Where(x => !foodDeck.Any(y => y.Name == x.Name)).ToList();
        foreach(var food in foodsNotUsed)
        {
            FoodByQuantity.Add(new FoodByQuantity() { Food = food.Clone(), Quantity = 0 });
        }

        int index = 0;
        FoodByQuantityRow newRow = null;

        foreach (var food in FoodByQuantity)
        {
            if (index == 3)
                index = 0;

            if (index == 0)
            {
                newRow = new FoodByQuantityRow();
                FoodByQuantityInRow.Add(newRow);
            }

            newRow?.RowFood.Add(food);
            index++;
        }



        deckSizeText.text = $"{FoodByQuantity.Sum(x => x.Quantity)} / {Constants.MAX_DECK_SIZE}";

        FillFoodList();

        updateDeck.RegisterCallback<MouseEnterEvent>((MouseOverEvent) => 
        {
            updateDeck.style.backgroundColor = new StyleColor(new Color32(235,235,235,255));
        
        });

        updateDeck.RegisterCallback<MouseLeaveEvent>((MouseOverEvent) =>
        {
            updateDeck.style.backgroundColor = new StyleColor(Color.white);

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

            newListItemLogic.SetVisualElements(newListItem, this, FoodByQuantity.Sum(x => x.Quantity));
        
            return newListItem;
        };

        foodList.bindItem = (item, index) => {

            (item.userData as FoodItemListController)?.SetFoodData(FoodByQuantityInRow[index]);
        
        };

        foodList.fixedItemHeight = 200;

        foodList.itemsSource = FoodByQuantityInRow;
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
