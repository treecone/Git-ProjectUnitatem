using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
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
    public Sprite[] sprites;

    [Header("Player Abilities")]
    public int currentWeapon;
    private GameObject playerArm;
    public GameObject hammerHitbox;

    public float[] abilityCooldowns;

    private bool[] abilityLocks;

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
        mainControls.Player.Ability1.started += EquipWeapon0;
        mainControls.Player.Ability2.started += EquipWeapon1;
        mainControls.Player.Ability3.started += EquipWeapon2;

        abilityLocks = new bool[3];
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
        mainControls.Player.Ability1.started -= EquipWeapon0;
        mainControls.Player.Ability2.started -= EquipWeapon1;
        mainControls.Player.Ability3.started -= EquipWeapon2;
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

    public void EquipWeapon0(InputAction.CallbackContext context) { currentWeapon = 0; WeaponSwitch1.Post(BGMManager); }
    public void EquipWeapon1(InputAction.CallbackContext context) { currentWeapon = 1; WeaponSwitch2.Post(BGMManager); }
    public void EquipWeapon2(InputAction.CallbackContext context) { currentWeapon = 2; WeaponSwitch3.Post(BGMManager); }

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

        #region Debugging
        if (Input.GetKeyDown(KeyCode.P))
        {
            TakeDamage();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            score++;
            highScoreEvent.Post(BGMManager);
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            setRegularScore.Post(BGMManager);
        }
        #endregion

        //Sprite transition
        Vector3 v = mainControls.Player.ActionRotation.ReadValue<Vector2>();
        if(Camera.main.ScreenToWorldPoint(new Vector3(v.x, v.y, 0)).y > gameObject.transform.position.y)
        {
            gameObject.transform.Find("PlayerSprite").GetComponent<SpriteRenderer>().sprite = sprites[1];
        }
        else
        {
            gameObject.transform.Find("PlayerSprite").GetComponent<SpriteRenderer>().sprite = sprites[0];
        }
    }

    IEnumerator AbilityCooldown(int abilityID, float timeToWait)
    {
        Debug.Log("Cooldown locked for " + abilityID);
        abilityLocks[abilityID] = true;
        yield return new WaitForSeconds(timeToWait);
        abilityLocks[abilityID] = false;
        Debug.Log("Cooldown unlocked for " + abilityID);
    }

    private void UseAction_started(InputAction.CallbackContext context)
    {
        if (abilityLocks[currentWeapon])
        {
            return;
        }

        playerArm.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
        playerArm.transform.rotation = Quaternion.Euler(0, 0, GetRotationForAbilities());
        float inputValue = context.ReadValue<float>();


        switch (currentWeapon)
        {
            case 0:
                if (inputValue > 0)
                {
                    playerArm.transform.GetChild(0).GetComponent<Animator>().SetBool("SwingBool", !playerArm.transform.GetChild(0).GetComponent<Animator>().GetBool("SwingBool"));
                }
                Transform HitboxSpawnPoint = playerArm.transform.Find("HitboxSpawnPoint").transform;
                GameObject hitBox = Instantiate(hammerHitbox, HitboxSpawnPoint.position, HitboxSpawnPoint.rotation);
                hitBox.GetComponent<HitboxScript>().pScript = this;
                hitBox.GetComponent<HitboxScript>().hitType = HITBOX_TYPE.SWORD;
                break;

            case 1:
                break;

            case 2:
                break;
        }

        StartCoroutine(AbilityCooldown(currentWeapon, abilityCooldowns[currentWeapon]));

    }

    public Vector2 VecToMouse ()
    {
        Vector2 v = mainControls.Player.ActionRotation.ReadValue<Vector2>(); //-1, 1
        return (Camera.main.ScreenToWorldPoint(new Vector3(v.x, v.y, 0)) - (Vector3)gameObject.transform.position).normalized;
    }

    public float GetRotationForAbilities()
    {

        Vector2 inputVector = VecToMouse();
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
