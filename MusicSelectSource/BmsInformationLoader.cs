using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FileController;
using System;

public class BmsInformationLoader : MonoBehaviour
{



    //ゲームフォルダ内の全曲フォルダからbms/bmeのファイル情報を抜いてくる
    public List<Dictionary<string, string>> getListMusicDict(string MUSIC_FOLDER_PATH) {
        List<Dictionary<string, string>> listMusicDict = new List<Dictionary<string, string>>();
        List<string> listFolder = fileController.getFolderList(MUSIC_FOLDER_PATH);
        int musicCount = 0;
        foreach (string folderName in listFolder) {
            List<string> listFile = fileController.getFileList(MUSIC_FOLDER_PATH + "/" + folderName);
            foreach (string fileName in listFile) {
                if ((fileName.Contains(".bms")) || 
                    (fileName.Contains(".bme")) ||
                    (fileName.Contains(".bml"))) 
                {
                    try {
                        listMusicDict.Add(
                            getBmsInfo(MUSIC_FOLDER_PATH, folderName, fileName, musicCount)
                        );
                        musicCount++;
                    }
                    catch (Exception e) {
                        Debug.Log("Can't open " + fileName);
                    }
                    
                }

            }

        }
        return listMusicDict;
    }

    //bms/bmeファイルの情報を抜く
    public Dictionary<string, string> getBmsInfo(
        string MUSIC_FOLDER_PATH, 
        string folderName, 
        string fileName,
        int musicCount
    ) {

        string[] lines = fileController.fileConvertUTF8(
            MUSIC_FOLDER_PATH + "/" + folderName + "/" + fileName
            );
        
        Dictionary<string, string> dict_info = UnityBm98Utilities.UnityBm98Utilities.getInfomation(lines);
        dict_info.Add("music_folder", folderName);
        dict_info.Add("music_bms", fileName);
        dict_info.Add("music_count", musicCount.ToString());
        return dict_info;
    }

}
