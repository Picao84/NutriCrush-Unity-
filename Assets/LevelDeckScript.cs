using Assets;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Utils;

public class LevelDeckScript : MonoBehaviour
{
    // Start is called before the first frame update

    public VisualTreeAsset levelTemplate;
    Button cancel;
    public VisualTreeAsset sectionTemplate;
    public VisualTreeAsset sectionBoxTemplate;


    public GameObject LevelDetail;

    public GameObject sceneLogic;
    SceneLogic3D sceneLogic3D;

    void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();

        var levelsArea = uiDocument.rootVisualElement.Q<VisualElement>("Levels");
        levelsArea.style.marginLeft = 0;
        levelsArea.style.marginRight = 0;
        cancel = uiDocument.rootVisualElement.Q<Button>("cancel");
        cancel.clicked += Cancel_clicked; 

        uiDocument.rootVisualElement.Q<ScrollView>().verticalScrollerVisibility = ScrollerVisibility.Hidden;
        uiDocument.rootVisualElement.Q<ScrollView>().horizontalScrollerVisibility = ScrollerVisibility.Hidden;
        uiDocument.rootVisualElement.Q<ScrollView>().mouseWheelScrollSize = 1000f;

        sceneLogic3D = sceneLogic.GetComponent<SceneLogic3D>();

        var column = 0;
        var section = 0;
        VisualElement row = null;

        foreach (Level level in Constants.Levels)
        {
            bool sectionUnlocked = false;

            if (column == 3)
            {
                column = 0;
                section++;
            }

            if (section < 10)
            {
                sectionUnlocked = Constants.Sections[section].FoodToUnlock.All(x => Constants.PlayerData.PlayerFood.Any(z => z.FoodId == x.FoodId));
            }

            if (section == 0)
            {
                sectionUnlocked = true;
            }

            if (column == 0)
            {
                var newSectionBox = sectionBoxTemplate.Instantiate();

                var newSection = sectionTemplate.Instantiate();

                newSection.Q<Label>("sectionName").text = Constants.Sections[section].SectionName;

                if (sectionUnlocked)
                {
                    newSection.Q<VisualElement>("unlockRequirements").style.display = DisplayStyle.None;
                    newSection.Q<Label>("sectionName").style.width = new StyleLength(Length.Percent(100));
                    newSection.Q<Label>("sectionName").style.unityTextAlign = TextAnchor.MiddleCenter;
                }
                else
                {
                    var foodImages = newSection.Q<VisualElement>("foodImages");

                    foreach(var foodToUnlock in Constants.Sections[section].FoodToUnlock)
                    {
                        if (!Constants.PlayerData.PlayerFood.Any(x => x.FoodId == foodToUnlock.FoodId))
                        {
                            var newFoodImage = new VisualElement();
                            newFoodImage.style.width = new StyleLength(15);
                            newFoodImage.style.height = new StyleLength(15);
                            newFoodImage.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>(Constants.FoodsDatabase.First(x => x.Id == foodToUnlock.FoodId).FileName));
                            foodImages.Add(newFoodImage);
                        }
                    }
                }

                newSectionBox.Q<VisualElement>("box").Add(newSection);
                
        

                row = new VisualElement();
                row.style.flexDirection = FlexDirection.Row;
                row.style.width = new StyleLength(new Length(115, LengthUnit.Percent));
                row.style.paddingLeft = new StyleLength(new Length(15, LengthUnit.Pixel));
                //row.style.paddingBottom = new StyleLength(new Length(5, LengthUnit.Pixel));

                newSectionBox.Q<VisualElement>("box").Add(row);

                levelsArea.Add(newSectionBox);
            }
            var levelBlock = levelTemplate.Instantiate();
            //levelBlock.Q<VisualElement>("root").style.width = new StyleLength(Screen.width * 0.155f);
            var levelItemController = new LevelItemController();

            levelBlock.userData = levelItemController;
            levelItemController.SetVisualElements(levelBlock, sceneLogic3D, this.gameObject, LevelDetail, sectionUnlocked);
            row.Add(levelBlock);
            levelBlock.style.width = new StyleLength(Length.Percent(33.3f));
            levelItemController.SetLevelData(level);

            column++;


        }
    }

    private async void Cancel_clicked()
    {
        var image = Resources.Load<Texture2D>("roundBackButtonPressed");
        cancel.style.backgroundImage = new StyleBackground(image);

        await AsyncTask.Await(100);

        image = Resources.Load<Texture2D>("roundBackButtonUnpressed");
        cancel.style.backgroundImage = new StyleBackground(image);

        sceneLogic3D.BackToMenu();
    }
}
