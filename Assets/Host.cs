using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Host : MonoBehaviour
{
    Vector3 InitialPosition;
    Vector3 ScreenPosition = new Vector3(6.194935f, 0.3377f, -3.027f);
    bool isShowing;
    bool isHiding;
    GameObject SpeechBalloon;
    GameObject Text;
    bool isShowingBalloon;
    bool isHidingBalloon;
    bool isShowingText;
    bool isHidingText;
    SpriteRenderer balloonSprite;
    TextMeshPro text;

    // Start is called before the first frame update
    void Start()
    {
        InitialPosition = transform.position;
        SpeechBalloon = transform.GetChild(0).transform.GetChild(0).gameObject;
        Text = SpeechBalloon.transform.GetChild(0).gameObject;
        balloonSprite = SpeechBalloon.GetComponent<SpriteRenderer>();
        text = Text.GetComponent<TextMeshPro>();
        text.color = new Color32(0, 0, 0, 0);
        balloonSprite.color = new Color(1f, 1f, 1f, 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (isShowing)
        {
            if (transform.position.x >= ScreenPosition.x)
            {
                transform.position = new Vector3(transform.position.x - 10 * Time.deltaTime, transform.position.y, transform.position.z);
            }
            else
            {
                isShowingBalloon = true;
                isShowingText = true;
                transform.position = ScreenPosition;
                isShowing = false;
            }
        }

        if (isShowingBalloon)
        {

            if (balloonSprite.color.a < 1f)
            {
                balloonSprite.color = new Color(1f, 1f, 1f, balloonSprite.color.a + 0.1f);
            }
            else
            {
                isShowingBalloon = false;
            }
        }

        if (isHidingBalloon)
        {

            if (balloonSprite.color.a > 0f)
            {
                balloonSprite.color = new Color(1f, 1f, 1f, balloonSprite.color.a - 0.1f);
            }
            else
            {
                isHidingBalloon = false;
            }
        }

        if (isShowingText)
        {

            if (text.color.a < 1f)
            {
                text.color = new Color(0f, 0f, 0f, text.color.a + 0.1f);
            }
            else
            {
                isShowingText = false;
            }
        }

        if (isHidingText)
        {

            if (text.color.a > 0f)
            {
                text.color = new Color(0f, 0f, 0f, text.color.a - 0.1f);
            }
            else
            {
                isHidingText = false;
            }
        }


        if (isHiding)
        {

            if (transform.position.x <= InitialPosition.x)
            {
                transform.position = new Vector3(transform.position.x + 10 * Time.deltaTime, transform.position.y, transform.position.z);
            }
            else
            {
                isHiding = false;
            }
        }
    }

    public void Show()
    {
        isShowing = true;
    }

    public void Hide()
    {
        isHiding = true;
        isHidingBalloon = true;
        isHidingText = true;
    }
}
