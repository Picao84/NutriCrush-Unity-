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


    void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        var levelsArea = uiDocument.rootVisualElement.Q<VisualElement>("Levels");

        foreach(Level level in Constants.Levels)
        {
            var levelBlock = levelTemplate.Instantiate();
            var levelItemController = new LevelItemController();

            levelBlock.userData = levelItemController;
            levelItemController.SetVisualElements(levelBlock);
            levelsArea.Add(levelBlock);
            levelItemController.SetLevelData(level);
    
        }
    }
}
