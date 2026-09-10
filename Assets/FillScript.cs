using Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using UnityEngine;
using Utils;
using UnityEngine.UI;

public class FillScript : MonoBehaviour
{
    public float MaxAmount = 100;
    public float currentAmount = 0;
   
    float newRatio;
    float currentRatio = 0;
    public bool simulate;
    public Vector3 initialPosition;
    public Vector3 initialScale;
    public GameObject hole;
    public PoppingTextScript poppingText;
   
    int effectDuration;
    public float amountToApply { get; private set; } = 1;

    Image Icon;
    Image Arrows;
    Image Cooldown;

    Coroutine Timer;
    Coroutine ArrowTimer;
    int TimeRan = 0;
    int numberOfArrows = 0;
    bool timerRunning;
    bool arrowRunning;

    Coroutine UseEnergyTimer;

    public void UseEnergy()
    {
        var onePercent = MaxAmount * 0.01f;

        UseEnergyTimer = StartCoroutine(CustomTimer.Timer(1, () =>
        {
            if (currentAmount > 0)
            {
                currentAmount -= onePercent;
                newRatio = currentAmount / MaxAmount;
            }

        }));
    }

    public void StopUsingEnergy()
    {
        if(UseEnergyTimer != null)
        {
            StopCoroutine(UseEnergyTimer);
        }
    }

    public void SetEffect(float amount, int duration)
    {
        amountToApply = amount;
        effectDuration = duration;

        TimeRan = 0;
        Cooldown.fillAmount = 0;

        if (!simulate) 
        { 
            Icon.enabled = true;
            Cooldown.enabled = true;
            Arrows.enabled = true;
        }

        if (!timerRunning)
        {
            timerRunning = true;
            Timer = StartCoroutine(CustomTimer.Timer(1, () =>
            {

                TimeRan++;

                if (!simulate)
                {
                    var coolDownRatio = (float)TimeRan / (float)effectDuration;
                    Cooldown.fillAmount = coolDownRatio;
                }

                if (TimeRan == effectDuration)
                {
                    if (!simulate)
                    {

                        Icon.enabled = false;
                        Cooldown.enabled = false;
                        Arrows.enabled = false;
                    }

                    timerRunning = false;
                    arrowRunning = false;
                    TimeRan = 0;
                    amountToApply = 1;
                    effectDuration = 0;

                    StopCoroutine(Timer);

                    if (!simulate)
                    {
                        StopCoroutine(ArrowTimer);
                    }
                }

            }));

        }

        if (!simulate)
        {
            if (!arrowRunning)
            {
                arrowRunning = true;

                ArrowTimer = StartCoroutine(CustomTimer.Timer(0.2f, () =>
                {

                switch (numberOfArrows)
                {
                    case 0:
                        var image0 = Resources.Load<Texture2D>("noarrows");

                        Arrows.sprite = Sprite.Create(image0, new Rect(0, 0, image0.width, image0.height), new Vector2(0.5f, 0.5f)); ;

                        numberOfArrows++;
                        break;

                    case 1:

                        var image1 = Resources.Load<Texture2D>("onearrow");

                        Arrows.sprite = Sprite.Create(image1, new Rect(0, 0, image1.width, image1.height), new Vector2(0.5f, 0.5f));

                        numberOfArrows++;
                        break;

                    case 2:

                        var image2 = Resources.Load<Texture2D>("downarrows");

                        Arrows.sprite = Sprite.Create(image2, new Rect(0, 0, image2.width, image2.height), new Vector2(0.5f, 0.5f));

                        numberOfArrows = 0;
                        break;
                }

            }));
            }

        }

    }

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;
        initialScale = transform.localScale;
        poppingText = transform.parent.gameObject.GetComponentInChildren<PoppingTextScript>();

        var images = transform.parent.GetComponentsInChildren<Image>();
        Icon = images[0];
        Cooldown = images[1];
        Arrows = images[2];

        Icon.enabled = false;
        Cooldown.enabled = false;
        Cooldown.fillAmount = 0;
        Arrows.enabled = false;
    }


   public void PauseEffect()
    {
        if(Timer != null)
        {
            StopCoroutine(Timer);
        }

        if(ArrowTimer != null)
        {
            StopCoroutine(ArrowTimer);
        }

    }

    public void ResumeEffect()
    {
        if(timerRunning)
        {
            Timer = StartCoroutine(CustomTimer.Timer(1, () =>
            {

                TimeRan++;

                if (!simulate)
                {
                    var coolDownRatio = (float)TimeRan / (float)effectDuration;
                    Cooldown.fillAmount = coolDownRatio;
                }

                if (TimeRan == effectDuration)
                {
                    if (!simulate)
                    {

                        Icon.enabled = false;
                        Cooldown.enabled = false;
                        Arrows.enabled = false;
                    }

                    timerRunning = false;
                    arrowRunning = false;
                    TimeRan = 0;
                    amountToApply = 1;
                    effectDuration = 0;

                    StopCoroutine(Timer);

                    if (!simulate)
                    {
                        StopCoroutine(ArrowTimer);
                    }
                }

            }));
        }

        if (arrowRunning)
        {
            ArrowTimer = StartCoroutine(CustomTimer.Timer(0.2f, () =>
            {

                switch (numberOfArrows)
                {
                    case 0:
                        var image0 = Resources.Load<Texture2D>("noarrows");

                        Arrows.sprite = Sprite.Create(image0, new Rect(0, 0, image0.width, image0.height), new Vector2(0.5f, 0.5f)); ;

                        numberOfArrows++;
                        break;

                    case 1:

                        var image1 = Resources.Load<Texture2D>("onearrow");

                        Arrows.sprite = Sprite.Create(image1, new Rect(0, 0, image1.width, image1.height), new Vector2(0.5f, 0.5f));

                        numberOfArrows++;
                        break;

                    case 2:

                        var image2 = Resources.Load<Texture2D>("downarrows");

                        Arrows.sprite = Sprite.Create(image2, new Rect(0, 0, image2.width, image2.height), new Vector2(0.5f, 0.5f));

                        numberOfArrows = 0;
                        break;
                }

            }));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!simulate)
        {
            if (Math.Round(currentRatio, 2) < Math.Round(newRatio, 2))
            {
                currentRatio = currentRatio + 0.01f;
                var beforeScaling = GetComponent<Renderer>().bounds.size.y;
                this.transform.localScale = new Vector3(this.transform.localScale.x, currentRatio, this.transform.localScale.z);
                var afterScaling = GetComponent<Renderer>().bounds.size.y;
                this.transform.Translate(new Vector3(0, (float)Math.Round(afterScaling - beforeScaling, 3), 0));
            }
            else if(Math.Round(currentRatio, 2) > Math.Round(newRatio, 2))
            {
                currentRatio = currentRatio - 0.01f;
                var beforeScaling = GetComponent<Renderer>().bounds.size.y;
                this.transform.localScale = new Vector3(this.transform.localScale.x, currentRatio, this.transform.localScale.z);
                var afterScaling = GetComponent<Renderer>().bounds.size.y;
                this.transform.Translate(new Vector3(0, (float)Math.Round(afterScaling - beforeScaling, 3), 0));
            }
        }
    }

    private void AnimatePoppingText(float newAmount)
    {
        var bounds = this.GetComponent<SpriteRenderer>().bounds;

        poppingText.Play($"+{Math.Round(newAmount, 1)}", bounds.size);
    }

    public bool AddAmount(float amount)
    {
       
        amount = amount * amountToApply;

        if (currentAmount >= MaxAmount)
        {
            hole.GetComponentInChildren<HoleCollider>().isFull = true;
            hole.GetComponentInChildren<HoleCollider>().CloseLid();
            //hole.GetComponent<HoleCollider>().Close("Full");
            return false;
        }

        if (currentAmount + amount <= MaxAmount)
        {
            currentAmount += amount;
        }
        else
        {
            //hole.GetComponent<HoleCollider>().Close("Over");
            return false;
        }

        if (currentAmount / MaxAmount < 1f)
        {
            newRatio = currentAmount / MaxAmount;
        }
        else
        {
            hole.GetComponentInChildren<HoleCollider>().isFull = true;
            hole.GetComponentInChildren<HoleCollider>().CloseLid();
            newRatio = 1f;
        }

      
        AnimatePoppingText(amount);
        return true;
    }

    private async void RotateParent()
    {
        GameObject parent = this.transform.parent.gameObject;
        parent.transform.Rotate(0,0,10);

        await AsyncTask.Await(50);

        parent.transform.Rotate(0,0,-10);

        await AsyncTask.Await(50);

        parent.transform.Rotate(0,0,10);

        await AsyncTask.Await(50);

        parent.transform.Rotate(0,0,-10);
    }

    public bool SimulateSingle(float currentAmount, float amount)
    {
       
        this.currentAmount = currentAmount;
        amount = amount * amountToApply;

        if (currentAmount >= MaxAmount)
        {
            //RotateParent();
            hole.GetComponentInChildren<HoleCollider>().CloseLid();
            return false;
        }

        if (currentAmount + amount <= MaxAmount)
        {
            currentAmount += amount;
        }
        else
        {
            //hole.GetComponent<HoleCollider>().Close("Over");
            hole.GetComponentInChildren<HoleCollider>().CloseLid();
            //RotateParent();

            return false;
        }
          

        if(currentAmount / MaxAmount < 1f)
        {
            currentRatio = currentAmount / MaxAmount;
        }
        else
        {
            currentRatio = 1f;
        }

        var beforeScaling = GetComponent<Renderer>().bounds.size.y;
        this.transform.localScale = new Vector3(this.transform.localScale.x, currentRatio, this.transform.localScale.z);
        var afterScaling = GetComponent<Renderer>().bounds.size.y;
        this.transform.Translate(new Vector3(0, (float)Math.Round(afterScaling - beforeScaling, 3), 0));

        return true;
    }

    public bool SimulateCombo(float amount)
    {
        
        amount = amount * amountToApply;
        if (currentAmount >= MaxAmount)
        {
            //RotateParent();
            hole.GetComponentInChildren<HoleCollider>().CloseLid();
            return false;
        }

        if (currentAmount + amount <= MaxAmount)
        {
            currentAmount += amount;
        }
        else
        {
            //hole.GetComponent<HoleCollider>().Close("Over");
            hole.GetComponentInChildren<HoleCollider>().CloseLid();
            //RotateParent();

            return false;
        }


        if (currentAmount / MaxAmount < 1f)
        {
            currentRatio = currentAmount / MaxAmount;
        }
        else
        {
            currentRatio = 1f;
        }

        var beforeScaling = GetComponent<Renderer>().bounds.size.y;
        this.transform.localScale = new Vector3(this.transform.localScale.x, currentRatio, this.transform.localScale.z);
        var afterScaling = GetComponent<Renderer>().bounds.size.y;
        this.transform.Translate(new Vector3(0, (float)Math.Round(afterScaling - beforeScaling, 3), 0));

        return true;
    }

    public void CloseLid()
    {
        hole.GetComponentInChildren<HoleCollider>().CloseLid();
    }

    public void RemoveLid()
    {
        hole.GetComponentInChildren<HoleCollider>().RemoveLid();
    }

    public void ResetPot()
    {
        hole.GetComponentInChildren<HoleCollider>().Reset();
    }

    public void Reset(bool resetamountToApply = true, bool firstReset = false, bool foodWasChosen = false, bool fullReset = false, bool isCombo = false)
    {
        if (!simulate)
        {
            Icon.enabled = false;
            Cooldown.enabled = false;
            Arrows.enabled = false;
            Cooldown.fillAmount = 0;
            if(Timer != null)
            {
                StopCoroutine(Timer);
            }
            if(ArrowTimer != null)
            {
                StopCoroutine(ArrowTimer);
            }
        }

        if (resetamountToApply)
        {
            timerRunning = false;
            TimeRan = 0;
            amountToApply = 1;
            effectDuration = 0;
        }

        currentRatio = 0;
        newRatio = 0;

        if (!foodWasChosen)
        {
            hole.GetComponentInChildren<HoleCollider>().Reset(isCombo);
        }

        if (fullReset)
        {
            hole.GetComponentInChildren<HoleCollider>().isFull = false;
            hole.GetComponentInChildren<HoleCollider>().Reset(isCombo);
        }
        if (UseEnergyTimer != null)
        {
            StopCoroutine(UseEnergyTimer);
        }
        currentAmount = 0;

        if (!firstReset)
        {
            this.transform.localScale = initialScale;
            this.transform.position = initialPosition;
        }
    }
}
