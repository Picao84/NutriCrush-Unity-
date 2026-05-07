using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
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
        Label levelText;
        VisualElement tile;
        Level level;
        SceneLogic3D sceneLogic;
        GameObject levelDeck;
        GameObject levelDetail;
        bool sectionUnlocked;
        VisualElement star1;
        VisualElement star2;
        VisualElement star3;

        public void SetVisualElements(VisualElement visualElement, SceneLogic3D sceneLogic3D, GameObject levelDeck, GameObject levelDetail, bool sectionUnlocked)
        {
            sceneLogic = sceneLogic3D;
            tile = visualElement[0];
            this.levelDeck = levelDeck;
            this.levelDetail = levelDetail;
            this.sectionUnlocked = sectionUnlocked;

            tile.AddManipulator(new Clickable(() => 
            {
                //sceneLogic.PlayLevel(level);
                levelDeck.SetActive(false);
                levelDetail.SetActive(true);
                levelDetail.GetComponent<LevelDetailScript>().SetLevelAndDeckPanel(level, levelDeck, sceneLogic);
                

            }));
            tile.RegisterCallback<MouseEnterEvent>((MouseOverEvent) =>
            {
                if (tile.enabledSelf)
                {
                    tile.style.backgroundColor = new StyleColor(new Color32(235, 235, 136, 255));
                }
            });
            tile.RegisterCallback<MouseLeaveEvent>((MouseLeaveEvent) =>
            {
                if (tile.enabledSelf)
                {
                    tile.style.backgroundColor = new StyleColor(new Color32(237, 238, 193, 255));
                }
            });

            levelText = visualElement.Q<Label>("level");
            calories = visualElement.Q<Label>("calories");
            fatText = visualElement.Q<Label>("fatText");
            saturatesText = visualElement.Q<Label>("saturatesText");
            saltText = visualElement.Q<Label>("saltText");
            sugarText = visualElement.Q<Label>("sugarText");

            star1 = visualElement.Q<VisualElement>("star1");
            star2 = visualElement.Q<VisualElement>("star2");
            star3 = visualElement.Q<VisualElement>("star3");

      
        }

        public void SetLevelData(Level level)
        {
            this.level = level;

            var fullstar = Resources.Load<Texture2D>("fullstar");
            var emptystar = Resources.Load<Texture2D>("emptystar");

            if (!Constants.Levels.First(x => x.Id == level.Id).Unlocked || !sectionUnlocked)
            {
                this.tile.SetEnabled(false);
                this.tile.style.opacity = 0.5f;
            }
            else
            {
                this.tile.SetEnabled(true);
                this.tile.style.opacity = 1.0f;
            }


            levelText.text = level.Name;
            calories.text = level.CaloriesObjective.ToString();
            fatText.text = level.MaxFat.ToString();
            saturatesText.text = level.MaxSaturates.ToString();
            saltText.text = level.MaxSalt.ToString();
            sugarText.text = level.MaxSugar.ToString();

            List<VisualElement> stars = new List<VisualElement>
            {
                star3,
                star2,
                star1
            };

            var grade = level.MaxGrade;

            if (grade != null)
            {
                for (int i = 3; i > 0; i--)
                {
                    if ((int)grade <= i)
                    {
                        stars[i - 1].style.backgroundImage = new StyleBackground(fullstar);
                    }
                    else
                    {
                        stars[i - 1].style.backgroundImage = new StyleBackground(emptystar);
                    }
                }
            }
            else
            {
                for (int i = 3; i > 0; i--)
                {
                   stars[i - 1].style.backgroundImage = new StyleBackground(emptystar);                  
                }
            }
        }
    }
}
