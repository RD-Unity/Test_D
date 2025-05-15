using System.Collections.Generic;
using System;
using Manager.UI;
using UnityEngine;
using UnityEngine.UI;
using Manager.Level;
using System.Threading;

namespace UI.Menu
{
    public class UIMenu : MonoBehaviour, IUIMenu
    {
        public const string UI_ID = "UIMenu";
        [SerializeField]
        Canvas m_canvas = null;
        [SerializeField]
        GraphicRaycaster m_graphicRaycaster = null;
        [SerializeField]
        UIMenuItem m_uiMenuItemPrefab = null;
        [SerializeField]
        Transform m_transformMenuItemParent = null;
        IUIMenu m_ownRef = null;
        void Awake()
        {
            UIManager.instance.RegisterUI(UI_ID, this);
            m_ownRef = this;
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
            a_callback?.Invoke();
        }
        void IGlobalUI.Hide(Action a_callback)
        {
            m_canvas.enabled = false;
            m_graphicRaycaster.enabled = false;
            a_callback?.Invoke();
        }
        void IGlobalUI.SetInteractable(bool a_bIsInteractable)
        {
            m_graphicRaycaster.enabled = a_bIsInteractable;
        }
        void IUIMenu.LoadData(List<Level> a_Levels)
        {
            foreach (Level i_level in a_Levels)
            {
                UIMenuItem a_menuItem = Instantiate(m_uiMenuItemPrefab, m_transformMenuItemParent);
                a_menuItem.transform.localScale = Vector3.one;
                a_menuItem.LoadData(i_level, OnClickLevel);
            }
        }
        #endregion
        void OnClickLevel(Level a_level)
        {
            LevelManager.instance.StartLevel(a_level);
            m_ownRef.Hide();
        }
    }
}