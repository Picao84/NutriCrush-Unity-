using Assets;
using System.Collections.Generic;
using Unity.VisualScripting;

public static class Constants
{
    public static int MAX_DECK_SIZE = 60;

    public static int MIN_DECK_SIZE = 25;


    public static List<Level> Levels = new List<Level>()
    {
        new Level(800, 10, 8, 2, 2, new Dictionary<GradesEnum, int>
        {
            { GradesEnum.A, 3 },
            { GradesEnum.B, 6 },
            { GradesEnum.C, 9 },
            { GradesEnum.D, 7 },
            { GradesEnum.E, 2 },
            { GradesEnum.F, 4 },
            { GradesEnum.G, 1 },
            { GradesEnum.H, 0 },

        }),
        new Level(900, 12, 10, 2, 2, new Dictionary<GradesEnum, int>
        {
            { GradesEnum.A, 8 },
            { GradesEnum.B, 3 },
            { GradesEnum.C, 6 },
            { GradesEnum.D, 9 },
            { GradesEnum.E, 7 },
            { GradesEnum.F, 2 },
            { GradesEnum.G, 4 },
            { GradesEnum.H, 1 },
        }),


    };

    public static List<Food> FoodsUnlockedByUser = new List<Food>()
    {
        new Food(){ Id = 0, Name = "Carrot", Calories = 30, FileName = "Carrot", NutritionElements = new Dictionary<NutritionElementsEnum, float>()
        {
            { NutritionElementsEnum.Fat, 0.2f },
            { NutritionElementsEnum.Salt, 0.05f },
            { NutritionElementsEnum.Sugar, 3.4f },
            { NutritionElementsEnum.Saturates, 0f }

        }},
         new Food(){ Id = 1, Name = "Egg", Calories = 78, FileName = "Egg", NutritionElements = new Dictionary<NutritionElementsEnum, float>()
        {
            { NutritionElementsEnum.Fat, 3.4f },
            { NutritionElementsEnum.Salt, 0.062f },
            { NutritionElementsEnum.Sugar, 0.6f },
            { NutritionElementsEnum.Saturates, 1.6f },

        }},
         new Food(){ Id = 2, Name = "Milk", Calories = 42, FileName = "Milk", NutritionElements = new Dictionary<NutritionElementsEnum, float>()
        {
            { NutritionElementsEnum.Fat, 0.4f },
            { NutritionElementsEnum.Salt, 0.044f },
            { NutritionElementsEnum.Sugar, 5f },
            { NutritionElementsEnum.Saturates, 0.6f }

        }},
        new Food(){ Id = 3, Name = "Chicken", Calories = 335, FileName = "Chicken", NutritionElements = new Dictionary<NutritionElementsEnum, float>()
        {
            { NutritionElementsEnum.Fat, 14f },
            { NutritionElementsEnum.Salt, 0.08f },
            { NutritionElementsEnum.Sugar, 0f },
            { NutritionElementsEnum.Saturates, 5f }

        }},
        new Food(){ Id = 4, Name = "Brown Bread", Calories = 75, FileName = "BrownBread", NutritionElements = new Dictionary<NutritionElementsEnum, float>()
        {
            { NutritionElementsEnum.Fat, 0.8f },
            { NutritionElementsEnum.Salt, 0.14f },
            { NutritionElementsEnum.Sugar, 1.5f },
            { NutritionElementsEnum.Saturates, 0.2f }

        }},
        new Food(){ Id = 5, Name = "Pasta", Calories = 75, FileName = "Pasta", NutritionElements = new Dictionary<NutritionElementsEnum, float>()
        {
            { NutritionElementsEnum.Fat, 0.5f },
            { NutritionElementsEnum.Salt, 0.034f },
            { NutritionElementsEnum.Sugar, 2.5f },
            { NutritionElementsEnum.Saturates, 0.1f }

        }},
         new Food(){ Id = 6, Name = "Salmon", Calories = 206, FileName = "Salmon", NutritionElements = new Dictionary<NutritionElementsEnum, float>()
        {
            { NutritionElementsEnum.Fat, 9.6f },
            { NutritionElementsEnum.Salt, 0.061f },
            { NutritionElementsEnum.Sugar, 0f },
            { NutritionElementsEnum.Saturates, 2.4f }

        }},
           new Food(){ Id = 7, Name = "Cheese", Calories = 112, FileName = "Cheese", NutritionElements = new Dictionary<NutritionElementsEnum, float>()
        {
            { NutritionElementsEnum.Fat, 3.37f },
            { NutritionElementsEnum.Salt, 0.17f },
            { NutritionElementsEnum.Sugar, 0.15f },
            { NutritionElementsEnum.Saturates, 5.91f }

        }},
           new Food(){ Id = 8, Name = "Crisps", Calories = 150, FileName = "Crisps", NutritionElements = new Dictionary<NutritionElementsEnum, float>()
        {
            { NutritionElementsEnum.Fat, 8.8f },
            { NutritionElementsEnum.Salt, 0.4f },
            { NutritionElementsEnum.Sugar, 0.4f },
            { NutritionElementsEnum.Saturates, 1.1f }

        }},
            new Food(){ Id = 9, Name = "Ham", Calories = 45, FileName = "Ham", NutritionElements = new Dictionary<NutritionElementsEnum, float>()
        {
            { NutritionElementsEnum.Fat, 1.22f },
            { NutritionElementsEnum.Salt, 0.4f },
            { NutritionElementsEnum.Sugar, 0f },
            { NutritionElementsEnum.Saturates, 1.22f }

        }}
    };

}