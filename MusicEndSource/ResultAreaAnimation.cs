using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultAreaAnimation : MonoBehaviour
{
    public GameObject RESULT_AREA_OBJECT;
    public GameObject PRESS_BUTTON;
    public AudioClip seCombo;
    public AudioClip seCalorie;

    private GameObject resultArea;
    private MusicPlayManager musicPlayManager;
    private MusicPlayData musicPlayData;
    private int liveCount = 0;
    private int animCount = 0;
    private int frameRate;
    private bool isAnimation = false;

    //スコア表示のところの処理
    public void startResultAreaDraw() {
        resultArea = Instantiate(RESULT_AREA_OBJECT) as GameObject;
        isAnimation = true;
    }

    void Start()
    {
        musicPlayManager = GameObject.Find("MusicPlayManager").GetComponent<MusicPlayManager>();
        musicPlayData = GameObject.Find("MusicPlayManager").GetComponent<MusicPlayData>();
        frameRate = musicPlayManager.FRAME_RATE;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAnimation) return;

        liveCount++;
        if (liveCount % frameRate == 0) {
            animCount++;
            switch (animCount) {
                case 1:
                    drawTotalNotes(musicPlayData.getTotalNotesNum());
                    
                    break;
                case 2:
                    drawMaxCombo(musicPlayData.getMaxComboNum());
                    break;
                case 3:
                    drawGreat(musicPlayData.getGreatNum());
                    break;
                case 4:
                    drawGood(musicPlayData.getGoodNum());
                    break;
                case 5:
                    drawPoor(musicPlayData.getPoorNum());
                    break;
                case 6:
                    drawCalorie(musicPlayData.getCalorie());
                    break;
            }
        }
    }

    private void drawMaxCombo(int num) {
        GameObject.Find("MaxComboNum").GetComponent<Text>().text = num.ToString();
        playSe(seCombo);
    }
    private void drawGreat(int num) {
        GameObject.Find("GreatNum").GetComponent<Text>().text = num.ToString();
        playSe(seCombo);
    }
    private void drawGood(int num) {
        GameObject.Find("GoodNum").GetComponent<Text>().text = num.ToString();
        playSe(seCombo);
    }
    private void drawPoor(int num) {
        GameObject.Find("PoorNum").GetComponent<Text>().text = num.ToString();
        playSe(seCombo);
    }
    private void drawTotalNotes(int num) {
        GameObject.Find("TotalNotesNum").GetComponent<Text>().text = num.ToString();
        playSe(seCombo);
    }
    private void drawCalorie(float calorie) {
        GameObject.Find("CalorieNum").GetComponent<Text>().text = calorie.ToString("f1") + " kcal";
        playSe(seCalorie);
        GameObject obj = Instantiate(PRESS_BUTTON) as GameObject;
    }

    private void playSe(AudioClip audioClip) {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(audioClip);
    }


}
