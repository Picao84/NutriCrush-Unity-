using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class RewardsScript : MonoBehaviour
{
    VisualElement reward1;
    VisualElement reward2;
    VisualElement reward3;
    List<VisualElement> rewards;


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
        var uiDocument = GetComponent<UIDocument>();
        var bottom = uiDocument.rootVisualElement.Q<VisualElement>("root").Q<VisualElement>("bottom");
        reward1 = bottom.Q<VisualElement>("reward1");
        reward2 = bottom.Q<VisualElement>("reward2");
        reward3 = bottom.Q<VisualElement>("reward3");

        rewards = new List<VisualElement>()
        {
            reward1, reward2, reward3
        };
    }

    public void SetRewards(Dictionary<string, int> rewardsGranted)
    {
        for (int i = 0; i < 3; i++)
        {
            if(i < rewardsGranted.Count)
            {
                rewards[i].Q<VisualElement>("foodImage").style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>(rewardsGranted.ToList()[i].Key));
                rewards[i].Q<Label>("quantity").text = $"x{rewardsGranted.ToList()[i].Value}";

                rewards[i].style.display = DisplayStyle.Flex;
            }
            else
            {
                rewards[i].style.display = DisplayStyle.None;
            }
        }

        
    }

}
