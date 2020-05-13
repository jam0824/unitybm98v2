using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityBm98Config;
using UnityBm98Utilities;
using FileController;
using System.IO;

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
    [SerializeField] public GameObject bpmChangeObj;
    [SerializeField] public GameObject movieObj;
    [SerializeField] public float MUSIC_WIDTH; //弾が飛んでくる範囲
    [SerializeField] public float GOOD_LINE; //GOOD判定の距離（絶対値）
    [SerializeField] public float GREAT_LINE; //GREAT判定の距離（絶対値）
    [SerializeField] public int AUTO_KEY_NO; //このキー以降はオート
    [SerializeField] private string music_folder;
    [SerializeField] private string music_bms;
    [SerializeField] public List<AudioClip> listSeClear;
    [SerializeField] public List<AudioClip> listSeFailed;
    [SerializeField] public string SAVE_DATA_FILE;

    public float MUSIC_OBJ_Y;

    private BmsConverter bmsConverter;
    private MusicPlay musicPlay;
    private MusicPlayPower musicPlayPower;
    private MusicPlayData musicPlayData;
    private AnimationManager animationManager;

    private Dictionary<string, string> dictMusicData; //MusicSelectからもらってくる曲データ
    private float musicObjVec; //オブジェクトの速度
    private float movingDistance = 0; //今使ってない。手の移動距離
    private float BPM;
    private int KEY_NUM = 60; //BMSで宣言されているキーの数
    private int MAX_OBJ_NUM = 42000; //配列の最大数(7分)
    private int playKeyNum; //BMSのKEY数（5key or 7key)
    private string hitHandStatus = ""; //直近の弾をどちらの手でたたいたか

    private Dictionary<string, string> dict_info; //BMSの上の情報部分が入る
    private int frameCount = 0; //frame数計算
    private float bpmChangeRate = 1.0f; //BPM変更の際に使用
    private float totalCalorie = 0;
    private bool isUpdate = false;
    private bool isGaming = true;

    private string MUSIC_FOLDER_PATH = "D:/download/game/bm98/music/";
    private int folderCount = 0; //選択した曲番号
    private string category = ""; //引継ぎ用のカテゴリーラベル保持

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
    
    public void setMusicFolder(string music_folder) {
        this.music_folder = music_folder;
    }
    public void setMusicCategoryFolderPath(string folderPath) {
        this.MUSIC_FOLDER_PATH = folderPath;
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
    public string getHitHandStatus() {
        return this.hitHandStatus;
    }
    public void setHitHandStatus(string tag) {
        this.hitHandStatus = tag;
    }
    public void setFolderCount(int folderCount) {
        this.folderCount = folderCount;
    }
    
    

    public int PlayKeyNum {
        set { this.playKeyNum = value; }
        get { return this.playKeyNum; }
    }
    public float TotalCalorie {
        get { return this.totalCalorie; }
        set { this.totalCalorie = value; }
    }
    public string Category {
        set { this.category = value; }
        get { return this.category; }
    }


    void Start() {
        init();
        startGame(this.music_folder, this.music_bms);
    }


    private void init() {
        Application.targetFrameRate = FRAME_RATE;
        bmsConverter = this.GetComponent<BmsConverter>();
        musicPlay = this.GetComponent<MusicPlay>();
        musicPlayPower = this.GetComponent<MusicPlayPower>();
        musicPlayData = this.GetComponent<MusicPlayData>();
        animationManager = GameObject.Find("AnimationManager").GetComponent<AnimationManager>();
        //MUSIC_FOLDER_PATH = config.getFolderPath();
        MUSIC_OBJ_Y = getMusicObjectY();
        setMusicData(dictMusicData);
        frameCount = 0;
        if (dictMusicData != null) {
            this.music_folder = dictMusicData["music_folder"];
            this.music_bms = dictMusicData["music_bms"];
        }
    }
    //ハイスコアなどを呼び出す
    private void setMusicData(Dictionary<string, string> dictMusicData) {
        if ((dictMusicData == null)  || (!dictMusicData.ContainsKey("HighScore"))) return;

        if (dictMusicData["HighScore"] != "")
            musicPlayData.HighScore = int.Parse(dictMusicData["HighScore"]);
    }

    //ヘッドセットからの距離でMusicObjectの高さを決定する
    private float getMusicObjectY() {
        float y = GameObject.Find("CenterEyeAnchor").transform.position.y;
        //return y * MUSIC_OBJ_Y_RATE;
        return 0.1f;
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
        //動画データ読み込み
        musicPlay.setDictMovie(bmsConverter.dictMovie);
        //弾の速さ計算
        musicObjVec = musicPlay.setMusicObjVec(4, FRAME_RATE, BPM);
        //5 or 7 keyかを取得
        this.playKeyNum = bmsConverter.PlayKeyNum;

        //パワーの計算
        Debug.Log("maxnotes : " + musicPlayData.getTotalNotesNum());
        Debug.Log("最終フレーム : " + bmsConverter.LastFrameNo);
        musicPlayPower.calcPower();
        isUpdate = true;
    }

    // Update is called once per frame
    void Update()
    {
        //ゲームが終了していたらAでもトリガーでも曲セレクトに戻る
        if (!isGaming) {
            if ((OVRInput.GetDown(OVRInput.Button.One))||
                (OVRInput.GetDown(OVRInput.Button.Two))||
                (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))) {
                returnMusicSelectScene();
            }
            return;
        }
        if (isUpdate) {
            bool isNull = musicPlay.playMusic(frameCount);

            
            //失敗時の終了判定
            if (musicPlayPower.isFailed) {
                failed();
            }
            
            //成功時の終了判定(曲終了5秒後)
            if (frameCount == bmsConverter.LastFrameNo + (FRAME_RATE * 5)) {
                finish();
            }
            frameCount++;
        }
        //Bボタンが押されたら終了
        if ((OVRInput.GetDown(OVRInput.Button.Two)) ||
            (Input.GetKeyUp(KeyCode.Space))||
            (Input.GetKeyUp(KeyCode.Escape))) {
            isGaming = false;
            returnMusicSelectScene();
        }

        
    }
    //成功時の曲終了処理
    void finish() {
        isUpdate = false;
        isGaming = false;
        AudioSource audioSource = this.GetComponent<AudioSource>();
        //歓声
        audioSource.clip = Resources.Load<AudioClip>("src/success");
        audioSource.PlayOneShot(audioSource.clip);
        //スコア保存
        saveData();
        //成功アニメーションスタート
        animationManager.startFinishAnimation();
    }

    //失敗時の曲終了処理
    private void failed() {
        isUpdate = false;
        isGaming = false;
        AudioSource audioSource = this.GetComponent<AudioSource>();
        //失敗ボイス
        audioSource.PlayOneShot(randomSe(listSeFailed));
        //失敗アニメーションスタート
        animationManager.startFailedAnimation();
        returnMusicSelectScene();
    }

    //musicselectに戻る
    public void returnMusicSelectScene() {
        // シーン切り替え
        this.totalCalorie += GetComponent<MusicPlayData>().getCalorie();
        GameObject.Find("CenterEyeAnchor").GetComponent<OVRScreenFade>().FadeOut();
        StartCoroutine(DelayMethod(2.0f));
    }
    private IEnumerator DelayMethod(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        SceneManager.sceneLoaded += GameSceneLoaded;
        SceneManager.LoadScene("MusicSelectScene");
    }
    private void GameSceneLoaded(Scene next, LoadSceneMode mode) {
        // シーン切り替え後のスクリプトを取得
        MusicSelectManager musicSelectManager = GameObject.Find("MusicSelectManager").GetComponent<MusicSelectManager>();
        musicSelectManager.setTotalCalorie(totalCalorie);
        musicSelectManager.setFolderCount(this.folderCount);
        musicSelectManager.setMusicFolderPath(this.MUSIC_FOLDER_PATH);
        musicSelectManager.setCategory(this.category);
        SceneManager.sceneLoaded -= GameSceneLoaded;
    }

    //ランダムでリストからSEを選択して返す
    public AudioClip randomSe(List<AudioClip> listAudioClip) {
        int num = Random.Range(0, listAudioClip.Count);
        return listAudioClip[num];
    }

    //スコアデータを保存する
    private void saveData() {
        string filePath = UnityBm98Config.config.getSaveDataFolderPath() + SAVE_DATA_FILE;
        List<Dictionary<string, string>> listData = new List<Dictionary<string, string>>();

        if (File.Exists(filePath)) {
            listData = fileController.readCsv(filePath);
            bool isExist = false;
            for (int i = 0; i < listData.Count; i++) {
                if ((listData[i]["music_folder"] == this.music_folder) &&
                    (listData[i]["music_bms"] == this.music_bms)) {
                    if(musicPlayData.HighScore > int.Parse(listData[i]["HighScore"]))
                        listData[i]["HighScore"] = musicPlayData.HighScore.ToString();
                    if(musicPlayData.MaxCombo > int.Parse(listData[i]["MaxCombo"]))
                        listData[i]["MaxCombo"] = musicPlayData.MaxCombo.ToString();
                    if (musicPlayData.getCalorie() > float.Parse(listData[i]["Calorie"]))
                        listData[i]["Calorie"] = musicPlayData.getCalorie().ToString("f1");
                    listData[i]["Rank"] = "A";
                    isExist = true;
                    break;
                }
            }
            if (!isExist) listData.Add(makeData());
        }
        else {
            listData.Add(makeData());
        }
        bool isOk = fileController.writeCsv(filePath, listData);
    }

    //データがないときは作る
    private Dictionary<string, string> makeData() {
        Dictionary<string, string> saveData = new Dictionary<string, string>();
        saveData.Add("music_folder", this.music_folder);
        saveData.Add("music_bms", this.music_bms);
        saveData.Add("HighScore", musicPlayData.HighScore.ToString());
        saveData.Add("MaxCombo", musicPlayData.MaxCombo.ToString());
        saveData.Add("Calorie", musicPlayData.getCalorie().ToString("f1"));
        saveData.Add("Rank", "A");
        return saveData;
    }
}
