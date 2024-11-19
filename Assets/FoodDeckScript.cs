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
    public GameObject DeckSize;
    TextMeshProUGUI deckSizeText;

    private void OnEnable()
    {
        deckSizeText = DeckSize.GetComponent<TextMeshProUGUI>();

        var uiDocument = GetComponent<UIDocument>();

        foodListController = new FoodListController();
        foodListController.InitialiseFoodDeck(uiDocument.rootVisualElement, listItemTemplate, deckSizeText);
    }

    public void UpdateDeck()
    {
        foodListController.SaveNewDeck();
    }
}
