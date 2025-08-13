using Assets;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using TMPro;
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
    BarsUIElement barsUIElement = new BarsUIElement();
    FoodListController FoodListController;
    VisualElement foodDataAndQuantity;
    VisualElement lockedFoodMessage;
    Label effectDesc;
       

    public void SetVisualElements(VisualElement visualElement, FoodListController foodListController)
    {
        this.FoodListController = foodListController;
        foodName = visualElement.Q<Label>("foodName");
        calories = visualElement.Q<Label>("calories");
        foodImage = visualElement.Q<VisualElement>("foodImage");
        bars = visualElement.Q<VisualElement>("bars");
        bars.Add(barsUIElement);
        foodQuantity = visualElement.Q<Label>("foodQuantity");
        effectDesc = visualElement.Q<Label>("effectDesc");
        foodDataAndQuantity = visualElement.Q<VisualElement>("foodDataAndQuantity");
        lockedFoodMessage = visualElement.Q<VisualElement>("lockedFoodMessage");
        plus = visualElement.Q<Button>("plus");
        plus.clicked += Plus_clicked;
        minus = visualElement.Q<Button>("minus");
        minus.clicked += Minus_clicked;
        foodListController.QuantityChanged += FoodListController_QuantityChanged;

       

        plus.RegisterCallback<MouseEnterEvent>((MouseOverEvent) =>
        {
            if (plus.enabledSelf)
            {
                plus.style.backgroundColor = new StyleColor(new Color32(235, 235, 235, 255));
            }

        });

        plus.RegisterCallback<MouseLeaveEvent>((MouseOverEvent) =>
        {
            plus.style.backgroundColor = new StyleColor(Color.white);

        });

        minus.RegisterCallback<MouseEnterEvent>((MouseOverEvent) =>
        {
            if (minus.enabledSelf)
            {
                minus.style.backgroundColor = new StyleColor(new Color32(235, 235, 235, 255));
            }

        });

        minus.RegisterCallback<MouseLeaveEvent>((MouseOverEvent) =>
        {
            minus.style.backgroundColor = new StyleColor(Color.white);

        });
    }

    private void FoodListController_QuantityChanged(object sender, int e)
    {
        if(e >= Constants.MAX_DECK_SIZE)
        {
            plus.SetEnabled(false);
        }
        else
        {
            if (Constants.PlayerData.PlayerFood.Any(x => x.FoodId == foodByQuantity.Food.Id))
            {
                if (foodByQuantity.Quantity < Constants.PlayerData.PlayerFood.First(x => x.FoodId == foodByQuantity.Food.Id).FoodTotal)
                {
                    plus.SetEnabled(true);
                }
                else
                {
                    plus.SetEnabled(false);
                }
            }
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

        if(foodByQuantity.Quantity == 0)
        {
            minus.SetEnabled(false);
        }

        plus.SetEnabled(true);
    }

    private void Plus_clicked()
    {
        minus.SetEnabled(true);

        if (foodByQuantity.Quantity < Constants.PlayerData.PlayerFood.First(x => x.FoodId == foodByQuantity.Food.Id).FoodTotal)
        {
            foodByQuantity.Quantity++;
            foodQuantity.text = foodByQuantity.Quantity.ToString();
            this.FoodListController.RefreshDeckSize();
        }

        if (foodByQuantity.Quantity == Constants.PlayerData.PlayerFood.First(x => x.FoodId == foodByQuantity.Food.Id).FoodTotal)
        {
            plus.SetEnabled(false);
        }
    }

    public void SetFoodData(FoodByQuantity foodByQuantity, int deckSize)
    {
        this.foodByQuantity = foodByQuantity;
        foodName.text = foodByQuantity.Food.Name;
        calories.text = foodByQuantity.Food.Calories.ToString();
        foodImage.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>(foodByQuantity.Food.FileName));
        barsUIElement.Food = foodByQuantity.Food;
        foodQuantity.text = this.foodByQuantity.Quantity.ToString();
        effectDesc.text = foodByQuantity.Food.Effect.Description;

        if(!Constants.PlayerData.PlayerFood.Any(x => x.FoodId == foodByQuantity.Food.Id))
        {
            foodDataAndQuantity.style.display = DisplayStyle.None;
            lockedFoodMessage.style.display = DisplayStyle.Flex;
            effectDesc.style.display = DisplayStyle.None;
        }
        else
        {
            foodDataAndQuantity.style.display = DisplayStyle.Flex;
            lockedFoodMessage.style.display = DisplayStyle.None;
            effectDesc.style.display = DisplayStyle.Flex;

            if (deckSize >= Constants.MAX_DECK_SIZE)
            {
                plus.SetEnabled(false);
            }
            else
            {
                if (foodByQuantity.Quantity < Constants.PlayerData.PlayerFood.First(x => x.FoodId == foodByQuantity.Food.Id).FoodTotal)
                {
                    plus.SetEnabled(true);
                }
                else
                {
                    plus.SetEnabled(false);
                }
            }
        }
    }
   
}
