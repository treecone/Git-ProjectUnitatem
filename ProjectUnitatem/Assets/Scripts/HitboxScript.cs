using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HITBOX_TYPE
{
    SWORD = 0,
    AXE = 1,
    BOW = 2
}

public class HitboxScript : MonoBehaviour
{
    public PlayerScript pScript;
    public float timeToDeath;
    public HITBOX_TYPE hitType;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("DieAfterSeconds");
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator DieAfterSeconds()
    {
        yield return new WaitForSeconds(timeToDeath);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        BulletBase bBase = collision.gameObject.GetComponent<BulletBase>();
        if (bBase != null)
        {
            switch(hitType)
            {
                case HITBOX_TYPE.SWORD:
                    bBase.baseSpeed *= 2;
                    collision.transform.rotation = Quaternion.Euler(0, 0, pScript.GetRotationForAbilities());
                    Destroy(gameObject);
                    break;

                case HITBOX_TYPE.AXE:
                    Destroy(collision.gameObject);
                    Destroy(gameObject);
                    break;
            }
        }
    }
}