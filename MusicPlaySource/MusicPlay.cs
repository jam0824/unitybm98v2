using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlay : MonoBehaviour
{
    private string[,] list_music_data;
    private Dictionary<string, Sprite> dict_image;
    private Dictionary<string, AudioClip> dict_audio;

    private MusicPlayManager musicPlayManager;
    private GameObject screenLeft;
    private GameObject screenRight;


    // Start is called before the first frame update
    void Start()
    {
        musicPlayManager = this.GetComponent<MusicPlayManager>();
        screenLeft = GameObject.Find("ScreenObjectLeft");
        screenRight = GameObject.Find("ScreenObjectRight");

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setDictAudio(Dictionary<string, AudioClip> dict_audio) {
        this.dict_audio = dict_audio;
    }
    public void setDictImage(Dictionary<string, Sprite> dict_image) {
        this.dict_image = dict_image;
    }

    public void setListMusicData(string[,] list_music_data) {
        this.list_music_data = list_music_data;
    }

    

    //音楽再生本体
    public bool playMusic(int frame_no) {
        bool isNull = true;
        int sameTimingOfMusicObject = 0;
        for (int i = 0; i < musicPlayManager.getKeyNum(); i++) {
            if (list_music_data[i, frame_no] != null) {
                //BGMが発生するkeyだった場合の処理
                if (i == 1) {
                    processMusicPart(i, frame_no);
                }
                //musicObjが発生するkeyだった場合の処理
                if ((i >= 10) && (i <= 19)) {
                    //オートモード
                    if (i >= musicPlayManager.AUTO_KEY_NO) {
                        processMusicPart(1, frame_no);
                    }
                    else {
                        sameTimingOfMusicObject++;
                        if(sameTimingOfMusicObject >= 3) {
                            //3つ以上同時はBGMにする
                            processMusicPart(1, frame_no);
                        }
                        else {
                            processMusicPart(i, frame_no);
                        }
                    }
                }
                    //拍子変更の際
                if (i == 2) {
                    //processChangeByoushi(i, frame_no);
                }
                //画像変更の際
                if (i == 4) {
                    processImagePart(i, frame_no);
                }
                isNull = false;
            }
        }

        return isNull;
    }

    private void processImagePart(int key_no, int frame_no) {
        string imageName = list_music_data[key_no, frame_no];
        //画像が同じフレームに複数設定される場合があるのでその際は最初の要素のみ取得
        if (imageName.Contains(",")) {
            string[] tmp = imageName.Split(',');
            imageName = tmp[0];
        }
        changeScreen(screenLeft, imageName);
        changeScreen(screenRight, imageName);
    }

    private void changeScreen(GameObject obj, string imageName) {
        try {
            SpriteRenderer s = obj.GetComponent<SpriteRenderer>();
            s.sprite = dict_image[imageName];
        }
        catch (Exception e) {
            Debug.LogError("#BMP" + imageName + "に該当する画像がありませんでした");
            Debug.LogError(e);
        }
    }

    //途中で何拍子かが変更された時に呼ばれる
    private void processChangeByoushi(int key_no, int frame_no) {
        float changeByoushi = float.Parse(list_music_data[key_no, frame_no]);
        setMusicObjVec(
            4 * changeByoushi,
            musicPlayManager.FRAME_RATE,
            musicPlayManager.getBpm()
        );
    }

    //MusicObjectが進む速さを求める
    public float setMusicObjVec(float byoushi, int frame, int BPM) {
        float z = this.transform.position.z;
        float how_long_syousetsu = 60 * byoushi * frame / BPM;
        return z / how_long_syousetsu;
    }

    //音楽のkeyだった場合の処理
    private void processMusicPart(int key_no, int frame_no) {
        if (list_music_data[key_no, frame_no].Contains(",")) {
            string[] command = list_music_data[key_no, frame_no].Split(',');
            foreach (string wav_name in command) {
                makeMusicObject(key_no, frame_no, wav_name);
            }
        }
        else {
            string wav_name = list_music_data[key_no, frame_no];
            makeMusicObject(key_no, frame_no, wav_name);
        }

    }

    //MusicObject作成
    private void makeMusicObject(int key_no, int frame_no, string wav_name) {
        GameObject obj;
        if (key_no == 1) {
            obj = musicPlayManager.bgmObj;
        }
        else if (key_no == 16) {
            obj = musicPlayManager.musicObjRed;
        }
        else if (key_no % 2 == 1) {
            obj = musicPlayManager.musicObjBlue;
        }
        else {
            obj = musicPlayManager.musicObjOrange;
        }
        GameObject musicObject = Instantiate(obj) as GameObject;
        musicObject.transform.localScale = new Vector3(
            musicPlayManager.MUSIC_OBJ_SIZE,
            musicPlayManager.MUSIC_OBJ_SIZE,
            musicPlayManager.MUSIC_OBJ_SIZE
        );
        try {
            AudioSource audioSource = musicObject.GetComponent<AudioSource>();
            audioSource.clip = dict_audio[wav_name];
            setPosition(musicObject, key_no);
        }
        catch (Exception e) {
            Debug.LogError("#WAV" + wav_name + "に該当する音がありませんでした");
            Debug.LogError(e);
        }
        
    }

    //MusicObjectの位置を設定
    private void setPosition(GameObject obj, int key_no) {
        float w = musicPlayManager.MUSIC_WIDTH / (musicPlayManager.getPlayKeyNum());
        float x = -(musicPlayManager.MUSIC_WIDTH / 2) + (w * (key_no - 10));
        x -= w;
        //Debug.Log("key=" + musicPlayManager.getPlayKeyNum() + " : w=" + w + " : x=" + x);
        obj.transform.position = new Vector3(
            x,
            musicPlayManager.MUSIC_OBJ_Y,
            this.transform.position.z
        );
    }
}
