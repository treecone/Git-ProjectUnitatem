using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    BulletManager BM;
    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        BM = BulletManager.instance;
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float GetZRotationTowardsObj(GameObject ToObj)
    {
        Vector2 dir = ToObj.transform.position - gameObject.transform.position;
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;
    }

    public void OnBeat()
    {
        BM.SpawnFromPool(BULLET_TYPE.Base, gameObject.transform.position, Quaternion.Euler(0, 0, GetZRotationTowardsObj(player)));
    }
}
