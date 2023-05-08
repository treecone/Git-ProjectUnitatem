using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
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
    public float IFrameLength;
    public bool invincible;
    public float timeBetweenBlinks;
    private float timeCounter;
    public bool dead;

    private Rigidbody2D rb;
    private GameObject mainCanvas;
    private GameObject BGMManager;
    public Sprite[] sprites;
    private SpriteRenderer playerSprite;
    private ParticleSystem dirtParticle;

    [Header("Player Abilities")]
    public int currentWeapon;
    private GameObject playerArm;
    public GameObject hammerHitbox;
    public GameObject arrowPrefab;
    public Sprite[] swordHammerSprites; 

    public float[] abilityCooldowns;

    private bool[] abilityLocks;
    public Image[] abilityUI;

    //Audio ---------------------
    public AK.Wwise.Event WeaponSwitch1;
    public AK.Wwise.Event WeaponSwitch2;
    public AK.Wwise.Event WeaponSwitch3;
    public AK.Wwise.RTPC healthRTPC;
    public AK.Wwise.Event highScoreEvent;
    public AK.Wwise.Event playerDamage;
    public AK.Wwise.Event setRegularScore;
    public GameObject deathEventObj;
    

    // Start is called before the first frame update
    void Start()
    {
        dead = false;
        rb = GetComponent<Rigidbody2D>();
        mainCanvas = GameObject.Find("MainCanvas");
        BGMManager = GameObject.Find("BGM Manager");
        playerArm = gameObject.transform.Find("PlayerArmRoot").gameObject;
        playerSprite = transform.Find("PlayerSprite").gameObject.GetComponent<SpriteRenderer>();
        dirtParticle = gameObject.transform.Find("Dirt").GetComponent<ParticleSystem>();

        //Input
        mainControls.Player.UseAction.started += UseAction_started;
        mainControls.Player.Ability1.started += EquipWeapon0;
        mainControls.Player.Ability2.started += EquipWeapon1;
        mainControls.Player.Ability3.started += EquipWeapon2;

        abilityLocks = new bool[3];
        UpdateUIButtons(0);
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Bullet")
        {
            Debug.Log("Taking Damage");
            TakeDamage();
        }
    }

    public void TakeDamage()
    {
        if (invincible || dead)
            return; 

        currentHealth -= 1;
        if(currentHealth <= 0)
        {
            Death();
        }
        healthRTPC.SetGlobalValue(currentHealth);
        playerDamage.Post(BGMManager);
        StartCoroutine(PlayerIFrames(IFrameLength));
    }

    public void EquipWeapon0(InputAction.CallbackContext context) { currentWeapon = 0; WeaponSwitch1.Post(BGMManager); Debug.Log("Switched to 1"); playerArm.transform.GetChild(0).Find("BowSprite").gameObject.SetActive(false); UpdateUIButtons(currentWeapon); }
    public void EquipWeapon1(InputAction.CallbackContext context) { currentWeapon = 1; WeaponSwitch2.Post(BGMManager); Debug.Log("Switched to 2"); playerArm.transform.GetChild(0).Find("BowSprite").gameObject.SetActive(false); UpdateUIButtons(currentWeapon); }
    public void EquipWeapon2(InputAction.CallbackContext context) { currentWeapon = 2; WeaponSwitch3.Post(BGMManager); Debug.Log("Switched to 3"); playerArm.transform.GetChild(0).Find("BowSprite").gameObject.SetActive(true); UpdateUIButtons(currentWeapon); }

    #region Death

    //Call wise event to look for end of beat
    private void Death()
    {
       Instantiate(deathEventObj);
    }


    #endregion

    void UpdateUIButtons(int type)
    {
        foreach(Image i in abilityUI)
        {
            i.rectTransform.anchoredPosition = new Vector2(i.rectTransform.anchoredPosition.x, -7);
        }

        abilityUI[type].rectTransform.anchoredPosition = new Vector2(abilityUI[type].rectTransform.anchoredPosition.x, 10);
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

        if(currentWeapon == 2)
        {
            playerArm.transform.rotation = Quaternion.Lerp(playerArm.transform.rotation, Quaternion.Euler(0, 0, GetRotationForAbilities()+180), 20 * Time.deltaTime);
        }

        //Blinking for I frames
        if(invincible)
        {
            timeCounter += Time.deltaTime;
            if(timeCounter >= timeBetweenBlinks)
            {
                playerSprite.enabled = !playerSprite.enabled;
                timeCounter = 0;
            }
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
    }

    IEnumerator AbilityCooldown(int abilityID, float timeToWait)
    {
        Debug.Log("Cooldown locked for " + abilityID);
        abilityLocks[abilityID] = true;
        abilityUI[abilityID].color = Color.grey;
        yield return new WaitForSeconds(timeToWait);
        abilityLocks[abilityID] = false;
        abilityUI[abilityID].color = Color.white;
        Debug.Log("Cooldown unlocked for " + abilityID);
    }

    IEnumerator PlayerIFrames(float time)
    {
        invincible = true;
        yield return new WaitForSeconds(time);
        invincible = false;
        playerSprite.enabled = true;
    }

    private void UseAction_started(InputAction.CallbackContext context)
    {
        if (abilityLocks[currentWeapon])
        {
            return;
        }

        float inputValue = context.ReadValue<float>();
        Transform HitboxSpawnPoint = playerArm.transform.Find("HitboxSpawnPoint").transform;

        switch (currentWeapon)
        {
            case 0:
                playerArm.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite = swordHammerSprites[currentWeapon];
                playerArm.transform.rotation = Quaternion.Euler(0, 0, GetRotationForAbilities());
                playerArm.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
                playerArm.transform.GetChild(0).GetComponent<Animator>().SetBool("SwingBool", !playerArm.transform.GetChild(0).GetComponent<Animator>().GetBool("SwingBool"));
                GameObject hitBox = Instantiate(hammerHitbox, HitboxSpawnPoint.position, HitboxSpawnPoint.rotation);
                hitBox.GetComponent<HitboxScript>().pScript = this;
                hitBox.GetComponent<HitboxScript>().hitType = HITBOX_TYPE.SWORD;
                break;

            case 1:
                playerArm.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite = swordHammerSprites[currentWeapon];
                playerArm.transform.rotation = Quaternion.Euler(0, 0, GetRotationForAbilities());
                playerArm.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
                playerArm.transform.GetChild(0).GetComponent<Animator>().SetBool("SwingBool", !playerArm.transform.GetChild(0).GetComponent<Animator>().GetBool("SwingBool"));
                GameObject hitBox2 = Instantiate(hammerHitbox, HitboxSpawnPoint.position, HitboxSpawnPoint.rotation);
                hitBox2.GetComponent<HitboxScript>().pScript = this;
                hitBox2.GetComponent<HitboxScript>().hitType = HITBOX_TYPE.AXE;
                break;

            case 2:
                GameObject arrow = Instantiate(arrowPrefab, gameObject.transform.position, Quaternion.identity);
                Vector2 v = mainControls.Player.ActionRotation.ReadValue<Vector2>();
                arrow.GetComponent<ArrowScript>().teleportPlayer = true;
                arrow.GetComponent<ArrowScript>().endPoint = Camera.main.ScreenToWorldPoint(v);
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

        #region SpriteRotation
        if (rb.velocity.x < 0)
        {
            playerSprite.flipX = true;
        }
        else if (rb.velocity.x > 0)
        {
            playerSprite.flipX = false;
        }

        if(rb.velocity.y < 0)
        {
            playerSprite.sprite = sprites[0];
        }
        else if(rb.velocity.y > 0)
        {
            playerSprite.sprite = sprites[1];
        }
        #endregion
    }
}
