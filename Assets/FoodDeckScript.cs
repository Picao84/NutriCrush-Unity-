using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class FoodDeckScript : MonoBehaviour
{
    [SerializeField]
    VisualTreeAsset listItemTemplate;

    FoodListController foodListController;

    public GameObject SceneLogic3D;

    private void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();


        foodListController = new FoodListController();
        foodListController.InitialiseFoodDeck(uiDocument.rootVisualElement, listItemTemplate, SceneLogic3D.GetComponent<SceneLogic3D>());
    }

    public void UpdateDeck()
    {
        foodListController.SaveNewDeck();
    }
}
