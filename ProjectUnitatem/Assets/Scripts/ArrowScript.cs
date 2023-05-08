using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowScript : MonoBehaviour
{
    public float projectileSpeed = 10f;
    public Vector2 endPoint;
    public float height;
    float distance;
    float timeIncrementor;
    private bool audioLock = false;
    GameObject BGMManager;
    float time;

    public AK.Wwise.Event landingEvent;

    public bool teleportPlayer;
    Rigidbody2D rb;
    GameObject childSprite;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        childSprite = gameObject.transform.GetChild(0).gameObject;
        LaunchProjectile();
        BGMManager = GameObject.Find("BGM Manager");

    }

    void LaunchProjectile ()
    {
        // apply the initial velocity to the projectile rigidbody
        Vector2 vec = endPoint - (Vector2)gameObject.transform.position;
        rb.velocity = (vec.normalized * projectileSpeed);
        distance = vec.magnitude;
        time = distance / projectileSpeed;
        StartCoroutine(TimeToLand(time));
    }

    private void Update()
    {
        timeIncrementor += Time.deltaTime;
        float x = timeIncrementor / time;
        float parabolicEquation = (-1 * height) * x * (x - 1);
        childSprite.transform.position = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + parabolicEquation);

        float slope = (-1 * height) * (2 * x - distance);

        float tangent = Mathf.Atan(slope);

        childSprite.transform.rotation = Quaternion.Euler(0, 0, (tangent * Mathf.Rad2Deg) + 90);

    }

    IEnumerator TimeToLand (float time)
    {
        yield return new WaitForSeconds(time);
        if(teleportPlayer)
        {
            Landed();
        }
        Destroy(gameObject);
    }

    void Landed ()
    {
        GameObject.Find("Player").transform.position = gameObject.transform.position;
        landingEvent.Post(BGMManager);
    }
}
