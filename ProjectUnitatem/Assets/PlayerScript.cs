using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using Input = UnityEngine.Input;

public class PlayerScript : MonoBehaviour
{
    public GameObject bullet;
    int movementCounter;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*float a = Input.GetAxis("Horizontal");
        gameObject.transform.Translate(Vector2.right * a * 0.2f);*/

        if(Input.GetKeyDown(KeyCode.A))
        {
            if(movementCounter > 0)
            {
                movementCounter--;
                gameObject.transform.Translate(Vector2.left * 2);
            }
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            if (movementCounter < 5)
            {
                movementCounter++;
                gameObject.transform.Translate(Vector2.right * 2);
            }
        }



        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject b = Instantiate(bullet);
            b.transform.position = gameObject.transform.position;
            b.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 400);
        }
    }
}
