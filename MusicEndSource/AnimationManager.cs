using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public GameObject COMPLETE_OBJECT;
    public GameObject FAILED_OBJECT;
    public bool isAnimation = false;
    public bool isSuccess = false;

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

        //成功時のコンプリートが上に上がってスコア表示が出るところ
        if((isSuccess) &&
            (completeObj.GetComponent<Transform>().transform.position.y < 4.0f) && 
            (completeFadeIn.liveCount > musicPlayManager.FRAME_RATE * 4)) {
            Vector3 pos = completeObj.GetComponent<Transform>().transform.position;
            pos.y += 0.05f;
            completeObj.GetComponent<Transform>().transform.position = pos;
            if (!isMakeResultArea) {
                playSe();
                GetComponent<ResultAreaAnimation>().startResultAreaDraw();
                isMakeResultArea = true;
            }
        }
        
    }

    //成功時のアニメーション開始
    public void startFinishAnimation() {
        isAnimation = true;
        isSuccess = true;
        makeCompleteObj();
    }

    private void makeCompleteObj() {
        completeObj = Instantiate(COMPLETE_OBJECT) as GameObject;
        completeFadeIn = completeObj.GetComponent<FadeIn>();
    }

    private void playSe() {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(
            musicPlayManager.randomSe(musicPlayManager.listSeClear)
        );
    }

    //失敗時のアニメーション開始
    public void startFailedAnimation() {
        isAnimation = true;
        isSuccess = false;
        makeFailedObj();
    }
    private void makeFailedObj() {
        GameObject obj = Instantiate(FAILED_OBJECT) as GameObject;
    }
}
