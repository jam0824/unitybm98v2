using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.IO;
using B83.Image.BMP;
using System;
using FileController;

public class BmsConverter : MonoBehaviour
{
    private MusicPlayManager musicPlayManager;
    public int playKeyNum = 5;
    private string beforeChar = "#";
    private string afterChar = "xxx";


    // Start is called before the first frame update
    void Start()
    {
        musicPlayManager = this.GetComponent<MusicPlayManager>();
        
    }

    //bmsファイル読み込み
    public string[] read(string filePath) {
        Debug.Log(filePath);
        return fileController.fileConvertUTF8(filePath);
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
                int index = tmp.IndexOf(" ");
                string key = tmp.Substring(0, index);
                string value = tmp.Substring(index + 1);
                //ファイル名に使えない文字列が入っていたら変換する
                value = changeNotUsingChar(value);

                try {
                    dic_audio.Add(key, getAudioClip(music_folder + "/" + value));
                }
                catch (Exception e) {
                    Debug.LogError("ファイル名：" + value + "が不正です");
                    Debug.LogError(e);
                }
                
            }

        }
        return dic_audio;
    }

    private string changeNotUsingChar(string value) {
        if (value.Contains(beforeChar)) {
            value = value.Replace(beforeChar, afterChar);
        }
        return value;
    }

    //曲ファイルを外部から読み込む
    private AudioClip getAudioClip(string fileName) {
        fileName = urlEncode(fileName);
        try {
            using (WWW www = new WWW("file:///" + fileName)) {
                while (!www.isDone) {
                }
                return www.GetAudioClip(false, false);
            }
        }
        catch(Exception e) {
            Debug.LogError(e);
            return null;
        }
    }

    private string urlEncode(string fileName) {
        fileName = fileName.Replace("#", "%23");
        fileName = fileName.Replace(" ", "%20");
        return fileName;
    }

    //画像ファイルをdictに格納
    public Dictionary<string, Sprite> readImageFiles(string[] list_string, string music_folder) {
        Dictionary<string, Sprite> dict_image = new Dictionary<string, Sprite>();
        foreach (string line in list_string) {
            if (line.Contains("#BMP")) {
                string tmp = line.Replace("#BMP", "");
                string[] command = tmp.Split(' ');
                dict_image.Add(command[0], getSprite(music_folder + "/" + command[1]));
            }

        }
        return dict_image;
    }
    
    //画像ファイルを外部から読み込む
    private Sprite getSprite(string filename) {
        byte[] bytes = LoadBytes(filename);
        BMPLoader loader = new BMPLoader();
        var bmpImage = loader.LoadBMP(bytes);
        Texture2D tex = bmpImage.ToTexture2D();
        Sprite s = Sprite.Create(
                        tex,
                        new Rect(0f, 0f, tex.width, tex.height),
                        new Vector2(0.5f, 0.5f)
        );
        return s;
    }

    //バイトで読み込む
    private byte[] LoadBytes(string path) {
        FileStream fs = new FileStream(path, FileMode.Open);
        BinaryReader bin = new BinaryReader(fs);
        byte[] result = bin.ReadBytes((int)bin.BaseStream.Length);
        bin.Close();
        return result;
    }

    //楽譜作成
    public string[,] makeMusicData(string[] list_lines, int BPM, int frame) {
        string[,] list_music_data = new string[
            musicPlayManager.getKeyNum(), 
            musicPlayManager.getMaxObjNum()
        ];
        float byoushi = 4;
        int oldSyousetsuNo = 0;
        foreach (string line in list_lines) {
            if (line.Contains("#") && line.Contains(":")) {
 
                string tmp = line.Replace("#", "");
                string[] command = tmp.Split(':');
                //取得したコマンドがNumでなければやり直し
                if (!isNum(command[0].Substring(0, 3))) continue;

                int syousetsu_no = int.Parse(command[0].Substring(0, 3));
                int key_no = int.Parse(command[0].Substring(3, 2));

                //キー数の判定もしておく
                judgeKeyNum(key_no);

                //"02"拍子変更の際の処理
                if (key_no == 2) {
                    byoushi = 4 * float.Parse(command[1]);
                    oldSyousetsuNo = syousetsu_no;
                    continue;
                }
                if (oldSyousetsuNo != syousetsu_no) byoushi = 4;

                //小節の所要フレーム数を求める
                float how_long_syousetsu = 60 * byoushi * frame / BPM;
                Debug.Log("小節の長さ=" + how_long_syousetsu);

                //音符間の所要フレーム数を求める
                float how_long_onpu = how_long_syousetsu / (command[1].Length / 2);
                Debug.Log("音符の長さ=" + how_long_onpu);
                Debug.Log("line = " + line);

                for (int i = 0; i < command[1].Length; i += 2) {
                    string wav = command[1].Substring(i, 2);
                    if (wav != "00") {

                        int key_frame = Mathf.RoundToInt((syousetsu_no * how_long_syousetsu) + (how_long_onpu * (i / 2)));
                        if (key_no == 2) {
                            list_music_data[key_no, key_frame] = command[1];
                            break;
                        }
                        else if (list_music_data[key_no, key_frame] == null) {
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


    private void judgeKeyNum(int key_no) {
        if((this.playKeyNum == 6)&&(key_no >= 17)&& (key_no <= 19)) {
            this.playKeyNum = 7;
        }
    }

    //数字だけかチェック
    bool isNum(string str_num) {
        return Regex.IsMatch(str_num, "^[0-9]+$");
    }

    //ファイル名に#が入っているとandroidで読み取れなくなるため処理
    public void changeFileName(string fullPath) {
        List<string> files = fileController.getFileList(fullPath);
        foreach (string name in files) {
            if (name.Contains(beforeChar)) {
                Debug.Log("fileName = " + name);
                bool result = fileController.changeFileName(fullPath + "/" + name, beforeChar, afterChar);
               
            }
        }
    }
}
