using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeIn : MonoBehaviour
{
    public float ALPHA_FADE_SPEED;
    public float SCALE_FADE_SPEED;
    public float MAX_SCALE;
    public bool isAlphaFadeIn; //これがtrueならalphaの変更
    public bool isScaleFadeIn; //これがtrueならサイズの変更
    public bool isAlphaFinish = false; //これはアニメが終わってるかの判定用
    public bool isScaleFinish = false;
    public int liveCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        Color color = GetComponent<SpriteRenderer>().color;
        color.a = 0f;
        GetComponent<SpriteRenderer>().color = color;
    }

    // Update is called once per frame
    void Update()
    {
        liveCount++;
        if (liveCount > 100000000000) liveCount = 0;

        Color color = GetComponent<SpriteRenderer>().color;
        if((isAlphaFadeIn) && (color.a < 1.0f)) {
            color.a += ALPHA_FADE_SPEED;
            if (color.a > 1.0f) {
                color.a = 1.0f;
                isAlphaFinish = true;
            }
            GetComponent<SpriteRenderer>().color = color;
        }

        Vector3 scale = GetComponent<SpriteRenderer>().transform.localScale;
        if ((isScaleFadeIn) && (scale.x < MAX_SCALE)) {
            scale.x += SCALE_FADE_SPEED;
            scale.y += SCALE_FADE_SPEED;
            scale.z += SCALE_FADE_SPEED;
            if (scale.x > MAX_SCALE) {
                scale.x = MAX_SCALE;
                scale.y = MAX_SCALE;
                scale.z = MAX_SCALE;
                isScaleFinish = true;
            }
            GetComponent<SpriteRenderer>().transform.localScale = scale;
        }

    }
}
