using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Funnel : MonoBehaviour
{
    public int Speed = 100;
    bool rotating = true;
    Vector3 m_EulerAngleVelocity;
    Rigidbody m_Rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        m_EulerAngleVelocity = new Vector3(0, 0, 100);
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    private void FixedUpdate()
    {
         if (rotating)
        {
            //Quaternion deltaRotation = Quaternion.Euler(m_EulerAngleVelocity * Time.fixedDeltaTime);
            //m_Rigidbody.MoveRotation(m_Rigidbody.rotation * deltaRotation);
            transform.Rotate(0, Speed, 0);
        }
    }

    public void PauseRotation()
    {
        rotating = false;
    }

    public void ResumeRotation()
    {
        rotating = true;
    }

}
