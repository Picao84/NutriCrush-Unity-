using Assets;
using Assets.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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
        { 1, new List<int>{ 4, 7, 10 } },
        { 2, new List<int>{ 4, 7, 10 } }
    };
   

    public static List<Level> Levels = new List<Level>()
    {
        //new Level(0, "Level 1", 500, 10, 8, 2, 8, new Dictionary<GradesEnum, LevelReward>
        //{
        //    { GradesEnum.A, new LevelReward(4, 1) },
        //    { GradesEnum.B, new LevelReward(7, 1) },
        //    { GradesEnum.C, new LevelReward(10, 1) },


        //}),
        //new Level(1, "Level 2", 900, 12, 10, 2, 10, new Dictionary<GradesEnum, LevelReward>
        //{
        //    { GradesEnum.A, new LevelReward(4, 1) },
        //    { GradesEnum.B, new LevelReward(7, 1) },
        //    { GradesEnum.C, new LevelReward(10, 1) },

        //}),
        //new Level(2, "Level 3", 900, 12, 10, 2, 12, new Dictionary<GradesEnum, LevelReward>
        //{
        //   { GradesEnum.A, new LevelReward(9,3) },
        //    { GradesEnum.B, new LevelReward(4,3) },
        //    { GradesEnum.C, new LevelReward(7,3) },
           
        //}),
        //  new Level(3, "Level 4", 900, 12, 10, 2, 14, new Dictionary<GradesEnum, LevelReward>
        //{
        //   { GradesEnum.A, new LevelReward(4, 1) },
        //    { GradesEnum.B, new LevelReward(7, 1) },
        //    { GradesEnum.C, new LevelReward(10, 1) },

        //}),
        //    new Level(4, "Level 5", 900, 12, 10, 2, 16, new Dictionary<GradesEnum, LevelReward>
        //{
        //   { GradesEnum.A, new LevelReward(4, 1) },
        //    { GradesEnum.B, new LevelReward(7, 1) },
        //    { GradesEnum.C, new LevelReward(10, 1) },

        //}),
        //      new Level(5, "Level 6", 900, 12, 10, 2, 2, new Dictionary<GradesEnum, LevelReward>
        //{
        //   { GradesEnum.A, new LevelReward(4, 1) },
        //    { GradesEnum.B, new LevelReward(7, 1) },
        //    { GradesEnum.C, new LevelReward(10, 1) },

        //}),
        //        new Level(6, "Level 7", 900, 12, 10, 2, 2, new Dictionary<GradesEnum, LevelReward>
        //{
        //  { GradesEnum.A, new LevelReward(4, 1) },
        //    { GradesEnum.B, new LevelReward(7, 1) },
        //    { GradesEnum.C, new LevelReward(10, 1) },

        //}),
        //    new Level(7, "Level 8", 900, 12, 10, 2, 2, new Dictionary<GradesEnum, LevelReward>
        //{
        //   { GradesEnum.A, new LevelReward(4, 1) },
        //    { GradesEnum.B, new LevelReward(7, 1) },
        //    { GradesEnum.C, new LevelReward(10, 1) },

        //}),
        //      new Level(8, "Level 9", 900, 12, 10, 2, 2, new Dictionary<GradesEnum, LevelReward>
        //{
        //  { GradesEnum.A, new LevelReward(4, 1) },
        //    { GradesEnum.B, new LevelReward(7, 1) },
        //    { GradesEnum.C, new LevelReward(10, 1) },

        //}),


    };

    public static PlayerData PlayerData { get; set; } = new PlayerData();

    public static List<Food> FoodsDatabase { get; set; }

}