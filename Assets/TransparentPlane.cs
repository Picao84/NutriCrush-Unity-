using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparentPlane : MonoBehaviour
{
    bool disappear;
    public bool show;
    Material originalMaterial;
    UnityEngine.Color originalColor;
    // Start is called before the first frame update
    void Start()
    {
        originalMaterial = this.GetComponent<Renderer>().material;
        originalColor = this.GetComponent<Renderer>().material.color;
    }

    public void Hide()
    {
        disappear = true;
    }

    public void Show()
    {
        show = true;
    }

    private void FadeOut()
    {
        UnityEngine.Color color = this.GetComponent<Renderer>().material.color;
        float fadeamount = color.a - (2 * Time.deltaTime);
        color = new UnityEngine.Color(color.r, color.g, color.b, fadeamount);
        this.GetComponent<Renderer>().material.color = color;

        if (color.a <= 0)
        {
            disappear = false;
            //gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (disappear)
        {
           FadeOut();
        }

        if(show)
        {
            FadeIn();
        }
    }

    private void FadeIn()
    {
        float fadeamount = originalColor.a + (2 * Time.deltaTime);
        var newColor = new UnityEngine.Color(originalColor.r, originalColor.g, originalColor.b, fadeamount);
 
        this.GetComponent<Renderer>().material.color = newColor;

        if (newColor.a >= originalMaterial.color.a)
        {
            show = false;
        }
    }
}
