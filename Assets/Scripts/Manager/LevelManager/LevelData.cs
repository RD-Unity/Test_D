using System;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Manager.Level
{
    public class LevelData : ScriptableObject
    {
        [SerializeField]
        public List<Level> m_levels = new List<Level>();

#if UNITY_EDITOR
        private const string INPUT_PATH = "/LevelDataXMLs/LevelData.xml";
        private const string OUTPUT_PATH = "Assets/Resources/ScriptableObjects/LevelData/LevelData.asset";

        private const string KEY_ROWS = "rows";
        private const string KEY_COLUMNS = "column";
        private const string KEY_TYPE = "type";
        private const string KEY_ICON = "Icon";
        private const string KEY_LEVELS = "/Levels";
        private const string KEY_LEVEL = "Level";
        private const string KEY_NAME_ID = "name_id";
        private const string KEY_MATCH_NEEDED_TO_CLEAR = "match_needed_to_clear";


        /// <summary>
        /// Validate and create level data scriptable object
        /// </summary>
        [MenuItem("LevelData/Check N Create Asset")]
        static void CheckNCreateAsset()
        {
            LevelData l_levelData = ScriptableObject.CreateInstance<LevelData>();
            XmlDocument l_LevelDataXmlDoc = new XmlDocument();
            l_LevelDataXmlDoc.LoadXml(File.ReadAllText(Application.dataPath + INPUT_PATH));


            List<Level> l_levels = new List<Level>();

            XmlNode l_nodeSuper = l_LevelDataXmlDoc.SelectSingleNode(KEY_LEVELS);
            XmlElement l_nodeEleSuper = (XmlElement)l_nodeSuper;
            XmlNodeList l_xmlLevelList = l_nodeEleSuper.GetElementsByTagName(KEY_LEVEL);
            Dictionary<IconType, int> m_iconCountData = new Dictionary<IconType, int>();
            foreach (XmlNode l_nodeLevel in l_xmlLevelList)
            {
                int l_rows = int.Parse(l_nodeLevel.Attributes[KEY_ROWS].Value);
                int l_columns = int.Parse(l_nodeLevel.Attributes[KEY_COLUMNS].Value);
                int l_matchNeededToClear = int.Parse(l_nodeLevel.Attributes[KEY_MATCH_NEEDED_TO_CLEAR].Value);
                string l_strName = l_nodeLevel.Attributes[KEY_NAME_ID].Value;

                if (l_matchNeededToClear < 2)
                {
                    Debug.LogError("CheckNCreateAsset::'match_needed_to_clear' is lesser than 2 for level '" + l_strName + "'");

                    continue;
                }

                XmlElement l_nodeEle = (XmlElement)l_nodeLevel;
                XmlNodeList l_xmlIconList = l_nodeEle.GetElementsByTagName(KEY_ICON);
                if (l_xmlIconList.Count != (l_rows * l_columns))
                {
                    Debug.LogError("CheckNCreateAsset :: icon count miss match for level '" + l_strName + "'");
                    continue;
                }

                List<IconType> l_icons = new List<IconType>();
                m_iconCountData.Clear();
                foreach (XmlNode l_iconNode in l_xmlIconList)
                {
                    IconType l_icon = Enum.Parse<IconType>(l_iconNode.Attributes[KEY_TYPE].Value);
                    l_icons.Add(l_icon);
                    if (m_iconCountData.ContainsKey(l_icon))
                    {
                        m_iconCountData[l_icon] = m_iconCountData[l_icon] + 1;
                    }
                    else
                    {
                        m_iconCountData[l_icon] = 1;
                    }
                }
                bool l_bIsValidLevel = true;
                foreach (IconType i_iconType in m_iconCountData.Keys)
                {
                    if (i_iconType == IconType.None)
                    {
                        continue;
                    }
                    if ((m_iconCountData[i_iconType] % l_matchNeededToClear) != 0)
                    {
                        l_bIsValidLevel = false;
                        Debug.LogError("CheckNCreateAsset :: icon type '" + i_iconType.ToString() + "' count must be multiple of '" + l_matchNeededToClear + "' for level '" + l_strName + "'");
                    }
                }
                if (l_bIsValidLevel)
                {
                    l_levels.Add(new Level
                    {
                        m_strLevelID = l_strName,
                        m_iRows = l_rows,
                        m_iColumns = l_columns,
                        m_iMatchNeededToClear = l_matchNeededToClear,
                        m_icons = l_icons,
                    });
                }
            }


            l_levelData.m_levels = l_levels;
            try
            {
                AssetDatabase.DeleteAsset(OUTPUT_PATH);
            }
            catch
            {

            }
            AssetDatabase.CreateAsset(l_levelData, OUTPUT_PATH);
        }
#endif
    }

    [Serializable]
    public class Level
    {
        /// <summary>
        /// Level ID
        /// </summary> 
        [SerializeField]
        public string m_strLevelID = string.Empty;

        /// <summary>
        /// Number of rows
        /// </summary>
        [SerializeField]
        public int m_iRows = 0;

        /// <summary>
        /// Number of Columns
        /// </summary>
        [SerializeField]
        public int m_iColumns = 0;

        /// <summary>
        /// how many match needed to clear the cards
        /// </summary>
        [SerializeField]
        public int m_iMatchNeededToClear = 0;

        /// <summary>
        /// grid detail, from left to right, top to bottom
        /// </summary>
        [SerializeField]
        public List<IconType> m_icons = null;
    }
}