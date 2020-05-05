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
    private MusicPlayData musicPlayData;
    private GameObject screenLeft;
    private GameObject screenRight;


    // Start is called before the first frame update
    void Start()
    {
        musicPlayManager = this.GetComponent<MusicPlayManager>();
        musicPlayData = GameObject.Find("MusicPlayManager").GetComponent<MusicPlayData>();
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
                    processMusicPart(i, frame_no, sameTimingOfMusicObject);
                }
                //musicObjが発生するkeyだった場合の処理
                if ((i >= 10) && (i <= 29)) {
                    sameTimingOfMusicObject++;
                    processMusicPart(i, frame_no, sameTimingOfMusicObject);
                }
                
                //画像変更の際
                if (i == 4) {
                    processImagePart(i, frame_no);
                }

                //BPM変更の際
                if (i == 3) {
                    //不要。デバッグ用
                    processBpmPart(i, frame_no);
                }
                isNull = false;
            }
        }

        return isNull;
    }

    //BPM変更オブジェクトを投げる
    private void processBpmPart(int key_no, int frame_no) {
        string bpmRate = list_music_data[key_no, frame_no];
        GameObject bpmChangeObject = Instantiate(musicPlayManager.bpmChangeObj) as GameObject;
        bpmChangeObject.transform.localScale = new Vector3(
            musicPlayManager.MUSIC_OBJ_SIZE,
            musicPlayManager.MUSIC_OBJ_SIZE,
            musicPlayManager.MUSIC_OBJ_SIZE
        );
        BpmChangeObject b = bpmChangeObject.GetComponent<BpmChangeObject>();
        b.setBpmRate(bpmRate); 
    }

    //画像が来たときの処理
    private void processImagePart(int key_no, int frame_no) {
        string imageName = list_music_data[key_no, frame_no];
        //画像が同じフレームに複数設定される場合があるのでその際は最初の要素のみ取得
        if (imageName.Contains(",")) {
            string[] tmp = imageName.Split(',');
            imageName = tmp[0];
        }
        if (!dict_image.ContainsKey(imageName)) {
            Debug.Log("#BMP" + imageName + "に該当する画像がありませんでした");
            return;
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
            Debug.Log(e);
        }
    }

    //MusicObjectが進む速さを求める
    public float setMusicObjVec(float byoushi, int frame, float BPM) {
        float z = this.transform.position.z;
        float how_long_syousetsu = 60.0f * byoushi * (float)frame / BPM;
        return z / how_long_syousetsu;
    }

    //音楽のkeyだった場合の処理
    private void processMusicPart(int key_no, int frame_no, int sameTimingOfMusicObject) {
        if (list_music_data[key_no, frame_no].Contains(",")) {
            string[] command = list_music_data[key_no, frame_no].Split(',');
            foreach (string wav_name in command) {
                int changeKey = changeKeyNo(key_no, sameTimingOfMusicObject);
                makeMusicObject(changeKey, frame_no, wav_name);
            }
        }
        else {
            string wav_name = list_music_data[key_no, frame_no];
            int changeKey = changeKeyNo(key_no, sameTimingOfMusicObject);
            makeMusicObject(changeKey, frame_no, wav_name);
        }

    }

    //同時押しとオート判定。判定した場合はBGMにする
    private int changeKeyNo(int key_no, int sameTimingOfMusicObject) {
        if (sameTimingOfMusicObject >= 3) {
            return 1;
        }
        if (key_no >= musicPlayManager.AUTO_KEY_NO) {
            return 1;
        }
        else {
            return key_no;
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
        if(key_no != 1) musicPlayData.addTotalNotesNum();
        try {
            if (!dict_audio.ContainsKey(wav_name)) {
                Debug.Log("#WAV" + wav_name + "に該当する音がありませんでした");
                return;
            }
            AudioSource audioSource = musicObject.GetComponent<AudioSource>();
            audioSource.clip = dict_audio[wav_name];
            setPosition(musicObject, key_no);
        }
        catch (Exception e) {
            Debug.Log(e);
        }
        
    }

    //MusicObjectの位置を設定
    private void setPosition(GameObject obj, int key_no) {
        //7keyのときはちょっと広げる
        //float musicWidth = (musicPlayManager.PlayKeyNum == 5) ? musicPlayManager.MUSIC_WIDTH : musicPlayManager.MUSIC_WIDTH + 0.2f;
        float musicWidth = musicPlayManager.MUSIC_WIDTH;
        float w = musicWidth / musicPlayManager.PlayKeyNum;
        float x = -(musicWidth / 2) + (w * (key_no - 10));
        x -= w;
        //Debug.Log("key=" + musicPlayManager.getPlayKeyNum() + " : w=" + w + " : x=" + x);
        obj.transform.position = new Vector3(
            x,
            musicPlayManager.getMusicObjY(),
            this.transform.position.z
        );
    }
}
