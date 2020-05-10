using FileController;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MusicSelectSaveFileLoader : MonoBehaviour
{
    //Saveファイルを読み込む。ないときは空。
    public List<Dictionary<string, string>> loadSaveData() {
        List<Dictionary<string, string>> saveData = new List<Dictionary<string, string>>();
        string filePath = UnityBm98Config.config.getSaveDataFolderPath() + UnityBm98Config.config.getSaveDataFile();

        if (File.Exists(filePath)) {
            saveData = fileController.readCsv(filePath);
        }
        return saveData;
    }

    //レコードデータのDictにsaveデータの情報をのっける
    public List<Dictionary<string, string>> appendDataToRecords(
        List<Dictionary<string, string>> listMusicDict,
        List<Dictionary<string, string>> loadSaveData) {
        //if (loadSaveData.Count == 0) return listMusicDict;

        for (int i = 0; i < listMusicDict.Count; i++) {
            Dictionary<string, string> returnData = getDictFromSaveData(
                listMusicDict[i]["music_folder"],
                listMusicDict[i]["music_bms"],
                loadSaveData);
            if (returnData != null) {
                listMusicDict[i].Add("HighScore", returnData["HighScore"]);
                listMusicDict[i].Add("MaxCombo", returnData["MaxCombo"]);
                listMusicDict[i].Add("Calorie", returnData["Calorie"]);
                //listMusicDict[i].Add("Rank", returnData["Rank"]);
            }
            else {
                listMusicDict[i].Add("HighScore", "");
                listMusicDict[i].Add("MaxCombo", "");
                listMusicDict[i].Add("Calorie", "");
                //listMusicDict[i].Add("Rank", "");
            }
        }
        return listMusicDict;
    }

    //music_folderとmusic_bmsが一致したセーブデータを返す
    private Dictionary<string, string> getDictFromSaveData(
        string folder,
        string file,
        List<Dictionary<string, string>> loadSaveData) {
        Dictionary<string, string> returnData = new Dictionary<string, string>();
        bool isFound = false;
        for (int i = 0; i < loadSaveData.Count; i++) {
            if ((folder == loadSaveData[i]["music_folder"]) &&
                (file == loadSaveData[i]["music_bms"])) {
                returnData = loadSaveData[i];
                isFound = true;
                break;
            }
        }
        return (isFound) ? returnData : null;
    }
}
