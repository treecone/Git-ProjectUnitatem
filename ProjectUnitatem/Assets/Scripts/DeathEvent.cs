using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathEvent : MonoBehaviour
{
    public AK.Wwise.Event deathEvent;
    private GameObject BGMManager;

    public void Start()
    {
        BGMManager = GameObject.Find("BGM Manager");
    }

    //Actual death function, called from wise when beat ends
    public void SuperWiseDeath()
    {
        deathEvent.Post(BGMManager);
        GameObject.Find("Main Camera").GetComponent<MainCameraScript>().cameraZoom = 4;
        GameObject.Find("MainCanvas").transform.Find("DeathPanel").gameObject.SetActive(true);
    }
}
