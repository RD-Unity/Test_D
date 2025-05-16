using System;
using System.Collections.Generic;
using Manager.Level;
using Newtonsoft.Json;
using UI.Grid;
using UnityEngine;

namespace Manager.Save
{
    [Serializable]
    public static class SaveManager
    {
        public static void Save(string a_strLevelID, SaveData a_saveData)
        {
            JsonSerializerSettings l_settings = new JsonSerializerSettings();
            l_settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            l_settings.TypeNameHandling = TypeNameHandling.All;
            string l_strJSONGameData = JsonConvert.SerializeObject(a_saveData, l_settings);
            System.IO.File.WriteAllText(Application.persistentDataPath + "/" + a_strLevelID + ".db", l_strJSONGameData);

        }
        public static SaveData Load(string a_strLevelID)
        {
            string l_filePath = Application.persistentDataPath + "/" + a_strLevelID + ".db";
            if (!System.IO.File.Exists(l_filePath))
            {
                Debug.LogWarning("SaveManager::Load::File not found at path: " + l_filePath);
                return null;
            }
            string l_strJSONGameData = System.IO.File.ReadAllText(l_filePath);
            JsonSerializerSettings l_settings = new JsonSerializerSettings();
            l_settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            l_settings.TypeNameHandling = TypeNameHandling.All;
            SaveData l_saveData = JsonConvert.DeserializeObject<SaveData>(l_strJSONGameData, l_settings);
            return l_saveData;
        }
        public static void DeleteSavedDataFile(string a_strLevelID)
        {
            string l_filePath = Application.persistentDataPath + "/" + a_strLevelID + ".db";
            if (System.IO.File.Exists(l_filePath))
            {
                System.IO.File.Delete(l_filePath);
            }
        }
    }
    public class SaveData
    {
        [SerializeField]
        public int m_iCurrentOpenedCards = 0;
        [SerializeField]
        public IconType m_currentOpenIconType = IconType.None;
        [SerializeField]
        public Dictionary<IconType, int> m_dictRemainingIconCount = null;
        [SerializeField]
        public List<CardIconStatus> m_gridIconStatus = null;
    }
}