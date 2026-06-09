using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flamesscript : MonoBehaviour
{
    float maxScale = 0.65f;
    float minScale = 0.55f;
    float currentScale = 0.65f;
    bool reachMaxed = false;
    bool reachMin = false;
    bool growOrShrink;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var grow = Random.Range(0, 2);
        growOrShrink = grow > 0 ? true : false;

        if (!reachMaxed)
        {
            if(growOrShrink)
            {
                var random = Random.Range(0.001f, 0.01f);

                if(currentScale + random < maxScale)
                {
                    currentScale += random;
                }
                else
                {
                    reachMaxed = true;
                    reachMin = false;
                }
            }
            else
            {
                var random = Random.Range(0.001f, 0.01f);

                if (currentScale - random >= minScale)
                {
                    currentScale -= random;
                }
                else
                {
                    reachMaxed = false;
                    reachMin = true;
                }
            }
        }
        else
        {
            if(!reachMin)
            {

                if (growOrShrink)
                {
                    var random = Random.Range(0.001f, 0.01f);

                    if (currentScale + random < maxScale)
                    {
                        currentScale += random;
                    }
                    else
                    {
                        reachMaxed = true;
                        reachMin = false;
                    }
                }
                else
                {
                    var random = Random.Range(0.001f, 0.01f);

                    if (currentScale - random >= minScale)
                    {
                        currentScale -= random;
                    }
                    else
                    {
                        reachMaxed = false;
                        reachMin = true;
                    }
                }
            }
        }

        this.transform.localScale = new Vector3(currentScale, currentScale, currentScale);
    }
}
