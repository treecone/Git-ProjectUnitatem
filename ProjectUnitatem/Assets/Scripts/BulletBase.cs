using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BULLET_TYPE
{
    Base = 0,
}

public class BulletBase : MonoBehaviour
{
    Rigidbody2D rb;

    [Header("Bullet Attributes")]
    [SerializeField] private float baseSpeed;
    [SerializeField] private float damage;
    [SerializeField] private BULLET_TYPE type;

    private float timeCounter;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public virtual void OnEnable()
    {

    }

    public virtual void OnDisable()
    {

    }

    // Update is called once per frame
    public virtual void Update()
    {
        gameObject.transform.Translate(gameObject.transform.up * Time.deltaTime * baseSpeed, Space.World);
    }
}
