using Assets;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelDeckScript : MonoBehaviour
{
    // Start is called before the first frame update

    public VisualTreeAsset levelTemplate;
    Button cancel;

    public GameObject sceneLogic;
    SceneLogic3D sceneLogic3D;

    void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        var levelsArea = uiDocument.rootVisualElement.Q<VisualElement>("Levels");
        var cancel = uiDocument.rootVisualElement.Q<Button>("cancel");
        cancel.clicked += Cancel_clicked;

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
        VisualElement row = null;

        foreach (Level level in Constants.Levels)
        {
            if(column == 3)
            {
                column = 0;
            }

            if(column == 0)
            {
                row = new VisualElement();
                row.style.flexDirection = FlexDirection.Row;
                levelsArea.Add(row);
            }

            var levelBlock = levelTemplate.Instantiate();
            var levelItemController = new LevelItemController();

            levelBlock.userData = levelItemController;
            levelItemController.SetVisualElements(levelBlock, sceneLogic3D);
            row.Add(levelBlock);
            levelItemController.SetLevelData(level);

            column++;

        }
    }

    private void Cancel_clicked()
    {
        sceneLogic3D.BackToMenu();
    }
}
