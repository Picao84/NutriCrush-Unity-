using Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Xml.Linq;
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
    VisualElement foodNameChart;
    VisualElement foodDataAndQuantity;
    VisualElement effectsAndMinus;
    VisualElement lockedFoodMessage;
    VisualElement lockImage;
    Label effectDesc;
    VisualElement row;
    Label lockedFoodText;
    VisualElement foodDataAndLock;

    Label fatText;
    Label saturatesText;
    Label saltText;
    Label sugarText;

    public void SetVisualElements(VisualElement visualElement, FoodListController foodListController)
    {
        this.FoodListController = foodListController;
        row = visualElement;
        foodName = visualElement.Q<Label>("foodName");
        calories = visualElement.Q<Label>("calories");
        foodImage = visualElement.Q<VisualElement>("foodImage");
        //bars = visualElement.Q<VisualElement>("bars");
        //bars.Add(barsUIElement);
        foodQuantity = visualElement.Q<Label>("foodQuantity");
        //effectDesc = visualElement.Q<Label>("effectDesc");
        foodNameChart = visualElement.Q<VisualElement>("foodNameChart");
        foodDataAndQuantity = visualElement.Q<VisualElement>("foodDataAndQuantity");
        effectsAndMinus = visualElement.Q<VisualElement>("effectsAndMinus");
        //lockedFoodMessage = visualElement.Q<VisualElement>("lockedFoodMessage");
        plus = visualElement.Q<Button>("plus");
        plus.clicked += Plus_clicked;
        minus = visualElement.Q<Button>("minus");
        minus.clicked += Minus_clicked;
        //lockedFoodText = lockedFoodMessage.Q<Label>("lockedFoodText");
        lockImage = visualElement.Q<VisualElement>("foodDataAndLock").Q<VisualElement>("lock");
        foodListController.QuantityChanged += FoodListController_QuantityChanged;
        foodDataAndLock = visualElement.Q<VisualElement>("foodDataAndLock");
        var originalWidth = 160;

        row.RegisterCallback<GeometryChangedEvent>((geometryChanged) => {

            var newScale = geometryChanged.newRect.width / originalWidth;

            row.style.alignSelf = Align.Center;
            row.style.scale = new StyleScale(new Vector2(newScale, newScale));

            row.style.height = 70 * newScale;

            foodListController.SetFixedItemHeight(geometryChanged.newRect.height);
        });
    


        plus.RegisterCallback<MouseEnterEvent>((MouseOverEvent) =>
        {
            if (plus.enabledSelf)
            {
                //plus.style.backgroundColor = new StyleColor(new Color32(235, 235, 235, 255));
            }

        });

     

        plus.RegisterCallback<MouseLeaveEvent>((MouseOverEvent) =>
        {
            //plus.style.backgroundColor = new StyleColor(Color.white);

        });

        minus.RegisterCallback<MouseEnterEvent>((MouseOverEvent) =>
        {
            if (minus.enabledSelf)
            {
                //minus.style.backgroundColor = new StyleColor(new Color32(235, 235, 235, 255));
            }

        });

        minus.RegisterCallback<MouseLeaveEvent>((MouseOverEvent) =>
        {
            //minus.style.backgroundColor = new StyleColor(Color.white);

        });

        var statsArea = effectsAndMinus.Q<VisualElement>("statsArea");

        fatText = effectsAndMinus.Q<VisualElement>("Fat").Q<Label>("fatText");
        saturatesText = effectsAndMinus.Q<VisualElement>("Saturates").Q<Label>("saturatesText");
        saltText = effectsAndMinus.Q<VisualElement>("Salt").Q<Label>("saltText");
        sugarText = effectsAndMinus.Q<VisualElement>("Sugar").Q<Label>("sugarText");

    }

    public void SetFirst()
    {
        if (row.worldBound.height > FoodListController.GetListItemHeight())
        {
            row.style.marginTop = Math.Abs(FoodListController.GetListItemHeight() - row.worldBound.height) / (int)(row.style.scale.value.value.y);
        }
    }


    public void RemoveFirst()
    {
       
        row.style.marginTop = 0;
        
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

    public void SetFoodData(FoodByQuantity foodByQuantity, int deckSize, Camera gameCamera)
    {
        this.foodByQuantity = foodByQuantity;
        foodName.text = foodByQuantity.Food.Name;
        calories.text = foodByQuantity.Food.Calories.ToString();

        try
        {
            foodImage.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>(foodByQuantity.Food.FileName));
        }
        catch { }

        //barsUIElement.Food = foodByQuantity.Food;
        foodQuantity.text = this.foodByQuantity.Quantity.ToString();
        //effectDesc.text = foodByQuantity.Food.Effect?.Description;
        //lockedFoodMessage.style.display = DisplayStyle.Flex;
        fatText.text = Math.Round(foodByQuantity.Food.NutritionElements[NutritionElementsEnum.Fat], 1).ToString();
        saturatesText.text = Math.Round(foodByQuantity.Food.NutritionElements[NutritionElementsEnum.Saturates],1).ToString();
        saltText.text = Math.Round(foodByQuantity.Food.NutritionElements[NutritionElementsEnum.Salt],1).ToString();
        sugarText.text = Math.Round(foodByQuantity.Food.NutritionElements[NutritionElementsEnum.Sugar],1).ToString();

        if (!Constants.PlayerData.PlayerFood.Any(x => x.FoodId == foodByQuantity.Food.Id))
        {
            
            plus.style.display = DisplayStyle.None;
            foodQuantity.style.display = DisplayStyle.None;
            minus.style.display = DisplayStyle.None;
            lockImage.style.display = DisplayStyle.Flex;
            foodDataAndLock.style.width = new StyleLength(StyleKeyword.Auto);
            //lockedFoodText.style.display = DisplayStyle.Flex;

        }
        else
        {
   
            plus.style.display = DisplayStyle.Flex;
            minus.style.display = DisplayStyle.Flex;
            foodQuantity.style.display = DisplayStyle.Flex;

            effectsAndMinus.style.display = DisplayStyle.Flex;
            //lockedFoodText.style.display = DisplayStyle.None;
            foodDataAndLock.style.width = new Length(85, LengthUnit.Pixel);
            lockImage.style.display = DisplayStyle.None;


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

            if (foodByQuantity.Quantity > 0)
            {
                minus.SetEnabled(true);
            }
            else
            {
                minus.SetEnabled(false);
            }
                
        }
    }
   
}
