using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FileController;

public class BmsInformationLoader : MonoBehaviour
{



    //ゲームフォルダ内の全曲フォルダからbms/bmeのファイル情報を抜いてくる
    public List<Dictionary<string, string>> getListMusicDict(string MUSIC_FOLDER_PATH) {
        List<Dictionary<string, string>> listMusicDict = new List<Dictionary<string, string>>();
        List<string> listFolder = fileController.getFolderList(MUSIC_FOLDER_PATH);
        foreach (string folderName in listFolder) {
            List<string> listFile = fileController.getFileList(MUSIC_FOLDER_PATH + "/" + folderName);
            foreach (string fileName in listFile) {
                if ((fileName.Contains(".bms")) || (fileName.Contains(".bme"))) {
                    listMusicDict.Add(
                        getBmsInfo(MUSIC_FOLDER_PATH, folderName, fileName)
                    );
                }

            }

        }
        return listMusicDict;
    }

    //bms/bmeファイルの情報を抜く
    public Dictionary<string, string> getBmsInfo(
        string MUSIC_FOLDER_PATH, 
        string folderName, 
        string fileName
    ) {

        string[] lines = fileController.fileConvertUTF8(
            MUSIC_FOLDER_PATH + "/" + folderName + "/" + fileName
            );
        
        Dictionary<string, string> dict_info = UnityBm98Utilities.UnityBm98Utilities.getInfomation(lines);
        dict_info.Add("music_folder", folderName);
        dict_info.Add("music_bms", fileName);
        return dict_info;
    }

}
