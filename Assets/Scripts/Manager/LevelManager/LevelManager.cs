using System.Collections;
using System.Collections.Generic;
using Manager.Save;
using Manager.Score;
using Manager.Sound;
using Manager.UI;
using UI.GameHUD;
using UI.Grid;
using UI.Menu;
using UnityEngine;
using UnityEngine.SceneManagement;
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
            StopAllCoroutines();
        }

        #region Current Level Handling
        Level m_currentLoadedLevel = null;
        Dictionary<IconType, int> m_dictRemainingIconCount = new Dictionary<IconType, int>();
        int m_iMatchNeededToClear = 0;
        int m_iCurrentOpenedCards = 0;
        IconType m_currentOpenIconType = IconType.None;
        SaveData m_saveData = null;
        void OnCardClick(IconType a_type)
        {
            ScoreManager.instance.IncreaseFlipCount();
            m_saveData.m_bIsLevelInProgress = true;
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

            if (CheckForLevelComplete())
            {
                m_saveData.m_bIsLevelInProgress = false;
                SoundManager.instance.PlaySFX(LEVEL_COMPLETE_SFX_KEY);
                int l_flipCount = ScoreManager.instance.GetFlipCount();
                int l_bestFlipCount = ScoreManager.instance.GetBestFlipCount();
                if (l_flipCount < l_bestFlipCount)
                {
                    ScoreManager.instance.SetBestFlipCount(l_flipCount);
                    m_saveData.m_iBestFlipCount = l_flipCount;
                }
                StartCoroutine(IE_WaitForBackToMenuAfterLevelComplete());
            }
            SaveData();
        }
        WaitForSeconds m_waitForHalfSeconds = new WaitForSeconds(1f);
        IEnumerator IE_WaitForBackToMenuAfterLevelComplete()
        {
            yield return m_waitForHalfSeconds;
            IUIGridRef.Hide();
            IUIGameHUDRef.Hide();
            IUIGridRef.ClearGrid();
            IUIMenuRef.Show();
        }
        void LoadCurrentLevelData()
        {
            m_saveData = SaveManager.Load(m_currentLoadedLevel.m_strLevelID);
            if (m_saveData == null || !m_saveData.m_bIsLevelInProgress)
            {
                IUIGridRef.LoadGrid(m_currentLoadedLevel.m_iRows, m_currentLoadedLevel.m_iColumns, m_currentLoadedLevel.m_icons, OnCardClick);
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
                ScoreManager.instance.Reset();
                if (m_saveData == null)
                    m_saveData = new SaveData();
                else
                    ScoreManager.instance.SetBestFlipCount(m_saveData.m_iBestFlipCount);
            }
            else
            {
                IUIGridRef.LoadGrid(m_currentLoadedLevel.m_iRows, m_currentLoadedLevel.m_iColumns, m_currentLoadedLevel.m_icons, OnCardClick, m_saveData.m_gridIconStatus);
                m_dictRemainingIconCount = m_saveData.m_dictRemainingIconCount;
                m_iCurrentOpenedCards = m_saveData.m_iCurrentOpenedCards;
                m_currentOpenIconType = m_saveData.m_currentOpenIconType;
                ScoreManager.instance.SetFlipCount(m_saveData.m_iCurrentFlipCount);
                ScoreManager.instance.SetBestFlipCount(m_saveData.m_iBestFlipCount);
            }
            IUIGridRef.Show();
            IUIGameHUDRef.Show();
            IUIGameHUDRef.SetMatchNeededValue(m_currentLoadedLevel.m_iMatchNeededToClear);
            m_iMatchNeededToClear = m_currentLoadedLevel.m_iMatchNeededToClear;
        }
        bool CheckForLevelComplete()
        {
            foreach (IconType i_key in m_dictRemainingIconCount.Keys)
            {
                if (m_dictRemainingIconCount[i_key] > 0)
                {
                    return false;
                }
            }
            return true;
        }
        void SaveData()
        {
            m_saveData.m_iCurrentOpenedCards = m_iCurrentOpenedCards;
            m_saveData.m_currentOpenIconType = m_currentOpenIconType;
            m_saveData.m_dictRemainingIconCount = m_dictRemainingIconCount;
            m_saveData.m_gridIconStatus = IUIGridRef.GetAllCardStatus();
            m_saveData.m_iCurrentFlipCount = ScoreManager.instance.GetFlipCount();
            SaveManager.Save(m_currentLoadedLevel.m_strLevelID, m_saveData);
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
        IUIGameHUD m_iUIGameHUDRef = null;
        IUIGameHUD IUIGameHUDRef
        {
            get
            {
                if (m_iUIGameHUDRef == null)
                {
                    m_iUIGameHUDRef = (IUIGameHUD)UIManager.instance.GetUI(UIGameHUD.UI_ID);
                }
                return m_iUIGameHUDRef;
            }
        }
        #endregion

    }
}