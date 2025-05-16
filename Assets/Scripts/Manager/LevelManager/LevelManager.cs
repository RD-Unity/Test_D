using System.Collections.Generic;
using Manager.Sound;
using Manager.UI;
using UI.Grid;
using UI.Menu;
using UnityEngine;
namespace Manager.Level
{
    public class LevelManager : MonoBehaviour
    {
        const string CARD_MATCH_SFX_KEY = "card_match";
        const string CARD_MISMATCH_SFX_KEY = "card_mismatch";
        const string LEVEL_COMPLETE_SFX_KEY = "level_complete";
        /// <summary>
        /// Singleton instance
        /// </summary>
        public static LevelManager instance { get; private set; }
        void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this);
            }
            else
            {
                instance = this;
            }
            LoadScriptableObjectData();
        }
        void Start()
        {
            IUIMenuRef.LoadData(m_allLevelData.m_levels);
            IUIMenuRef.Show();
        }

        public void StartLevel(string a_levelName)
        {
            foreach (Level i_level in m_allLevelData.m_levels)
            {
                if (i_level.m_strLevelID.Equals(a_levelName))
                {
                    StartLevel(i_level);
                    return;
                }
            }
        }
        public void StartLevel(Level a_level)
        {
            m_currentLoadedLevel = a_level;
            LoadCurrentLevelData();
        }

        #region Current Level Handling
        Level m_currentLoadedLevel = null;
        Dictionary<IconType, int> m_dictRemainingIconCount = new Dictionary<IconType, int>();
        int m_iMatchNeededToClear = 0;
        int m_iCurrentOpenedCards = 0;
        IconType m_currentOpenIconType = IconType.None;
        void OnCardClick(IconType a_type)
        {
            m_iCurrentOpenedCards++;
            if (m_iCurrentOpenedCards == 1)
            {
                m_currentOpenIconType = a_type;
            }
            else
            {
                if (a_type == m_currentOpenIconType)
                {
                    if (m_iMatchNeededToClear == m_iCurrentOpenedCards)
                    {
                        // clear the cards, combo is correct
                        SoundManager.instance.PlaySFX(CARD_MATCH_SFX_KEY);
                        IUIGridRef.ClearCurrentFlippedCards();
                        m_dictRemainingIconCount[a_type] -= m_iMatchNeededToClear;
                        m_iCurrentOpenedCards = 0;
                        m_currentOpenIconType = IconType.None;
                    }
                }
                else
                {
                    // incorrect combo, flip the cards
                    SoundManager.instance.PlaySFX(CARD_MISMATCH_SFX_KEY);
                    IUIGridRef.HideCurrentFlippedCards();
                    m_iCurrentOpenedCards = 0;
                    m_currentOpenIconType = IconType.None;
                }
            }
            CheckForLevelComplete();
        }
        void LoadCurrentLevelData()
        {
            IUIGridRef.LoadGrid(m_currentLoadedLevel.m_iRows, m_currentLoadedLevel.m_iColumns, m_currentLoadedLevel.m_icons, OnCardClick);
            IUIGridRef.Show();
            m_dictRemainingIconCount.Clear();
            foreach (IconType i_iconType in m_currentLoadedLevel.m_icons)
            {
                if (i_iconType == IconType.None)
                {
                    continue;
                }
                if (m_dictRemainingIconCount.ContainsKey(i_iconType))
                {
                    m_dictRemainingIconCount[i_iconType]++;
                }
                else
                {
                    m_dictRemainingIconCount.Add(i_iconType, 1);
                }
            }
            m_iMatchNeededToClear = m_currentLoadedLevel.m_iMatchNeededToClear;
        }
        void CheckForLevelComplete()
        {
            foreach (IconType i_key in m_dictRemainingIconCount.Keys)
            {
                if (m_dictRemainingIconCount[i_key] > 0)
                {
                    return;
                }
            }
            SoundManager.instance.PlaySFX(LEVEL_COMPLETE_SFX_KEY);
            IUIGridRef.Hide();
            IUIGridRef.ClearGrid();
            IUIMenuRef.Show();
        }
        #endregion


        #region All Level Data
        const string RESOURCES_PATH = "ScriptableObjects/LevelData/LevelData";
        LevelData m_allLevelData = null;
        void LoadScriptableObjectData()
        {
            m_allLevelData = (LevelData)Resources.Load(RESOURCES_PATH);
        }
        #endregion

        #region UI References
        IUIGrid m_iUIGridRef = null;
        IUIGrid IUIGridRef
        {
            get
            {
                if (m_iUIGridRef == null)
                {
                    m_iUIGridRef = (IUIGrid)UIManager.instance.GetUI(UIGrid.UI_ID);
                }
                return m_iUIGridRef;
            }
        }
        IUIMenu m_iUIMenuRef = null;
        IUIMenu IUIMenuRef
        {
            get
            {
                if (m_iUIMenuRef == null)
                {
                    m_iUIMenuRef = (IUIMenu)UIManager.instance.GetUI(UIMenu.UI_ID);
                }
                return m_iUIMenuRef;
            }
        }
        #endregion

    }
}