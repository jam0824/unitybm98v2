using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityBm98Config {
    public static class config{
        private static string pcPath = "D:/download/game/bm98/music/";
        private static string oculusPath = "/storage/emulated/0/bmsfitness/";
        private static string saveDataFile = "save.csv";
        private static string categoriesPath = "categories/";

        //テスト時とoculus時でパスを自動で変更
        public static string getFolderPath() {
            if (Application.platform == RuntimePlatform.WindowsEditor) {
                return pcPath;
            }
            else {
                return oculusPath;
            }
        }

        //カテゴリーのパスを返す
        public static string getCategoryFolderPath() {
            if (Application.platform == RuntimePlatform.WindowsEditor) {
                return pcPath + categoriesPath;
            }
            else {
                return oculusPath + categoriesPath;
            }
        }

        //テスト時とoculus時でパスを自動で変更
        public static string getSaveDataFolderPath() {
            if (Application.platform == RuntimePlatform.WindowsEditor) {
                return pcPath;
            }
            else {
                return Application.persistentDataPath + "/";
            }
        }

        public static string getSaveDataFile() {
            return saveDataFile;
        }
    }
}

