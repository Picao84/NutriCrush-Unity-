using System.Collections.Generic;
using Assets;
using SQLite4Unity3d;


public class Food
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Name { get; set; }

    public int Calories { get; set; }  

    public string FileName { get; set; }

    public float Fat { set { NutritionElements.Add(NutritionElementsEnum.Fat, value); } }

    public float Sugar { set {  NutritionElements.Add(NutritionElementsEnum.Sugar, value); } }

    public float Saturates { set { NutritionElements.Add(NutritionElementsEnum.Saturates, value); } }

    public float Salt { set { NutritionElements.Add(NutritionElementsEnum.Salt, value); } }

    public Dictionary<NutritionElementsEnum, float> NutritionElements = new Dictionary<NutritionElementsEnum, float>();

    public int ExpiresIn { get; set; }

    public int EffectId {  get; set; }

    public int EffectAmount { get; set; }

    public FoodEffect Effect { get; set; }

    public Food Clone()
    {
        return new Food()
        {
            Id = Id,
            Name = Name,
            Calories = Calories,
            FileName = FileName,
            NutritionElements = NutritionElements,
            Effect = Effect,
            EffectId = EffectId,
            EffectAmount = EffectAmount,
            ExpiresIn = ExpiresIn,
            
        };
    }

}