using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArmScript : MonoBehaviour
{

    private PlayerScript PS;
    // Start is called before the first frame update
    void Start()
    {
        PS = transform.root.gameObject.GetComponent<PlayerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DeactivateWeapon()
    {
        switch (PS.currentWeapon)
        {
            case (0):
                gameObject.transform.GetChild(0).gameObject.SetActive(false);
                break;
        }
    }
}
