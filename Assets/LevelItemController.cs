using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace Assets
{
    public class LevelItemController
    {
        Label calories;
        Label fatText;
        Label saturatesText;
        Label saltText;
        Label sugarText;



        public void SetVisualElements(VisualElement visualElement)
        {
            calories = visualElement.Q<Label>("calories");
            fatText = visualElement.Q<Label>("fatText");
            saturatesText = visualElement.Q<Label>("saturatesText");
            saltText = visualElement.Q<Label>("saltText");
            sugarText = visualElement.Q<Label>("sugarText");
        }

        public void SetLevelData(Level level)
        {
            calories.text = level.CaloriesObjective.ToString();
            fatText.text = level.MaxFat.ToString();
            saturatesText.text = level.MaxSaturates.ToString();
            saltText.text = level.MaxSalt.ToString();
            sugarText.text = level.MaxSugar.ToString();
        }
    }
}
