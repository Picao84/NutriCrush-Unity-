using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FunnelBottom : MonoBehaviour
{
    Gradient greenGradient;
    float currentLevel = 0;
    ParticleSystem WasteParticleSystem;
    public GameObject SceneLogic3D;

    // Start is called before the first frame update
    void Start()
    {
        greenGradient = new Gradient();
        var colors = new GradientColorKey[2];
        colors[0] = new GradientColorKey(Color.black, 0.0f);
        colors[1] = new GradientColorKey(new Color(135/255f,171/255f,8/255f), 0.5f);

        var alphas = new GradientAlphaKey[1];
        alphas[0] = new GradientAlphaKey(1.0f, 1.0f);

        greenGradient.SetKeys(colors,alphas);

        WasteParticleSystem = transform.parent.GetChild(1).transform.GetChild(0).GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.transform.gameObject.GetComponent<Sphere>() != null)
        {
            //WasteParticleSystem.Emit(15);
            //currentLevel = currentLevel + 0.2f;
            //var funnelBottomVisual = transform.parent.GetChild(1).gameObject;
            //funnelBottomVisual.transform.position = new Vector3(funnelBottomVisual.transform.position.x, funnelBottomVisual.transform.position.y + 0.2f, funnelBottomVisual.transform.position.z);
            //funnelBottomVisual.GetComponent<SpriteRenderer>().color = greenGradient.Evaluate(currentLevel);
            //funnelBottomVisual.GetComponent<SpriteRenderer>().color = transform.parent.GetChild(1).GetComponent<SpriteRenderer>().color.WithAlpha(0.5f);
            SceneLogic3D.GetComponent<SceneLogic3D>().RemoveSphere(other.transform.gameObject.GetComponent<Sphere>());
            Destroy(other.transform.root.gameObject);
        }
    }

}
