using Assets;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Utils;

public class HoleCollider : MonoBehaviour
{
    public NutritionElementsEnum element;
    public GameObject SceneLogic3D;
    GameObject GaugeFill;
    public Sprite EnableSprite;
    public Sprite DisableSprite;
    public GameObject childText;
    GameObject SoundEffects;
    public GameObject Vortex;
    GameObject pot;
    GameObject lid;
    public Vector3 lidOutPosition;
    public bool isFull;
    bool goToPot;
    Vector3 step;
    bool lidIsDown;
    bool animate;

    // Start is called before the first frame update
    void Start()
    {
        GaugeFill = GameObject.FindWithTag(element.ToString());
        SoundEffects = GameObject.FindWithTag("SoundEffects");
        pot = transform.parent.Find("pot")?.gameObject;
        lid = transform.parent.Find("lid")?.gameObject;
        lidOutPosition = lid.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (animate)
        {

            if (goToPot)
            {
                if (lid.transform.position != pot.transform.position)
                {
                    lid.GetComponent<Rigidbody>().MovePosition(lid.transform.position - step);
                }
                else
                {
                    goToPot = false;
                    lidIsDown = true;
                    animate = false;
                }
            }
        }
    }

    public void CloseLid()
    {
        if (!isFull)
        {
            animate = true;
            goToPot = true;
            step = (lidOutPosition - pot.transform.position) / 5;
        }

        //lid.GetComponent<Rigidbody>().MovePosition(pot.transform.position);
    }

    public void RemoveLid()
    {
        if (!isFull)
        {
            animate = false;   
            goToPot = false;
            lidIsDown = false;
            lid.GetComponent<Rigidbody>()?.MovePosition(lidOutPosition);         
        }
    }

    public void Reset(bool isCombo = false)
    {
        if (!isFull)
        {
            if (!isCombo)
            {
                lid.GetComponent<Rigidbody>()?.MovePosition(lidOutPosition);
                lidIsDown = false;
            }
        }
        else
        {
            lid.GetComponent<Rigidbody>()?.MovePosition(pot.transform.position);
            lidIsDown = true;
        }
    }

    private async void ShakePot()
    {
        pot.transform.Rotate(0, 0, 20);

        await AsyncTask.Await(50);

        pot.transform.Rotate(0, 0, -20);

        await AsyncTask.Await(50);

        pot.transform.Rotate(0, 0, 20);

        await AsyncTask.Await(50);

        pot.transform.Rotate(0, 0, -20);
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject != null && other.gameObject.GetComponent<Sphere>() != null && !other.gameObject.GetComponent<Sphere>().IsGhost && !other.gameObject.GetComponent<Sphere>().wasConsumed && !other.gameObject.GetComponent<Sphere>().isPicked)
        {
            if (other.gameObject.GetComponent<Sphere>().element == this.element)
            {
                var consumed = GaugeFill.GetComponent<FillScript>().AddAmount(other.gameObject.GetComponent<Sphere>().elementQuantity * SceneLogic3D.GetComponent<SceneLogic3D>().CurrentLevel.Multiplier);

                if (consumed)
                {
                    other.gameObject.GetComponent<Sphere>().ConsumeSphere(this.transform.position);
                }
                else
                {
                    //SoundEffects.GetComponent<SoundEffects>().PlayWrong();

                    var logic = SceneLogic3D.GetComponent<SceneLogic3D>();

                    /*if (logic.messagesShown.First(x => x.Id == (int)TutorialMessagesEnum.NutritionElementFull + 1).Showed == 0)
                    {
                        logic.Tutorial.GetComponent<TutorialScript>().ShowWithTextGroup(new List<string> { string.Format(Constants.TutorialMessages[TutorialMessagesEnum.NutritionElementFull][0], element.ToString()), Constants.TutorialMessages[TutorialMessagesEnum.NutritionElementFull][1] }, 3);

                        logic.messagesShown.First(x => x.Id == (int)TutorialMessagesEnum.NutritionElementFull + 1).Showed = 1;
                        logic.dataService.UpdateTutorialMessages(logic.messagesShown);

                        logic.pausedBalls = true;
                    }*/

                    other.gameObject.GetComponent<Rigidbody>().velocity = (Vortex.transform.position - this.transform.position) * 5;
                }
            }
            else
            {
                //SoundEffects.GetComponent<SoundEffects>().PlayWrong();
                //GetComponent<SpriteRenderer>().sprite = DisableSprite;

                other.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, this.transform.position.y * 3, this.transform.position.z * 5);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject != null && other.gameObject.GetComponent<Sphere>() != null && !other.gameObject.GetComponent<Sphere>().IsGhost &&!other.gameObject.GetComponent<Sphere>().wasConsumed && !other.gameObject.GetComponent<Sphere>().isPicked)
        {
            if (other.gameObject.GetComponent<Sphere>().element == this.element)
            {
                var consumed = GaugeFill.GetComponent<FillScript>().AddAmount(other.gameObject.GetComponent<Sphere>().elementQuantity * SceneLogic3D.GetComponent<SceneLogic3D>().CurrentLevel.Multiplier);

                if (consumed)
                {
                    other.gameObject.GetComponent<Sphere>().ConsumeSphere(this.transform.position);
                }
                else
                {
                    other.gameObject.GetComponent<Sphere>().cannotBeAbsorbed = true;
                    SoundEffects.GetComponent<SoundEffects>().PlayWrong();
                    other.gameObject.GetComponent<Rigidbody>().velocity = (Vortex.transform.position - this.transform.position) * 5;
                }
            }
            else
            {
                //SoundEffects.GetComponent<SoundEffects>().PlayWrong();
                ShakePot();
                //GetComponent<SpriteRenderer>().sprite = DisableSprite;
               
                other.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, this.transform.position.y * 3, this.transform.position.z * 5);
            }
        }
    }

    private async void OnTriggerExit(Collider other)
    {
        await AsyncTask.Await(500);
        Open();
    }

    public void Close(string text)
    {
        childText.SetActive(true);
        childText.GetComponent<TextMeshPro>().text = text;
        //GetComponent<SpriteRenderer>().sprite = DisableSprite;
    }

    public void Open()
    {
        childText.SetActive(false);
        //GetComponent<SpriteRenderer>().sprite = EnableSprite;
    }
}
