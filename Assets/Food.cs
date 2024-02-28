using System.Collections.Generic;

public class Food
{
    public int Id { get; set; }
    public string Name { get; set; }

    public int Calories { get; set; }  

    public string FileName { get; set; }

    public Dictionary<NutritionElementsEnum, float> NutritionElements = new Dictionary<NutritionElementsEnum, float>();

}