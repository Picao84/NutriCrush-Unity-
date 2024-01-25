using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bottom : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var worldSize = Camera.main.ScreenToWorldPoint(Camera.main.transform.right);
        this.transform.localScale = new Vector3(worldSize.x * 2, 0.5f,1);        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(collision.transform.parent.gameObject);
    }

}
