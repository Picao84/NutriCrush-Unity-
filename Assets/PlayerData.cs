using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets
{
    public static class PlayerData
    {

        public static Dictionary<int, int> TotalCardsByPlayer = new Dictionary<int, int>()
        {

            { 0, 6 },{1, 6}, {2, 6}, {4,6}, {7, 6}
        };

        public static List<Food> FoodDeck { get; private set; } = new List<Food>();

        public static void UpdateFoodDeck (List<Food> foodDeck)
        {
            FoodDeck = foodDeck;
        }

        public static void InitialiseFoodDeck()
        {
            foreach (var food in TotalCardsByPlayer) 
            { 
                for(int index = 0; index < 6; index++)
                {
                    FoodDeck.Add(Constants.FoodsDatabase.FirstOrDefault(x => x.Id == food.Key));
                }
            
            }
        }
    }
}
