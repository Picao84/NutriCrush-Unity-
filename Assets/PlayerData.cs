using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets
{
    public static class PlayerData
    {
        public static List<int> FoodsUnlockedByPlayer = new List<int>()
        {
            0, 1, 2, 4, 7
        };


        public static List<Food> FoodDeck { get; private set; } = new List<Food>();

        public static void UpdateFoodDeck (List<Food> foodDeck)
        {
            FoodDeck = foodDeck;
        }

        public static void InitialiseFoodDeck()
        {
            foreach (var food in PlayerData.FoodsUnlockedByPlayer) 
            { 
                for(int index = 0; index < 6; index++)
                {
                    FoodDeck.Add(Constants.FoodsDatabase.FirstOrDefault(x => x.Id == food));
                }
            
            }
        }
    }
}
