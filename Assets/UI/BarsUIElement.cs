using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public partial class BarsUIElement : VisualElement
{
    Food food;

    int MAX_FAT = 45;
    int MAX_SATURATES = 25;
    int MAX_SALT = 5;
    int MAX_SUGAR = 36;


    public BarsUIElement(Food food)
    {
        this.food = food;
        generateVisualContent += OnGenerateVisualContent;
    
    }

    Dictionary<NutritionElementsEnum, Color> Colors = new Dictionary<NutritionElementsEnum, Color>
    {
        { NutritionElementsEnum.Fat, new Color32(217, 28, 28, 255) },
         { NutritionElementsEnum.Saturates, new Color32(79, 121, 79, 255) },
          { NutritionElementsEnum.Salt, new Color32(211, 211, 29, 255) },
           { NutritionElementsEnum.Sugar, new Color32(218, 43, 177, 255) },
    };

    void OnGenerateVisualContent(MeshGenerationContext context)
    {
        if (food != null)
        {

            var painter = context.painter2D;
            var width = context.visualElement.localBound.width;


            for (int index = 0; index < food.NutritionElements.Count; index++)
            {
                painter.fillColor = Colors[(NutritionElementsEnum)index];
                painter.BeginPath();
                painter.MoveTo(new Vector2(index * (width / 4), 28));
                painter.LineTo(new Vector2(index * (width / 4), 28 - GetAdjustedHeight((NutritionElementsEnum)index, food.NutritionElements[(NutritionElementsEnum)index])));
                painter.LineTo(new Vector2(index * (width / 4) + (width / 4), 28 - GetAdjustedHeight((NutritionElementsEnum)index, food.NutritionElements[(NutritionElementsEnum)index])));
                painter.LineTo(new Vector2(index * (width / 4) + (width / 4), 28));
                painter.ClosePath();
                painter.Fill();
            }
        }

    }

    float GetAdjustedHeight(NutritionElementsEnum type, float nutritionValue)
    {


        return type switch
        {
            NutritionElementsEnum.Fat => nutritionValue * 100 / MAX_FAT,
            NutritionElementsEnum.Saturates => nutritionValue * 100 / MAX_SATURATES,
            NutritionElementsEnum.Salt => nutritionValue * 100 / MAX_SALT,
            NutritionElementsEnum.Sugar => nutritionValue * 100 / MAX_SUGAR,
            _ => 0
        }; 
    }


    
}
