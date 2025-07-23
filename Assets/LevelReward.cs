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
        public int LevelId { get; set; }

        public GradesEnum Grade { get; set; }

        public int FoodId { get; set; }

        public int FoodQuantity { get; set; }
    }
}
