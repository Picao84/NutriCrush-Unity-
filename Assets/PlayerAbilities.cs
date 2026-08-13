using SQLite4Unity3d;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets
{
    public class PlayerAbilities
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Name { get; set; }
        public int Unlocked { get; set; }   
    }
}
