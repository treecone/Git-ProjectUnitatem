using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
using Input = UnityEngine.Input;

public class PlayerScript : MonoBehaviour
{
    public GameObject bullet;

    [Header("Input")]
    private MainControls mainControls;

    [Header("Player Attributes")]
    public float movementSpeed;

    private Rigidbody2D rb;
    private GameObject mainCanvas;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCanvas = GameObject.Find("MainCanvas");
    }

    private void Awake()
    {
        mainControls = new MainControls();
    }

    private void OnEnable()
    {
        mainControls.Enable();
    }

    private void OnDisable()
    {
        mainControls.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject b = Instantiate(bullet);
            b.transform.position = gameObject.transform.position;
            b.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 400);
        }

        /*
        actionWheelInput = actionWheelRefrence.action.ReadValue<float>() > 0.9f;
        if(actionWheelInput)
        {
            mainCanvas.transform.Find("ActionWheel").gameObject.SetActive(true);
        }
        else
        {
            mainCanvas.transform.Find("ActionWheel").gameObject.SetActive(false);
        }
        */
    }

    public void FixedUpdate()
    {
        Movement();
    }

    void Movement()
    {
        Vector2 inputMovement = mainControls.Player.Movement.ReadValue<Vector2>();

        rb.velocity = inputMovement * Time.deltaTime * movementSpeed;   
    }
}
