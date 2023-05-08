using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlopBullet : BulletBase
{
    private bool _collisionEnabled;
    private CircleCollider2D _circleCollider2D;

    protected override void Init()
    {
        base.Init();
        SetAlpha(0.25f);
        transform.localScale = Vector3.one * Description.StartScale;
        _circleCollider2D = GetComponent<CircleCollider2D>();
        _circleCollider2D.enabled = false;
        _collisionEnabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        _timeAliveS += Time.deltaTime;
        // Kill it!
        if (_timeAliveS > Description.FadeInDurationS + Description.ActiveDurationS)
        {
            gameObject.SetActive(false);
        }
        // Time to enable collision!
        else if (!_collisionEnabled && _timeAliveS > Description.FadeInDurationS)
        {
            SetAlpha(1.0f);
            _collisionEnabled = true;
            _circleCollider2D.enabled = true;
        }
        else if (_collisionEnabled)
        {
            float t = (_timeAliveS - Description.FadeInDurationS) / Description.ActiveDurationS;
            float scale = Mathf.Lerp(Description.StartScale, Description.EndScale, t);
            transform.localScale = Vector3.one * scale;
        }
    }
}
