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
    Dictionary<string, string> dicChangeChars;

    private string beforeChar = "#";
    private string afterChar = "xxx";
    //小節の長さのレート保管用
    private float[] listSyousetsuRate;


    // Start is called before the first frame update
    void Start()
    {
        musicPlayManager = this.GetComponent<MusicPlayManager>();
        dicChangeChars = getDicChangeChars();
    }

    //andoridで使えない文字列の変換リスト
    private Dictionary<string, string> getDicChangeChars() {
        Dictionary<string, string> dicChangeChars = new Dictionary<string, string>();
        dicChangeChars.Add("#", "xxx");
        dicChangeChars.Add("+", "yyy");
        return dicChangeChars;
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
        string extention = getAudioExtention(music_folder);

        foreach (string line in list_string) {
            if (line.Contains("#WAV")) {
                string tmp = line.Replace("#WAV", "");
                int index = tmp.IndexOf(" ");
                string key = tmp.Substring(0, index);
                string value = tmp.Substring(index + 1);

                //ファイル名に使えない文字列が入っていたら変換する
                value = changeNotUsingChar(value);
                //ファイルに実際に存在するファイルがoggの方だったらbms変換
                if (extention == "ogg") value = value.Replace(".wav", ".ogg");

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

        foreach (string key in this.dicChangeChars.Keys) {
            if (value.Contains(key)) {
                value = value.Replace(key, this.dicChangeChars[key]);
            }
        }
        return value;
    }

    //曲ファイルを外部から読み込む
    private AudioClip getAudioClip(string fileName) {
        Debug.Log("AudioFileName=" + fileName);
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

    //画像ファイルをdictに格納
    public Dictionary<string, Sprite> readImageFiles(string[] list_string, string music_folder) {
        Dictionary<string, Sprite> dict_image = new Dictionary<string, Sprite>();
        foreach (string line in list_string) {
            if (line.Contains("#BMP")) {
                string tmp = line.Replace("#BMP", "");
                string[] command = tmp.Split(' ');
                if (fileController.isFileExist(music_folder + "/" + command[1])) {
                    dict_image.Add(command[0], getSprite(music_folder + "/" + command[1]));
                }
            }

        }
        return dict_image;
    }
    
    //画像ファイルを外部から読み込む
    private Sprite getSprite(string filename) {
        string tmpFileName = filename.ToLower();
        byte[] bytes = LoadBytes(filename);

        Texture2D tex = new Texture2D(0, 0); ;
        if (tmpFileName.Contains(".bmp")) {
            BMPLoader loader = new BMPLoader();
            var bmpImage = loader.LoadBMP(bytes);
            tex = bmpImage.ToTexture2D();
        }
        else {
            tex.LoadImage(bytes);
        }
        

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
    public string[,] makeMusicData(string[] list_lines, float BPM, int frame) {
        string[,] list_music_data = new string[
            musicPlayManager.getKeyNum(), 
            musicPlayManager.getMaxObjNum()
        ];

        //[]の中は小節番号。その小節番号までのフレーム数がfloatで格納されている
        float[] listSyousetsuFrameCount = getSyousetsuFrameCount(list_lines, BPM, frame);

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

                //処理しなくていいkey_noのときは戻す
                if (key_no == 2) {
                    continue;
                }
                //ロングのヤツはBGMにしてしまう
                if((key_no >= 50) && (key_no <= 60)) {
                    key_no = 1;
                }

                //該当小節1つの所要フレーム数を求める
                float how_long_syousetsu = (60.0f * 4.0f * frame / BPM) * this.listSyousetsuRate[syousetsu_no];
                Debug.Log(syousetsu_no + " : 小節の長さ=" + how_long_syousetsu);
                //音符間の所要フレーム数を求める
                float how_long_onpu = how_long_syousetsu / (command[1].Length / 2);
      

                for (int i = 0; i < command[1].Length; i += 2) {
                    string wav = command[1].Substring(i, 2);
                    if (wav != "00") {
                        int key_frame = Mathf.RoundToInt(listSyousetsuFrameCount[syousetsu_no] + (how_long_onpu * (i / 2)));
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

    //プレイキー数取得
    private void judgeKeyNum(int key_no) {
        if((this.playKeyNum == 6)&&(key_no >= 17)&& (key_no <= 19)) {
            this.playKeyNum = 7;
        }
    }

    //小節のフレーム数を返す。[]の中は小節番号
    private float[] getSyousetsuFrameCount(string[] list_lines, float BPM, int FRAME_RATE) {
        float[] listSyousetsuFrameCount = new float[musicPlayManager.getMaxObjNum()];
        float[] listSyousetsuRate = getSyousetsuHowLongRate(list_lines);
        this.listSyousetsuRate = listSyousetsuRate; //全体にも保持。楽譜作成時に再度利用
        //等倍時の小節の長さ
        float howLongSyousetsu = 60.0f * 4.0f * (float)FRAME_RATE / (float)BPM;
        float frameCount = 0;

        for (int i = 0; i < musicPlayManager.getMaxObjNum(); i++) {
            listSyousetsuFrameCount[i] = frameCount;
            frameCount += howLongSyousetsu * listSyousetsuRate[i];
        }
        return listSyousetsuFrameCount;
    }
    //小節の長さの倍率を全体をなめて取得。[]の中は小節番号
    private float[] getSyousetsuHowLongRate(string[] list_lines) {
        float[] listSyousetsuRate = new float[musicPlayManager.getMaxObjNum()];
        //初期か
        for (int i = 0; i < musicPlayManager.getMaxObjNum(); i++) {
            listSyousetsuRate[i] = 1.0f;
        }
        foreach (string line in list_lines) {
            if (line.Contains("#") && line.Contains(":")) {
                string tmp = line.Replace("#", "");
                string[] command = tmp.Split(':');
                if (!isNum(command[0].Substring(0, 3))) continue;
                int syousetsu_no = int.Parse(command[0].Substring(0, 3));
                
                int key_no = int.Parse(command[0].Substring(3, 2));
                if(key_no == 2) {
                    listSyousetsuRate[syousetsu_no] = float.Parse(command[1]);
                    Debug.Log(syousetsu_no + " : Rate = " + command[1]);
                }

            }
        }
        return listSyousetsuRate;
    }


    //数字だけかチェック
    bool isNum(string str_num) {
        return Regex.IsMatch(str_num, "^[0-9]+$");
    }

    //ファイル名に#が入っているとandroidで読み取れなくなるため処理
    public void changeFileName(string fullPath) {
        List<string> files = fileController.getFileList(fullPath);

        foreach (string name in files) {
            foreach (string key in this.dicChangeChars.Keys) {
                if (name.Contains(key)) {
                    Debug.Log("fileName = " + name);
                    bool result = fileController.changeFileName(
                        fullPath + "/" + name, 
                        key, 
                        this.dicChangeChars[key]
                    );
                }
            }
            
        }
    }

    //フォルダに含まれる音楽ファイルがwavかoggか判断して返す
    private string getAudioExtention(string filePath) {
        string extention = "wav";
        List<string> listFile = fileController.getFileList(filePath);
        foreach (string fileName in listFile) {
            if (fileName.Contains(".ogg")) {
                extention = "ogg";
                break;
            }
        }
        return extention;
    }
}
