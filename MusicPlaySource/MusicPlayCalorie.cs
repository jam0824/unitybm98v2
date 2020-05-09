using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayCalorie : MonoBehaviour
{
    public float WEIGHT;
    public float METs;
    public float PUNCH_PER_SEC;

    private float startSec;
    private MusicPlayData musicPlayData;


    // Start is called before the first frame update
    void Start()
    {
        musicPlayData = GetComponent<MusicPlayData>();
        startSec = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        calcCalorie();
    }

    //カロリー計算
    private void calcCalorie() {
        float nowSec = Time.time - startSec;
        float nowPunchPerSec = 
            (musicPlayData.getExcellentNum() + musicPlayData.getGreatNum() + musicPlayData.getGoodNum()) / nowSec;
        float nowMETs = this.METs * nowPunchPerSec / this.PUNCH_PER_SEC;
        float calorie = nowMETs * this.WEIGHT * (nowSec / 3600.0f) * 1.05f;
        musicPlayData.METs = nowMETs;
        musicPlayData.setCalorie(calorie);
    }
}
