using Microsoft.Win32.SafeHandles;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Utils;

public class FoodBubble : MonoBehaviour
{
    bool chosen;
    public bool disappear;
    bool show;
    Vector3 initialScale;
    ParticleSystem drops;
    public GameObject VisualFunnel;
    Material originalMaterial;
    UnityEngine.Color originalColor;
    public Food Food;
    SpriteRenderer FoodImage;
    TextMeshPro ExpireText;
    public int expiresIn;
    bool increaseFont;
    bool decreaseFont;
    public Vector3 initialPosition { get; private set; }
    bool gobackToOriginal;
    Vector3 step;
    public bool OnPlate;

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;
        initialScale = transform.localScale;
        originalMaterial = this.GetComponent<MeshRenderer>().material;
        originalColor = this.GetComponent<MeshRenderer>().material.color;
        FoodImage = GetComponentInChildren<SpriteRenderer>();
        ExpireText = GetComponentInChildren<TextMeshPro>();
        SetupParticles();
    }

    public void ReduceExpiration()
    {
        if (Food != null)
        {
            if (expiresIn > 1)
            {
                expiresIn--;

                if(expiresIn == 1)
                {
                    ExpireText.color = UnityEngine.Color.red;
                }

                if(expiresIn == 2)
                {
                    ExpireText.color = UnityEngine.Color.yellow;
                }

                ExpireText.text = expiresIn.ToString();
            }
        }
    }

    public void GoBackToOriginalPosition()
    {
        gobackToOriginal = true;
        step = (this.transform.position - initialPosition) / 5;
    }

    private void SetupParticles()
    {
        drops = gameObject.transform.GetChild(1).gameObject.GetComponent<ParticleSystem>();

        if (drops != null)
        {
            var main = drops.main;
            main.duration = 5;
            main.prewarm = true;
            main.startLifetime = 2;
            main.startSpeed = 0.25f;
            main.startSize = 0.3f;
            main.simulationSpeed = 3;

            var shape = drops.shape;
            shape.radius = 1f;
        }
    }

    public void SetFood(Food food, bool showExpiry = true)
    {
        Food = food;

        if (!string.IsNullOrEmpty(food.FileName))
        {
            var image = Resources.Load<Texture2D>(food.FileName);
            FoodImage.sprite = Sprite.Create(image, new Rect(0,0, image.width, image.height), new Vector2(0.5f,0.5f));
        }
        else
        {
            FoodImage.sprite = null;
        }
        ExpireText.fontSize = 5;
        expiresIn = food.ExpiresIn;
        ExpireText.color = UnityEngine.Color.white;

        if (showExpiry)
        {
            ExpireText.text = expiresIn.ToString();
        }
        else
        {
            ExpireText.text = string.Empty;
        }
    }

    private void FadeOut()
    {
        UnityEngine.Color color = this.GetComponent<MeshRenderer>().material.color;
        float fadeamount = color.a - (chosen ? 1f * Time.deltaTime : 3 * Time.deltaTime);
        color = new UnityEngine.Color(color.r, color.g, color.b, fadeamount);
        this.GetComponent<MeshRenderer>().material.color = color;

        if (color.a <= 0)
        {
            disappear = false;
            gameObject.SetActive(false);
        }
    }

    private void FadeIn()
    {
        float fadeamount = originalColor.a + (2 * Time.deltaTime);
        var newColor = new UnityEngine.Color(originalColor.r, originalColor.g, originalColor.b, fadeamount);
        this.GetComponent<MeshRenderer>().material.color = newColor;
        this.transform.localScale = initialScale;

        if (newColor.a >= originalMaterial.color.a)
        {
            show = false;

        }
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

        if (increaseFont)
        {
            if (ExpireText.fontSize < 8)
            {
                ExpireText.fontSize += 0.5f;
            }
            else
            {
                increaseFont = false;
                decreaseFont = true;
            }
        }

        if (decreaseFont)
        {
            if (ExpireText.fontSize > 5)
            {
                ExpireText.fontSize -= 0.5f;
            }
            else
            {
                decreaseFont = false;
            }
        }

    }

    public void Show(bool animateNumbers = false)
    {
        chosen = false;
        gameObject.SetActive(true);
        show = true;
        increaseFont = animateNumbers;
    }

    public async void FoodChosen(Dictionary<NutritionElementsEnum, float> leftOnBars, bool createNutritionBalls = true)
    {
       drops.Emit(15);
       chosen = true;  
      
        await AsyncTask.Await(250);

        this.transform.position = initialPosition;

        if (createNutritionBalls)
        {
            VisualFunnel.GetComponent<Funnel>().CreateNutritionBubbles(initialPosition, Food, leftOnBars: leftOnBars);
        }

        Food = null;
    }

    public void FoodSpoiled(bool spawnNutritionBalls = true)
    {
        drops.Emit(15);
        chosen = true;
        if (spawnNutritionBalls)
        {
            VisualFunnel.GetComponent<Funnel>().CreateNutritionBubbles(this.transform.position, Food, isGhost: true);
        }
        Food = null;
    }
}
