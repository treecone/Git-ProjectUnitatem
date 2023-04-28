using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowScript : MonoBehaviour
{
    private Transform player;
    [SerializeField]
    private Vector3 baseScale;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = new Vector3(player.position.x, player.position.y, 0);
    }
}
