using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FileController;
using UnityBm98Config;
using System.IO;
using UnityBm98Utilities;

public class MusicSelectManager : MonoBehaviour
{
    private string MUSIC_FOLDER_PATH;
    public List<Dictionary<string, string>> listMusicDict;
    public bool isReady = false;

    void Start()
    {
        MUSIC_FOLDER_PATH = config.getFolderPath();
        listMusicDict = getListMusicDict();
        isReady = true;
        
    }

    //ゲームフォルダ内の全曲フォルダからbms/bmeのファイル情報を抜いてくる
    private List<Dictionary<string, string>> getListMusicDict() {
        listMusicDict = new List<Dictionary<string, string>>();
        List<string> listFolder = fileController.getFolderList(MUSIC_FOLDER_PATH);
        foreach (string folderName in listFolder) {
            List<string> listFile = fileController.getFileList(MUSIC_FOLDER_PATH + "/" + folderName);
            foreach (string fileName in listFile) {
                if ((fileName.Contains(".bms")) || (fileName.Contains(".bme"))) {
                    listMusicDict.Add(
                        getBmsInfo(folderName, fileName)
                    );
                }

            }

        }
        return listMusicDict;
    }

    //bms/bmeファイルの情報を抜く
    private Dictionary<string, string> getBmsInfo(string folderName, string fileName) {
        //string[] lines = fileController.fileConvertUTF8(MUSIC_FOLDER_PATH + "/" + folderName + "/" + fileName);
        string[] lines = UnityBm98Utilities.UnityBm98Utilities.read(MUSIC_FOLDER_PATH + "/" + folderName + "/" + fileName);
        Dictionary<string, string> dict_info = UnityBm98Utilities.UnityBm98Utilities.getInfomation(lines);
            dict_info.Add("music_folder", folderName);
            dict_info.Add("music_bms", fileName);
            return dict_info;
    }

    void showDebug() {
        foreach (Dictionary<string, string>dict_info in listMusicDict) {
            Debug.Log("title:" + dict_info["#TITLE"]);

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //bmsファイル読み込み
    public string[] read(string filePath) {
        Debug.Log(filePath);
        return File.ReadAllLines(filePath);
    }

    //インフォメーション部分読み込み
    public Dictionary<string, string> getInfomation(string[] list_string) {
        Dictionary<string, string> dict_info = new Dictionary<string, string>();
        foreach (string line in list_string) {
            if (line.Contains("#WAV")) break;
            int index = line.IndexOf(" ");
            if (index == -1) continue;
            if (line.Substring(0, 1) == "*") continue;
            string key = line.Substring(0, index);
            string value = line.Substring(index + 1);
            dict_info.Add(key, value);
        }
        return dict_info;
    }
}
