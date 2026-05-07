using SQLite4Unity3d;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets
{
    public class UnlockedLevels
    {
        [PrimaryKey]
        public int LevelId { get; set; }

        public int? MaxGrade { get; set; }
    }
}
