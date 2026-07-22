using Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkipShuffle : MonoBehaviour
{
    private const int COOLDOWN_MAX = 5;

    private int COOLDOWN_TIME = 60;

    int coolDown = COOLDOWN_MAX;
    bool canSkip = true;
    SpriteRenderer buttonRenderer;
    TextMeshPro cooldownText;
    Coroutine Timer;
    TimeSpan TimeLeft;
    Image coolDownImage;
    bool animate;
    float minAlpha = 0.2f;
    bool wentoToMin = false;


    // Start is called before the first frame update
    void Start()
    {
        buttonRenderer = GetComponent<SpriteRenderer>();
        cooldownText = GetComponentInChildren<TextMeshPro>();

        if (canSkip)
        {
            cooldownText.alpha = 0.0f;
        }

        TimeLeft = TimeSpan.FromSeconds(COOLDOWN_TIME);
        coolDownImage = GetComponentInChildren<Image>();

        coolDownImage.fillAmount = 0;
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

    public bool CanSKip { get { return canSkip;} private set { canSkip = value; } }

    public void Deactivate()
    {
        canSkip = false;
      
        //var image = Resources.Load<Texture2D>("skipShuffleUnpressed");
        //buttonRenderer.sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), new Vector2(0.5f, 0.5f));
     
        coolDown = COOLDOWN_MAX;
        //cooldownText.text = coolDown.ToString();
        //cooldownText.alpha = 1.0f;

    }

    public void Skipped()
    {
        canSkip = false;
        coolDownImage.fillAmount = 1;

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

    public void Reset(bool animate = false)
    {
        canSkip = true;
        cooldownText.alpha = 0.0f;
        //var image = Resources.Load<Texture2D>("skipShuffleUnpressed");
        //buttonRenderer.sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), new Vector2(0.5f, 0.5f));
        TimeLeft = TimeSpan.FromSeconds(COOLDOWN_TIME);
        coolDownImage.fillAmount = 0;
        this.animate = animate;
    }

    public void ReduceCooldown()
    {
        if (!canSkip)
        {
            coolDown--;
            cooldownText.text = coolDown.ToString();

            if (coolDown <= 0)
            {
                canSkip = true;
                var image = Resources.Load<Texture2D>("skipShuffleUnpressed");
                buttonRenderer.sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), new Vector2(0.5f, 0.5f));
                cooldownText.alpha = 0.0f;
            }
        }
    }


}
