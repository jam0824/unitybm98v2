using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalcFps : MonoBehaviour
{
    int frameCount;
    float prevTime;

    
    public float CALC_TIME;
    private float fps;

    public float getFPS() {
        return this.fps;
    }

    void Start() {
        frameCount = 0;
        prevTime = 0.0f;
    }

    void Update() {
        ++frameCount;
        float time = Time.realtimeSinceStartup - prevTime;

        if (time >= CALC_TIME) {
            this.fps = frameCount / time;
            frameCount = 0;
            prevTime = Time.realtimeSinceStartup;
        }
    }
}
