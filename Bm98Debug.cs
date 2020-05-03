using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bm98Debug : SingletonMonoBehaviour<Bm98Debug> {

    public bool isDebug;
    private List<string> listDebugMsg = new List<string>();


    public void Log(string msg) {
        if (!isDebug) return;
        Debug.Log(msg);
        listDebugMsg.Insert(0, msg);
        redrawLogMsg();
    }

    private void redrawLogMsg() {
        string msg = "";
        int max = (listDebugMsg.Count > 100) ? 100 : listDebugMsg.Count;
        for (int i = 0; i < max; i++) {
            msg += listDebugMsg[i] + "\n";
        }
        this.GetComponent<Text>().text = msg;
    }

    public void Awake() {
        if (this != Instance) {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

}