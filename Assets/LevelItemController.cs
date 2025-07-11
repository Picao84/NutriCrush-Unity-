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

        public void SetVisualElements(VisualElement visualElement, SceneLogic3D sceneLogic3D, GameObject levelDeck, GameObject levelDetail)
        {
            sceneLogic = sceneLogic3D;
            tile = visualElement[0];
            this.levelDeck = levelDeck;
            this.levelDetail = levelDetail;

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
        }

        public void SetLevelData(Level level)
        {
            this.level = level;
            if (!Constants.PlayerData.LevelsUnlocked.Contains(level.LevelID))
            {
                this.tile.SetEnabled(false);
                this.tile.style.opacity = 0.5f;
            }

            levelText.text = level.Name;
            calories.text = level.CaloriesObjective.ToString();
            fatText.text = level.MaxFat.ToString();
            saturatesText.text = level.MaxSaturates.ToString();
            saltText.text = level.MaxSalt.ToString();
            sugarText.text = level.MaxSugar.ToString();
        }
    }
}
