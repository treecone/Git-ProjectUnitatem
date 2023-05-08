using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            Debug.Log("NEW PHASE DEBUG");
            ChangeTheme(a);
        }
    }

    public void SetNewTransition()
    {
        Debug.Log("------------------- NEW PHASE --------------------");
        tryToTransition = false;
        int newPhase = BPM.IncrementPhase();
        ChangeTheme(newPhase);

        if(newPhase > 2)
        {
            //Win game!

        }
        else
        {
            gameObject.GetComponent<Animator>().Play("BossPoint");
            StartCoroutine("Phase");
        }
    }

    IEnumerator FirstPhase()
    {
        yield return new WaitForSeconds(phaseLength);
        tryToTransition = true;
    }

    IEnumerator Phase()
    {
        BPM.SetInTransition(true);
        yield return new WaitForSeconds(5f);
        
        BPM.SetInTransition(false);
        yield return new WaitForSeconds(phaseLength);
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
