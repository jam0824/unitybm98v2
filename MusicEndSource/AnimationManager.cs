using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public GameObject COMPLETE_OBJECT;
    public bool isAnimation = false;

    private MusicPlayManager musicPlayManager;
    private GameObject completeObj;
    private FadeIn completeFadeIn;

    private float v = 0.01f;
    private bool isMakeResultArea = false;

    // Start is called before the first frame update
    void Start()
    {
        musicPlayManager = GameObject.Find("MusicPlayManager").GetComponent<MusicPlayManager>();


    }

    // Update is called once per frame
    void Update()
    {
        if (!isAnimation) return;

        if((completeObj.GetComponent<Transform>().transform.position.y < 4.0f) && 
            (completeFadeIn.liveCount > musicPlayManager.FRAME_RATE * 4)) {
            Vector3 pos = completeObj.GetComponent<Transform>().transform.position;
            pos.y += 0.05f;
            completeObj.GetComponent<Transform>().transform.position = pos;
            if (!isMakeResultArea) {
                GetComponent<ResultAreaAnimation>().startResultAreaDraw();
                isMakeResultArea = true;
            }
        }
        
    }

    public void startFinishAnimation() {
        isAnimation = true;
        makeCompleteObj();
    }

    private void makeCompleteObj() {
        completeObj = Instantiate(COMPLETE_OBJECT) as GameObject;
        completeFadeIn = completeObj.GetComponent<FadeIn>();
    }
}
