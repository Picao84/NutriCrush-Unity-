using Assets.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Assets
{
    public class Level
    {
        public string Name { get; }

        public int CaloriesObjective { get; }

        public int MaxFat { get; }

        public int MaxSaturates { get; }

        public int MaxSalt { get; }

        public int MaxSugar { get; }

        public Dictionary<GradesEnum, LevelReward> Rewards { get; }

        public Level(string name, int caloriesObjective, int maxFat, int maxSaturates, int maxSalt, int maxSugar, Dictionary<GradesEnum, LevelReward> rewards)
        {
            Name = name;
            CaloriesObjective = caloriesObjective;
            MaxFat = maxFat;
            MaxSaturates = maxSaturates;
            MaxSalt = maxSalt;
            MaxSugar = maxSugar;
            Rewards = rewards;
        }


    }
}
