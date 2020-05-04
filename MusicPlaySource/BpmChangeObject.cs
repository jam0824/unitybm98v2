using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BpmChangeObject : MonoBehaviour
{
    private string bpmRate;
    private MusicPlayManager musicPlayManager;
    private float v;

    void Start() {
        musicPlayManager = GameObject.Find("MusicPlayManager").GetComponent<MusicPlayManager>();
        v = musicPlayManager.getMusicObjVec();

    }

    public void setBpmRate(string bpmRate) {
        this.bpmRate = bpmRate;
    }

    private void Update() {
        v = musicPlayManager.getMusicObjVec();
        transform.Translate(0, 0, -v);
        if (this.transform.position.z <= 0){
            changeBpmRate();
            Destroy(this.gameObject);
        }
    }

    //現在のBPMと変更後BPMから変化の割合を出す。速度には割合がかけられる
    private void changeBpmRate() {
        int changedBpm = Convert.ToInt32(bpmRate, 16);
        float rate = (float)changedBpm / (float)musicPlayManager.getBpm();
        Debug.Log("changedBpm=" + changedBpm + " : BPM=" + musicPlayManager.getBpm()  + " : BPMRate:" + rate);
        musicPlayManager.setBpmChageRate(rate);
    }
    
}
