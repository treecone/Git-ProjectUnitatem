using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BULLET_TYPE
{
    Base = 0,
    Beam,
    Plop,
}

public enum ROTATION_DIRECTION
{
    CounterClockwise = 0,
    Clockwise,
    None,
}

public class BulletBase : MonoBehaviour
{
    [Header("Bullet Attributes")]
    public float damage;

    protected SpriteRenderer _spriteRenderer;

    public BulletDescription Description;

    protected float _timeAliveS = 0;

    private bool _hit;
    public bool Hit { set { _hit = value; } }

    public void OnEnable()
    {
        // Bullet has been created for pool so skip initialization
        if(Description == null)
        {
            return;
        }

        Init();
    }

    protected virtual void Init()
    {
        // Set basic info
        _timeAliveS = 0;
        transform.position = Description.Position;
        transform.rotation = Description.Rotation;
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        _spriteRenderer.size = new Vector2(Description.Width, Description.Height);
    }

    public void OnDisable()
    {

    }

    // Update is called once per frame
    public virtual void Update()
    {
        _timeAliveS += Time.deltaTime;
        if (Description.Homing && !_hit)
        {
            Vector2 dir = Description.PlayerRef.transform.position - gameObject.transform.position;
            float rad = Mathf.Atan2(dir.y, dir.x);
            gameObject.transform.rotation = Quaternion.Euler(0, 0, (Mathf.Rad2Deg * rad) - 90);
        }
        gameObject.transform.Translate(gameObject.transform.up * Time.deltaTime * Description.Speed, Space.World);
        
        if(_timeAliveS > Description.ActiveDurationS)
        {
            gameObject.SetActive(false);
        }
    }

    protected void SetAlpha(float alpha)
    {
        Color color = _spriteRenderer.color;
        _spriteRenderer.color = new Color(color.r, color.g, color.b, alpha);
    }
}
