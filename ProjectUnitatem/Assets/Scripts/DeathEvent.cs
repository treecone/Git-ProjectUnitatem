using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathEvent : MonoBehaviour
{
    private GameObject BGMManager;

    public void Start()
    {
        BGMManager = GameObject.Find("BGM Manager");
        GameObject.Find("Player").transform.Find("PlayerSprite").transform.Translate(Vector2.down);
        GameObject.Find("Player").transform.Find("PlayerSprite").Rotate(Vector3.forward, 90);
        GameObject.Find("Player").GetComponent<PlayerScript>().movementSpeed = 0;
        GameObject.Find("Player").GetComponent<PlayerScript>().dead = true;
        GameObject.Find("Main Camera").GetComponent<MainCameraScript>().cameraZoom = 4;
        GameObject.Find("MainCanvas").transform.Find("DeathPanel").gameObject.SetActive(true);
    }

    //Actual death function, called from wise when beat ends

   
}
