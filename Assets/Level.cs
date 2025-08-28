using Assets.UI;
using SQLite4Unity3d;
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
        [PrimaryKey]
        public int Id { get; set; }

        public string Name { get; set; }

        public int CaloriesObjective { get; set; }

        public float MaxFat { get; set; }

        public float MaxSaturates { get; set; }

        public float MaxSalt { get; set; }

        public float MaxSugar { get; set; }

        public Dictionary<GradesEnum, LevelReward> Rewards { get; } = new Dictionary<GradesEnum, LevelReward>();

        public List<LevelReward> RewardsList { get; set; } = new List<LevelReward>();

        public bool Unlocked { get; set; }

        public int FoodExpires { get; set; }

        public int FoodEffects { get; set; }


    }
}
