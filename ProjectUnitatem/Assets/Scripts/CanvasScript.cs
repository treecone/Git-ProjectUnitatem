using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasScript : MonoBehaviour
{

    public AK.Wwise.Event click;
    public AK.Wwise.Event hover;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClickStart()
    {
        click.Post(gameObject);

    }

    public void HoverStart()
    {
        hover.Post(gameObject);

    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void MoveToScene(int sceneNum)
    {
        SceneManager.LoadScene(sceneNum);
    }
}
