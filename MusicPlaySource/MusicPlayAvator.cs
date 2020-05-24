using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayAvator : MonoBehaviour
{
    public List<GameObject> listAvatorObject;

    private bool isActive = false;

    public bool changeAvatorMode() {
        isActive = (isActive) ? false : true;
        setActiveAvator(isActive);
        return isActive;
    }

    //avatorをアクティブ/非アクティブにする
    public void setActiveAvator(bool isActive) {
        foreach(GameObject obj in listAvatorObject) {
            obj.gameObject.SetActive(isActive);
        }
    }

}
