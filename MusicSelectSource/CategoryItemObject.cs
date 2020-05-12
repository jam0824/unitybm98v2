using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CategoryItemObject : MonoBehaviour
{
    
    public string categoryPath;
    public string categoryName;

    private MusicSelectManager musicSelectManager;
    private MusicSelectCategory musicSelectCategory;

    // Start is called before the first frame update
    void Start()
    {
        musicSelectManager = GameObject.Find("MusicSelectManager").GetComponent<MusicSelectManager>();
        musicSelectCategory = GameObject.Find("CategorySelect").GetComponent<MusicSelectCategory>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //クリックしたらパスとジャンル名を入れて、シーンを再読み込みさせる。
    public void clickCategoryItem() {
        musicSelectManager.setMusicFolderPath(this.categoryPath);
        musicSelectManager.setCategory(this.categoryName);
        musicSelectCategory.clickSe();
        musicSelectCategory.closeCategorySelect();
        Bm98Debug.Instance.Log("category path : " + this.categoryPath);
        musicSelectManager.returnMusicSelectScene();
    }
}
