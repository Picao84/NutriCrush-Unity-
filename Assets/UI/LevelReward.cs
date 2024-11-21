using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.UI
{
    public class LevelReward
    {
        public int FoodId { get; }

        public int Quantity { get; }

        public LevelReward(int foodId, int quantity)
        {
            FoodId = foodId;

            Quantity = quantity;
        }
    }
}
