using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossScript : MonoBehaviour
{
    public SpriteRenderer[] ChangingColorSprites;
    public Color[] GenreColors; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeTheme(int genre)
    {
        foreach(SpriteRenderer s in ChangingColorSprites)
        {
            s.color = GenreColors[genre];
        }
    }
}
