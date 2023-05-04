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
        base.Init();
        SetAlpha(0.5f);
    }

    public void OnDisable()
    {
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
    }

    private void SetAlpha(float alpha)
    {
        Color color = _spriteRenderer.color;
        _spriteRenderer.color = new Color(color.r, color.g, color.b, alpha);
    }
}
