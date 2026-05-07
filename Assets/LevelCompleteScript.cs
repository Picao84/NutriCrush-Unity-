using Assets;
using Assets.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using UnityEngine;
using UnityEngine.UIElements;
using Utils;
using Label = UnityEngine.UIElements.Label;

public class LevelCompleteScript : MonoBehaviour
{
    VisualElement root;

    Label level;
    VisualElement star1;
    VisualElement star2;
    VisualElement star3;

    VisualElement reward1;
    VisualElement reward2;
    VisualElement reward3;
    public GameObject SceneLogic;

    Button retry;
    Button nextLevel;
    Button backToMainMenu;

    int levelId;
    GradesEnum grade;
    Dictionary<string, int> rewardsGranted;

    Dictionary<NutritionElementsEnum, int> percentageValues;
    Label timeLabel;
    int timeRatio;
    TimeSpan timeLeft;
    List<VisualElement> stars;
    List<VisualElement> rewards;

    VisualElement timeBar;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        level = root.Q<Label>("level");
        level.text = $"Level {levelId}";

        Dictionary<NutritionElementsEnum, Label> percentages = new Dictionary<NutritionElementsEnum, Label>
        {
            { NutritionElementsEnum.Fat, root.Q<Label>("fatPercentage") },
            { NutritionElementsEnum.Saturates, root.Q<Label>("saturatesPercentage") },
            { NutritionElementsEnum.Salt, root.Q<Label>("saltPercentage") },
            { NutritionElementsEnum.Sugar, root.Q<Label>("sugarPercentage") }
        };

        Dictionary<NutritionElementsEnum, VisualElement> percentageBars = new Dictionary<NutritionElementsEnum, VisualElement>
        {
            { NutritionElementsEnum.Fat, root.Q<VisualElement>("totalFatBar") },
            { NutritionElementsEnum.Saturates, root.Q<VisualElement>("totalSaturatesBar") },
            { NutritionElementsEnum.Salt, root.Q<VisualElement>("totalSaltBar") },
            { NutritionElementsEnum.Sugar, root.Q<VisualElement>("totalSugarBar") }
        };

        star1 = root.Q<VisualElement>("star1");
        star2 = root.Q<VisualElement>("star2");
        star3 = root.Q<VisualElement>("star3");

        stars = new List<VisualElement>
        {
            star3,
            star2,
            star1
        };

        reward1 = root.Q<VisualElement>("reward1");
        reward2 = root.Q<VisualElement>("reward2");
        reward3 = root.Q<VisualElement>("reward3");

        rewards = new List<VisualElement>()
        {
            reward3, reward2, reward1
        };

        for (int i = 3; i > 0; i--)
        {
            stars[i - 1].style.backgroundImage = null;
            stars[i - 1].style.scale = new Vector2(2,2);
        }

        foreach (var percentageValue in percentageValues)
        {
            percentages[percentageValue.Key].text = $"0%";
            percentageBars[percentageValue.Key].style.width = new Length(0, LengthUnit.Percent);
        }

        for (int i = 3; i > 0; i--)
        {
            rewards[i - 1].style.backgroundImage= null;
        }

        timeBar = root.Q<VisualElement>("timeBar");
        timeBar.style.width = new Length(0, LengthUnit.Percent);

        foreach (var percentageValue in percentageValues) 
        {
            AnimateBarAndNumbers(percentageBars[percentageValue.Key], percentages[percentageValue.Key], percentageValue.Value, percentageValue.Key == NutritionElementsEnum.Sugar);
            //await AsyncTask.Await(50);
            //percentageBars[percentageValue.Key].style.width = new Length(percentageValue.Value > 99 ? 99 : percentageValue.Value, LengthUnit.Percent);
        }



        timeLabel = root.Q<Label>("timeLabel");
        timeLabel.text = string.Format("{0:00}:{1:00}:{2:00}", timeLeft.Minutes, timeLeft.Seconds, timeLeft.Milliseconds);



        retry = root.Q<Button>("retry");
        retry.clicked += Retry_clicked;

        nextLevel = root.Q<Button>("nextLevel");
        nextLevel.clicked += NextLevel_clicked;

        backToMainMenu = root.Q<Button>("backToMainMenu");
        backToMainMenu.clicked += BackToMainMenu_clicked;


        if (levelId % 3 == 0)
        {
            var section = levelId / 3;
            var sectionUnlocked = Constants.Sections[section].FoodToUnlock.All(x => Constants.PlayerData.PlayerFood.Any(z => z.FoodId == x.FoodId));

            if (!sectionUnlocked)
            {
                nextLevel.SetEnabled(false);

            }
            else
            {
                nextLevel.SetEnabled(true);

            }
        }

     

      
        /*timeBar.style.width = new Length(timeRatio > 99 ? 99 : timeRatio, LengthUnit.Percent);
        timeBar.style.marginLeft = new Length(timeRatio > 99 ? 0 : 100 - timeRatio, LengthUnit.Percent);*/

    }

    private void BackToMainMenu_clicked()
    {
        SceneLogic.GetComponent<SceneLogic3D>().BackToMenu();
    }

    private void Retry_clicked()
    {
        SceneLogic.GetComponent<SceneLogic3D>().Reset();
    }

    private void NextLevel_clicked()
    {
        SceneLogic.GetComponent<SceneLogic3D>().PlayNextLevel();
    }

    public void SetFinishedLevelData(int levelId, GradesEnum grade, Dictionary<string, int> rewards, Dictionary<NutritionElementsEnum, int> percentageValues, int timeRatio, TimeSpan timeLeft)
    {
        this.levelId = levelId;
        this.grade = grade;
        this.rewardsGranted = rewards;
        this.percentageValues = percentageValues;
        this.timeRatio = timeRatio;
        this.timeLeft = timeLeft;

    }

    private void AnimateTimeBar()
    {
        int percentageValue = 0;

        timeBar.schedule.Execute(() =>
        {
            percentageValue++;

            if (percentageValue < timeRatio)
            {
                timeBar.style.width = new Length(percentageValue, LengthUnit.Percent);
                timeBar.style.marginLeft = new Length(100 - percentageValue, LengthUnit.Percent);
            }

        }).Every(8).Until(() =>
        {

            if (percentageValue >= timeRatio)
            {
                ShowStarsAndRewards();
                return true;
            }

            return false;
        });

    }

    private void AnimateBarAndNumbers(VisualElement bar, Label label, int target, bool isLast = false)
    {
        int percentageValue = 0;
        int percentageValueText = 0;

        bar.schedule.Execute(() =>
        {
            if (percentageValue < target)
            {
                percentageValue++;
                bar.style.width = new Length(percentageValue, LengthUnit.Percent);
            }

        }).Every(8).Until(() =>
        {

            if (percentageValue >= target)
            {
                if (isLast)
                {
                    AnimateTimeBar();
                }

                return true;
            }
        
            return false;
        });

        label.schedule.Execute(() =>
        {
            if (percentageValueText < target)
            {
                percentageValueText++;
                label.text = $"{percentageValue.ToString()}%";
            }

        }).Every(8).Until(() =>
        {

            if (percentageValueText >= target)
            {
              
                return true;
            }

            return false;
        });

    }

    private void AnimateStar(VisualElement star, VisualElement reward, int order)
    {
        star.schedule.Execute(() => 
        {
            var currentScale = star.style.scale.value.value;
            star.style.scale = new Vector2(currentScale.x - 0.1f, currentScale.y - 0.1f);

        }).Every(8).Until(() => { 
        
            if(star.style.scale.value.value.x <= 1)
            {
                if (rewardsGranted.Count >= order)
                {
                    reward.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>(rewardsGranted.ToList()[order - 1].Key));
                    reward.style.display = DisplayStyle.Flex;
                }
                else
                {
                    reward.style.display = DisplayStyle.None;
                }

                return true;
            }

            return false;

        });
    }

    private async void ShowStarsAndRewards()
    {
        var fullstar = Resources.Load<Texture2D>("fullstar");
        var emptystar = Resources.Load<Texture2D>("emptystar");

        for (int i = 3; i > 0; i--)
        {
            stars[i - 1].style.display = DisplayStyle.Flex;

            if ((int)grade <= i)
            {
                stars[i - 1].style.backgroundImage = new StyleBackground(fullstar);
            }
            else
            {
                stars[i - 1].style.backgroundImage = new StyleBackground(emptystar);
            }

            AnimateStar(stars[i-1], rewards[i - 1], i);

            /*if (rewardsGranted.Count >= i)
            {
                rewards[i - 1].style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>(rewardsGranted.ToList()[i - 1].Key));
                rewards[i - 1].style.display = DisplayStyle.Flex;
            }
            else
            {
                rewards[i - 1].style.display = DisplayStyle.None;
            }*/

            await AsyncTask.Await(150);
        }
    }


}
