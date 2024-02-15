using Microsoft.Win32.SafeHandles;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class FoodBubble : MonoBehaviour
{
    bool chosen;
    bool disappear;
    bool show;
    Vector3 initialScale;
    ParticleSystem drops;
    public GameObject VisualFunnel;
    Material originalMaterial;
    UnityEngine.Color originalColor;

    // Start is called before the first frame update
    void Start()
    {
        initialScale = transform.localScale;
        originalMaterial = this.GetComponent<Renderer>().material;
        originalColor = this.GetComponent<Renderer>().material.color;
        SetupParticles();
    }

    private void SetupParticles()
    {
        drops = gameObject.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>();

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
            shape.radius = 0.5f;
        }
    }

    private void FadeOut()
    {
        UnityEngine.Color color = this.GetComponent<Renderer>().material.color;
        float fadeamount = color.a - (chosen ? 3 * Time.deltaTime : 2 * Time.deltaTime);
        color = new UnityEngine.Color(color.r, color.g, color.b, fadeamount);
        this.GetComponent<Renderer>().material.color = color;

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
        this.GetComponent<Renderer>().material.color = newColor;
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
    void Update()
    {
        if (disappear || chosen)
        {
            FadeOut();
        }

        if(chosen)
        {
            AnimateChosen();
        }

        if(show)
        {
            FadeIn();
        }

    }

    public void Show()
    {
        chosen = false;
        gameObject.SetActive(true);
        show = true;      
    }

    public void FoodChosen()
    {
       drops.Emit(15);
       chosen = true;  
       var foodBubbles = GameObject.FindGameObjectsWithTag("FoodBubble");
       var otherFoodBubbles = foodBubbles.Where(x => x != this.gameObject).ToList();
       foreach(GameObject foodBubble in otherFoodBubbles)
       {
            foodBubble.GetComponent<FoodBubble>().disappear = true;
       }

        VisualFunnel.GetComponent<Funnel>().CreateNutritionBubbles(this.transform.position);       
    }
}
