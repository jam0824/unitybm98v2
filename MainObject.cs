﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO; //System.IO.FileInfo, System.IO.StreamReader, System.IO.StreamWriter
using System.Text.RegularExpressions;

public class MainObject : MonoBehaviour
{
    [SerializeField] public bool isAutoPlay = false;
    [SerializeField] public int FRAME_RATE;

    [SerializeField] private GameObject musicObjBlue;
    [SerializeField] private GameObject musicObjRed;
    [SerializeField] private GameObject musicObjOrange;
    [SerializeField] private GameObject bgmObj;
    [SerializeField] private string music_folder;
    [SerializeField] private string music_bms;
    [SerializeField] private float MUSIC_WIDTH;
    [SerializeField] private int PLAY_KEY_NUM;

    public float musicObjVec;
    private float musicObjSize = 0.3f;
    private float musicObjY = 0;
    private int KEY_NUM = 30;
    private int MAX_OBJ_NUM = 100000;
    private bool isUpdate = false;
    private Dictionary<string, AudioClip> dict_audio;
    private Dictionary<string, Sprite> dict_image;
    private Dictionary<string, string> dict_info;
    private string[,] list_music_data;
    private int frame_num = 0;
    private int nullCount = 0;
    private GameObject screenLeft;
    private GameObject screenRight;

    private void init() {
        Application.targetFrameRate = FRAME_RATE;
        dict_audio = new Dictionary<string, AudioClip>();
        dict_image = new Dictionary<string, Sprite>();
        list_music_data = new string[KEY_NUM, MAX_OBJ_NUM];
        screenLeft = GameObject.Find("ScreenObjectLeft");
        screenRight = GameObject.Find("ScreenObjectRight");
    }

    void Start()
    {
        init();

        string[] lines = read(music_folder + "/" + music_bms);

        dict_info = getInfomation(lines);
        dict_audio = readAudioFiles(lines);
        dict_image = readImageFiles(lines);

        list_music_data = makeMusicData(
            lines, 
            int.Parse(dict_info["#BPM"]), 
            FRAME_RATE
        );
        musicObjVec = setMusicObjVec(4, FRAME_RATE, int.Parse(dict_info["#BPM"]));
        isUpdate = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isUpdate) {
            playMusic(frame_num);
            frame_num++;
        }
        //Bボタンが押されたら最初から
        if (OVRInput.GetDown(OVRInput.Button.Two)) {
            frame_num = 0;
            nullCount = 0;
        }

        if(nullCount == FRAME_RATE * 3) {
            finish();
        }
    }

    


    //bmsファイル読み込み
    private string[] read(string filePath) {
        Debug.Log(filePath);
        TextAsset text = Resources.Load<TextAsset>(filePath);
        string allText = text.text;
        allText = allText.Replace("\r", "");
        return allText.Split('\n');
    }

    //インフォメーション部分読み込み
    private Dictionary<string, string> getInfomation(string[] list_string) {
        Dictionary<string, string> dict_info = new Dictionary<string, string>();
        foreach (string line in list_string) {
            if (line.Contains("#WAV")) break;
            int index = line.IndexOf(" ");
            if (index == -1) continue;
            if (line.Substring(0, 1) == "*") continue;
            string key = line.Substring(0, index);
            string value = line.Substring(index + 1);
            dict_info.Add(key, value);
        }
        return dict_info;
    }

    //曲ファイルをdictに格納
    private Dictionary<string, AudioClip> readAudioFiles(string[] list_string) {
        Dictionary<string, AudioClip> dic_audio = new Dictionary<string, AudioClip>();
        foreach (string line in list_string) {
            if (line.Contains("#WAV")) {
                string tmp = line.Replace("#WAV", "");
                string[] command = tmp.Split(' ');
                string wav_name = command[1].Replace(".wav", "");
                wav_name = wav_name.Replace(".ogg", "");
                AudioClip ac = Resources.Load<AudioClip>(music_folder + "/" + wav_name);
                dic_audio.Add(command[0], ac);
            }

        }
        return dic_audio;
    }

    //画像ファイルをdictに格納
    private Dictionary<string, Sprite> readImageFiles(string[] list_string) {
        Dictionary<string, Sprite> dict_image = new Dictionary<string, Sprite>();
        foreach (string line in list_string) {
            if (line.Contains("#BMP")) {
                string tmp = line.Replace("#BMP", "");
                string[] command = tmp.Split(' ');
                string image_name = command[1].Replace(".bmp", "");
                Sprite image = Resources.Load<Sprite>(music_folder + "/" + image_name);
                dict_image.Add(command[0], image);
            }

        }
        return dict_image;
    }

    //楽譜作成
    private string[,] makeMusicData(string[] list_lines, int BPM, int frame) {
        string[,] list_music_data = new string[KEY_NUM, MAX_OBJ_NUM];
        float byoushi = 4;
        foreach (string line in list_lines) {
            if (line.Contains("#") && line.Contains(":")) {
                string tmp = line.Replace("#", "");
                string[] command = tmp.Split(':');
                //取得したコマンドがNumでなければやり直し
                if (!isNum(command[0].Substring(0, 3))) continue;

                int syousetsu_no = int.Parse(command[0].Substring(0, 3));
                int key_no = int.Parse(command[0].Substring(3, 2));

                //"02"拍子変更の際の処理
                if (key_no == 2) byoushi = 4 * float.Parse(command[1]);
                //小節の所要フレーム数を求める
                float how_long_syousetsu = 60 * byoushi * frame / BPM;
                Debug.Log("小節の長さ=" + how_long_syousetsu);

                //音符間の所要フレーム数を求める
                float how_long_onpu = how_long_syousetsu / (command[1].Length / 2);
                Debug.Log("音符の長さ=" + how_long_onpu);
                for (int i = 0; i < command[1].Length; i += 2) {
                    string wav = command[1].Substring(i, 2);
                    if (wav != "00") {
                        int key_frame = Mathf.RoundToInt((syousetsu_no * how_long_syousetsu) + (how_long_onpu * (i / 2)));

                        if (list_music_data[key_no, key_frame] == null) {
                            list_music_data[key_no, key_frame] = wav;
                        }
                        else {
                            list_music_data[key_no, key_frame] += "," + wav;
                        }
                    }
                }

            }

        }
        return list_music_data;
    }

    //数字だけかチェック
    bool isNum(string str_num) {
        return Regex.IsMatch(str_num, "^[0-9]+$");
    }

    //音楽再生本体
    private void playMusic(int frame_no) {
        bool isNull = true;
        for(int i = 0; i < KEY_NUM; i++) {
            if (list_music_data[i, frame_no] != null) {
                //バックグラウンド、musicObjが発生するkeyだった場合の処理
                if ((i == 1) || ((i > 9) && (i < 20))) {
                    processMusicPart(i, frame_no);
                }
                //拍子変更の際
                if (i == 2) {
                    processChangeByoushi(i, frame_no);
                }
                //画像変更の際
                if(i == 4) {
                    string imageName = list_music_data[i, frame_no];
                    changeScreen(screenLeft, imageName);
                    changeScreen(screenRight, imageName);
                }
                isNull = false;
            }
        }

        if (isNull) {
            nullCount++;
        }
        else {
            nullCount = 0;
        }

    }

    private void changeScreen(GameObject obj, string imageName) {
        SpriteRenderer s = obj.GetComponent<SpriteRenderer>();
        s.sprite = dict_image[imageName];
    }

    //途中で何拍子かが変更された時に呼ばれる
    private void processChangeByoushi(int key_no, int frame_no) {
        float changeByoushi = float.Parse(list_music_data[key_no, frame_no]);
        setMusicObjVec(
            4 * changeByoushi,
            FRAME_RATE,
            int.Parse(dict_info["#BPM"])
        );
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
        if(key_no == 1) {
            obj = bgmObj;
        }
        else if (key_no == 16) {
            obj = musicObjRed;
        }
        else if(key_no % 2 == 1) {
            obj = musicObjBlue;
        }
        else {
            obj = musicObjOrange;
        }
        GameObject musicObject = Instantiate(obj) as GameObject;
        musicObject.transform.localScale = new Vector3(
            musicObjSize,
            musicObjSize,
            musicObjSize
        );
        AudioSource audioSource = musicObject.GetComponent<AudioSource>();
        audioSource.clip = dict_audio[wav_name];
        setPosition(musicObject, key_no);
    }

    //MusicObjectの位置を設定
    private void setPosition(GameObject obj, int key_no) {
        float w = MUSIC_WIDTH / (PLAY_KEY_NUM - 1);
        float x = -(MUSIC_WIDTH / 2) + (w * (key_no - 10));
        x -= w;
        Debug.Log("x=" + x);
        obj.transform.position = new Vector3(
            x,
            musicObjY,
            this.transform.position.z
        );
    }

    //MusicObjectが進む速さを求める
    private float setMusicObjVec(float byoushi, int frame, int BPM) {
        float z = this.transform.position.z;
        float how_long_syousetsu = 60 * byoushi * frame / BPM;
        return z / how_long_syousetsu;
    }

    //曲終了処理
    void finish() {
        AudioSource audioSource = this.GetComponent<AudioSource>();
        audioSource.clip = Resources.Load<AudioClip>("src/success");
        audioSource.PlayOneShot(audioSource.clip);
    }

}
