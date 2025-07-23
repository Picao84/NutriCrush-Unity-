using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets
{
    public class PlayerData
    {
        public List<int> SectionsUnlocked = new List<int>()
        {
            1,
        };

        public List<int> LevelsUnlocked = new List<int>()
        {
            1,
        };

        public List<PlayerFood> PlayerFood { get; set; }


        public List<Food> FoodDeck { get; private set; } = new List<Food>();

        public void UpdateFoodDeck (List<Food> foodDeck)
        {
            FoodDeck = foodDeck;
        }

        public void InitialiseFoodDeck()
        {
            foreach (var food in PlayerFood) 
            {
                for (int index = 0; index < food.FoodOnDeck; index++)
                {
                    FoodDeck.Add(Constants.FoodsDatabase.FirstOrDefault(x => x.Id == food.FoodId));
                }
            }
        }
    }
}
