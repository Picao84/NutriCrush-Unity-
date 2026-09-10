using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets
{
    public class TraceScript : MonoBehaviour
    {
        SpriteRenderer circle;
        bool animate;
        bool finishedAppearing;
        bool disappear;

        private void Start()
        {
            
            
        }

        private void Update()
        {
            if(animate)
            {
                if(!finishedAppearing && circle.color.a < 1f)
                {
                    circle.color = new Color(circle.color.r, circle.color.g, circle.color.b, circle.color.a + 0.1f);
                   
                    if(circle.color.a >= 1f)
                    {
                        finishedAppearing = true;
                    }
                }
                else
                {
                    if (!disappear)
                    {
                        animate = false;
                        StartCoroutine(CustomTimer.Timer(0.2f, () =>
                        {
                            animate = true;
                            disappear = true;
                        }));

                    }
                    else
                    {
                        if(circle.color.a > 0)
                        {
                            circle.color = new Color(circle.color.r, circle.color.g, circle.color.b, circle.color.a - 0.1f);
                        }
                        else
                        {
                            Destroy(transform.parent.gameObject);
                        }
                    }
                }
            }
        }

        private void OnEnable()
        {
            circle = GetComponent<SpriteRenderer>();
            circle.color = new Color(circle.color.r, circle.color.g, circle.color.b, 0f);
            animate = true;
        }

    }
}
