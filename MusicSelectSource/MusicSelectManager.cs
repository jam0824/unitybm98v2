using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FileController;
using UnityBm98Config;
using System.IO;
using UnityBm98Utilities;
using UnityEngine.SceneManagement;

public class MusicSelectManager : MonoBehaviour
{
    private string MUSIC_FOLDER_PATH;
    private BmsInformationLoader bmsInformationLoader;
    private MusicSelect musicSelect;
    public int folderCount = 0;

    public List<Dictionary<string, string>> listMusicDict;
    public bool isReady = false;

    public Dictionary<string, string> getDictMusicData() {
        return listMusicDict[folderCount];
    }

    void Start()
    {
        MUSIC_FOLDER_PATH = config.getFolderPath();
        musicSelect = this.GetComponent<MusicSelect>();
        listMusicDict = this.GetComponent<BmsInformationLoader>().getListMusicDict(MUSIC_FOLDER_PATH);
        musicSelect.showInfomation(listMusicDict[folderCount]);
        isReady = true;
        
    }


    // Update is called once per frame
    void Update()
    {
        if (isReady) {

            if ((Input.GetKeyUp(KeyCode.RightArrow)) ||
                (OVRInput.GetDown(OVRInput.Button.SecondaryThumbstickRight))) {
                musicSelect.nextMusic();
            }
            if ((Input.GetKeyUp(KeyCode.LeftArrow)) ||
                (OVRInput.GetDown(OVRInput.Button.SecondaryThumbstickLeft))) {
                musicSelect.prevMusic();
            }
            if ((Input.GetKeyUp(KeyCode.Space)) ||
                (OVRInput.GetUp(OVRInput.Button.One, OVRInput.Controller.RTouch))) {
                selectedMusic();
            }
            musicSelect.showInfomation(listMusicDict[folderCount]);
        }

    }

    //曲の決定
    public void selectedMusic() {
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

}
