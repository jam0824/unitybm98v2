using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace FileController {
    public static class fileController{
        public static List<string> getFileList(string folderPath) {
            List<string> listFiles = new List<string>();
            DirectoryInfo dir = new DirectoryInfo(folderPath);
            FileInfo[] info = dir.GetFiles("*.*");
            foreach (FileInfo f in info) {
                listFiles.Add(f.Name);
            }
            return listFiles;
        }

        public static List<string> getFolderList(string folderPath) {
            List<string> listFolders = new List<string>();
            DirectoryInfo dir = new DirectoryInfo(folderPath);
            DirectoryInfo[] info = dir.GetDirectories();
            foreach (DirectoryInfo f in info) {
                listFolders.Add(f.Name);
            }
            return listFolders;
        }

        public static bool changeFileName(string fileName, string beforeChar, string afterChar) {
            if (!File.Exists(fileName)) return false;
            string afterFileName = fileName.Replace(beforeChar, afterChar);
            try {
                File.Move(fileName, afterFileName);
                Debug.Log(fileName + "->" + afterFileName);
                return true;
            }
            catch (Exception e) {
                Debug.LogError("Faild file name change :" + fileName + " -> " + afterFileName);
                Debug.LogError(e);
                return false;
            }
            
        }

    }


}

