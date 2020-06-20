
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSelectRandomly : MonoBehaviour
{
    public int LEVEL_MIN;
    public int LEVEL_MAX;
    private MusicSelectManager musicSelectManager;
    private OVRGrabbable ovrGrabbable;


    // Start is called before the first frame update
    void Start()
    {
        ovrGrabbable = this.GetComponent<OVRGrabbable>();
        musicSelectManager = GameObject.Find("MusicSelectManager").GetComponent<MusicSelectManager>();

    }

    // Update is called once per frame
    void Update()
    {
        if (ovrGrabbable.isGrabbed) {
            startRandomly();
        }
    }

    public void startRandomly() {
        //そもそも開始されてない場合
        if (!musicSelectManager.isReady) return;

        List<Dictionary<string, string>> listRandomMusicDict = new List<Dictionary<string, string>>();
        foreach (Dictionary<string, string> musicDictData in musicSelectManager.listMusicDict) {
            if (musicDictData.ContainsKey("#PLAYLEVEL")) {
                try {
                    int level = int.Parse(musicDictData["#PLAYLEVEL"]);
                    if ((level >= LEVEL_MIN) && (level <= LEVEL_MAX)) {
                        listRandomMusicDict.Add(musicDictData);
                    }
                }
                catch {
                    Debug.LogError("プレイレベルが不正です。");
                }
            }
        }
        //無かった場合
        if (listRandomMusicDict.Count == 0) return;

        int r = Random.Range(0, listRandomMusicDict.Count);
        musicSelectManager.setFolderCount(int.Parse(listRandomMusicDict[r]["music_count"]));
        musicSelectManager.selectedMusic();
    }
    
}
