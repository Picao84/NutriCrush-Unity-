using Assets;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelDeckScript : MonoBehaviour
{
    // Start is called before the first frame update

    public VisualTreeAsset levelTemplate;
    Button cancel;
    public VisualTreeAsset sectionTemplate;

    public GameObject LevelDetail;

    public GameObject sceneLogic;
    SceneLogic3D sceneLogic3D;

    void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();

        var levelsArea = uiDocument.rootVisualElement.Q<VisualElement>("Levels");
        cancel = uiDocument.rootVisualElement.Q<Button>("cancel");
        cancel.clicked += Cancel_clicked;

        uiDocument.rootVisualElement.Q<ScrollView>().verticalScrollerVisibility = ScrollerVisibility.Hidden;
        uiDocument.rootVisualElement.Q<ScrollView>().mouseWheelScrollSize = 1000f;


        cancel.RegisterCallback<MouseEnterEvent>((MouseOverEvent) =>
        {
            cancel.style.backgroundColor = new StyleColor(new Color32(235, 235, 235, 255));

        });

        cancel.RegisterCallback<MouseLeaveEvent>((MouseOverEvent) =>
        {
            cancel.style.backgroundColor = new StyleColor(Color.white);

        });

        sceneLogic3D = sceneLogic.GetComponent<SceneLogic3D>();

        var column = 0;
        var section = 0;
        VisualElement row = null;

        foreach (Level level in Constants.Levels)
        {
            if(column == 3)
            {
                column = 0;
            }

            if(column == 0)
            {
                var newSection = sectionTemplate.Instantiate();
              
                
                newSection.Q<Label>("sectionName").text = Constants.SectionNames[section];

                if (Constants.PlayerData.SectionsUnlocked.Contains(section) && (Constants.FoodRequiredPerSection[section].Count == 0 || Constants.FoodRequiredPerSection[section].All(x =>  Constants.PlayerData.PlayerFood.Any(z => z.FoodId == x))))
                {
                    newSection.Q<VisualElement>("unlockRequirements").style.display = DisplayStyle.None;
                    newSection.Q<Label>("sectionName").style.width = new StyleLength(Length.Percent(100));
                    newSection.Q<Label>("sectionName").style.unityTextAlign = TextAnchor.MiddleCenter;
                }
                else
                {
                    var foodImages = newSection.Q<VisualElement>("foodImages");

                    foreach(var foodId in Constants.FoodRequiredPerSection[section])
                    {
                        var newFoodImage = new VisualElement();
                        newFoodImage.style.width = new StyleLength(30);
                        newFoodImage.style.height = new StyleLength(30);
                        newFoodImage.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>(Constants.FoodsDatabase[foodId].FileName));
                        foodImages.Add(newFoodImage);
                    }
                }

                levelsArea.Add(newSection);
                section++;

                row = new VisualElement();
                row.style.flexDirection = FlexDirection.Row;
                levelsArea.Add(row);
            }
            var levelBlock = levelTemplate.Instantiate();
            //levelBlock.Q<VisualElement>("root").style.width = new StyleLength(Screen.width * 0.155f);
            var levelItemController = new LevelItemController();

            levelBlock.userData = levelItemController;
            levelItemController.SetVisualElements(levelBlock, sceneLogic3D, this.gameObject, LevelDetail);
            row.Add(levelBlock);
            levelBlock.style.width = new StyleLength(Length.Percent(33.3f));
            levelItemController.SetLevelData(level);

            column++;

        }
    }

    private void Cancel_clicked()
    {
        sceneLogic3D.BackToMenu();
    }
}
