using Assets;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class FoodItemListController
{
    FoodByQuantityRow foodByQuantityRow;
    FoodListController FoodListController;
    List<VisualElement> FoodElements = new List<VisualElement>();
    List<BarsUIElement> BarElements = new List<BarsUIElement>();
    List<Button> plusButtons = new List<Button>();
    List<Button> minusButtons = new List<Button>();
    List<Label> foodQuantityLabels = new List<Label>();


    public void SetVisualElements(VisualElement visualElement, FoodListController foodListController, int deckSize)
    {
        this.FoodListController = foodListController;
        FoodElements.Add(visualElement.Q<VisualElement>("Food1"));
        FoodElements.Add(visualElement.Q<VisualElement>("Food2"));
        FoodElements.Add(visualElement.Q<VisualElement>("Food3"));

        foreach(VisualElement foodElement in FoodElements)
        {
            var bars = foodElement.Q<VisualElement>("bars");
            var barElement = new BarsUIElement();
            BarElements.Add(barElement);
            bars.Add(barElement);

            foodQuantityLabels.Add(foodElement.Q<Label>("foodQuantity"));
        }

        for(int index =0; index < FoodElements.Count; index ++)
        {
            var plus = FoodElements[index].Q<Button>("plus");
            plusButtons.Add(plus);

            switch (index)
            {
                case 0:

                    plus.clicked += Plus_clicked1;

                    break;

                case 1:

                    plus.clicked += Plus_clicked2;

                    break;

                case 2:

                    plus.clicked += Plus_clicked3;

                    break;
            }

            var minus = FoodElements[index].Q<Button>("minus");
            minusButtons.Add(minus);

            switch (index)
            {
                case 0:

                    minus.clicked += Minus_clicked1;

                    break;

                case 1:

                    minus.clicked += Minus_clicked2;

                    break;

                case 2:

                    minus.clicked += Minus_clicked3;

                    break;
            }
        }


        foodListController.QuantityChanged += FoodListController_QuantityChanged;

        if (deckSize >= Constants.MAX_DECK_SIZE)
        {
            foreach(var button in plusButtons)
            {
                button.SetEnabled(false);
            }          
        }
        else
        {
            foreach (var button in plusButtons)
            {
                button.SetEnabled(true);
            }
        }

        if (deckSize > Constants.MIN_DECK_SIZE)
        {
            foreach (var button in minusButtons)
            {
                button.SetEnabled(true);
            }
        }
        else
        {
            foreach (var button in minusButtons)
            {
                button.SetEnabled(false);
            }
        }

        foreach (var button in plusButtons)
        {
            button.RegisterCallback<MouseEnterEvent>((MouseOverEvent) =>
            {
                if (button.enabledSelf)
                {
                    button.style.backgroundColor = new StyleColor(new Color32(235, 235, 235, 255));
                }

            });
        }


        foreach (var button in plusButtons)
        {
            button.RegisterCallback<MouseLeaveEvent>((MouseOverEvent) =>
            {
                button.style.backgroundColor = new StyleColor(Color.white);

            });
        }

        foreach (var button in minusButtons)
        {

            button.RegisterCallback<MouseEnterEvent>((MouseOverEvent) =>
            {
                if (button.enabledSelf)
                {
                    button.style.backgroundColor = new StyleColor(new Color32(235, 235, 235, 255));
                }

            });
        }


        foreach (var button in minusButtons)
        {
            button.RegisterCallback<MouseLeaveEvent>((MouseOverEvent) =>
            {
                button.style.backgroundColor = new StyleColor(Color.white);

            });
        }
    }

    private void FoodListController_QuantityChanged(object sender, int e)
    {
        if(e >= Constants.MAX_DECK_SIZE)
        {
            foreach (var button in plusButtons)
            {
                button.SetEnabled(false);
            }
        }
        else
        {
            for(int index = 0; index < plusButtons.Count; index++)
            {
                if (foodByQuantityRow.RowFood.Count > index && foodByQuantityRow.RowFood[index].Quantity < 10)
                {
                    plusButtons[index].SetEnabled(true);
                }
            }
        }

        if(e > Constants.MIN_DECK_SIZE) 
        {
            for (int index = 0; index < minusButtons.Count; index++)
            {
                if (foodByQuantityRow.RowFood.Count > index && foodByQuantityRow.RowFood[index].Quantity > 0)
                {
                    minusButtons[index].SetEnabled(true);
                }
            }
        }
        else
        {
            foreach (var button in minusButtons)
            {
                button.SetEnabled(false);
            }
            
        }
    }

    private void Minus_clicked1()
    {
        var foodByQuantity = foodByQuantityRow.RowFood[0];

        if (foodByQuantity.Quantity > 0)
        {
            foodByQuantity.Quantity--;
            foodQuantityLabels[0].text = foodByQuantity.Quantity.ToString();
            this.FoodListController.RefreshDeckSize();
        }

        if(foodByQuantity.Quantity == 0)
        {
            minusButtons[0].SetEnabled(false);
        }

        plusButtons[0].SetEnabled(true);
    }

    private void Minus_clicked2()
    {
        var foodByQuantity = foodByQuantityRow.RowFood[1];

        if (foodByQuantity.Quantity > 0)
        {
            foodByQuantity.Quantity--;
            foodQuantityLabels[1].text = foodByQuantity.Quantity.ToString();
            this.FoodListController.RefreshDeckSize();
        }

        if (foodByQuantity.Quantity == 0)
        {
            minusButtons[1].SetEnabled(false);
        }

        plusButtons[1].SetEnabled(true);
    }

    private void Minus_clicked3()
    {
        var foodByQuantity = foodByQuantityRow.RowFood[2];

        if (foodByQuantity.Quantity > 0)
        {
            foodByQuantity.Quantity--;
            foodQuantityLabels[2].text = foodByQuantity.Quantity.ToString();
            this.FoodListController.RefreshDeckSize();
        }

        if (foodByQuantity.Quantity == 0)
        {
            minusButtons[2].SetEnabled(false);
        }

        plusButtons[2].SetEnabled(true);
    }

    private void Plus_clicked1()
    {
        minusButtons[0].SetEnabled(true);

        var foodByQuantity = foodByQuantityRow.RowFood[0];

        if (foodByQuantity.Quantity < 10)
        {
            foodByQuantity.Quantity++;
            foodQuantityLabels[0].text = foodByQuantity.Quantity.ToString();
            this.FoodListController.RefreshDeckSize();
        }

        if (foodByQuantity.Quantity == 10)
        {
            plusButtons[0].SetEnabled(false);
        }
    }

    private void Plus_clicked2()
    {
        minusButtons[1].SetEnabled(true);

        var foodByQuantity = foodByQuantityRow.RowFood[1];

        if (foodByQuantity.Quantity < 10)
        {
            foodByQuantity.Quantity++;
            foodQuantityLabels[1].text = foodByQuantity.Quantity.ToString();
            this.FoodListController.RefreshDeckSize();
        }

        if (foodByQuantity.Quantity == 10)
        {
            plusButtons[1].SetEnabled(false);
        }
    }

    private void Plus_clicked3()
    {
        minusButtons[2].SetEnabled(true);

        var foodByQuantity = foodByQuantityRow.RowFood[2];

        if (foodByQuantity.Quantity < 10)
        {
            foodByQuantity.Quantity++;
            foodQuantityLabels[2].text = foodByQuantity.Quantity.ToString();
            this.FoodListController.RefreshDeckSize();
        }

        if (foodByQuantity.Quantity == 10)
        {
            plusButtons[2].SetEnabled(false);
        }
    }

    public void SetFoodData(FoodByQuantityRow foodByQuantityRow)
    {
        this.foodByQuantityRow = foodByQuantityRow;
        for (int index = 0; index < foodByQuantityRow.RowFood.Count; index++)
        {
            SetFoodElementInRow(FoodElements[index], foodByQuantityRow.RowFood[index], index);
        }

        for (int index = FoodElements.Count - 1; index > 0; index--)
        {
            if (foodByQuantityRow.RowFood.Count <= index)
            {
                FoodElements[index].style.display = DisplayStyle.None;
            }
            else
            {
                FoodElements[index].style.display = DisplayStyle.Flex;
            }
        }
    }

    void SetFoodElementInRow(VisualElement foodElement, FoodByQuantity foodByQuantity, int order)
    {
        foodElement.Q<Label>("foodName").text = foodByQuantity.Food.Name;
        foodElement.Q<Label>("calories").text = foodByQuantity.Food.Calories.ToString();
        foodElement.Q<VisualElement>("foodImage").style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>(foodByQuantity.Food.FileName));
        BarElements[order].Food = foodByQuantity.Food;
        foodElement.Q<Label>("foodQuantity").text = foodByQuantity.Quantity.ToString();

        if (!PlayerData.FoodDeck.Any(x => x.Id == foodByQuantity.Food.Id) && !PlayerData.FoodsUnlockedByPlayer.Contains(foodByQuantity.Food.Id))
        {
            foodElement.Q<VisualElement>("foodDataAndQuantity").style.display = DisplayStyle.None;
            foodElement.Q<VisualElement>("lockedFoodMessage").style.display = DisplayStyle.Flex;
        }
        else
        {
            foodElement.Q<VisualElement>("foodDataAndQuantity").style.display = DisplayStyle.Flex;
            foodElement.Q<VisualElement>("lockedFoodMessage").style.display = DisplayStyle.None;
        }
    }
   
}
