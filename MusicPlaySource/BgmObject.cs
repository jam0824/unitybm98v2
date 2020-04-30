using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgmObject : MonoBehaviour
{
    private AudioSource audio;
    private float v;
    private bool isSound = false;
    // Start is called before the first frame update
    void Start() {
        GameObject mainObject = GameObject.Find("MusicPlayManager");
        MusicPlayManager m = mainObject.GetComponent<MusicPlayManager>();
        v = m.getMusicObjVec();

    }

    private void Update() {
        transform.Translate(0, 0, -v);
        if ((this.transform.position.z <= 0) && (!isSound)) {
            sound();
            isSound = true;
        }
    }

    void sound() {
        audio = this.GetComponent<AudioSource>();
        audio.PlayOneShot(audio.clip);
        StartCoroutine(Checking(() => {
            Destroy(this.gameObject);
        }));
    }

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
