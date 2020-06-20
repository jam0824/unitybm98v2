using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FileController;
using System.IO;

public class MusicSelectCache : MonoBehaviour
{
    private string KV_SPLIT_CHAR = ":";
    private string ITME_SPLIT_CHAR = ",";
    private string FILE_NAME_LIST_MUSIC_DICT = "listMusicDict.csv";
    private string FILE_NAME_LIST_FOLDER = "listFolder.csv";

    //キャッシュ用ファイルがあるか確認
    public bool isExistCache(string folderPath) {
        if ((fileController.isFileExist(folderPath + "/" + FILE_NAME_LIST_MUSIC_DICT)) && 
            (fileController.isFileExist(folderPath + "/" + FILE_NAME_LIST_FOLDER))) {
            return true;
        }
        else {
            return false;
        }
    }

    //listMusicDictをcsvに変換して保存
    public void saveListMusicDict(
        List<Dictionary<string, string>> listMusicDict, 
        string folderPath) 
    {
        string strSaveString = "";
        foreach (Dictionary<string, string> musicDict in listMusicDict) {
            foreach (KeyValuePair<string, string> item in musicDict) {
                string value = item.Value;
                value = value.Replace(",", "，");
                value = value.Replace(":", "：");
                strSaveString += item.Key + KV_SPLIT_CHAR + value + ITME_SPLIT_CHAR;
            }
            strSaveString += "\n";
        }
        bool isOk = FileController.fileController.writeFile(
            folderPath + "/" + FILE_NAME_LIST_MUSIC_DICT, 
            strSaveString
            );
    }

    //現在のフォルダを記録
    public void saveListFolder(string folderPath) {
        string strSaveString = "";
        List<string> listFolder = fileController.getFolderList(folderPath);
        foreach(string folder in listFolder) {
            strSaveString += folder + "\n";
        }
        bool isOk = FileController.fileController.writeFile(
            folderPath + "/" + FILE_NAME_LIST_FOLDER,
            strSaveString
            );
    }

    //保存されたフォルダ数と、現在のフォルダ数が一致しているか確認（キャッシュ再作成に使用）
    public bool isSameCache(string folderPath) {
        string[] lines = File.ReadAllLines(folderPath + "/" + FILE_NAME_LIST_FOLDER);
        Debug.Log("folder NUM = " + lines.Length);
        List<string> listFolder = fileController.getFolderList(folderPath);
        return (lines.Length == listFolder.Count) ? true : false;
    }

    //csvにしたlistMusicDictをロードする
    public List<Dictionary<string, string>> loadListMusicDict(string folderPath) {
        List<Dictionary<string, string>> listMusicDict = new List<Dictionary<string, string>>();
        string[] lines = File.ReadAllLines(folderPath + "/" + FILE_NAME_LIST_MUSIC_DICT);
        foreach(string line in lines) {
            string[] items = line.Split(',');
            Dictionary<string, string> musicDict = new Dictionary<string, string>();
            foreach (string item in items) {
                if(item != "") {
                    string[] key_value = item.Split(':');
                    musicDict.Add(key_value[0], key_value[1]);
                }
            }
            listMusicDict.Add(musicDict);
        }
        return listMusicDict;
    }
}
