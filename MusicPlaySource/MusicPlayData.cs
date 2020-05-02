using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayData : MonoBehaviour
{
    private int maxComboNum = 0;
    private int comboNum = 0;
    private int greatNum = 0;
    private int goodNum = 0;
    private int poorNum = 0;
    private int totalNotesNum = 0;
    private float calorie = 0f;

    public int getMaxComboNum() {
        return this.maxComboNum;
    }
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
    public int getGreatNum() {
        return this.greatNum;
    }
    public void addGreatNum() {
        greatNum++;
    }
    public int getGoodNum() {
        return this.goodNum;
    }
    public void addGoodNum() {
        goodNum++;
    }
    public int getPoorNum() {
        return this.poorNum;
    }
    public void addPoorNum() {
        poorNum++;
    }
    public int getTotalNotesNum() {
        return this.totalNotesNum;
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


}
