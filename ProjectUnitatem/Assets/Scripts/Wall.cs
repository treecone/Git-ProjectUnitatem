using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.name == "Player")
        {
            gameObject.GetComponent<Animation>().Play();
            Debug.Log("Wall");
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }

    }
}
