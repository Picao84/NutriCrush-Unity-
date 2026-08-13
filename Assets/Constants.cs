using Assets;
using Assets.UI;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public static class Constants
{
    public static int MAX_DECK_SIZE = 60;

    public static int MIN_DECK_SIZE = 30;

    public static Dictionary<NutritionElementsEnum, ParticleSystem.MinMaxGradient> ParticleGradients = new Dictionary<NutritionElementsEnum, ParticleSystem.MinMaxGradient>
    {
        { NutritionElementsEnum.Fat, new ParticleSystem.MinMaxGradient(new Color32(236, 223, 137, 255), new Color32(236, 223, 137, 0)) },
        { NutritionElementsEnum.Saturates, new ParticleSystem.MinMaxGradient(new Color32(145, 179, 134, 255), new Color32(145, 179, 134, 0)) },
         { NutritionElementsEnum.Salt, new ParticleSystem.MinMaxGradient(new Color32(127, 198, 252, 255), new Color32(127, 198, 252, 0)) },
           { NutritionElementsEnum.Sugar, new ParticleSystem.MinMaxGradient(new Color32(232, 149, 148, 255), new Color32(232, 149, 148, 0)) }


    };

    public static List<Section> Sections = new List<Section>();


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

    public static readonly Dictionary<TutorialMessagesEnum, List<string>> TutorialMessages = new Dictionary<TutorialMessagesEnum, List<string>>
    {
        { TutorialMessagesEnum.BallAbsorbed, new List<string>{ "Claw-some! Lit-ter-ally crushing it! Fur-ward momentum!" } },
        { TutorialMessagesEnum.BallDownVortex, new List<string>{ "Balls getting shredded affect the health bar! Purr-tect the vitals and throw those balls in!" } },
        { TutorialMessagesEnum.Flawless, new List<string>{ "Well done! You threw all the balls in without them fall" +
            "ing back in the vortex!", "This flawless performance turns the clock back 5 seconds! Keep them coming!" } },
        { TutorialMessagesEnum.NutritionElementFull, new List<string>{ "The {0} bar is now quite full! It will not accept balls that make it go over the limit!", "Ignore balls of that color if you see the bar shaking on food selection!" } },
        { TutorialMessagesEnum.ToddlerTier, new List<string> { "Congratulations on reaching the Toddler tier!", "As a reward, you can now skip meals and refresh food!" } },
        { TutorialMessagesEnum.ChildTier, new List<string> { "Congratulations on reaching the Child tier!", "Food now expires after some time and desintegrates..." } },
        { TutorialMessagesEnum.TeenTier, new List<string> { "Placeholder" } },
        { TutorialMessagesEnum.YoungAdultTier, new List<string> { "Placeholder" } },
        { TutorialMessagesEnum.AdultTier, new List<string> { "Placeholder" } },
        { TutorialMessagesEnum.MiddleAgedTier, new List<string> { "Placeholder" } },
        { TutorialMessagesEnum.RetiredTier, new List<string> { "Placeholder" } },
        { TutorialMessagesEnum.SeniorTier, new List<string> { "Placeholder" } },
        { TutorialMessagesEnum.ElderTier, new List<string> { "Placeholder" } }

    };

    public static UserSettings UserSettings { get; set; }

    public static Dictionary<NutritionElementsEnum, Texture2D> FoodBallTextures = new Dictionary<NutritionElementsEnum, Texture2D>
    {
        
    };

    public static Dictionary<NutritionElementsEnum, Material> FoodBubbleMaterials = new Dictionary<NutritionElementsEnum, Material>
    {
       
    };


}