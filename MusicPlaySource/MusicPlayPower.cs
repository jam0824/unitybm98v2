using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayPower : MonoBehaviour
{
    public Sprite LIFEBAR_BLUE;
    public Sprite LIFEBAR_YELLOW;
    public Sprite LIFEBAR_RED;
    public float DROW_MIN_LINE = -3.6f;
    public bool isFailed = false;

    private GameObject powerBar;
    private float maxPower = 200;
    private float power = 100;
    private float lineYellow = 0.25f;
    private float lineRed = 0.125f;

    private float excellent = 1.0f;
    private float great = 0.5f;
    private float good = 0.2f;
    private float poor = -20.0f;

    private string status = "blue";


    // Start is called before the first frame update
    void Start()
    {
        powerBar = GameObject.Find("PowerBar");
        showLifeBar();
    }

    public void calcPower() {
        int totalNotes = this.GetComponent<MusicPlayData>().getTotalNotesNum();
        this.excellent = (maxPower * 1.5f) / (float)totalNotes;
        this.great = excellent / 2.0f;
        this.good = great / 2.0f;
    }

    // Update is called once per frame
    void Update()
    {
        showLifeBar();
        changeBarColor();
    }

    public float getPower() {
        return this.power;
    }

    //パワーの計算
    public void calcPower(string status) {
        switch (status) {
            case "excellent":
                power += excellent;
                break;
            case "great":
                power += great;
                break;
            case "good":
                power += good;
                break;
            case "poor":
                power += poor;
                break;
        }
        if (power >= maxPower) power = maxPower;
        if (power < 0) {
            isFailed = true;
            power = 0;
        }
    }

    //ライフバーを表示
    private void showLifeBar() {
        //ライフバーが中央寄せで表示されちゃうので左寄せに修正する
        float par = power / maxPower;
        float x = DROW_MIN_LINE * (1 - par);
        powerBar.transform.localScale = new Vector3(par, 1, 1);
        Vector3 pos = powerBar.transform.position;
        pos.x = x;
        powerBar.transform.position = pos;
    }

    //ライフバーの色変え
    private void changeBarColor() {
        if((status != "blue") && (power > maxPower * lineYellow)){
            status = "blue";
            powerBar.GetComponent<SpriteRenderer>().sprite = LIFEBAR_BLUE;
        }
        else if ((status != "yellow") &&
            (power <= maxPower * lineYellow) &&
            (power > maxPower * lineRed)) {
            status = "yellow";
            powerBar.GetComponent<SpriteRenderer>().sprite = LIFEBAR_YELLOW;
        }
        else if ((status != "red") &&
            (power <= maxPower * lineRed)) {
            status = "red";
            powerBar.GetComponent<SpriteRenderer>().sprite = LIFEBAR_RED;
        }
    }
}
