using Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkipShuffle : MonoBehaviour
{
    private int COOLDOWN_TIME = 30;

    bool canSkip = true;
    SpriteRenderer buttonRenderer;

    Coroutine Timer;
    TimeSpan TimeLeft;
    Image coolDownImage;
    bool animate;
    float minAlpha = 0.2f;
    bool wentoToMin = false;
    public bool isVisible = false;
    TextMeshPro text;
    SpriteRenderer textLine;


    // Start is called before the first frame update
    void Start()
    {
        buttonRenderer = GetComponent<SpriteRenderer>();

        //TimeLeft = TimeSpan.FromSeconds(COOLDOWN_TIME);
        coolDownImage = GetComponentInChildren<Image>();

        coolDownImage.fillAmount = 0;

        text = GetComponentInChildren<TextMeshPro>();
        textLine = transform.GetChild(1).GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (animate)
        {
            if (buttonRenderer.color.a > minAlpha && !wentoToMin)
            {
                buttonRenderer.color = new Color(buttonRenderer.color.r, buttonRenderer.color.g, buttonRenderer.color.b, buttonRenderer.color.a - 0.1f);

                if(buttonRenderer.color.a <= minAlpha)
                {
                    wentoToMin=true;
                }
            }
            else
            {
                if(buttonRenderer.color.a < 1f)
                {
                    buttonRenderer.color = new Color(buttonRenderer.color.r, buttonRenderer.color.g, buttonRenderer.color.b, buttonRenderer.color.a + 0.1f);

                    if(buttonRenderer.color.a >= 1f)
                    {
                        animate = false;
                        wentoToMin = false;
                    }
                }
            }
        }
    }

    public void Appear()
    {
        isVisible = true;
        GetComponent<Renderer>().enabled = true;
        text.enabled = true;
        textLine.enabled = true;
        coolDownImage.enabled = true;
    }

    public void Disappear()
    {
        isVisible = false;
        GetComponent<Renderer>().enabled = false;
        text.enabled = false;
        textLine.enabled = false;
        coolDownImage.enabled = false;

    }

    public bool CanSKip { get { return canSkip;} private set { canSkip = value; } }

    public void Skipped()
    {
        canSkip = false;
        coolDownImage.fillAmount = 1;
        TimeLeft = TimeSpan.FromSeconds(COOLDOWN_TIME);

        Timer = StartCoroutine(CustomTimer.Timer(1, () => {

            TimeLeft = TimeLeft - TimeSpan.FromSeconds(1);

            var coolDownRatio = TimeLeft.TotalSeconds / COOLDOWN_TIME;
            coolDownImage.fillAmount = (float)coolDownRatio;

            if (TimeLeft.TotalSeconds == 0)
            {
                Reset(true);
                StopCoroutine(Timer);
            }

        }));

    }

    public void Pause()
    {
        if (Timer != null)
        {
            StopCoroutine(Timer);
        }
    }

    public void Resume()
    {
        if(TimeLeft.TotalSeconds > 0)
        {
            Timer = StartCoroutine(CustomTimer.Timer(1, () => {

                TimeLeft = TimeLeft - TimeSpan.FromSeconds(1);

                var coolDownRatio = TimeLeft.TotalSeconds / COOLDOWN_TIME;
                coolDownImage.fillAmount = (float)coolDownRatio;

                if (TimeLeft.TotalSeconds == 0)
                {
                    Reset(true);
                    StopCoroutine(Timer);
                }

            }));
        }
    }

    public void Reset(bool animate = false)
    {
        canSkip = true;
    
        //var image = Resources.Load<Texture2D>("skipShuffleUnpressed");
        //buttonRenderer.sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), new Vector2(0.5f, 0.5f));
       
        if (Timer != null)
        {
            StopCoroutine(Timer);
        }
        coolDownImage.fillAmount = 0;
        this.animate = animate;
    }

}
