using System;
using Manager.Score;
using Manager.UI;
using TMPro;
using UI.Grid;
using UI.Menu;
using UnityEngine;
using UnityEngine.UI;
namespace UI.GameHUD
{
    public class UIGameHUD : MonoBehaviour, IUIGameHUD
    {
        public const string UI_ID = "UIGameHUD";
        [SerializeField]
        Canvas m_canvas = null;
        [SerializeField]
        GraphicRaycaster m_graphicRaycaster = null;

        IUIGameHUD m_ownRef;

        [SerializeField]
        TextMeshProUGUI m_textFlipCount = null, m_textBestFlipCount = null;

        [SerializeField]
        TextMeshProUGUI m_textMatchNeeded = null;

        [SerializeField]
        GameObject m_goBestFlipCountPanel = null;
        void Awake()
        {
            UIManager.instance.RegisterUI(UI_ID, this);
            m_ownRef = this;
        }
        private void OnScoreUpdated(int a_iFlipCount)
        {
            m_textFlipCount.text = a_iFlipCount.ToString();
        }
        private void OnBestScoreUpdated(int a_iFlipCount)
        {
            if (a_iFlipCount != int.MaxValue)
            {
                m_goBestFlipCountPanel.SetActive(true);
                m_textBestFlipCount.text = a_iFlipCount.ToString();
            }
            else
            {
                m_goBestFlipCountPanel.SetActive(false);
            }
        }

        void OnDestroy()
        {
            if (UIManager.instance != null)
            {
                UIManager.instance.UnRegisterUI(UI_ID);
            }
        }
        #region interface implementation
        void IGlobalUI.Show(Action a_callback)
        {
            m_canvas.enabled = true;
            m_graphicRaycaster.enabled = true;
            ScoreManager.instance.OnScoreUpdated += OnScoreUpdated;
            ScoreManager.instance.OnBestScoreUpdated += OnBestScoreUpdated;
            m_textFlipCount.text = ScoreManager.instance.GetFlipCount().ToString();
            int l_bestFlipCount = ScoreManager.instance.GetBestFlipCount();
            if (l_bestFlipCount != int.MaxValue)
            {
                m_goBestFlipCountPanel.SetActive(true);
                m_textBestFlipCount.text = l_bestFlipCount.ToString();
            }
            else
            {
                m_goBestFlipCountPanel.SetActive(false);
            }
            a_callback?.Invoke();
        }
        void IGlobalUI.Hide(Action a_callback)
        {
            m_canvas.enabled = false;
            m_graphicRaycaster.enabled = false;
            ScoreManager.instance.OnScoreUpdated -= OnScoreUpdated;
            ScoreManager.instance.OnBestScoreUpdated -= OnBestScoreUpdated;
            a_callback?.Invoke();
        }
        void IGlobalUI.SetInteractable(bool a_bIsInteractable)
        {
            m_graphicRaycaster.enabled = a_bIsInteractable;
        }
        void IUIGameHUD.TryShowBestFlipCountPanel()
        {
            int l_bestFlipCount = ScoreManager.instance.GetBestFlipCount();
            if (l_bestFlipCount != int.MaxValue)
            {
                m_goBestFlipCountPanel.SetActive(true);
                m_textBestFlipCount.text = l_bestFlipCount.ToString();
            }
        }
        void IUIGameHUD.SetMatchNeededValue(int a_iMatchNeeded)
        {
            m_textMatchNeeded.text = a_iMatchNeeded.ToString();
        }
        #endregion

        public void OnClickHome()
        {
            IUIGridRef.Hide();
            IUIGridRef.ClearGrid();
            IUIMenuRef.Show();
            m_ownRef.Hide();
        }

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