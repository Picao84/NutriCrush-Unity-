using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets
{
    public class Section
    {
        public int Id { get; set; }

        public string SectionName { get; set; }

        public bool Unlocked { get; set; }

        public List<SectionFood> FoodToUnlock { get; set; } = new List<SectionFood>();
    }
}
