using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkipShuffle : MonoBehaviour
{
    private const int COOLDOWN_MAX = 5;

    int coolDown = COOLDOWN_MAX;
    bool canSkip = true;
    SpriteRenderer buttonRenderer;
    TextMeshPro cooldownText;

    // Start is called before the first frame update
    void Start()
    {
        buttonRenderer = GetComponent<SpriteRenderer>();
        cooldownText = GetComponentInChildren<TextMeshPro>();

        if (canSkip)
        {
            cooldownText.alpha = 0.0f;
        }
     
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool CanSKip { get { return canSkip;} private set { canSkip = value; } }

    public void Deactivate()
    {
        canSkip = false;
      
        var image = Resources.Load<Texture2D>("SkipShuffleOff");
        buttonRenderer.sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), new Vector2(0.5f, 0.5f));
     
        coolDown = COOLDOWN_MAX;
        cooldownText.text = coolDown.ToString();
        cooldownText.alpha = 1.0f;

    }

    public void Reset()
    {
        canSkip = true;
        cooldownText.alpha = 0.0f;
        var image = Resources.Load<Texture2D>("SkipShuffle");
        buttonRenderer.sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), new Vector2(0.5f, 0.5f));
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
                var image = Resources.Load<Texture2D>("SkipShuffle");
                buttonRenderer.sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), new Vector2(0.5f, 0.5f));
                cooldownText.alpha = 0.0f;
            }
        }
    }


}
