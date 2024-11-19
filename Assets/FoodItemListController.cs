using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FoodItemListController
{
    Label foodName;
    Label calories;
    VisualElement foodImage;
    VisualElement bars;
    Label foodQuantity;
    FoodByQuantity foodByQuantity;
    Button plus;
    Button minus;
    FoodListController FoodListController;

    public void SetVisualElements(VisualElement visualElement, FoodListController foodListController, int deckSize)
    {
        this.FoodListController = foodListController;
        foodName = visualElement.Q<Label>("foodName");
        calories = visualElement.Q<Label>("calories");
        foodImage = visualElement.Q<VisualElement>("foodImage");
        bars = visualElement.Q<VisualElement>("bars");
        foodQuantity = visualElement.Q<Label>("foodQuantity");
        plus = visualElement.Q<Button>("plus");
        plus.clicked += Plus_clicked;
        minus = visualElement.Q<Button>("minus");
        minus.clicked += Minus_clicked;
        foodListController.QuantityChanged += FoodListController_QuantityChanged;

        if (deckSize >= 60)
        {
            plus.SetEnabled(false);
        }
        else
        {
            plus.SetEnabled(true);
        }

        if (deckSize > 40)
        {
            minus.SetEnabled(true);
        }
        else
        {
            minus.SetEnabled(false);
        }
    }

    private void FoodListController_QuantityChanged(object sender, int e)
    {
        if(e >= 60)
        {
            plus.SetEnabled(false);
        }
        else
        {
            plus.SetEnabled(true);
        }

        if(e > 40) 
        {
            minus.SetEnabled(true);
        }
        else
        {
            minus.SetEnabled(false);
        }
    }

    private void Minus_clicked()
    {
        if (foodByQuantity.Quantity > 0)
        {
            foodByQuantity.Quantity--;
            foodQuantity.text = foodByQuantity.Quantity.ToString();
            this.FoodListController.RefreshDeckSize();
        }
    }

    private void Plus_clicked()
    {
        if (foodByQuantity.Quantity < 10)
        {
            foodByQuantity.Quantity++;
            foodQuantity.text = foodByQuantity.Quantity.ToString();
            this.FoodListController.RefreshDeckSize();
        }
    }

    public void SetFoodData(FoodByQuantity foodByQuantity)
    {
        this.foodByQuantity = foodByQuantity;
        foodName.text = foodByQuantity.Food.Name;
        calories.text = foodByQuantity.Food.Calories.ToString();
        foodImage.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>(foodByQuantity.Food.FileName));
        bars.Add(new BarsUIElement(foodByQuantity.Food));
        foodQuantity.text = this.foodByQuantity.Quantity.ToString();
    }
   
}
