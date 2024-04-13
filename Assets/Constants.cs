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

        }},
         new Food(){ Name = "Salmon", Calories = 206, FileName = "", NutritionElements = new Dictionary<NutritionElementsEnum, float>()
        {
            { NutritionElementsEnum.Fat, 9.6f },
            { NutritionElementsEnum.Salt, 0.061f },
            { NutritionElementsEnum.Sugar, 0f },
            { NutritionElementsEnum.Saturates, 2.4f }

        }},
           new Food(){ Name = "Cheese", Calories = 112, FileName = "", NutritionElements = new Dictionary<NutritionElementsEnum, float>()
        {
            { NutritionElementsEnum.Fat, 3.37f },
            { NutritionElementsEnum.Salt, 0.17f },
            { NutritionElementsEnum.Sugar, 0.15f },
            { NutritionElementsEnum.Saturates, 5.91f }

        }},
           new Food(){ Name = "Crisps", Calories = 150, FileName = "", NutritionElements = new Dictionary<NutritionElementsEnum, float>()
        {
            { NutritionElementsEnum.Fat, 8.8f },
            { NutritionElementsEnum.Salt, 0.4f },
            { NutritionElementsEnum.Sugar, 0.4f },
            { NutritionElementsEnum.Saturates, 1.1f }

        }},
            new Food(){ Name = "Ham", Calories = 45, FileName = "", NutritionElements = new Dictionary<NutritionElementsEnum, float>()
        {
            { NutritionElementsEnum.Fat, 1.22f },
            { NutritionElementsEnum.Salt, 0.4f },
            { NutritionElementsEnum.Sugar, 0f },
            { NutritionElementsEnum.Saturates, 1.22f }

        }}
    };

}