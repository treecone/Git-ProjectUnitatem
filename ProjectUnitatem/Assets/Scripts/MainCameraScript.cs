using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraScript : MonoBehaviour
{
    private GameObject player;

    public float cameraSpeed;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, new Vector3(player.transform.position.x, player.transform.position.y, -10), cameraSpeed * Time.deltaTime);
    }
}
