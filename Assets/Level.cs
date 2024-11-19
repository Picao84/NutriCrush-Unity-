using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets
{
    public class Level
    {
        public int CaloriesObjective { get; }

        public int MaxFat { get; }

        public int MaxSaturates { get; }

        public int MaxSalt { get; }

        public int MaxSugar { get; }

        public Dictionary<GradesEnum, int> Rewards { get; }

        public Level(int caloriesObjective, int maxFat, int maxSaturates, int maxSalt, int maxSugar, Dictionary<GradesEnum, int> rewards)
        {
            CaloriesObjective = caloriesObjective;
            MaxFat = maxFat;
            MaxSaturates = maxSaturates;
            MaxSalt = maxSalt;
            MaxSugar = maxSugar;
            Rewards = rewards;
        }


    }
}
