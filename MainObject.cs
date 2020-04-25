using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO; //System.IO.FileInfo, System.IO.StreamReader, System.IO.StreamWriter
using System.Text.RegularExpressions;

public class MainObject : MonoBehaviour
{
    public bool isAutoPlay = true;
    public GameObject musicObj;
    public GameObject bgmObj;
    public int FRAME_RATE;
    public string music_folder;
    public string music_bms;
    public float MUSIC_WIDTH;
    public int PLAY_KEY_NUM;


    public float musicObjVec;
    private int KEY_NUM = 30;
    private int MAX_OBJ_NUM = 100000;
    private bool isUpdate = false;
    private Dictionary<string, AudioClip> dic_audio;
    private Dictionary<string, string> dict_info;
    private string[,] list_music_data;
    private int frame_num = 0;
    

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = FRAME_RATE;
        list_music_data = new string[KEY_NUM, MAX_OBJ_NUM];
        string[] lines = read(music_folder + "/" + music_bms);
        dict_info = getInfomation(lines);
        dic_audio = new Dictionary<string, AudioClip>();
        dic_audio = readAudioFiles(lines);
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
    }

    string[] read(string filePath) {
        Debug.Log(filePath);
        TextAsset text = Resources.Load<TextAsset>(filePath);
        string allText = text.text;
        allText = allText.Replace("\r", "");
        return allText.Split('\n');
    }

    Dictionary<string, string> getInfomation(string[] list_string) {
        Dictionary<string, string> dict_info = new Dictionary<string, string>();
        foreach (string line in list_string) {
            if (line.Contains("#WAV")) break;
            int index = line.IndexOf(" ");
            if (index == -1) continue;
            if (line.Substring(0, 1) == "*") continue;
            string key = line.Substring(0, index);
            string value = line.Substring(index + 1);
            dict_info.Add(key, value);
            //Debug.Log("key=" + key + " value=" + value);
        }
        return dict_info;
    }

    Dictionary<string, AudioClip> readAudioFiles(string[] list_string) {
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

    string[,] makeMusicData(string[] list_lines, int BPM, int frame) {
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

    bool isNum(string str_num) {
        return Regex.IsMatch(str_num, "^[0-9]+$");
    }

    //音楽再生本体
    void playMusic(int frame_no) {
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
            }
           
        }

    }

    void processChangeByoushi(int key_no, int frame_no) {
        float changeByoushi = float.Parse(list_music_data[key_no, frame_no]);
        setMusicObjVec(
            4 * changeByoushi,
            FRAME_RATE,
            int.Parse(dict_info["#BPM"])
        );
    }

   
    void processMusicPart(int key_no, int frame_no) {
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
    void makeMusicObject(int key_no, int frame_no, string wav_name) {
        GameObject obj = (key_no == 1) ? bgmObj : musicObj;
        GameObject musicObject = Instantiate(obj) as GameObject;
        AudioSource audioSource = musicObject.GetComponent<AudioSource>();
        audioSource.clip = dic_audio[wav_name];
        setPosition(musicObject, key_no);
    }

    //位置を設定
    void setPosition(GameObject obj, int key_no) {
        float x = -(MUSIC_WIDTH / 2) + ((MUSIC_WIDTH / (PLAY_KEY_NUM - 1)) * (key_no - 10));
        Debug.Log("x=" + x);
        obj.transform.position = new Vector3(
            x,
            0.0f,
            this.transform.position.z
        );
    }

    //MusicObjectが進む速さを求める
    float setMusicObjVec(float byoushi, int frame, int BPM) {
        float z = this.transform.position.z;
        float how_long_syousetsu = 60 * byoushi * frame / BPM;
        return z / how_long_syousetsu;
    }

}
