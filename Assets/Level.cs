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

        public int DoubleHalfAbsorption { get; set; }

        public int SpeedUpSlowDown { get; set; }

        public int Time { get; set; }

        public int ChangeFood { get; set; }

        public float Multiplier { get; set; }

        public TimerType TimeType { get; set; }

        public int? MaxGrade { get; set; }

        public Dictionary<NutritionElementsEnum, float> Objectives = new Dictionary<NutritionElementsEnum, float>();

        public Level Clone()
        {
            return new Level()
            {
                Id = Id,
                Name = Name,
                CaloriesObjective = CaloriesObjective,
                MaxFat = MaxFat,
                MaxSaturates = MaxSaturates,
                MaxSalt = MaxSalt,
                MaxSugar = MaxSugar,
                RewardsList = RewardsList,
                Unlocked = Unlocked,
                FoodExpires = FoodExpires,
                DoubleHalfAbsorption = DoubleHalfAbsorption,
                SpeedUpSlowDown = SpeedUpSlowDown,
                Time = Time,
                ChangeFood = ChangeFood,
                Multiplier = Multiplier,
                TimeType = TimeType,
                MaxGrade = MaxGrade,

            };
        }

    }
}
