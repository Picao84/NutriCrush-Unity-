using Assets;
using Microsoft.Win32.SafeHandles;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using Utils;

public class FoodBubble : MonoBehaviour
{
    public bool chosen;
    public bool disappear;
    bool show;
    public Vector3 initialScale;
    public GameObject particles;
    ParticleSystem drops;
    public GameObject VisualFunnel;
    Material originalMaterial;
    UnityEngine.Color originalColor;
    public Food Food;
    SpriteRenderer FoodImage;
    SpriteRenderer BallImage;
    SpriteRenderer EffectImage;
    TextMeshPro ExpireText;
    public int expiresIn = 0;
    bool spinTurn;
    bool showSpinTurn;
    bool foodEffectsEnabled;

    SpriteRenderer turn;
    SpriteRenderer warning;
    public Vector3 initialPosition { get; private set; }
    bool gobackToOriginal;
    Vector3 step;
    public bool OnPlate;
    public Vector3 platePosition;


    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;
        initialScale = transform.localScale;
        expiresIn = 0;
      
        FoodImage = transform.GetChild(1).GetComponent<SpriteRenderer>();
        BallImage = transform.GetChild(0).GetComponent<SpriteRenderer>();
        EffectImage = transform.GetChild(3).GetComponent<SpriteRenderer>();

        ExpireText = GetComponentInChildren<TextMeshPro>();

        turn = ExpireText.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
        warning = ExpireText.gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>();

        turn.enabled = false;
        warning.enabled = false;

        MeshRenderer mr = GetComponent<MeshRenderer>();
        if (mr != null)
        {
            // Match the sorting layer of your sprites
            mr.sortingLayerName = "Foreground"; // Must exist in Edit > Project Settings > Tags and Layers
            mr.sortingOrder = 5; // Higher number = drawn later (on top)
        }

        drops = particles.GetComponent<ParticleSystem>();
    }

    public void EnableFoodEffects()
    {
        foodEffectsEnabled = true;
    }

    public void ReduceExpiration()
    {
        if (Food != null)
        {
            if (expiresIn > 1)
            {
                expiresIn--;

                /*if(expiresIn == 2)
                {
                    ExpireText.color = UnityEngine.Color.yellow;
                }*/

                spinTurn = true;
               
            }
        }
    }

    public void ResetScale()
    {
        transform.localScale = initialScale;
    }

    public void GoBackToOriginalPosition(bool animate = true)
    {
        OnPlate = false;

        if (animate)
        {
            gobackToOriginal = true;
            step = (this.transform.position - initialPosition) / 5;
        }
        else
        {
            transform.position = initialPosition;
        }

        transform.localScale = initialScale;
    }

    private void SetupParticles(NutritionElementsEnum element)
    {

        if (drops != null)
        {
            var particlesMain = drops.main;
            particlesMain.startColor = Constants.ParticleGradients[element];
        }
    }

    public void SetFood(Food food, Level level)
    {
        Food = food;

        if (!string.IsNullOrEmpty(food.FileName))
        {
            var image = Resources.Load<Texture2D>(food.FileName);

            if (image != null)
            {
                FoodImage.sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), new Vector2(0.5f, 0.5f));
            }
        }
        else
        {
            FoodImage.sprite = null;
        }

        if(food.Effect != null && food.Effect.IconName != null)
        {
            var image = Resources.Load<Texture2D>(food.Effect.IconName);

            if (image != null)
            {
                EffectImage.sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), new Vector2(0.5f, 0.5f));
            }
        }
        //ExpireText.fontSize = 5;
       
        //ExpireText.color = UnityEngine.Color.white;

        if (level.FoodExpires == 1)
        {
            expiresIn = food.ExpiresIn;
            ExpireText.text = expiresIn.ToString();
            turn.enabled = true;
            turn.transform.localRotation = Quaternion.identity;
        }
        else
        {
            expiresIn = 0;
            turn.enabled = false;
            ExpireText.text = string.Empty;
        }

        warning.enabled = false;
        GetHigherNutrient(food, level);

      
    }

    private void FadeOut()
    {
        UnityEngine.Color color = this.GetComponent<MeshRenderer>().material.color;
        float fadeamount = color.a - (chosen ? 2 * Time.deltaTime : 3 * Time.deltaTime);
        color = new UnityEngine.Color(color.r, color.g, color.b, fadeamount);
        this.GetComponent<MeshRenderer>().material.color = color;

        var particlesColor = drops.GetComponent<Renderer>().material.color;
        particlesColor = new UnityEngine.Color(particlesColor.r, particlesColor.g, particlesColor.b, fadeamount);
        drops.GetComponent<Renderer>().material.color = particlesColor;

        if (color.a <= 0)
        {
            disappear = false;
            gameObject.SetActive(false);
            this.transform.position = initialPosition;
        }
    }

    private void GetHigherNutrient(Food food, Level currentLevel)
    {
        Dictionary<NutritionElementsEnum, float> percentageOfNutritionElements = new Dictionary<NutritionElementsEnum, float>();

        foreach(var nutritionElement in food.NutritionElements)
        {
            percentageOfNutritionElements.Add(nutritionElement.Key, (nutritionElement.Value * currentLevel.Multiplier) / currentLevel.Objectives[nutritionElement.Key]);
        }

        var maxValue = percentageOfNutritionElements.Max(x => x.Value);
        var maxNutrient = percentageOfNutritionElements.First(x => x.Value >= maxValue).Key;

        var ballImage = Constants.FoodBallTextures[maxNutrient];

        BallImage.sprite = Sprite.Create(ballImage, new Rect(0, 0, ballImage.width, ballImage.height), new Vector2(0.5f, 0.5f));

        this.GetComponent<MeshRenderer>().material = Constants.FoodBubbleMaterials[maxNutrient];
        originalMaterial = Constants.FoodBubbleMaterials[maxNutrient];
        originalColor = originalMaterial.color;

        SetupParticles(maxNutrient);
    }

    private void FadeIn()
    {
        float fadeamount = originalColor.a + 2 * Time.deltaTime;
        var newColor = new UnityEngine.Color(originalColor.r, originalColor.g, originalColor.b, fadeamount);
        if (newColor.a >= originalColor.a)
        {
            show = false;

        }

        this.GetComponent<MeshRenderer>().material.color = newColor;
        this.transform.localScale = initialScale;
       
    }

    private void AnimateChosen()
    {
        if (chosen && transform.localScale.x < initialScale.x * 1.5)
        {
            this.transform.localScale = this.transform.localScale + initialScale * 0.1f;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(transform.position != initialPosition)
        {
            ExpireText.enabled = false;
            turn.enabled = false;
            warning.enabled = false;
            EffectImage.enabled = false;
        }
        else
        {
            if (foodEffectsEnabled)
            {
                EffectImage.enabled = true;
            }
            else
            {
                EffectImage.enabled = false;
            }

            if (expiresIn > 0)
            {
                ExpireText.enabled = true;
                turn.enabled = true;

                if (expiresIn < 2 && !spinTurn)
                {
                    warning.enabled = true;
                }
            }
        }

        if(gobackToOriginal)
        {
            if (transform.position != initialPosition)
            {
                GetComponent<Rigidbody>().MovePosition(transform.position - step);
            }
            else
            {
                gobackToOriginal = false;
              
            }
        }

        if (disappear || chosen)
        {
            FadeOut();
        }

        if (chosen)
        {
            AnimateChosen();
        }

        if (show)
        {
            FadeIn();
        }

        if (!show && showSpinTurn && spinTurn)
        {
            if (turn.transform.rotation.eulerAngles.y < 180)
            {
                turn.transform.Rotate(0, 0, -10);
            }
            else
            {
                turn.transform.localRotation = Quaternion.identity;
                ExpireText.text = expiresIn.ToString();
                spinTurn = false; 
                showSpinTurn = false;

                if (expiresIn < 2)
                {
                    warning.enabled = true;
                }
            }
        }
    }

    public void Show(bool showSpinTurn = false)
    {
        chosen = false;
        gameObject.SetActive(true);
        show = true;
        this.showSpinTurn = showSpinTurn;
    }

    public async void FoodChosen(Dictionary<NutritionElementsEnum, float> leftOnBars, bool createNutritionBalls = true)
    {
        turn.enabled = false;
        warning.enabled = false;
        particles.transform.position = this.gameObject.transform.position;

        drops.Emit(100);

        await AsyncTask.Await(100);

        chosen = true;
        OnPlate = false;
        platePosition = Vector3.zero;

        await AsyncTask.Await(100);

        if (createNutritionBalls)
        {
            VisualFunnel.GetComponent<Funnel>().CreateNutritionBubbles(initialPosition, Food, leftOnBars: leftOnBars);
        }

        Food = null;
    }

    public async void FoodSpoiled(bool spawnNutritionBalls = true)
    {
        turn.enabled = false;
        warning.enabled = false;

        particles.transform.position = this.gameObject.transform.position;

        drops.Emit(100);

        await AsyncTask.Await(100);

        chosen = true;
        if (spawnNutritionBalls)
        {
            VisualFunnel.GetComponent<Funnel>().CreateNutritionBubbles(this.transform.position, Food, isGhost: true);
        }
        Food = null;
    }
}
