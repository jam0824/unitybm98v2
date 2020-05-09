using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class MovieObject : MonoBehaviour
{
    private MusicPlayManager musicPlayManager;
    private float v;
    private float PLAY_LINE = -10.0f;

    void Start() {
        musicPlayManager = GameObject.Find("MusicPlayManager").GetComponent<MusicPlayManager>();
        v = musicPlayManager.getMusicObjVec();

    }

    private void Update() {
        v = musicPlayManager.getMusicObjVec();
        transform.Translate(0, 0, -v);
        if (this.transform.position.z <= PLAY_LINE) {
            playVideo();
            Destroy(this.gameObject);
        }
    }

    //動画再生
    private void playVideo() {
        playVideoMain("MovieRenderImageLeft");
        playVideoMain("MovieRenderImageRight");
    }
    private void playVideoMain(string objName) {
        VideoPlayer obj = GameObject.Find(objName).GetComponent<VideoPlayer>();
        obj.Play();
    }
}
