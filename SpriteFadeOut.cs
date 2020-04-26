using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteFadeOut : MonoBehaviour
{
    private SpriteRenderer spRenderer;
    private float v = 0.002f;

    // Start is called before the first frame update
    void Start()
    {
        spRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        var color = spRenderer.color;
        if(color.a > 0) {
            color.a -= v;
            spRenderer.color = color;
        }
      
    }
}
