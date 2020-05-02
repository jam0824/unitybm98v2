using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicObject : MonoBehaviour
{
    public GameObject hitPrefab;
    private MusicPlayManager m;
    private MusicPlayData musicPlayData;
    private float GOOD_LINE;
    private float GREAT_LINE;
    private bool isPoor = false;
    private AudioSource audio;
    private UiController ui;
    private float v;
    private bool isSound = false;
    private bool isAutoPlay = false;
    private float DEAD_LINE = -5.0f;
    // Start is called before the first frame update
    void Start()
    {
        GameObject mainObject = GameObject.Find("MusicPlayManager");
        musicPlayData = GameObject.Find("MusicPlayManager").GetComponent<MusicPlayData>();

        m = mainObject.GetComponent<MusicPlayManager>();
        v = m.getMusicObjVec();
        isAutoPlay = m.isAutoPlay;
        GOOD_LINE = m.GOOD_LINE;
        GREAT_LINE = m.GREAT_LINE;
        ui = GameObject.Find("UiArea").GetComponent<UiController>();
        
    }

    private void Update() {
        v = m.getMusicObjVec();
        transform.Translate(0, 0, -v);
        //オートプレイだった場合は0ラインに来たら再生
        if ((this.transform.position.z <= 0)&&(!isSound)&&(isAutoPlay)) {
            sound();
            isSound = true;
        }
        //音楽が再生されておらず後ろに行ってしまったら
        if(!isSound) {
            if ((this.transform.position.z < -GOOD_LINE) && (!isPoor)) {
                musicPlayData.setComboNum(0);
                musicPlayData.addPoorNum();
                ui.setStatus("poor");
                isPoor = true;
            }

            if(this.transform.position.z <= DEAD_LINE) {
                Destroy(this.gameObject);
            }
        }
        
    }

    //曲ならす
    void sound() {
        audio = this.GetComponent<AudioSource>();
        audio.PlayOneShot(audio.clip);
        StartCoroutine(Checking(() => {
            Destroy(this.gameObject);
        }));
    }

    //hit tagのオブジェクトとぶつかったら音を鳴らしてヒットマークを出す
    void OnTriggerEnter(Collider other) {
        if ((other.gameObject.tag == "hit")&&(!isSound)) {
            transparentObject(this.gameObject);
            judge();
            hit();
            sound();
            isSound = true;
        }
    }

    //オブジェクトをただ削除すると音も消えるので、縮小して消えたように見せる
    void transparentObject(GameObject obj) {
        this.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
    }

    //結果の判定
    void judge() {
        musicPlayData.addComboNum();
        float z = Mathf.Abs(this.transform.position.z);
        if (z <= GREAT_LINE) {
            ui.setStatus("great");
            musicPlayData.addGreatNum();
        }
        else if ((z > GREAT_LINE) && (z < GOOD_LINE)) {
            ui.setStatus("good");
            musicPlayData.addGoodNum();
        }
        else {
            ui.setStatus("poor");
            musicPlayData.addPoorNum();
        }
    }

    //ヒットスパーク
   void hit() {
        if (hitPrefab != null) {
            GameObject hitVFX = Instantiate(
                hitPrefab,
                this.transform.position,
                Quaternion.identity
            ) as GameObject;

            var ps = hitVFX.GetComponent<ParticleSystem>();
            if (ps == null) {
                var psChild = hitVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(hitVFX, psChild.main.duration);
            }
            else
                Destroy(hitVFX, ps.main.duration);
        }
    }

    //音楽が鳴っている間活かす処理
    public delegate void functionType();
    private IEnumerator Checking(functionType callback) {
        while (true) {
            yield return new WaitForFixedUpdate();
            if (!audio.isPlaying) {
                callback();
                break;
            }
        }
    }
}
