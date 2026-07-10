using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using Utils;

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
        VisualElement lockImage;
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
            tile = visualElement[0][0];
            lockImage = visualElement[0][1];
            lockImage.pickingMode = PickingMode.Ignore;
            this.levelDeck = levelDeck;
            this.levelDetail = levelDetail;
            this.sectionUnlocked = sectionUnlocked;

            tile.AddManipulator(new Clickable(() => 
            {
                //sceneLogic.PlayLevel(level);
                //levelDeck.SetActive(false);
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

            tile.RegisterCallback<GeometryChangedEvent>((geometryChanged) => {

                if (geometryChanged.newRect.width > geometryChanged.newRect.height)
                {
                    visualElement[0].style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>("pauseBackground"));
                }
            

            });




        }

        public async void SetLevelData(Level level, bool wasLocked = false)
        {
            this.level = level;

            var fullstar = Resources.Load<Texture2D>("fullstar");
            var emptystar = Resources.Load<Texture2D>("emptystar");

            if (!Constants.Levels.First(x => x.Id == level.Id).Unlocked || !sectionUnlocked)
            {
                this.tile.SetEnabled(false);
                this.tile.style.opacity = 0.4f;
            }
            else
            {
                if (wasLocked)
                {
                    await AsyncTask.Await(1000);
                    RemoveLockAnimation();
                }
                else
                {
                    this.tile.SetEnabled(true);
                    this.tile.style.opacity = 1.0f;
                    this.lockImage.style.opacity = 0.0f;
                }
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

        private void RemoveLockAnimation()
        {

            this.tile.schedule.Execute(() => { lockImage.style.scale = new StyleScale(new Vector3(lockImage.style.scale.value.value.x - 0.1f, lockImage.style.scale.value.value.y - 0.1f, lockImage.style.scale.value.value.z - 0.1f)); })
                .Every(1)
                .Until(() =>
                {
                    if (lockImage.style.scale.value.value.x <= 0f)
                    {
                        RemoveTileOpacityAnimation();
                        return true;
                    }

                    return false;

                });
        }

        private void RemoveTileOpacityAnimation()
        {
            this.tile.schedule.Execute(() => { tile.style.opacity = new StyleFloat(tile.style.opacity.value + 0.05f); })
                       .Every(1)
                       .Until(() =>
                       {
                           if (tile.style.opacity.value == 1f)
                           {
                               this.tile.SetEnabled(true);
                               return true;
                           }

                           return false;

                       });
        }
    }
}

  
