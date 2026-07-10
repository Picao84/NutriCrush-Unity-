using Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
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
    List<Level> levelBackUp = new List<Level>();

    public GameObject LevelDetail;
    public int lastLevel = 1;

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

                    foreach (var foodToUnlock in Constants.Sections[section].FoodToUnlock)
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
                row.style.alignSelf = Align.Stretch;
                row.style.marginLeft = new StyleLength(new Length(3, LengthUnit.Percent));
                //row.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
                //row.style.paddingLeft = new StyleLength(new Length(15, LengthUnit.Pixel));
                //row.style.paddingBottom = new StyleLength(new Length(5, LengthUnit.Pixel));

                newSectionBox.Q<VisualElement>("box").Add(row);
                uiDocument.rootVisualElement.Q<ScrollView>().Add(newSectionBox);
                //levelsArea.Add(newSectionBox);
            }
            var levelBlock = levelTemplate.Instantiate();
            //levelBlock.Q<VisualElement>("root").style.width = new StyleLength(Screen.width * 0.155f);
            var levelItemController = new LevelItemController();

            levelBlock.userData = levelItemController;
            levelItemController.SetVisualElements(levelBlock, sceneLogic3D, this.gameObject, LevelDetail, sectionUnlocked);
            row.Add(levelBlock);

            levelBlock.style.width = new StyleLength(Length.Percent(33f));

            levelItemController.SetLevelData(level, levelBackUp.Count > 0 && levelBackUp.First(x => x.Id == level.Id).Unlocked == false);

            column++;

        }

        levelBackUp.Clear();
        foreach (Level level in Constants.Levels)
        {
            levelBackUp.Add(level.Clone());
        }

        uiDocument.rootVisualElement.Q<ScrollView>().RegisterCallback<GeometryChangedEvent>(FirstLayoutCallback);
    }

    private void FirstLayoutCallback(GeometryChangedEvent evt)
    {
        if (sceneLogic3D.GetComponent<SceneLogic3D>().CurrentLevel?.Id > 0)
        {
            var scroll = evt.currentTarget as ScrollView;
            var elementId = (lastLevel / 3) + 1;
            var element = scroll.Children().ElementAt(elementId);

            float yPos = element.layout.position.y;
            float height = element.layout.height / 2;
            float position = yPos - height;

            StartCoroutine(SmoothScrollToElement(scroll, position));
        }
    }

    private IEnumerator SmoothScrollToElement(ScrollView scrollView, float endPosition)
    {
        float smoothTime = 1f;
        float elapsedTime = 0f;
        float startPosition = scrollView.verticalScroller.value;

        while (elapsedTime < smoothTime)
        {
            float t = elapsedTime / smoothTime;

            float smoothPosition = Mathf.SmoothStep(startPosition, endPosition, t);
            scrollView.verticalScroller.value = smoothPosition;

            yield return null;

            elapsedTime += Time.deltaTime;
        }
        scrollView.verticalScroller.value = endPosition;
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
