using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicObject : MonoBehaviour
{
    public GameObject hitPrefab;
    private AudioSource audio;
    private float v;
    private bool isSound = false;
    private bool isAutoPlay = false;
    private float DEAD_LINE = -5.0f;
    // Start is called before the first frame update
    void Start()
    {
        GameObject mainObject = GameObject.Find("MainObject");
        MainObject m = mainObject.GetComponent<MainObject>();
        v = m.musicObjVec;
        isAutoPlay = m.isAutoPlay;
        
    }

    private void Update() {
        transform.Translate(0, 0, -v);
        //オートプレイだった場合は0ラインに来たら再生
        if ((this.transform.position.z <= 0)&&(!isSound)&&(isAutoPlay)) {
            sound();
            isSound = true;
        }
        //音楽が再生されておらず、deadLineを越えたらオブジェクト削除
        if((this.transform.position.z <= DEAD_LINE)&&(!isSound)) {
            Destroy(this.gameObject);
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
            hit();
            sound();
            isSound = true;
        }
    }

    //オブジェクトをただ削除すると音も消えるので、縮小して消えたように見せる
    void transparentObject(GameObject obj) {
        this.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
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
