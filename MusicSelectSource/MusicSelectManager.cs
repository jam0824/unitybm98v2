using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FileController;
using UnityBm98Config;
using System.IO;
using UnityBm98Utilities;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;

public class MusicSelectManager : MonoBehaviour
{
    public GameObject RECORD_OBJECT;
    public GameObject ZERO_OBJECT;
    public int folderCount = 0;
    public int localRecordCount = -1; //現在表示されているレコードオブジェクトの番号。サークルを出すときに使う。
    public List<Dictionary<string, string>> listMusicDict;
    public bool isReady = false;

    public AudioClip SE_START;
    public AudioClip SE_DECIDE;
    public AudioClip SE_MOVE;

    private BmsInformationLoader bmsInformationLoader;
    private MusicSelect musicSelect;
    private MusicSelectSaveFileLoader musicSelectSaveFileLoader;

    private List<Dictionary<string, string>> listSaveData;
    private List<GameObject> listRecordObject;
    private string MUSIC_FOLDER_PATH;

    private int SHOW_RECOED_NUM = 30;
    private int[] listShowRecordFolderCount;
    private int showRecordMidNum = 0;  //レコード表示の際に中央となる曲番号(folderCount)
    private float recordDist = 0.5f;
    private float MAX_SHOE_RECORD_X;
    private float MIN_SHOE_RECORD_X;

    private float recordY = 1.0f;
    private float recordZ = 1.0f;
    private float selectCircleAng = 0;

    private float totalCalorie = 0;
    private int oldFolderCount = -1;

    private string category = "None";

    public Dictionary<string, string> getDictMusicData() {
        return listMusicDict[folderCount];
    }

    public void setTotalCalorie(float calorie) {
        this.totalCalorie = calorie;
    }

    public void setFolderCount(int folderCount) {
        this.folderCount = folderCount;
        this.showRecordMidNum = folderCount;
    }

    public void setMusicFolderPath(string folderPath) {
        this.MUSIC_FOLDER_PATH = folderPath;
    }

    public void setCategory(string category) {
        this.category = category;
    }

    void Start()
    {
        bool isOk = false;
        if (MUSIC_FOLDER_PATH == null)
            MUSIC_FOLDER_PATH = config.getFolderPath();
        musicSelect = this.GetComponent<MusicSelect>();
        musicSelectSaveFileLoader = this.GetComponent<MusicSelectSaveFileLoader>();
        //全描画
        isOk = allRedraw(MUSIC_FOLDER_PATH);

        //カテゴリーの描画
        redrawCategoryLabel(this.category);

        //スタートフラグtrue
        isReady = isOk;

        //画面表示前にSEが表示されてしまうので、コールチンで遅らせて再生
        StartCoroutine(startSeDelayMethod(2.0f, SE_START));

        Bm98Debug.Instance.Log(MUSIC_FOLDER_PATH);
    }

    //持ってる曲がない、フォルダがないときは説明用オブジェクトを表示
    private void makeZeroObject() {
        GameObject obj = Instantiate(ZERO_OBJECT) as GameObject;
    }

    //各種メンバー変数初期化
    private void valueInit() {
        folderCount = 0;
        localRecordCount = -1;
        showRecordMidNum = 0;
        oldFolderCount = -1;
        SHOW_RECOED_NUM = 30;
        listShowRecordFolderCount = new int[SHOW_RECOED_NUM];
    }

    public bool allRedraw(string folderPath) {
        bool isOk = false;
        

        //もしレコードオブジェクトがあるなら全部消す
        deleteAllRecords();
        valueInit();

        //フォルダがなかったら処理終わり
        if (!fileController.isFolderExist(folderPath)) {
            makeZeroObject();
            return isOk;
        }
        //レコード作成。レコードがあればtrue
        isOk = recordInit(folderPath);
        if (!isOk) makeZeroObject();

        //トータルカロリー表示
        showTotalCalorie(this.totalCalorie);
        return isOk;
    }

    //セーブデータ、bmsデータを読み込んでレコードオブジェクト作成まで
    bool recordInit(string folderPath) {
        //セーブファイルをロード
        listSaveData = musicSelectSaveFileLoader.loadSaveData();
        //BMSのリストを作る
        listMusicDict = this.GetComponent<BmsInformationLoader>().getListMusicDict(folderPath);

        if(listMusicDict.Count == 0) {
            return false;
        }
        if (SHOW_RECOED_NUM > listMusicDict.Count) SHOW_RECOED_NUM = listMusicDict.Count;

        //BMSのリストにセーブデータを載せる
        listMusicDict = musicSelectSaveFileLoader.appendDataToRecords(listMusicDict, listSaveData);
        //リスト作成
        listShowRecordFolderCount = makeListShowRecord(
            showRecordMidNum,
            listMusicDict.Count
        );
        //レコードオブジェクトを作成
        listRecordObject = makeAllRecords(
            listMusicDict,
            listShowRecordFolderCount,
            new Vector3(0, recordY, recordZ)
        );
        return true;
    }

    


    //各種入力チェック
    void Update()
    {
        if (isReady) {
            Vector2 stickL = OVRInput.Get(OVRInput.RawAxis2D.LThumbstick);
            Vector2 stickR = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick);

            //選曲が変わった際の処理
            if(oldFolderCount != folderCount) {
                showScore();
                oldFolderCount = folderCount;
            }
            //曲の決定
            if ((Input.GetKeyUp(KeyCode.Space)) ||
                (OVRInput.GetUp(OVRInput.Button.One, OVRInput.Controller.RTouch))) {
                musicSelect.showInfomation(listMusicDict[folderCount]);
                selectedMusic();
            }

            //曲の移動
            if ((Input.GetKeyUp(KeyCode.RightArrow)) ||
                (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstickRight))) {
                musicSelect.nextMusic();
                musicSelect.showInfomation(listMusicDict[folderCount]);
            }
            if ((Input.GetKeyUp(KeyCode.LeftArrow)) ||
                (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstickLeft))) {
                musicSelect.prevMusic();
                musicSelect.showInfomation(listMusicDict[folderCount]);
            }
            
            if (Input.GetKey(KeyCode.RightArrow)) {
                moveRecords(0.2f, MIN_SHOE_RECORD_X, MAX_SHOE_RECORD_X);
            }
            if (Input.GetKey(KeyCode.LeftArrow)) {
                moveRecords(-0.2f, MIN_SHOE_RECORD_X, MAX_SHOE_RECORD_X);
            }
            //スティックを倒した瞬間だけ音を鳴らす
            if ((OVRInput.GetDown(OVRInput.RawButton.RThumbstickLeft)) ||
                (OVRInput.GetDown(OVRInput.RawButton.RThumbstickRight))) {
                playSe(SE_MOVE);
            }
            if (stickR.x != 0) {
                moveRecords(stickR.x, MIN_SHOE_RECORD_X, MAX_SHOE_RECORD_X);
            }
            musicSelect.showInfomation(listMusicDict[folderCount]);
            moveSelectCircle();
        }

    }

    //選択サークルを移動させる
    private void moveSelectCircle() {
        if (localRecordCount == -1) return;
        Transform t = this.listRecordObject[localRecordCount].GetComponent<Transform>();
        Vector3 pos = t.transform.position;
        Vector3 ang = t.transform.localEulerAngles;
        moveSelectMsg(pos, ang);
        moveScoreMsg(pos, ang);
        selectCircleAng += 0.2f;
        if (selectCircleAng > 360.0f) selectCircleAng = 0;
        ang.z = selectCircleAng;
        GameObject.Find("SelectCircle").GetComponent<Transform>().transform.position = pos;
        GameObject.Find("SelectCircle").GetComponent<Transform>().transform.localEulerAngles = ang;
    }
    //選択時のAボタン押してのウィンドウの移動
    private void moveSelectMsg(Vector3 pos, Vector3 ang) {
        GameObject.Find("PressButton").GetComponent<Transform>().transform.position = pos;
        GameObject.Find("PressButton").GetComponent<Transform>().transform.localEulerAngles = ang;
    }
    //スコア表示のウィンドウの移動
    private void moveScoreMsg(Vector3 pos, Vector3 ang) {
        GameObject.Find("SelectScore").GetComponent<Transform>().transform.position = pos;
        GameObject.Find("SelectScore").GetComponent<Transform>().transform.localEulerAngles = ang;
    }

    //スコアの表示
    private void showScore() {
        GameObject.Find("HighScoreText").GetComponent<Text>().text = 
            (listMusicDict[folderCount]["HighScore"] != "")? 
            "High Score : " + (int.Parse(listMusicDict[folderCount]["HighScore"])).ToString("N0") :
            "High Score : ";

        GameObject.Find("MaxComboText").GetComponent<Text>().text =
            (listMusicDict[folderCount]["MaxCombo"] != "") ?
            "Max Combo : " + (int.Parse(listMusicDict[folderCount]["MaxCombo"])).ToString("N0") :
            "Max Combo : ";
        GameObject.Find("MaxCalorieText").GetComponent<Text>().text =
            (listMusicDict[folderCount]["Calorie"] != "") ?
            "Calorie : " + listMusicDict[folderCount]["Calorie"] + "kcal" :
            "Calorie : ";
    }

    //レコードの移動
    private void moveRecords(float velocity, float MIN_X, float MAX_X) {
        foreach (GameObject record in this.listRecordObject) {
            Vector3 pos = record.GetComponent<Transform>().transform.position;
            if (pos.z != this.recordZ) continue;

            pos.x += velocity / 5;
            if (pos.x > MAX_X) {
                pos.x = MIN_X + 0.1f;
                showRecordMidNum--;
                if (showRecordMidNum < 0) showRecordMidNum = listMusicDict.Count - 1;
                changeRecordData(record, 0);


            }
            else if (pos.x < MIN_X) {
                pos.x = MAX_X - 0.1f;
                showRecordMidNum++;
                if (showRecordMidNum == listMusicDict.Count) showRecordMidNum = 0;
                changeRecordData(record, SHOW_RECOED_NUM - 1);
            }

            record.GetComponent<Transform>().transform.position = pos;
        }
    }

    //レコードが端まで行って、逆の端に移動するときの処理
    private void changeRecordData(GameObject record, int listCount) {
        listShowRecordFolderCount = makeListShowRecord(showRecordMidNum, listMusicDict.Count);
        int folderCount = listShowRecordFolderCount[listCount];
        RecordObject rd = record.GetComponent<RecordObject>();
        Debug.Log("midNum : " + showRecordMidNum + " show_recoed_num :" + SHOW_RECOED_NUM);
        rd.setDictMusicData(listMusicDict[folderCount]);
        rd.showInfomation();
    }

    //曲の決定
    public void selectedMusic() {
        if (isReady) {
            isReady = false;
            playSe(SE_DECIDE);
            GameObject.Find("CenterEyeAnchor").GetComponent<OVRScreenFade>().FadeOut();
            StartCoroutine(DelayMethod(2.0f));
        }
    }
    private IEnumerator DelayMethod(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        startChangeScene();
    }
    //シーン切り替え
    private void startChangeScene() {
        // イベントに登録
        SceneManager.sceneLoaded += GameSceneLoaded;
        // シーン切り替え
        SceneManager.LoadScene("MusicPlayScene");
    }
    private void GameSceneLoaded(Scene next, LoadSceneMode mode) {
        // シーン切り替え後のスクリプトを取得
        MusicPlayManager musicPlayManager = GameObject.Find("MusicPlayManager").GetComponent<MusicPlayManager>();
        musicPlayManager.setMusicCategoryFolderPath(this.MUSIC_FOLDER_PATH);
        musicPlayManager.setDictMusicData(listMusicDict[folderCount]);
        musicPlayManager.TotalCalorie = this.totalCalorie;
        musicPlayManager.setFolderCount(this.folderCount);
        musicPlayManager.Category = this.category;
        SceneManager.sceneLoaded -= GameSceneLoaded;
    }

    //見せるレコードを全て描画
    private List<GameObject> makeAllRecords(
        List<Dictionary<string, string>> listMusicDict, 
        int[] listShowRecordFolderCount,
        Vector3 pos) 
    {
        List<GameObject> listRecordObject = new List<GameObject>();
        Vector3 oldPos = pos;
        MAX_SHOE_RECORD_X = SHOW_RECOED_NUM * recordDist / 2;
        MIN_SHOE_RECORD_X = -MAX_SHOE_RECORD_X;

        pos.x = pos.x - (SHOW_RECOED_NUM * recordDist /2);

        for (int i = 0; i < listShowRecordFolderCount.Length; i++) {
            int folderCount = listShowRecordFolderCount[i];
            pos.y = (i % 2 == 1) ? oldPos.y + recordDist : oldPos.y;
            pos.x += recordDist;
            GameObject obj = makeRecord(listMusicDict[folderCount], pos, i);
            listRecordObject.Add(obj);
        }
        return listRecordObject;
    }

    //レコードの作成
    private GameObject makeRecord(Dictionary<string, string> dictMusicData, Vector3 pos, int localRecordCount) {
        GameObject obj = Instantiate(RECORD_OBJECT) as GameObject;
        RecordObject rd = obj.GetComponent<RecordObject>();
        rd.setDictMusicData(dictMusicData);
        rd.setLocalRecordCount(localRecordCount);
        rd.showInfomation();
        obj.transform.position = pos;
        return obj;
    }

    //レコードを全部消す（カテゴリー切り替え用)
    private void deleteAllRecords() {
        if ((this.listRecordObject == null) || (this.listRecordObject.Count == 0)) return;
        foreach (GameObject record in this.listRecordObject) {
            Destroy(record.gameObject);
        }

    }

    //SE再生
    private void playSe(AudioClip audioClip) {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.volume = 1.0f;
        audioSource.PlayOneShot(audioClip);
    }
    private IEnumerator startSeDelayMethod(float waitTime, AudioClip audioClip) {
        yield return new WaitForSeconds(waitTime);
        playSe(audioClip);
    }

    private void showTotalCalorie(float totalCalorie) {
        GameObject.Find("TotalCalorieText").GetComponent<Text>().text = totalCalorie.ToString("f1") + " kcal";
    }

    //表示する曲番号の配列を作る。中央値、曲ナンバーの最大値
    //考え方。まずは表示用に30個のリストがある。
    //そこの真ん中の数字（全体の曲番号で指定）を基準にindex0から曲番号を入れていく。
    //もし曲番号が0未満だったら曲番号の最後の方に戻る。曲番号がマックスだったらゼロから入れる
    private int[] makeListShowRecord(int midNum, int maxFolderCount) {
        int[] listShowRecordFolderCount = new int[SHOW_RECOED_NUM];
        int index = midNum - SHOW_RECOED_NUM / 2;
        //Debug.Log("******************************" + midNum +"********************************");
        //Debug.Log("index = " + index);
        for (int i = 0; i < SHOW_RECOED_NUM; i++) {
            int folderCount = index + i;
            if (folderCount < 0) {
                //配列のおしりの方から取ってくる
                folderCount += maxFolderCount;
            }
            else if (folderCount >= maxFolderCount) {
                //配列のゼロのほうから取ってくる
                folderCount -= maxFolderCount;
            }
            //Debug.Log("i=" + i + " folderCount=" + folderCount);
            listShowRecordFolderCount[i] = folderCount;
        }
        return listShowRecordFolderCount;
    }


    //ジャンル変更後の再読み込み
    public void returnMusicSelectScene() {
        // シーン切り替え
        GameObject.Find("CenterEyeAnchor").GetComponent<OVRScreenFade>().FadeOut();
        StartCoroutine(reloadDelayMethod(2.0f));
    }
    private void GameSceneReloaded(Scene next, LoadSceneMode mode) {
        // シーン切り替え後のスクリプトを取得
        MusicSelectManager musicSelectManager = GameObject.Find("MusicSelectManager").GetComponent<MusicSelectManager>();
        musicSelectManager.setTotalCalorie(this.totalCalorie);
        musicSelectManager.setFolderCount(this.folderCount);
        musicSelectManager.setMusicFolderPath(this.MUSIC_FOLDER_PATH);
        musicSelectManager.setCategory(this.category);
        SceneManager.sceneLoaded -= GameSceneReloaded;
    }
    private IEnumerator reloadDelayMethod(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        SceneManager.sceneLoaded += GameSceneReloaded;
        SceneManager.LoadScene("MusicSelectScene");
    }

    private void redrawCategoryLabel(string category) {
        GameObject.Find("CategoryLabelText").GetComponent<Text>().text = "Category : " + category;
    }


}
