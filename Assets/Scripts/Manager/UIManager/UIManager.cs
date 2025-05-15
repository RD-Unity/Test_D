using System;
using System.Collections.Generic;
using UnityEngine;
namespace Manager.UI
{
    public class UIManager : MonoBehaviour
    {
        /// <summary>
        /// Singleton instance
        /// </summary>
        public static UIManager instance { get; private set; }
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
        }


        private Dictionary<string, IGlobalUI> m_dictUIs = new Dictionary<string, IGlobalUI>();

        /// <summary>
        /// Register UI
        /// </summary>
        /// <param name="a_strID">UI id</param>
        /// <param name="a_ui">UI</param>
        public void RegisterUI(string a_strID, IGlobalUI a_ui)
        {
            if (string.IsNullOrEmpty(a_strID))
            {
                Debug.LogError("UIManager::RegisterUI::Can not register the UI with null string");
                return;
            }
            if (m_dictUIs.ContainsKey(a_strID))
            {
                Debug.LogError("UIManager::RegisterUI::UI with the ID '" + a_strID + "' already registered, so updating");
                m_dictUIs[a_strID] = a_ui;
                return;
            }
            m_dictUIs.Add(a_strID, a_ui);
        }

        /// <summary>
        /// UnRegister UI
        /// </summary>
        /// <param name="a_strID">UI id</param>
        public void UnRegisterUI(string a_strID)
        {
            m_dictUIs.Remove(a_strID);
        }

        /// <summary>
        /// Show UI
        /// </summary>
        /// <param name="a_strID">UI id</param>
        /// <param name="a_callback">callback</param>
        public void ShowUI(string a_strID, Action a_callback)
        {
            if (m_dictUIs.TryGetValue(a_strID, out IGlobalUI l_UI))
            {
                l_UI.Show(a_callback);
            }
            else
            {
                Debug.LogError("UIManager::ShowUI::UI with the ID '" + a_strID + "' is not registered");
            }

        }

        /// <summary>
        /// Hide UI
        /// </summary>
        /// <param name="a_strID">UI id</param>
        /// <param name="a_callback">callback</param>
        public void HideUI(string a_strID, Action a_callback)
        {
            if (m_dictUIs.TryGetValue(a_strID, out IGlobalUI l_UI))
            {
                l_UI.Hide(a_callback);
            }
            else
            {
                Debug.LogError("UIManager::HideUI::UI with the ID '" + a_strID + "' is not registered");
            }
        }

        /// <summary>
        /// Set UI intractability
        /// </summary>
        /// <param name="a_strID">UI id</param>
        /// <param name="a_bIsInteractable">Whether UI is interactable</param>
        public void SetInteractableUI(string a_strID, bool a_bIsInteractable)
        {
            if (m_dictUIs.TryGetValue(a_strID, out IGlobalUI l_UI))
            {
                l_UI.SetInteractable(a_bIsInteractable);
            }
            else
            {
                Debug.LogError("UIManager::SetInteractableUI::UI with the ID '" + a_strID + "' is not registered");
            }
        }

        /// <summary>
        /// Get particular UI
        /// </summary>
        /// <param name="a_strID">UI id</param>
        /// <returns></returns>
        public IGlobalUI GetUI(string a_strID)
        {
            if (m_dictUIs.TryGetValue(a_strID, out IGlobalUI l_UI))
            {
                return l_UI;
            }
            else
            {
                Debug.LogError("UIManager::ShowUI::UI with the ID '" + a_strID + "' is not registered, so retuning null");
                return null;
            }
        }
    }
}