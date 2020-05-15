using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecordObject : MonoBehaviour
{
    public bool isGrabbled = false;
    public AudioClip AUDIO_DECIDE;
    public Sprite TITLE_CLEARED;
    public Sprite TITLE_NOT_CLEAR;
    public Sprite RANK_TRANCE;
    public List<Sprite> RankImage;
    private Dictionary<string, string> dictMusicData;
    private MusicSelectManager musicSelectManager;
    private OVRGrabbable ovrGrabbable;
    private int localRecordCount; //曲選択画面で表示されているレコードの何番目か
    Transform[] transformArray;

    void Start() 
    {
        ovrGrabbable = this.GetComponent<OVRGrabbable>();
        musicSelectManager = GameObject.Find("MusicSelectManager").GetComponent<MusicSelectManager>();
    }
    public void setDictMusicData(Dictionary<string, string> dictMusicData) {
        this.dictMusicData = dictMusicData;
    }
    //曲選択画面で表示用レコードの何番目なのか保持。（選択サークル表示のため）
    public void setLocalRecordCount(int localRecordCount) {
        this.localRecordCount = localRecordCount;
    }

    public void showInfomation() {
        transformArray = GetComponentsInChildren<Transform>();
        foreach (Transform t in transformArray) {

            switch (t.name) {
                case "RecordMusicTitleArea":
                    if (dictMusicData.ContainsKey("#TITLE"))
                        t.gameObject.GetComponent<Text>().text = dictMusicData["#TITLE"];
                    break;
                case "RecordMusicGenreArea":
                    if (dictMusicData.ContainsKey("#GENRE"))
                        t.gameObject.GetComponent<Text>().text = dictMusicData["#GENRE"];
                    break;
                case "RecordMusicArtistArea":
                    if (dictMusicData.ContainsKey("#ARTIST"))
                        t.gameObject.GetComponent<Text>().text = dictMusicData["#ARTIST"];
                    break;
                case "RecordMusicLevelNum":
                    if (dictMusicData.ContainsKey("#PLAYLEVEL"))
                        t.gameObject.GetComponent<Text>().text = dictMusicData["#PLAYLEVEL"];
                    break;
                case "RecordMusicSelectLevel":
                    if (dictMusicData.ContainsKey("#PLAYLEVEL")) {
                        string levelName = getLevelImageFileName(int.Parse(dictMusicData["#PLAYLEVEL"]));
                        t.GetComponent<Image>().sprite = Resources.Load<Sprite>("src/MusicSelect/" + levelName);
                    }
                    break;
                case "MusicNoText":
                    t.gameObject.GetComponent<Text>().text = "No." + dictMusicData["music_count"];
                    break;
                case "RecordTitleImage":
                    changeTitleColor(t, dictMusicData["HighScore"]);
                    break;
                case "RankImage":
                    changeRankImage(t, dictMusicData["Rank"]);
                    break;
            }
        }
    }

    void Update()
    {
        if (ovrGrabbable.isGrabbed) {
            if (!this.isGrabbled) {
                Bm98Debug.Instance.Log(dictMusicData["music_count"] + " : " + dictMusicData["#TITLE"]);
                musicSelectManager.setFolderCount(int.Parse(dictMusicData["music_count"]));
                musicSelectManager.localRecordCount = this.localRecordCount;
                playSe();
                this.isGrabbled = true;
            }
        }
        else {
            this.isGrabbled = false;
        }
    }

    private void changeRankImage(Transform t, string rank) {
        if(rank == "") {
            t.gameObject.GetComponent<Image>().sprite = RANK_TRANCE;
            return;
        }
        switch (rank) {
            case "S":
                t.gameObject.GetComponent<Image>().sprite = RankImage[0];
                break;
            case "A":
                t.gameObject.GetComponent<Image>().sprite = RankImage[1];
                break;
            case "B":
                t.gameObject.GetComponent<Image>().sprite = RankImage[2];
                break;
            case "C":
                t.gameObject.GetComponent<Image>().sprite = RankImage[3];
                break;
            case "D":
                t.gameObject.GetComponent<Image>().sprite = RankImage[4];
                break;
        }

    }

    //タイトルの色を変える
    private void changeTitleColor(Transform t, string score) {
        if (score == "") {
            t.gameObject.GetComponent<Image>().sprite = TITLE_NOT_CLEAR;
        }
        else {
            //ハイスコアが入っていたらクリアしてるのでタイトル色を変える
            t.gameObject.GetComponent<Image>().sprite = TITLE_CLEARED;
        }
    }

    private void playSe() {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(AUDIO_DECIDE);
    }

    //曲の難易度はPlayLevelから出す
    string getLevelImageFileName(int dificurity) {
        string level = "easy";
        if ((dificurity >= 4) && (dificurity <= 5)) {
            level = "normal";
        }
        else if ((dificurity >= 6) && (dificurity <= 7)) {
            level = "hard";
        }
        else if (dificurity >= 8) {
            level = "very_hard";
        }
        return level;
    }

    //クリックされた時の動作
    public void clickRecord() {
        if(musicSelectManager.getFolderCount() != int.Parse(dictMusicData["music_count"])) {
            Bm98Debug.Instance.Log(dictMusicData["music_count"] + " : " + dictMusicData["#TITLE"]);
            musicSelectManager.setFolderCount(int.Parse(dictMusicData["music_count"]));
            musicSelectManager.localRecordCount = this.localRecordCount;
            playSe();
        }
        else {
            musicSelectManager.selectedMusic();
        }
        
    }

}
