using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamBullet : BulletBase

{
    private bool _collisionEnabled;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnEnable()
    {
        if(Description == null)
        {
            return;
        }

        Init();
    }

    protected override void Init()
    {
        _timeAliveS = 0;
        _collisionEnabled = false;
        // Set pivot to desired position
        transform.position = Description.Position;
        // Move child so the pivot is at the end
        Transform child = transform.GetChild(0);
        child.position = transform.position + new Vector3(Description.Width / 2 - 0.5f, 0);
        // Rotate whole object about pivot
        transform.rotation = Description.Rotation;
        _spriteRenderer = child.GetComponent<SpriteRenderer>();
        _spriteRenderer.size = new Vector2(Description.Width, Description.Height);
        SetAlpha(0.25f);
    }

    public void OnDisable()
    {
        if (Description == null)
        {
            return;
        }
        Transform child = transform.GetChild(0);
        child.position = Vector3.zero;
        _spriteRenderer.size = new Vector2(1,1);
        transform.rotation = Quaternion.identity;
    }

    // Update is called once per frame
    public override void Update()
    {
        _timeAliveS += Time.deltaTime;
        // Kill it!
        if(_timeAliveS > Description.FadeInDurationS + Description.ActiveDurationS)
        {
            gameObject.SetActive(false);
        }
        // Time to enable collision!
        else if(!_collisionEnabled &&_timeAliveS > Description.FadeInDurationS)
        {
             SetAlpha(1.0f);
             _collisionEnabled = true;
            // TODO: SL enable collision
        }
        else if (_collisionEnabled)
        {
           gameObject.transform.Rotate(new Vector3(0, 0, Time.deltaTime * 60 * Description.RotationSpeed * (Description.RotationDirection == ROTATION_DIRECTION.CounterClockwise ? 1 : -1)));
        }
    }
}
