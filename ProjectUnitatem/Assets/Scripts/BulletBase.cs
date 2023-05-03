using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BULLET_TYPE
{
    Base = 0,
    Beam,
}

public class BulletBase : MonoBehaviour
{
    [Header("Bullet Attributes")]
    public float baseSpeed;
    public float damage;
    [SerializeField] private BULLET_TYPE type;

    private float timeCounter;
    protected SpriteRenderer _spriteRenderer;

    public BulletDescription Description;

    protected Rigidbody2D rigidBody;

    protected float _timeAliveS = 0;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public virtual void OnEnable()
    {
        if(Description == null)
        {
            Debug.Log("Trying to initialize bullet but Description is null!");
            return;
        }

        // Set basic info
        _timeAliveS = 0;
        transform.position = Description.Position;
        transform.rotation = Description.Rotation;
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        _spriteRenderer.size = new Vector2(Description.Width, Description.Height);
    }

    public virtual void OnDisable()
    {

    }

    // Update is called once per frame
    public virtual void Update()
    {
        _timeAliveS += Time.deltaTime;
        gameObject.transform.Translate(gameObject.transform.up * Time.deltaTime * baseSpeed, Space.World);
        if(_timeAliveS > Description.ActiveDurationS)
        {
            gameObject.SetActive(false);
        }
    }
}
