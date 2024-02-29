using System.Collections.Generic;

public static class Constants3D
{
    public static List<Food> Foods = new List<Food>()
    {
        new Food(){ Name = "Carrot", Calories = 30, FileName = "Carrot", NutritionElements = new Dictionary<NutritionElementsEnum, float>()
        {
            { NutritionElementsEnum.Fat, 0.2f },
            { NutritionElementsEnum.Salt, 0.05f },
            { NutritionElementsEnum.Sugar, 3.4f },
            { NutritionElementsEnum.Saturates, 0f }

        }},
         new Food(){ Name = "Egg", Calories = 78, FileName = "Egg", NutritionElements = new Dictionary<NutritionElementsEnum, float>()
        {
            { NutritionElementsEnum.Fat, 3.4f },
            { NutritionElementsEnum.Salt, 0.062f },
            { NutritionElementsEnum.Sugar, 0.6f },
            { NutritionElementsEnum.Saturates, 1.6f },

        }},
         new Food(){ Name = "Milk", Calories = 42, FileName = "Milk", NutritionElements = new Dictionary<NutritionElementsEnum, float>()
        {
            { NutritionElementsEnum.Fat, 0.4f },
            { NutritionElementsEnum.Salt, 0.044f },
            { NutritionElementsEnum.Sugar, 5f },
            { NutritionElementsEnum.Saturates, 0.6f }

        }},
        new Food(){ Name = "Chicken", Calories = 335, FileName = "Chicken", NutritionElements = new Dictionary<NutritionElementsEnum, float>()
        {
            { NutritionElementsEnum.Fat, 14f },
            { NutritionElementsEnum.Salt, 0.08f },
            { NutritionElementsEnum.Sugar, 0f },
            { NutritionElementsEnum.Saturates, 5f }

        }},
        new Food(){ Name = "Brown Bread", Calories = 75, FileName = "BrownBread", NutritionElements = new Dictionary<NutritionElementsEnum, float>()
        {
            { NutritionElementsEnum.Fat, 0.8f },
            { NutritionElementsEnum.Salt, 0.14f },
            { NutritionElementsEnum.Sugar, 1.5f },
            { NutritionElementsEnum.Saturates, 0.2f }

        }},
        new Food(){ Name = "Pasta", Calories = 75, FileName = "Pasta", NutritionElements = new Dictionary<NutritionElementsEnum, float>()
        {
            { NutritionElementsEnum.Fat, 0.5f },
            { NutritionElementsEnum.Salt, 0.034f },
            { NutritionElementsEnum.Sugar, 2.5f },
            { NutritionElementsEnum.Saturates, 0.1f }

        }}
    };

}