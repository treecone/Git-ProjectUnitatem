using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using Input = UnityEngine.Input;

public class PlayerScript : MonoBehaviour
{
    public GameObject bullet;

    [Header("Player Attributes")]
    public float movementSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetKey(KeyCode.A))
        {
            gameObject.transform.Translate(Vector2.left * movementSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.D))
        {
            gameObject.transform.Translate(Vector2.right * movementSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.W))
        {
            gameObject.transform.Translate(Vector2.up * movementSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.S))
        {
            gameObject.transform.Translate(Vector2.down * movementSpeed * Time.deltaTime);
        }



        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject b = Instantiate(bullet);
            b.transform.position = gameObject.transform.position;
            b.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 400);
        }
    }
}
