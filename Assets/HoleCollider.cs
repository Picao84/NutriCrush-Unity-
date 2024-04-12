using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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

    // Start is called before the first frame update
    void Start()
    {
        GaugeFill = GameObject.FindWithTag(element.ToString());
        SoundEffects = GameObject.FindWithTag("SoundEffects");
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject != null && other.gameObject.GetComponent<Sphere>() != null && !other.gameObject.GetComponent<Sphere>().wasConsumed && !other.gameObject.GetComponent<Sphere>().isPicked)
        {
            if (other.gameObject.GetComponent<Sphere>().element == this.element)
            {
                var consumed = GaugeFill.GetComponent<FillScript>().AddAmount(other.gameObject.GetComponent<Sphere>().elementQuantity);

                if (consumed)
                {
                    other.gameObject.GetComponent<Sphere>().ConsumeSphere(this.transform.position);
                }
                else
                {
                    SoundEffects.GetComponent<SoundEffects>().PlayWrong();
                    other.gameObject.GetComponent<Rigidbody>().velocity = (Vortex.transform.position - this.transform.position) * 5;
                }
            }
            else
            {
                SoundEffects.GetComponent<SoundEffects>().PlayWrong();
                GetComponent<SpriteRenderer>().sprite = DisableSprite;

                other.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, this.transform.position.y * 3, this.transform.position.z * 5);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject != null && other.gameObject.GetComponent<Sphere>() != null && !other.gameObject.GetComponent<Sphere>().wasConsumed && !other.gameObject.GetComponent<Sphere>().isPicked)
        {
            if (other.gameObject.GetComponent<Sphere>().element == this.element)
            {
                var consumed = GaugeFill.GetComponent<FillScript>().AddAmount(other.gameObject.GetComponent<Sphere>().elementQuantity);

                if (consumed)
                {
                    other.gameObject.GetComponent<Sphere>().ConsumeSphere(this.transform.position);
                }
                else
                {
                    other.gameObject.GetComponent<Sphere>().canBeAbsorbed = false;
                    SoundEffects.GetComponent<SoundEffects>().PlayWrong();
                    other.gameObject.GetComponent<Rigidbody>().velocity = (Vortex.transform.position - this.transform.position) * 5;
                }
            }
            else
            {
                SoundEffects.GetComponent<SoundEffects>().PlayWrong();
                GetComponent<SpriteRenderer>().sprite = DisableSprite;
               
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
        GetComponent<SpriteRenderer>().sprite = DisableSprite;
    }

    public void Open()
    {
        childText.SetActive(false);
        GetComponent<SpriteRenderer>().sprite = EnableSprite;
    }
}
