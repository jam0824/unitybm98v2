using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayData : MonoBehaviour
{
    public int SCORE_EXCELLENT;
    public int SCORE_GREAT;
    public int SCORE_GOOD;
    public int SCORE_COMBO;
    
    //プレイ中のデータをコイツで管理したい
    private int maxComboNum = 0;
    private int comboNum = 0;
    private int excellentNum = 0;
    private int greatNum = 0;
    private int goodNum = 0;
    private int poorNum = 0;
    private int totalNotesNum = 0;
    private float calorie = 0;
    private float mets = 0;
    private int score = 0;
    private int highScore = 0;

    public int getComboNum() {
        return this.comboNum;
    }
    public void addComboNum() {
        comboNum++;
        if (comboNum > maxComboNum) maxComboNum = comboNum;
    }
    public void setComboNum(int combo) {
        this.comboNum = combo;
    }
    public int getExcellentNum() {
        return this.excellentNum;
    }
    public int getGreatNum() {
        return this.greatNum;
    }
    
    public int getGoodNum() {
        return this.goodNum;
    }
    
    public int getPoorNum() {
        return this.poorNum;
    }
    
    public int getTotalNotesNum() {
        return this.totalNotesNum;
    }
    public void setTotalNotesNum(int num) {
        this.totalNotesNum = num;
    }
    public void addTotalNotesNum() {
        totalNotesNum++;
    }
    public float getCalorie() {
        return this.calorie;
    }
    public void setCalorie(float calorie) {
        this.calorie = calorie;
    }

    public int MaxCombo {
        set { this.maxComboNum = value; }
        get { return this.maxComboNum; }
    }

    public int Score {
        set { this.score = value; }
        get { return this.score; }
    }

    public int HighScore {
        set { this.highScore = value; }
        get { return this.highScore; }
    }
    
    //スコア計算
    public void addScore(string status) {
        switch (status) {
            case "excellent":
                this.score += this.SCORE_EXCELLENT;
                break;
            case "great":
                this.score += this.SCORE_GREAT;
                break;
            case "good":
                this.score += this.SCORE_GOOD;
                break;

        }
        this.score += this.comboNum * this.SCORE_COMBO;
        if (this.score > this.highScore) this.highScore = this.score;
    }

    //コンボ計算
    public void addComboNum(string status) {
        switch (status) {
            case "excellent":
                excellentNum++;
                break;
            case "great":
                greatNum++;
                break;
            case "good":
                goodNum++;
                break;
            case "poor":
                poorNum++;
                break;
        }

    }

    public float METs {
        set { this.mets = value; }
        get { return this.mets; }
    }
    


}
