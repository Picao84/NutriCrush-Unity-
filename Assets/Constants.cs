using Assets;
using Assets.UI;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    public static int MAX_DECK_SIZE = 60;

    public static int MIN_DECK_SIZE = 30;

    public static List<string> SectionNames = new List<string>()
    {
        "Baby",
        "Toddler",
        "Child",
        "Teenager",
        "Young Adult",
        "Adult",
        "Middle Aged",
        "Early Senior",
        "Senior",
        "Old Senior"
    };

    public static Dictionary<NutritionElementsEnum, ParticleSystem.MinMaxGradient> ParticleGradients = new Dictionary<NutritionElementsEnum, ParticleSystem.MinMaxGradient> 
    {
        { NutritionElementsEnum.Fat, new ParticleSystem.MinMaxGradient(new Color32(217, 28, 28, 255), new Color32(255, 255, 255, 255)) },
        { NutritionElementsEnum.Saturates, new ParticleSystem.MinMaxGradient(new Color32(79, 121, 79, 255), new Color32(255, 255, 255, 255)) },
         { NutritionElementsEnum.Salt, new ParticleSystem.MinMaxGradient(new Color32(211, 211, 29, 255), new Color32(255, 255, 255, 255)) },
           { NutritionElementsEnum.Sugar, new ParticleSystem.MinMaxGradient(new Color32(218, 43, 177, 255), new Color32(255, 255, 255, 255)) }


    };

    public static Dictionary<int, List<int>> FoodRequiredPerSection = new Dictionary<int, List<int>>
    {
        { 0, new List<int>{ } },
        { 1, new List<int>{ 3, 6, 9 } },
        { 2, new List<int>{ 3, 6, 9 } }
    };
   

    public static List<Level> Levels = new List<Level>()
    {
        new Level(0, "Level 1", 500, 10, 8, 2, 8, new Dictionary<GradesEnum, LevelReward>
        {
            { GradesEnum.A, new LevelReward(3, 3) },
            { GradesEnum.B, new LevelReward(6, 3) },
            { GradesEnum.C, new LevelReward(9, 3) },
            { GradesEnum.D, new LevelReward(7, 2) },
            { GradesEnum.E, new LevelReward(2,2) },
            { GradesEnum.F, new LevelReward(4,2) },
            { GradesEnum.G, new LevelReward(1,1) },
            { GradesEnum.H, new LevelReward(0,1) },

        }),
        new Level(1, "Level 2", 900, 12, 10, 2, 10, new Dictionary<GradesEnum, LevelReward>
        {
            { GradesEnum.A, new LevelReward(8,3) },
            { GradesEnum.B, new LevelReward(3,3) },
            { GradesEnum.C, new LevelReward(6,3) },
            { GradesEnum.D, new LevelReward(9,2) },
            { GradesEnum.E, new LevelReward(7,2) },
            { GradesEnum.F, new LevelReward(2,2) },
            { GradesEnum.G, new LevelReward(4,1) },
            { GradesEnum.H, new LevelReward(1,2) },
        }),
        new Level(2, "Level 3", 900, 12, 10, 2, 12, new Dictionary<GradesEnum, LevelReward>
        {
           { GradesEnum.A, new LevelReward(8,3) },
            { GradesEnum.B, new LevelReward(3,3) },
            { GradesEnum.C, new LevelReward(6,3) },
            { GradesEnum.D, new LevelReward(9,2) },
            { GradesEnum.E, new LevelReward(7,2) },
            { GradesEnum.F, new LevelReward(2,2) },
            { GradesEnum.G, new LevelReward(4,1) },
            { GradesEnum.H, new LevelReward(1,2) },
        }),
          new Level(3, "Level 4", 900, 12, 10, 2, 14, new Dictionary<GradesEnum, LevelReward>
        {
           { GradesEnum.A, new LevelReward(8,3) },
            { GradesEnum.B, new LevelReward(3,3) },
            { GradesEnum.C, new LevelReward(6,3) },
            { GradesEnum.D, new LevelReward(9,2) },
            { GradesEnum.E, new LevelReward(7,2) },
            { GradesEnum.F, new LevelReward(2,2) },
            { GradesEnum.G, new LevelReward(4,1) },
            { GradesEnum.H, new LevelReward(1,2) },
        }),
            new Level(4, "Level 5", 900, 12, 10, 2, 16, new Dictionary<GradesEnum, LevelReward>
        {
           { GradesEnum.A, new LevelReward(8,3) },
            { GradesEnum.B, new LevelReward(3,3) },
            { GradesEnum.C, new LevelReward(6,3) },
            { GradesEnum.D, new LevelReward(9,2) },
            { GradesEnum.E, new LevelReward(7,2) },
            { GradesEnum.F, new LevelReward(2,2) },
            { GradesEnum.G, new LevelReward(4,1) },
            { GradesEnum.H, new LevelReward(1,2) },
        }),
              new Level(5, "Level 6", 900, 12, 10, 2, 2, new Dictionary<GradesEnum, LevelReward>
        {
           { GradesEnum.A, new LevelReward(8,3) },
            { GradesEnum.B, new LevelReward(3,3) },
            { GradesEnum.C, new LevelReward(6,3) },
            { GradesEnum.D, new LevelReward(9,2) },
            { GradesEnum.E, new LevelReward(7,2) },
            { GradesEnum.F, new LevelReward(2,2) },
            { GradesEnum.G, new LevelReward(4,1) },
            { GradesEnum.H, new LevelReward(1,2) },
        }),
                new Level(6, "Level 7", 900, 12, 10, 2, 2, new Dictionary<GradesEnum, LevelReward>
        {
           { GradesEnum.A, new LevelReward(8,3) },
            { GradesEnum.B, new LevelReward(3,3) },
            { GradesEnum.C, new LevelReward(6,3) },
            { GradesEnum.D, new LevelReward(9,2) },
            { GradesEnum.E, new LevelReward(7,2) },
            { GradesEnum.F, new LevelReward(2,2) },
            { GradesEnum.G, new LevelReward(4,1) },
            { GradesEnum.H, new LevelReward(1,2) },
        }),
            new Level(7, "Level 8", 900, 12, 10, 2, 2, new Dictionary<GradesEnum, LevelReward>
        {
           { GradesEnum.A, new LevelReward(8,3) },
            { GradesEnum.B, new LevelReward(3,3) },
            { GradesEnum.C, new LevelReward(6,3) },
            { GradesEnum.D, new LevelReward(9,2) },
            { GradesEnum.E, new LevelReward(7,2) },
            { GradesEnum.F, new LevelReward(2,2) },
            { GradesEnum.G, new LevelReward(4,1) },
            { GradesEnum.H, new LevelReward(1,2) },
        }),
              new Level(8, "Level 9", 900, 12, 10, 2, 2, new Dictionary<GradesEnum, LevelReward>
        {
           { GradesEnum.A, new LevelReward(8,3) },
            { GradesEnum.B, new LevelReward(3,3) },
            { GradesEnum.C, new LevelReward(6,3) },
            { GradesEnum.D, new LevelReward(9,2) },
            { GradesEnum.E, new LevelReward(7,2) },
            { GradesEnum.F, new LevelReward(2,2) },
            { GradesEnum.G, new LevelReward(4,1) },
            { GradesEnum.H, new LevelReward(1,2) },
        }),


    };

    public static List<Food> FoodsDatabase = new List<Food>()
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