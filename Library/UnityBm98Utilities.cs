using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace UnityBm98Utilities {
    public class UnityBm98Utilities {
        //bmsファイル読み込み
        public static string[] read(string filePath) {
            Debug.Log(filePath);
            return File.ReadAllLines(filePath);
        }

        //インフォメーション部分読み込み
        public static Dictionary<string, string> getInfomation(string[] list_string) {
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

        public static string getLevel(int dificurity) {
            string level = "easy";
            if ((dificurity >= 4) && (dificurity <= 5)) {
                level = "normal";
            }
            else if ((dificurity >= 6) && (dificurity <= 7)) {
                level = "hard";
            }
            else if (dificurity >= 8) {
                level = "very_hard";
            }
            return level;
        }
    }
}

