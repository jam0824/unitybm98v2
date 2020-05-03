using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FileController;
using UnityBm98Config;
using System.IO;
using UnityBm98Utilities;
using UnityEngine.SceneManagement;
using System;

public class MusicSelectManager : MonoBehaviour
{
    public GameObject RECORD_OBJECT;
    public int folderCount = 0;
    public List<Dictionary<string, string>> listMusicDict;
    public bool isReady = false;

    public AudioClip SE_START;
    public AudioClip SE_DECIDE;
    public AudioClip SE_MOVE;

    private string MUSIC_FOLDER_PATH;
    private BmsInformationLoader bmsInformationLoader;
    private MusicSelect musicSelect;
    private List<GameObject> listRecordObject;

    private float recordY = 1.0f;
    private float recordZ = 1.0f;
    private float selectCircleAng = 0;

    public Dictionary<string, string> getDictMusicData() {
        return listMusicDict[folderCount];
    }

    void Start()
    {
        MUSIC_FOLDER_PATH = config.getFolderPath();
        musicSelect = this.GetComponent<MusicSelect>();
        listMusicDict = this.GetComponent<BmsInformationLoader>().getListMusicDict(MUSIC_FOLDER_PATH);
        listRecordObject = makeAllRecords(listMusicDict, new Vector3(0,recordY, recordZ));
        isReady = true;
        StartCoroutine(startSeDelayMethod(2.0f, SE_START));
    }


    // Update is called once per frame
    void Update()
    {
        if (isReady) {
            Vector2 stickL = OVRInput.Get(OVRInput.RawAxis2D.LThumbstick);
            Vector2 stickR = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick);

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
            if ((Input.GetKeyUp(KeyCode.Space)) ||
                (OVRInput.GetUp(OVRInput.Button.One, OVRInput.Controller.RTouch))) {
                musicSelect.showInfomation(listMusicDict[folderCount]);
                selectedMusic();
            }
            if (Input.GetKey(KeyCode.RightArrow)) {
                moveRecords(0.2f);
            }
            if (Input.GetKey(KeyCode.LeftArrow)) {
                moveRecords(-0.2f);
            }
            //スティックを倒した瞬間だけ音を鳴らす
            if ((OVRInput.GetDown(OVRInput.RawButton.RThumbstickLeft)) ||
                (OVRInput.GetDown(OVRInput.RawButton.RThumbstickRight))) {
                playSe(SE_MOVE);
            }
            if (stickR.x != 0) {
                moveRecords(stickR.x);
            }
            musicSelect.showInfomation(listMusicDict[folderCount]);
            moveSelectCircle();
        }

    }

    //選択サークルを移動させる
    private void moveSelectCircle() {
        Transform t = this.listRecordObject[folderCount].GetComponent<Transform>();
        Vector3 pos = t.transform.position;
        Vector3 ang = t.transform.localEulerAngles;
        moveSelectMsg(pos, ang);
        selectCircleAng += 0.2f;
        if (selectCircleAng > 360.0f) selectCircleAng = 0;
        ang.z = selectCircleAng;
        GameObject.Find("SelectCircle").GetComponent<Transform>().transform.position = pos;
        GameObject.Find("SelectCircle").GetComponent<Transform>().transform.localEulerAngles = ang;
    }

    private void moveSelectMsg(Vector3 pos, Vector3 ang) {
        GameObject.Find("PressButton").GetComponent<Transform>().transform.position = pos;
        GameObject.Find("PressButton").GetComponent<Transform>().transform.localEulerAngles = ang;
    }

    //レコードの移動
    private void moveRecords(float x) {
        foreach (GameObject record in this.listRecordObject) {
            Vector3 pos = record.GetComponent<Transform>().transform.position;
            if (pos.z != this.recordZ) continue;

            pos.x += x / 5;
            record.GetComponent<Transform>().transform.position = pos;
        }
    }

    //曲の決定
    public void selectedMusic() {
        playSe(SE_DECIDE);
        GameObject.Find("CenterEyeAnchor").GetComponent<OVRScreenFade>().FadeOut();
        StartCoroutine(DelayMethod(2.0f));
    }
    private IEnumerator DelayMethod(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        startChangeScene();
    }
    private void startChangeScene() {
        // イベントに登録
        SceneManager.sceneLoaded += GameSceneLoaded;
        // シーン切り替え
        SceneManager.LoadScene("MusicPlayScene");
    }

    private void GameSceneLoaded(Scene next, LoadSceneMode mode) {
        // シーン切り替え後のスクリプトを取得
        MusicPlayManager musicPlayManager = GameObject.Find("MusicPlayManager").GetComponent<MusicPlayManager>();
        musicPlayManager.setDictMusicData(listMusicDict[folderCount]);
        SceneManager.sceneLoaded -= GameSceneLoaded;
    }

    private List<GameObject> makeAllRecords(List<Dictionary<string, string>> listMusicDict, Vector3 pos) {
        List<GameObject> listRecordObject = new List<GameObject>();
        Vector3 oldPos = pos;
        float recordDist = 0.5f;
        pos.x = pos.x - (listMusicDict.Count * recordDist /2);

        for (int i = 0; i < listMusicDict.Count; i++) {
            pos.y = (i % 2 == 1) ? oldPos.y + recordDist : oldPos.y;
            pos.x += recordDist;
            GameObject obj = makeRecord(listMusicDict[i], pos);
            listRecordObject.Add(obj);
        }
        return listRecordObject;
    }

    //レコードの作成
    private GameObject makeRecord(Dictionary<string, string> dictMusicData, Vector3 pos) {
        GameObject obj = Instantiate(RECORD_OBJECT) as GameObject;
        RecordObject rd = obj.GetComponent<RecordObject>();
        rd.setDictMusicData(dictMusicData);
        rd.showInfomation();
        obj.transform.position = pos;
        return obj;
    }

    private void playSe(AudioClip audioClip) {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.volume = 1.0f;
        audioSource.PlayOneShot(audioClip);
    }

    private IEnumerator startSeDelayMethod(float waitTime, AudioClip audioClip) {
        yield return new WaitForSeconds(waitTime);
        playSe(audioClip);
    }

}
