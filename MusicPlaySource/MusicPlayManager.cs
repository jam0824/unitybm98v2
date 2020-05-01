using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicPlayManager : MonoBehaviour
{
    [SerializeField] public bool isAutoPlay = false;
    [SerializeField] public int FRAME_RATE;
    [SerializeField] public float MUSIC_OBJ_SIZE;
    [SerializeField] public float MUSIC_OBJ_Y_RATE;
    [SerializeField] public float DEC_M_PAR_CALORIE;
    [SerializeField] public GameObject musicObjBlue;
    [SerializeField] public GameObject musicObjRed;
    [SerializeField] public GameObject musicObjOrange;
    [SerializeField] public GameObject bgmObj;
    [SerializeField] public float MUSIC_WIDTH; //弾が飛んでくる範囲
    [SerializeField] public float GOOD_LINE; //GOOD判定の距離（絶対値）
    [SerializeField] public float GREAT_LINE; //GREAT判定の距離（絶対値）
    [SerializeField] public int AUTO_KEY_NO; //このキー以降はオート
    [SerializeField] private string music_folder;
    [SerializeField] private string music_bms;

    private float MUSIC_OBJ_Y;
    private Dictionary<string, string> dictMusicData; //MusicSelectからもらってくる曲データ
    private float musicObjVec;
    private float movingDistance = 0;
    private float BPM;
    private int KEY_NUM = 30;
    private int MAX_OBJ_NUM = 100000;
    private bool isUpdate = false;
    private Dictionary<string, string> dict_info;
    private int frame_num = 0;
    private int nullCount = 0;
    private BmsConverter bmsConverter;
    private MusicPlay musicPlay;
    private int playKeyNum; //BMSのKEY数（5key or 7key)
    private float bpmChangeRate = 1.0f; //BPM変更の際に使用

    private string MUSIC_FOLDER_PATH = "D:/download/game/bm98/music/";

    //別のシーンから呼ばれた時に入れる
    public void setDictMusicData(Dictionary<string, string> dictMusicData) {
        this.dictMusicData = dictMusicData;
    }

    public float getMusicObjY() {
        return this.MUSIC_OBJ_Y;
    }
    public float getBpm() {
        return this.BPM;
    }
    public int getKeyNum() {
        return this.KEY_NUM;
    }
    public int getMaxObjNum() {
        return this.MAX_OBJ_NUM;
    }
    public float getMusicObjVec() {
        return this.musicObjVec * this.bpmChangeRate;
    }
    public float getMovingDistance() {
        return this.movingDistance;
    }
    public int getPlayKeyNum() {
        return this.playKeyNum;
    }
    public void setMusicFolder(string music_folder) {
        this.music_folder = music_folder;
    }
    public void setMusic_bms(string music_bms) {
        this.music_bms = music_bms;
    }
    public void setBpmChageRate(float rate) {
        this.bpmChangeRate = rate;
    }
    public float getBpmChageRate() {
        return this.bpmChangeRate;
    }
    public void addMovingDistance(float dist) {
        this.movingDistance += dist;
    }
    

    void Start() {
        init();
        startGame(this.music_folder, this.music_bms);
    }


    private void init() {
        Application.targetFrameRate = FRAME_RATE;
        bmsConverter = this.GetComponent<BmsConverter>();
        musicPlay = this.GetComponent<MusicPlay>();
        MUSIC_FOLDER_PATH = getFolderPath();
        MUSIC_OBJ_Y = getMusicObjectY();
        frame_num = 0;
        nullCount = 0;
        if (dictMusicData != null) {
            this.music_folder = dictMusicData["music_folder"];
            this.music_bms = dictMusicData["music_bms"];
        }
    }

    //テスト時とoculus時でパスを自動で変更
    private string getFolderPath() {
        if (Application.platform == RuntimePlatform.WindowsEditor) {
            return "D:/download/game/bm98/music/";
        }
        else {
            return "/storage/emulated/0/unitybm98/";
        }
    }

    //ヘッドセットからの距離でMusicObjectの高さを決定する
    private float getMusicObjectY() {
        float y = GameObject.Find("CenterEyeAnchor").transform.position.y;
        return y * MUSIC_OBJ_Y_RATE;
    }

    void startGame(string musicFolder, string musicBms) {
        string[] lines = bmsConverter.read(MUSIC_FOLDER_PATH + musicFolder + "/" + musicBms);

        //インフォメーション部分読み込み
        dict_info = bmsConverter.getInfomation(lines);
        BPM = float.Parse(dict_info["#BPM"]);
        //曲データを作成
        musicPlay.setListMusicData(
            bmsConverter.makeMusicData(
                lines,
                BPM,
                FRAME_RATE
            )
        );
        //androidで使えないファイル名がある場合の変換処理
        bmsConverter.changeFileName(MUSIC_FOLDER_PATH + musicFolder);
        //音データ読み込み
        musicPlay.setDictAudio(
            bmsConverter.readAudioFiles(lines, MUSIC_FOLDER_PATH + musicFolder)
        );
        //画像データ読み込み
        musicPlay.setDictImage(
            bmsConverter.readImageFiles(lines, MUSIC_FOLDER_PATH + musicFolder)
        );
        //弾の速さ計算
        musicObjVec = musicPlay.setMusicObjVec(4, FRAME_RATE, BPM);
        //5 or 7 keyかを取得
        playKeyNum = bmsConverter.playKeyNum;
        isUpdate = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isUpdate) {
            bool isNull = musicPlay.playMusic(frame_num);
            if (isNull) {
                nullCount++;
            }
            else {
                nullCount = 0;
            }
            
            if (nullCount == FRAME_RATE * 5) {
                isUpdate = false;
                finish();
            }
            frame_num++;
        }
        //Bボタンが押されたら曲選択に戻る
        if (OVRInput.GetDown(OVRInput.Button.Two)) {
            // シーン切り替え
            SceneManager.LoadScene("MusicSelectScene");
        }

        
    }

    //曲終了処理
    void finish() {
        AudioSource audioSource = this.GetComponent<AudioSource>();
        audioSource.clip = Resources.Load<AudioClip>("src/success");
        audioSource.PlayOneShot(audioSource.clip);
    }

}
