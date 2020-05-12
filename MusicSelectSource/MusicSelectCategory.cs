using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityBm98Config;
using FileController;

public class MusicSelectCategory : MonoBehaviour
{
    public RectTransform content_;
    public GameObject categoryItem;
    public AudioClip openSe;
    public AudioClip closeSe;

    private float CATEGORY_Y = 1.3f;
    private float itemHeight;
    private string categoriesPath;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        categoriesPath = config.getCategoryFolderPath();
        List<string> listCategory = fileController.getFolderList(this.categoriesPath);
        audioSource = GetComponent<AudioSource>();
        updateListView(this.categoryItem, listCategory);
    }

    // Update is called once per frame
    void Update()
    {
        

    }

    //作るときに呼ぶ。位置を変えているだけ
    public void makeCategoryList() {
        Vector3 pos = GetComponent<Transform>().transform.position;
        pos.y = CATEGORY_Y;
        GetComponent<Transform>().transform.position = pos;
        clickSe();
    }

    //Closeボタンを押したときに呼ぶ
    public void closeCategorySelect() {
        Vector3 pos = this.transform.position;
        pos.y = -50.0f;
        this.transform.position = pos;
        audioSource.PlayOneShot(closeSe);
    }

    public void clickSe() {
        audioSource.PlayOneShot(openSe);
    }

    //リスト更新
    private void updateListView(GameObject categoryItem, List<string> listCategory) {
        float itemHeight = getItemHeight(categoryItem);
        int settingCount = listCategory.Count;
        float newHeight = settingCount * itemHeight;
        content_.sizeDelta = new Vector2(content_.sizeDelta.x, newHeight);


        for(int i = 0; i < settingCount; i++) {
            makeitem(listCategory[i], categoriesPath + listCategory[i] + "/");
        }

        //最後にNoneを追加
        makeitem("None", config.getFolderPath());
    }

    //リストアイテム作成
    private void makeitem(string categoryName, string categoryPath) {
        GameObject item = GameObject.Instantiate(categoryItem) as GameObject;
        CategoryItemObject categoryItemObject = item.GetComponent<CategoryItemObject>();

        categoryItemObject.categoryName = categoryName;
        categoryItemObject.categoryPath = categoryPath;

        Transform[] transformArray = item.GetComponentsInChildren<Transform>();
        foreach (Transform t in transformArray) {
            if (t.name == "CategoryItemText") {
                t.gameObject.GetComponent<Text>().text = categoryName;
                break;
            }
        }
        RectTransform itemTransform = (RectTransform)item.transform;
        itemTransform.SetParent(content_, false);
    }

    //アイテムの高さを取得
    private float getItemHeight(GameObject categoryItem) {
        GameObject item = GameObject.Instantiate(categoryItem) as GameObject;
        RectTransform rect = item.GetComponent<RectTransform>();
        itemHeight = rect.rect.height;
        GameObject.Destroy(item);
        return itemHeight;
    }

    
}
