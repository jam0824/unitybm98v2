using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.IO;
using B83.Image.BMP;
using System;
using FileController;
using UnityEngine.Video;
using UnityEngine.UI;

public class BmsConverter : MonoBehaviour
{
    public Dictionary<string, string> dictMovie;
    private MusicPlayManager musicPlayManager;
    private MusicPlayData musicPlayData;
    private int playKeyNum = 5;

    Dictionary<string, string> dicChangeChars;
    Dictionary<string, float> dicBpm;

    private string beforeChar = "#";
    private string afterChar = "xxx";
    //小節の長さのレート保管用
    private float[] listSyousetsuRate;
    private int lastFrameNo;

    public int PlayKeyNum {
        set { this.playKeyNum = value; }
        get { return this.playKeyNum; }
    }

    public int LastFrameNo {
        set { this.lastFrameNo = value; }
        get { return this.lastFrameNo; }
    }


    // Start is called before the first frame update
    void Start()
    {
        musicPlayManager = this.GetComponent<MusicPlayManager>();
        musicPlayData = this.GetComponent<MusicPlayData>();
        dictMovie = new Dictionary<string, string>();
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

    //インフォメーション部分読み込み。現状はwav宣言より上にないと読み込まない
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

    //拡張BPMをdictに格納
    public void readBpm(string[] list_string) {
        dicBpm = new Dictionary<string, float>();
        foreach (string line in list_string) {
            string tmpLine = line.ToLower();
            if (Regex.IsMatch(tmpLine, "^#bpm[0-9a-z]+ ?.+$")) {
                
                string tmp = tmpLine.Replace("#bpm", "");
                int index = tmp.IndexOf(" ");
                string key = tmp.Substring(0, index);
                string value = tmp.Substring(index + 1);
                try {
                    dicBpm.Add(key, float.Parse(value));
                }
                catch (Exception e) {
                    Debug.LogError("拡張BPM" + value + "が不正です");
                    Debug.LogError(e);
                }
            }
        }
    }

    //曲ファイルをdictに格納
    public Dictionary<string, AudioClip> readAudioFiles(string[] list_string, string music_folder) {
        Dictionary<string, AudioClip> dic_audio = new Dictionary<string, AudioClip>();
        string extention = getAudioExtention(music_folder);

        foreach (string line in list_string) {
            //この段階で大文字小文字変換をかけるとファイル名まで変換されてしまうので注意
            if ((line.Contains("#WAV")) || (line.Contains("#wav"))) {
                string tmp = line.Replace("#WAV", "");
                tmp = tmp.Replace("#wav", "");
                int index = tmp.IndexOf(" ");
                string key = tmp.Substring(0, index);
                string value = tmp.Substring(index + 1);
                value = value.Replace("\\", "/");

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

    //www読み込みでうまくいかない文字列の変換処理
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
            //この段階で大文字小文字変換をかけるとファイル名まで変換されてしまうので注意
            if ((line.Contains("#BMP")) || (line.Contains("#bmp"))) {
                string tmp = line.Replace("#BMP", "");
                tmp = tmp.Replace("#bmp", "");
                int index = tmp.IndexOf(" ");
                string key = tmp.Substring(0, index);
                string value = tmp.Substring(index + 1);
                value = value.Replace("\\", "/");
                if (fileController.isFileExist(music_folder + "/" + value)) {
                    //読み込めない動画だった場合は何もしない
                    if ((value.Contains(".mp4")) || 
                        (value.Contains(".mpg")) ||
                        (value.Contains(".avi")) ||
                        (value.Contains(".wmv")) ||
                        (value.Contains(".mpeg"))) {
                        value = exchangeExtention(value);
                        loadMovie(key, music_folder + "/" + value);
                        continue;
                    }
               
                    //画像だった場合で既に登録されているキー以外の場合
                    if(!dict_image.ContainsKey(key))
                        dict_image.Add(key, getSprite(music_folder + "/" + value));
                }
            }

        }
        return dict_image;
    }

    //拡張子をmp4に変換
    private string exchangeExtention(string fileName) {
        string[] listExtention = { ".mpg", ".mpeg", ".avi", ".wmv" };
        foreach (string extention in listExtention) {
            fileName = fileName.Replace(extention, ".mp4");
        }
        return fileName;
    }

    //動画は予めurlを入れてスタンバイさせておく
    private void loadMovie(string key, string url) {
        //dictMovieにkeyとurlは保持しておく
        this.dictMovie.Add(key, url);
        loadMovieMain("MovieRenderImageLeft", url);
        loadMovieMain("MovieRenderImageRight", url);
    }
    private void loadMovieMain(string objName, string url) {
        GameObject obj = GameObject.Find(objName);
        RawImage ri = obj.GetComponent<RawImage>();
        ri.enabled = true;
        VideoPlayer videoPlayer = obj.GetComponent<VideoPlayer>();
        videoPlayer.playOnAwake = false;
        videoPlayer.url = url;
        
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

                //処理しなくていいkey_noのときは戻す
                if (key_no == 2) {
                    continue;
                }
                //20から後ろは全部BGMにしてしまう
                if((key_no >= 20) && (key_no <= 69)) {
                    key_no = 1;
                }

                //キー数の判定もしておく
                judgeKeyNum(key_no);

                //該当小節1つの所要フレーム数を求める
                float how_long_syousetsu = (60.0f * 4.0f * frame / BPM) * this.listSyousetsuRate[syousetsu_no];
                Debug.Log(syousetsu_no + " : 小節の長さ=" + how_long_syousetsu);
                //音符間の所要フレーム数を求める
                float how_long_onpu = how_long_syousetsu / (command[1].Length / 2);

                command[1] = checkDataCount(command[1]);
                //データ部分を2バイトずつ読み込む
                for (int i = 0; i < command[1].Length; i += 2) {
                    
                    string wav = command[1].Substring(i, 2);
                    if (wav != "00") {
                        int key_frame = Mathf.RoundToInt(listSyousetsuFrameCount[syousetsu_no] + (how_long_onpu * (i / 2)));
                        //Debug.Log("key_no:" + key_no + " key_frame:" + key_frame);
                        if (list_music_data[key_no, key_frame] == null) {
                            list_music_data[key_no, key_frame] = wav;
                        }
                        else {
                            list_music_data[key_no, key_frame] += "," + wav;
                        }
                        //トータルノーツ数を数える
                        countTotalNotes(key_no);

                    }
                }

            }
        }

        //続けてBPM変更対応実行
        list_music_data = changeBpm(list_music_data, BPM);
        //最終フレーム取得
        this.LastFrameNo = getLastFrame(list_music_data);
        return list_music_data;
    }

    //楽譜データが2バイトか確認。
    private string checkDataCount(string data) {
        if(data.Length % 2 == 0) {
            return data;
        }
        else {
            Debug.Log("Music Data Length error : " + data);
            int len = data.Length - 1;
            return data.Substring(0, len);
        }


    }

    private void countTotalNotes(int key_no) {
        if ((key_no > 10) && (key_no < musicPlayManager.AUTO_KEY_NO))
            musicPlayData.addTotalNotesNum();
    }

    //プレイキー数取得
    private void judgeKeyNum(int key_no) {
        if((key_no >= 17) && (key_no <= 19)) {
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

    //楽譜を作り終わった後に、次はBPM制御を入れて全体を書き換える
    private string[,] changeBpm(string[,] list_music_data, float BPM) {
        for (int frame_no = 0; frame_no < musicPlayManager.getMaxObjNum(); frame_no++) {
            for (int key_no = 0; key_no < musicPlayManager.getKeyNum(); key_no++) {
                if(list_music_data[key_no, frame_no] != null) {
                    //BPM変更だったら
                    if (key_no == 3) {
                        int changedBpm = Convert.ToInt32(list_music_data[key_no, frame_no], 16);
                        float bpmRate = (float)changedBpm / (float)BPM;
                        Debug.Log("Frame:" + frame_no + " BPM:" + BPM + " chandedBpm:" + changedBpm + " rate:" + bpmRate);
                        list_music_data = changeBpmMain(list_music_data, bpmRate, frame_no);
                        BPM = (float)changedBpm;
                    }
                    //拡張BPMだったら
                    else if(key_no == 8) {
                        string id = list_music_data[key_no, frame_no];
                        float changedBpm = dicBpm[id];
                        float bpmRate = (float)changedBpm / (float)BPM;
                        Debug.Log("Frame:" + frame_no + " BPM:" + BPM + " chandedBpm:" + changedBpm + " rate:" + bpmRate);
                        list_music_data = changeBpmMain(list_music_data, bpmRate, frame_no);
                        BPM = (float)changedBpm;
                    }
                }
            }

        }
        return list_music_data;
    }
    //そのフレーム以降の曲データの格納場所をBpmRate分だけずらす。音符間をBPMの差分倍率分伸び縮みさせる思想。
    private string[,] changeBpmMain(
        string[,] list_music_data, 
        float BpmRate,
        int key_frame) 
    {
        //変更後データを格納する配列
        string[,] list_changed = new string[musicPlayManager.getKeyNum(), musicPlayManager.getMaxObjNum()];
        for (int frame_no = 0; frame_no < musicPlayManager.getMaxObjNum(); frame_no++) {
            for (int key_no = 0; key_no < musicPlayManager.getKeyNum(); key_no++) {
                if (list_music_data[key_no, frame_no] != null) {
                    //キーフレーム以前だったらそのまま代入
                    if (frame_no <= key_frame) {
                        list_changed[key_no, frame_no] = list_music_data[key_no, frame_no];
                    }
                    else {
                        int changedFrame = Mathf.RoundToInt((frame_no - key_frame) / BpmRate) + key_frame;
                        Debug.Log("key_no : " + key_no + " changedFrame : " + changedFrame);
                        list_changed[key_no, changedFrame] = list_music_data[key_no, frame_no];
                    }
                }
            }

        }
        return list_changed;
    }

    //最終フレームを検索する
    public int getLastFrame(string[,] list_music_data) {
        int lastFrameNo = 0;
        for (int frame_no = 0; frame_no < musicPlayManager.getMaxObjNum(); frame_no++) {
            for (int key_no = 0; key_no < musicPlayManager.getKeyNum(); key_no++) {
                if (list_music_data[key_no, frame_no] != null) {
                    lastFrameNo = frame_no;
                    break;
                }
            }

        }
        return lastFrameNo;
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
