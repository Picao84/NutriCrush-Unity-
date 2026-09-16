using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;
using Utils;

public class EffectDescScript : MonoBehaviour
{
    Button closeButton;
    VisualElement effectImage;
    Label effectName;
    Label effectDescription;
    FoodEffect foodEffect;
    int duration;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetEffect(FoodEffect foodEffect, int duration)
    {
        this.foodEffect = foodEffect;
        this.duration = duration;
    }

    private void OnEnable()
    {
        var rootElement = GetComponent<UIDocument>().rootVisualElement;
        closeButton = rootElement.Q<Button>("closeButton");
        effectImage = rootElement.Q<VisualElement>("effectImage");
        effectName = rootElement.Q<Label>("effectName");
        effectDescription = rootElement.Q<Label>("effectDescription");

        effectImage.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>(foodEffect.IconName));
        effectName.text = Constants.FoodEffectsNames[(FoodEffects) foodEffect.Id];
        effectDescription.text = string.Format(foodEffect.Description, duration);

        closeButton.clicked += CloseButton_clicked;
    }

    private async void CloseButton_clicked()
    {
        closeButton.clicked -= CloseButton_clicked;

        var image = Resources.Load<Texture2D>("exitRoundPressed");
        closeButton.style.backgroundImage = new StyleBackground(image);

        await AsyncTask.Await(100);

        image = Resources.Load<Texture2D>("exitRoundUnpressed");
        closeButton.style.backgroundImage = new StyleBackground(image);

        await AsyncTask.Await(100);

        gameObject.SetActive(false);
    }
}
