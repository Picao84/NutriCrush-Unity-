using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TutorialHandScript : MonoBehaviour
{
    Vector3 InitialPosition = Vector3.zero;
    Vector3 EndPosition = Vector3.zero;
    Vector3 step;
    bool animate;
    Rigidbody rigidBody;
    SpriteRenderer handSprite;
    public GameObject Traces;
    bool showTraces;
    Vector3 resetPosition;

    // Start is called before the first frame update
    void Start()
    {
        resetPosition = transform.position;
        rigidBody = GetComponent<Rigidbody>();
        handSprite = GetComponent<SpriteRenderer>();    
    }

    public void Reset()
    {
        transform.position = resetPosition;
    }

    public void SetPath(Vector3 initialPosition, Vector3 endPosition, bool showTraces = false)
    {
        InitialPosition = new Vector3 (initialPosition.x, initialPosition.y + 1.1f, initialPosition.z);
        EndPosition = new Vector3(endPosition.x, endPosition.y + 1.1f, endPosition.z);
        this.transform.position = InitialPosition;
        step = (InitialPosition - EndPosition) / 30;
        animate = true;
        this.showTraces = showTraces;
    }

    public void Stop()
    {
        animate = false;
        this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (animate)
        {
            if (rigidBody.transform.position == InitialPosition)
            {
                handSprite.color = new Color(1, 1, 1, 1f);
            }

            if (transform.position != EndPosition)
            {
                if (showTraces && transform.position != InitialPosition)
                {
                    Instantiate(Traces, transform.position, Quaternion.identity);
                }
                rigidBody.MovePosition(transform.position - step);
                
            }
            else
            {   if(handSprite.color.a > 0f)
                {
                    handSprite.color = new Color(1,1,1, handSprite.color.a - 0.2f);
                }
                else
                {
                    rigidBody.MovePosition(InitialPosition);
                }
            
               
            }
        }
    }
}
