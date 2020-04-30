using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MusicSelect : MonoBehaviour
{
    private GameObject lightCanvas;
    private RectTransform recordSetTransform;
    private Vector3 oldRecordPos;
    private AudioSource audioSource;
    private MusicSelectManager musicSelectManager;
    private Dictionary<string, string> dictMusicData;
    private int folderCount = 0;
    private Text musicTitleText;
    private List<GameObject> listStars;
    private float moveCount = 0;
    private float anim_v = 0.02f;
    public bool isRightAnim = true;

    // Start is called before the first frame update
    void Start()
    {
        musicSelectManager = GameObject.Find("MusicSelectManager").GetComponent<MusicSelectManager>();
        musicTitleText = GameObject.Find("MusicTitleArea").GetComponent<Text>();
        lightCanvas = GameObject.Find("LightCanvas");
        recordSetTransform = GameObject.Find("RecordSet").GetComponent<RectTransform>();
        oldRecordPos = recordSetTransform.position;
        audioSource = this.GetComponent<AudioSource>();
        audioSource.clip = Resources.Load<AudioClip>("src/MusicSelect/scratch");
    }

    // Update is called once per frame
    void Update()
    {
        if (musicSelectManager.isReady) {
          
            if (dictMusicData == null) {
                showInfomation(folderCount);
            }
                
            if ((Input.GetKeyUp(KeyCode.RightArrow)) || 
                (OVRInput.GetDown(OVRInput.Button.SecondaryThumbstickRight))) {
                isRightAnim = nextMusic(true);
            }
            if ((Input.GetKeyUp(KeyCode.LeftArrow)) ||
                (OVRInput.GetDown(OVRInput.Button.SecondaryThumbstickLeft))){
                
                isRightAnim = nextMusic(false);
            }
            if ((Input.GetKeyUp(KeyCode.Space))||
                (OVRInput.GetUp(OVRInput.Button.One, OVRInput.Controller.RTouch))) {
                selectedMusic();
            }
            showInfomation(folderCount);
        }
        if (moveCount > 0) animationRecord();
    }

    //次の曲、前の曲
    public bool nextMusic(bool isRightAnim) {
        audioSource.PlayOneShot(audioSource.clip);
        showInfomation(folderCount);
        resetAnim();
        moveCount = 1.0f;
        if (isRightAnim) {
            folderCount++;
            if (folderCount >= musicSelectManager.listMusicDict.Count) folderCount = 0;
            changeArrow("allow_right_blue");
            return true;
        }
        else {
            folderCount--;
            if (folderCount < 0) folderCount = musicSelectManager.listMusicDict.Count - 1;
            changeArrow("allow_left_blue");
            return false;
        }
    }

    //曲の決定
    public void selectedMusic() {
        // イベントに登録
        SceneManager.sceneLoaded += GameSceneLoaded;
        // シーン切り替え
        SceneManager.LoadScene("MusicPlayScene");
    }

    //バックのレコードのアニメーション
    void animationRecord() {
        Vector3 pos = recordSetTransform.position;
        pos.x += (isRightAnim) ? anim_v : -anim_v;
        moveCount -= anim_v;
        recordSetTransform.position = pos;

        if (moveCount <= 0) {
            resetAnim();
        }
    }

    void resetAnim() {
        recordSetTransform.position = oldRecordPos;
        if (isRightAnim) {
            changeArrow("allow_right_black");
        }
        else {
            changeArrow("allow_left_black");
        }
    }

    void changeArrow(string arrowName) {
        Sprite s;
        if (arrowName.Contains("left")) {
            GameObject.Find("ArrowLeft").GetComponent<Image>().sprite = Resources.Load<Sprite>("src/MusicSelect/" + arrowName);
        }
        else {
            GameObject.Find("ArrowRight").GetComponent<Image>().sprite = Resources.Load<Sprite>("src/MusicSelect/" + arrowName);
        }
        Debug.Log("src/MusicSelect/" + arrowName);
    }

    void showInfomation(int folderCount) {
        dictMusicData = musicSelectManager.listMusicDict[folderCount];
        GameObject.Find("MusicTitleArea").GetComponent<Text>().text = dictMusicData["#TITLE"];
        GameObject.Find("MusicGenreArea").GetComponent<Text>().text = dictMusicData["#GENRE"];
        GameObject.Find("MusicArtistArea").GetComponent<Text>().text = dictMusicData["#ARTIST"];
        int dificurity = int.Parse(dictMusicData["#PLAYLEVEL"]);
        showLevel(dificurity);
        if (listStars != null) destroyStars();
        listStars = showStar(dificurity);
        Debug.Log("select : " + musicTitleText.text);
    }

    void showLevel(int dificurity) {
        string level = "easy";
        if((dificurity >= 4) && (dificurity <= 5)) {
            level = "normal";
        }
        else if ((dificurity >= 6) && (dificurity <= 7)) {
            level = "hard";
        }
        else if (dificurity >= 8) {
            level = "very_hard";
        }
        GameObject.Find("MusicSelectLevel").GetComponent<Image>().sprite = Resources.Load<Sprite>("src/MusicSelect/" + level);
    }

    List<GameObject> showStar(int dificurity) {
        List<GameObject> listStars = new List<GameObject>();
        Vector3 pos = lightCanvas.GetComponent<RectTransform>().position;
        pos.y -= 0.14f;
        for (int i = 0; i < dificurity; i++) {
            listStars.Add(makeStar(pos, i));
        }
        return listStars;
    }

    private GameObject makeStar(Vector3 pos, int count) {
        GameObject star = new GameObject("star" + count);
        star.transform.SetParent(lightCanvas.transform, false);
        Image img = star.AddComponent<Image>();
        Texture2D texture = Resources.Load("src/MusicSelect/star") as Texture2D;
        img.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        star.GetComponent<RectTransform>().sizeDelta = new Vector2(texture.width, texture.height);
        pos.x += 0.1f * count;
        star.GetComponent<RectTransform>().position = pos;
        return star;
    }

    private void destroyStars() {
        foreach (GameObject star in listStars) {
            Destroy(star.gameObject);
        }
    }

    private void GameSceneLoaded(Scene next, LoadSceneMode mode) {
        // シーン切り替え後のスクリプトを取得
        MusicPlayManager musicPlayManager = GameObject.Find("MusicPlayManager").GetComponent<MusicPlayManager>();
        musicPlayManager.setDictMusicData(dictMusicData);
        SceneManager.sceneLoaded -= GameSceneLoaded;
    }
}
