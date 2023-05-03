using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.Windows;
using Input = UnityEngine.Input;

public class PlayerScript : MonoBehaviour
{
    public GameObject bullet;


    [Header("Input")]
    private MainControls mainControls;

    [Header("Player Attributes")]
    public float movementSpeed;
    public int currentHealth;
    public int score;

    private Rigidbody2D rb;
    private GameObject mainCanvas;
    private GameObject BGMManager;

    //Abilites
    private int currentWeapon;
    private GameObject playerArm;

    //Audio ---------------------
    public AK.Wwise.Event WeaponSwitch1;
    public AK.Wwise.Event WeaponSwitch2;
    public AK.Wwise.Event WeaponSwitch3;
    public AK.Wwise.RTPC healthRTPC;
    public AK.Wwise.Event highScoreEvent;
    public AK.Wwise.Event playerDamage;
    public AK.Wwise.Event deathEvent;
    public AK.Wwise.Event setRegularScore;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCanvas = GameObject.Find("MainCanvas");
        BGMManager = GameObject.Find("BGM Manager");
        playerArm = gameObject.transform.Find("PlayerArmRoot").gameObject;

        //Input
        mainControls.Player.UseAction.started += UseAction_started;
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
        mainControls.Player.UseAction.started -= UseAction_started;
    }

    public void TakeDamage()
    {
        currentHealth -= 1;
        if(currentHealth <= 0)
        {
            Death();
        }
        healthRTPC.SetGlobalValue(currentHealth);
        playerDamage.Post(BGMManager);
    }

    #region Death

    //Call wise event to look for end of beat
    private void Death()
    {
        deathEvent.Post(BGMManager);
    }

    //Actual death function, called from wise when beat ends
    public void WiseDeath()
    {
        GameObject.Find("Main Camera").GetComponent<MainCameraScript>().cameraZoom = 4;
        mainCanvas.transform.Find("DeathPanel").gameObject.SetActive(true);
    }

    #endregion

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject b = Instantiate(bullet);
            b.transform.position = gameObject.transform.position;
            b.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 400);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            TakeDamage();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            score++;
            highScoreEvent.Post(BGMManager);
        }

        /*       float actionWheelInput = mainControls.Player.ActionWheel.ReadValue<float>();
               if (actionWheelInput > 0.1f)
               {
                   mainCanvas.transform.Find("ActionWheel").gameObject.SetActive(true);
               }
               else
               {
                   mainCanvas.transform.Find("ActionWheel").gameObject.SetActive(false);
               }*/

        if (Input.GetKeyDown(KeyCode.F1))
        {
            currentWeapon = 1;
            WeaponSwitch1.Post(BGMManager);
            Debug.Log("Switched to weapon: 1");
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            currentWeapon = 2;
            WeaponSwitch2.Post(BGMManager);
            Debug.Log("Switched to weapon: 2");

        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            currentWeapon = 3;
            WeaponSwitch3.Post(BGMManager);
            Debug.Log("Switched to weapon: 3");
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            setRegularScore.Post(BGMManager);
        }

        //Abilites
        if (playerArm.transform.GetChild(0).GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !playerArm.transform.GetChild(0).GetComponent<Animator>().IsInTransition(0))
            playerArm.transform.rotation = Quaternion.Lerp(playerArm.transform.rotation, Quaternion.Euler(0, 0, GetRotationForAbilities()), 5 * Time.deltaTime);

    }

    private void UseAction_started(InputAction.CallbackContext context)
    {
        playerArm.transform.rotation = Quaternion.Lerp(playerArm.transform.rotation, Quaternion.Euler(0, 0, GetRotationForAbilities()), 0.5f);
        float jumpValue = context.ReadValue<float>();
        if (jumpValue > 0)
        {
            playerArm.transform.GetChild(0).GetComponent<Animator>().SetBool("SwingBool", !playerArm.transform.GetChild(0).GetComponent<Animator>().GetBool("SwingBool"));
        }
    }

    public float GetRotationForAbilities()
    {
        Vector2 v = mainControls.Player.ActionRotation.ReadValue<Vector2>(); //-1, 1
        Vector2 inputVector = Camera.main.ScreenToWorldPoint(new Vector3(v.x, v.y, 0)) - (Vector3)gameObject.transform.position;
        //Vector2 dir = (gameObject.transform.) - gameObject.transform.position;
        return Mathf.Atan2(inputVector.y, inputVector.x) * Mathf.Rad2Deg - 90;
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
