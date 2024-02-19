using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class HoleCollider : MonoBehaviour
{
    public NutritionElementsEnum element;
    public GameObject SceneLogic3D;
    GameObject GaugeFill;
    public Sprite EnableSprite;
    public Sprite DisableSprite;

    // Start is called before the first frame update
    void Start()
    {
        GaugeFill = GameObject.FindWithTag(element.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject != null && other.gameObject.GetComponent<Sphere>() != null && !other.gameObject.GetComponent<Sphere>().wasConsumed && !other.gameObject.GetComponent<Sphere>().isPicked)
        {
            if (other.gameObject.GetComponent<Sphere>().element == this.element)
            {
                GaugeFill.GetComponent<FillScript>().AddAmount(other.gameObject.GetComponent<Sphere>().elementQuantity);
                SceneLogic3D.GetComponent<SceneLogic3D>().RemoveSphere(other.gameObject.GetComponent<Sphere>());
                other.gameObject.GetComponent<Sphere>().ConsumeSphere(this.transform.position);
            }
            else
            {
                GetComponent<SpriteRenderer>().sprite = DisableSprite;
                other.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, this.transform.position.y * 3, this.transform.position.z * 5);
            }
        }
    }

    private async void OnTriggerExit(Collider other)
    {
        await Task.Delay(500);
        GetComponent<SpriteRenderer>().sprite = EnableSprite;
    }
}
