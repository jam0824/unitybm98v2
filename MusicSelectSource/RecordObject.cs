using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecordObject : MonoBehaviour
{
    public bool isGrabbled = false;
    public AudioClip AUDIO_DECIDE;
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
            }
        }
    }

    void Update()
    {
        if (ovrGrabbable.isGrabbed) {
            if (!this.isGrabbled) {
                Bm98Debug.Instance.Log(dictMusicData["music_count"] + " : " + dictMusicData["#TITLE"]);
                musicSelectManager.folderCount = int.Parse(dictMusicData["music_count"]);
                musicSelectManager.localRecordCount = this.localRecordCount;
                playSe();
                this.isGrabbled = true;
            }
        }
        else {
            this.isGrabbled = false;
        }
    }

    private void playSe() {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(AUDIO_DECIDE);
    }

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

}
