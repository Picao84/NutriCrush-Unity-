using SQLite4Unity3d;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets
{
    
    public class PlayerFood
    {
        [PrimaryKey]
        public int FoodId { get; set; }

        public int FoodTotal { get; set; }

        public int FoodOnDeck { get; set; }
    }
}
