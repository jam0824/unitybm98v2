using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO; //System.IO.FileInfo, System.IO.StreamReader, System.IO.StreamWriter


public class MusicPlayManager : MonoBehaviour
{
    [SerializeField] public bool isAutoPlay = false;
    [SerializeField] public int FRAME_RATE;
    [SerializeField] public float MUSIC_OBJ_SIZE;
    [SerializeField] public float MUSIC_OBJ_Y;
    [SerializeField] public GameObject musicObjBlue;
    [SerializeField] public GameObject musicObjRed;
    [SerializeField] public GameObject musicObjOrange;
    [SerializeField] public GameObject bgmObj;
    [SerializeField] public float MUSIC_WIDTH; //弾が飛んでくる範囲
    [SerializeField] public int PLAY_KEY_NUM; //BMSのKEY数（5key or 7key)
    [SerializeField] public float GOOD_LINE; //GOOD判定の距離（絶対値）
    [SerializeField] public float GREAT_LINE; //GREAT判定の距離（絶対値）
    [SerializeField] private string music_folder;
    [SerializeField] private string music_bms;
    
    private float musicObjVec;
    private int BPM;
    private int KEY_NUM = 30;
    private int MAX_OBJ_NUM = 100000;
    private bool isUpdate = false;
    private Dictionary<string, string> dict_info;
    private int frame_num = 0;
    private int nullCount = 0;
    private BmsConverter bmsConverter;
    private MusicPlay musicPlay;

    public int getBpm() {
        return this.BPM;
    }
    public int getKeyNum() {
        return this.KEY_NUM;
    }
    public int getMaxObjNum() {
        return this.MAX_OBJ_NUM;
    }
    public float getMusicObjVec() {
        return this.musicObjVec;
    }
    public void setMusicFolder(string music_folder) {
        this.music_folder = music_folder;
    }
    public void setMusic_bms(string music_bms) {
        this.music_bms = music_bms;
    }

    private void init() {
        Application.targetFrameRate = FRAME_RATE;
        
        bmsConverter = this.GetComponent<BmsConverter>();
        musicPlay = this.GetComponent<MusicPlay>();
    }

    void Start()
    {
        init();

        string[] lines = bmsConverter.read(music_folder + "/" + music_bms);

        dict_info = bmsConverter.getInfomation(lines);
        BPM = int.Parse(dict_info["#BPM"]);
      
        musicPlay.setListMusicData(
            bmsConverter.makeMusicData(
                lines,
                BPM,
                FRAME_RATE
            )
        );
        musicPlay.setDictAudio(
            bmsConverter.readAudioFiles(lines, music_folder)
        );
        musicPlay.setDictImage(
            bmsConverter.readImageFiles(lines, music_folder)
        );
        musicObjVec = musicPlay.setMusicObjVec(4, FRAME_RATE, BPM);
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
            frame_num++;
        }
        //Bボタンが押されたら最初から
        if (OVRInput.GetDown(OVRInput.Button.Two)) {
            frame_num = 0;
            nullCount = 0;
        }

        if(nullCount == FRAME_RATE * 3) {
            finish();
        }
    }

    //曲終了処理
    void finish() {
        AudioSource audioSource = this.GetComponent<AudioSource>();
        audioSource.clip = Resources.Load<AudioClip>("src/success");
        audioSource.PlayOneShot(audioSource.clip);
    }

}
