using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiController : MonoBehaviour
{
    private MusicPlayManager musicPlayManager;
    private MusicPlayData musicPlayData;
    private CalcFps calcFps;
    private Text calorieText;
    private Text metsText;
    private Text scoreText;
    private Dictionary<string, Sprite> dict_image;
    private Dictionary<string, GameObject> dict_object;
    private string status = "";
    private string oldStatus = "";
    private string oldComboNum = "";

    private bool isRedraw = false;

    public void setStatus(string stat) {
        status = stat;
    }

    private void init() {
        musicPlayManager = GameObject.Find("MusicPlayManager").GetComponent<MusicPlayManager>();
        musicPlayData = GameObject.Find("MusicPlayManager").GetComponent<MusicPlayData>();
        calcFps = GameObject.Find("FpsObject").GetComponent<CalcFps>();
        calorieText = GameObject.Find("CalorieArea").GetComponent<Text>();
        metsText = GameObject.Find("METsArea").GetComponent<Text>();
        scoreText = GameObject.Find("ScoreArea").GetComponent<Text>();
        dict_object = new Dictionary<string, GameObject>();
        GameObject state = GameObject.Find("UiStateObject");
        dict_object.Add("state", state);
        for(int i = 0; i < 4; i++) {
            string objName = "UiNumObject" + i.ToString();
            GameObject obj = GameObject.Find(objName);
            dict_object.Add(objName, obj);
        }
        
    }

    // Start is called before the first frame update
    void Start()
    {
        init();
        string[] list_sprite_name = read("src/sprite_list");
        dict_image = readImageFiles(list_sprite_name);
    }

    // Update is called once per frame
    void Update()
    {
        updateSpriteArea();
        calorieText.text = redrawCalorieText();
        metsText.text = redrawMetsText();
        scoreText.text = redrawScore();
    }

    private string redrawScore() {
        return musicPlayData.Score.ToString("N0") + " / " + musicPlayData.HighScore.ToString("N0");
    }

    //ファイル読み込み。たぶんどこでも使ってない
    string[] read(string filePath) {
        TextAsset text = Resources.Load<TextAsset>(filePath);
        string allText = text.text;
        allText = allText.Replace("\r", "");
        return allText.Split('\n');
    }

    //イメージファイルを読み込む。たぶんどこでも使ってない
    private Dictionary<string, Sprite> readImageFiles(string[] list_string) {
        Dictionary<string, Sprite> dict_image = new Dictionary<string, Sprite>();
        foreach (string line in list_string) {
            Sprite image = Resources.Load<Sprite>("src/" + line);
            dict_image.Add(line, image);
        }
        return dict_image;
    }

    //コンボ数のところのアップデート
    private void updateSpriteArea() {
        if (musicPlayData.getComboNum() == 0) {
            changeState();
            deleteSpriteArea();
        }
        else {
            changeState();
            changeNum();
        }
        if (isRedraw) resetAlpha();
    }

    //現在のコンボステータスを変更
    private void changeState() {
        if (oldStatus != status) {
            SpriteRenderer s = dict_object["state"].GetComponent<SpriteRenderer>();
            s.sprite = dict_image[status];
            oldStatus = status;
            isRedraw = true;
        }
        else {
            isRedraw = false;
        }
        
    }
    //現在のコンボ数を変更
    private void changeNum() {
        int comboNum = musicPlayData.getComboNum();
        string str_combo_num = fill(comboNum.ToString(), "x", 4);

        if(oldComboNum != str_combo_num) {
            for (int i = 0; i < str_combo_num.Length; i++) {
                string c = str_combo_num.Substring(i, 1);

                string objName = "UiNumObject" + i.ToString();
                SpriteRenderer numberSprite = dict_object[objName].GetComponent<SpriteRenderer>();

                if (c == "x") {
                    numberSprite.sprite = null;
                }
                else {
                    numberSprite.sprite = dict_image[c];
                }

            }
            oldComboNum = str_combo_num;
            isRedraw = true;
        }
        else {
            isRedraw = false;
        }

        
    }
    private string fill(string target, string fillChar, int max) {
        string tmp = "";
        for (int i = 0; i < max - target.Length; i++) {
            tmp += "x";
        }
        target += tmp;
        return target;
    }
    //コンボ数のところを削除。コンボキレたとき
    private void deleteSpriteArea() {
        
        for (int i = 0; i < 4; i++) {
            string objName = "UiNumObject" + i.ToString();
            SpriteRenderer numberSprite = dict_object[objName].GetComponent<SpriteRenderer>();
            numberSprite.sprite = null;
        }

    }
    //コンボは放っておくと薄くなっていくので、コンボに変更があったらalpha値を直す
    private void resetAlpha() {
        SpriteRenderer s = dict_object["state"].GetComponent<SpriteRenderer>();
        resetAlphaExec(s);
        for (int i = 0; i < 4; i++) {
            string objName = "UiNumObject" + i.ToString();
            SpriteRenderer numberSprite = dict_object[objName].GetComponent<SpriteRenderer>();
            resetAlphaExec(numberSprite);
        }
    }
    private void resetAlphaExec(SpriteRenderer s) {
        var color = s.color;
        color.a = 1.0f;
        s.color = color;
    }

    //消費カロリーの表示
    string redrawCalorieText() {
        float calorie = musicPlayData.getCalorie();

        return calorie.ToString("f1") + " kcal";
    }
    //METsの表示
    string redrawMetsText() {
        float mets = musicPlayData.METs;

        return mets.ToString("f1") + " METs";
    }
}
