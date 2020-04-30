using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiController : MonoBehaviour
{
    private MusicPlayManager musicPlayManager;
    private Text calorieText;
    private Dictionary<string, Sprite> dict_image;
    private Dictionary<string, GameObject> dict_object;
    private string status = "";
    private string oldStatus = "";

    private int combo_num = 0;
    private string oldComboNum = "";

    private bool isRedraw = false;

    public void addCombo() {
        combo_num++;
    }
    public void setCombo(int c) {
        combo_num = c;
    }
    public void setStatus(string stat) {
        status = stat;
    }

    private void init() {
        musicPlayManager = GameObject.Find("MusicPlayManager").GetComponent<MusicPlayManager>();
        calorieText = GameObject.Find("CalorieArea").GetComponent<Text>();
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
    }

    string[] read(string filePath) {
        TextAsset text = Resources.Load<TextAsset>(filePath);
        string allText = text.text;
        allText = allText.Replace("\r", "");
        return allText.Split('\n');
    }

    private Dictionary<string, Sprite> readImageFiles(string[] list_string) {
        Dictionary<string, Sprite> dict_image = new Dictionary<string, Sprite>();
        foreach (string line in list_string) {
            Sprite image = Resources.Load<Sprite>("src/" + line);
            dict_image.Add(line, image);
        }
        return dict_image;
    }

    private void updateSpriteArea() {
        if (combo_num == 0) {
            changeState();
            deleteSpriteArea();
        }
        else {
            changeState();
            changeNum();
        }
        if (isRedraw) resetAlpha();
    }

   
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
    private void changeNum() {

        string str_combo_num = fill(combo_num.ToString(), "x", 4);

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

    private void deleteSpriteArea() {
        
        for (int i = 0; i < 4; i++) {
            string objName = "UiNumObject" + i.ToString();
            SpriteRenderer numberSprite = dict_object[objName].GetComponent<SpriteRenderer>();
            numberSprite.sprite = null;
        }

    }

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

    string redrawCalorieText() {
        float dist = musicPlayManager.getMovingDistance();
        float calorie = dist / musicPlayManager.DEC_M_PAR_CALORIE;
        
        return "消費カロリー\n" + calorie.ToString("f1") + "kcal\n\n拳の総移動距離\n" + dist.ToString("f1") + "m";
    }
}
