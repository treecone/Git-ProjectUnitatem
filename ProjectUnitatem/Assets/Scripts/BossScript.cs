using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossScript : MonoBehaviour
{
    public SpriteRenderer[] ChangingColorSprites;
    BossPhaseManager BPM;
    public Color[] GenreColors;
    bool tryToTransition;
    private GameObject player;
    public float phaseLength;

    bool start;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        start = true;
        BPM = GetComponent<BossPhaseManager>();
        BPM.SetInTransition(true);
        ChangeTheme(0);
    }

    // Update is called once per frame
    void Update()
    {
        if(start)
        {
            if (Vector2.Distance(gameObject.transform.position, player.transform.position) < 10)
            {
                start = false;
                BPM.SetInTransition(false);
                StartCoroutine("FirstPhase");
            }
        }

        if(tryToTransition)
        {
            if(BPM.IsCurrentMoveComplete())
            {
                SetNewTransition();
            }
        }

        if(Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            int a = BPM.IncrementPhase();
            player.GetComponent<PlayerScript>().currentPhase = a;
            Debug.Log("NEW PHASE DEBUG Phase: " + a);
            ChangeTheme(a);
        }
    }

    public void SetNewTransition()
    {
        tryToTransition = false;
        int newPhase = BPM.IncrementPhase();
        player.GetComponent<PlayerScript>().currentPhase = newPhase;
        Debug.Log("------------------- NEW PHASE (" + newPhase + ") --------------------");
        ChangeTheme(newPhase);

        if(newPhase > 2)
        {
            //Win game!
            Debug.Log("------------------ Game win -----------------------------");
            SceneManager.LoadScene(2);

        }
        else
        {
            gameObject.GetComponent<Animator>().Play("BossPoint");
            GameObject.Find("MainCanvas").GetComponent<CanvasScript>().CallTutorialPanel(newPhase);
            StartCoroutine(Phase(newPhase));
        }
    }

    IEnumerator FirstPhase()
    {
        yield return new WaitForSeconds(phaseLength);
        tryToTransition = true;
    }

    IEnumerator Phase(int phaseID)
    {
        BPM.SetInTransition(true);
        yield return new WaitForSeconds(5f);
        
        BPM.SetInTransition(false);

        switch(phaseID)
        {
            case 0:
                yield return new WaitForSeconds(30);
                break;
            case 1:
                yield return new WaitForSeconds(50);
                break;
            case 2:
                yield return new WaitForSeconds(110);
                break;
        }

        tryToTransition = true;
    }

    public void ChangeTheme(int genre)
    {
        foreach(SpriteRenderer s in ChangingColorSprites)
        {
            s.color = GenreColors[genre];
        }
    }
}
