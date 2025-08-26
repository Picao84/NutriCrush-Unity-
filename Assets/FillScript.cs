using Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using UnityEngine;
using Utils;

public class FillScript : MonoBehaviour
{
    public float MaxAmount = 100;
    public float currentAmount = 0;
    bool animate;
    float newRatio;
    float currentRatio = 0;
    public bool simulate;
    public Vector3 initialPosition;
    public Vector3 initialScale;
    public GameObject hole;
    public PoppingTextScript poppingText;
    List<GameObject> Arrows = new List<GameObject>();
    int effectDuration;
    float amountToApply;
    float TimePassed;
    bool waitArrows;
    int arrowIndex = 6;
    public GameObject Arrow1;
    public GameObject Arrow2;
    public GameObject Arrow3;

   

    public void TurnPassed()
    {
        effectDuration--;

        if (!simulate)
        {
            if (amountToApply > 1)
            {
                Arrows.First(x => x.activeSelf == true).SetActive(false);
            }
            else
            {
                Arrows[effectDuration].SetActive(false);
            }
        }

        if (effectDuration == 0)
        {

            if (amountToApply > 1)
            {
                Arrows.Reverse();
            }

            amountToApply = 0;
        }
    }

    public void SetEffect(float amount, int duration)
    {
        if (!simulate)
        {
            for (int i = 0; i < duration; i++)
            {
                Arrows[i].SetActive(true);
                var arrowColor = Arrows[i].GetComponent<SpriteRenderer>().color;
                Arrows[i].GetComponent<SpriteRenderer>().color = new Color(arrowColor.r, arrowColor.g, arrowColor.b, 1);

            }
        }

        if (amount > 1) 
        {
            if (!simulate)
            {
                foreach (GameObject arrow in Arrows)
                {
                    arrow.GetComponent<SpriteRenderer>().flipY = true;
                }
          
                Arrows.Reverse();
            }
        }
        else
        {
            if (!simulate)
            {

                foreach (GameObject arrow in Arrows)
                {
                    arrow.GetComponent<SpriteRenderer>().flipY = false;
                }
            }
        }

        amountToApply = amount;
        effectDuration = duration;

       

    }

    // Start is called before the first frame update
    void Start()
    {
        if (!simulate)
        {
            Arrows.Add(Arrow1);
            Arrows.Add(Arrow2);
            Arrows.Add(Arrow3);

            foreach (var ar in Arrows)
            {
                ar.gameObject.SetActive(false);
            }

            Arrows = Arrows.OrderBy(x => x.name).ToList();
        }
        
        initialPosition = transform.position;
        initialScale = transform.localScale;
        poppingText = transform.parent.gameObject.GetComponentInChildren<PoppingTextScript>();
    }

    void AnimateArrows()
    {
        var activeArrows = Arrows.Where(x => x.activeSelf == true).ToList();

        if (activeArrows.Count > 0)
        {

            if (activeArrows.Count > 1)
            {

                if (activeArrows.All(x => x.GetComponent<SpriteRenderer>().color.a == 1) && arrowIndex == 6)
                {
                    foreach (var arrow in activeArrows)
                    {
                        var arrowColor = arrow.GetComponent<SpriteRenderer>().color;
                        arrow.GetComponent<SpriteRenderer>().color = new Color(arrowColor.r, arrowColor.g, arrowColor.b, 0);
                    }

                    arrowIndex = 0;
                    return;
                }

                if (activeArrows.All(x => x.GetComponent<SpriteRenderer>().color.a == 0) && !waitArrows)
                {

                    waitArrows = true;
                    return;

                }

                waitArrows = false;

                arrowIndex++;

                if (activeArrows.Count > 2 && activeArrows[2].GetComponent<SpriteRenderer>().color.a == 0)
                {

                    var arrow = activeArrows[2];
                    var arrowColor = arrow.GetComponent<SpriteRenderer>().color;
                    arrow.GetComponent<SpriteRenderer>().color = new Color(arrowColor.r, arrowColor.g, arrowColor.b, 1);
                    return;

                }

                if (activeArrows.Count > 1 && activeArrows[1].GetComponent<SpriteRenderer>().color.a == 0 && arrowIndex == 2)
                {

                    var arrow = activeArrows[1];

                    var arrowColor = arrow.GetComponent<SpriteRenderer>().color;
                    arrow.GetComponent<SpriteRenderer>().color = new Color(arrowColor.r, arrowColor.g, arrowColor.b, 1);
                    return;

                }


                if (activeArrows.Count > 0 && activeArrows[0].GetComponent<SpriteRenderer>().color.a == 0 && arrowIndex == 4)
                {

                    var arrow = activeArrows[0];

                    var arrowColor = arrow.GetComponent<SpriteRenderer>().color;
                    arrow.GetComponent<SpriteRenderer>().color = new Color(arrowColor.r, arrowColor.g, arrowColor.b, 1);

                    return;

                }
            }
            else
            {
                if (activeArrows[0].GetComponent<SpriteRenderer>().color.a == 1 && arrowIndex == 6)
                {
                    var arrowColor = activeArrows[0].GetComponent<SpriteRenderer>().color;
                    activeArrows[0].GetComponent<SpriteRenderer>().color = new Color(arrowColor.r, arrowColor.g, arrowColor.b, 0);
                    arrowIndex = 0;
                    return;
                }

                if (activeArrows[0].GetComponent<SpriteRenderer>().color.a == 0 && !waitArrows)
                {

                    waitArrows = true;
                    return;

                }

                arrowIndex++;

                if (activeArrows[0].GetComponent<SpriteRenderer>().color.a == 0 && arrowIndex == 2)
                {
                    var arrowColor = activeArrows[0].GetComponent<SpriteRenderer>().color;
                    activeArrows[0].GetComponent<SpriteRenderer>().color = new Color(arrowColor.r, arrowColor.g, arrowColor.b, 1);
                }
            }
        }

    }

   

    // Update is called once per frame
    void Update()
    {
        if (!simulate)
        {
            if (animate && currentRatio < newRatio)
            {
                currentRatio = currentRatio + 0.01f;
                var beforeScaling = GetComponent<Renderer>().bounds.size.y;
                this.transform.localScale = new Vector3(this.transform.localScale.x, currentRatio, this.transform.localScale.z);
                var afterScaling = GetComponent<Renderer>().bounds.size.y;
                this.transform.Translate(new Vector3(0, (float)Math.Round(afterScaling - beforeScaling, 3), 0));
            }
        }

        TimePassed += Time.deltaTime;

        if (TimePassed > 0.1f)
        {
            TimePassed = 0;


            AnimateArrows();


        }

    }

    private void AnimatePoppingText(float newAmount)
    {
        var bounds = this.GetComponent<SpriteRenderer>().bounds;

        poppingText.Play($"+{Math.Round(newAmount, 1)}", bounds.size);
    }

    public bool AddAmount(float amount)
    {
        if(amountToApply != 0)
        {
            amount = amount * amountToApply;
            
        }

        if (currentAmount >= MaxAmount)
        {
            hole.GetComponent<HoleCollider>().Close("Full");
            return false;
        }

        if (currentAmount + amount <= MaxAmount)
        {
            currentAmount += amount;
        }
        else
        {
            hole.GetComponent<HoleCollider>().Close("Over");
            return false;
        }

        if (currentAmount / MaxAmount < 0.95)
        {
            newRatio = currentAmount / MaxAmount;
        }
        else
        {
            newRatio = 0.95f;
        }

        animate = true;
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

    public bool Simulate(float amount)
    {
        if (amountToApply != 0)
        {
            amount = amount * amountToApply;

        }

        if (currentAmount >= MaxAmount)
        {
            RotateParent();
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

            RotateParent();

            return false;
        }
          

        if(currentAmount / MaxAmount < 0.98)
        {
            currentRatio = currentAmount / MaxAmount;
        }
        else
        {
            currentRatio = 0.98f;
        }

        var beforeScaling = GetComponent<Renderer>().bounds.size.y;
        this.transform.localScale = new Vector3(this.transform.localScale.x, currentRatio, this.transform.localScale.z);
        var afterScaling = GetComponent<Renderer>().bounds.size.y;
        this.transform.Translate(new Vector3(0, (float)Math.Round(afterScaling - beforeScaling, 3), 0));

        return true;
    }

    public void Reset(bool resetamountToApply = true, bool firstReset = false)
    {
        waitArrows = false;
      
        if (amountToApply > 1)
        {
            Arrows.Reverse();
        }

        foreach (GameObject obj in Arrows)
        {
            obj.SetActive(false);
        }

        if (resetamountToApply)
        {
            amountToApply = 0;
            effectDuration = 0;
        }
      
        currentRatio = 0;
        newRatio = 0;
        hole.GetComponent<HoleCollider>().Open();
            currentAmount = 0;

        if (!firstReset)
        {
            this.transform.localScale = initialScale;
            this.transform.position = initialPosition;
        }
    }
}
