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

    public List<Sprite> rankSprite;

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
        if (liveCount % (frameRate / 2) == 0) {
            animCount++;
            switch (animCount) {
                case 1:
                    drawTotalNotes(musicPlayData.getTotalNotesNum());
                    
                    break;
                case 2:
                    drawMaxCombo(musicPlayData.MaxCombo);
                    break;
                case 3:
                    drawExcellent(musicPlayData.getExcellentNum());
                    break;
                case 4:
                    drawGreat(musicPlayData.getGreatNum());
                    break;
                case 5:
                    drawGood(musicPlayData.getGoodNum());
                    break;
                case 6:
                    drawPoor(musicPlayData.getPoorNum());
                    break;
                case 7:
                    drawLike(musicPlayData.Score);
                    break;
                case 8:
                    drawCalorie(musicPlayData.getCalorie());
                    break;
                case 9:
                    drawRank();
                    break;
            }
        }
    }

    private void drawMaxCombo(int num) {
        GameObject.Find("MaxComboNum").GetComponent<Text>().text = num.ToString();
        playSe(seCombo);
    }
    private void drawExcellent(int num) {
        GameObject.Find("ExcellentNum").GetComponent<Text>().text = num.ToString();
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
    private void drawLike(int score) {
        GameObject.Find("LikeNum").GetComponent<Text>().text = score.ToString("N0");
        playSe(seCombo);
    }
    private void drawCalorie(float calorie) {
        GameObject.Find("CalorieNum").GetComponent<Text>().text = calorie.ToString("f1") + " kcal";
        playSe(seCombo);
    }
    private void drawRank() {
        string rank = musicPlayManager.getRank();

        switch (rank) {
            case "S":
                GameObject.Find("RankImage").GetComponent<Image>().sprite = rankSprite[0];
                break;
            case "A":
                GameObject.Find("RankImage").GetComponent<Image>().sprite = rankSprite[1];
                break;
            case "B":
                GameObject.Find("RankImage").GetComponent<Image>().sprite = rankSprite[2];
                break;
            case "C":
                GameObject.Find("RankImage").GetComponent<Image>().sprite = rankSprite[3];
                break;
            case "D":
                GameObject.Find("RankImage").GetComponent<Image>().sprite = rankSprite[4];
                break;
        }
        playSe(seCalorie);
        GameObject obj = Instantiate(PRESS_BUTTON) as GameObject;
    }

    private void playSe(AudioClip audioClip) {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(audioClip);
    }


}
