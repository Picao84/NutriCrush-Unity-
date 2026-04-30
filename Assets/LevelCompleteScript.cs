using Assets;
using Assets.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.UIElements;
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

    Button retry;
    Button nextLevel;

    int levelId;
    GradesEnum grade;
    Dictionary<string, int> rewardsGranted;


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
        
        star1 = root.Q<VisualElement>("star1");
        star2 = root.Q<VisualElement>("star2");
        star3 = root.Q<VisualElement>("star3");

        List<VisualElement> stars = new List<VisualElement>
        {
            star3,
            star2,
            star1
        };

        var fullstar = Resources.Load<Texture2D>("fullstar");
        var emptystar = Resources.Load<Texture2D>("emptystar");

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

        reward1 = root.Q<VisualElement>("reward1");
        reward2 = root.Q<VisualElement>("reward2");
        reward3 = root.Q<VisualElement>("reward3");


        retry = root.Q<Button>("retry");
        nextLevel = root.Q<Button>("nextLevel");

        List<VisualElement> rewards = new List<VisualElement>()
        {
            reward1, reward2, reward3
        };

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

        for (int i = 0; i < 3; i++)
        {
            if (i < rewardsGranted.Count)
            {
                rewards[i].style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>(rewardsGranted.ToList()[i].Key));
                //rewards[i].Q<Label>("quantity").text = $"x{rewardsGranted.ToList()[i].Value}";

                rewards[i].style.display = DisplayStyle.Flex;
            }
            else
            {
                rewards[i].style.display = DisplayStyle.None;
            }
        }

    }

    public void SetFinishedLevelData(int levelId, GradesEnum grade, Dictionary<string, int> rewards)
    {
        this.levelId = levelId;
        this.grade = grade;
        this.rewardsGranted = rewards;
    }

}
