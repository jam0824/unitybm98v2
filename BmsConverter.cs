using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class BmsConverter : MonoBehaviour
{
    private MusicPlayManager musicPlayManager;


    // Start is called before the first frame update
    void Start()
    {
        musicPlayManager = this.GetComponent<MusicPlayManager>();
        
    }

    //bmsファイル読み込み
    public string[] read(string filePath) {
        Debug.Log(filePath);
        TextAsset text = Resources.Load<TextAsset>(filePath);
        string allText = text.text;
        allText = allText.Replace("\r", "");
        return allText.Split('\n');
    }

    //インフォメーション部分読み込み
    public Dictionary<string, string> getInfomation(string[] list_string) {
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
    public Dictionary<string, AudioClip> readAudioFiles(string[] list_string, string music_folder) {
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
    public Dictionary<string, Sprite> readImageFiles(string[] list_string, string music_folder) {
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
    public string[,] makeMusicData(string[] list_lines, int BPM, int frame) {
        string[,] list_music_data = new string[
            musicPlayManager.getKeyNum(), 
            musicPlayManager.getMaxObjNum()
        ];
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
}
