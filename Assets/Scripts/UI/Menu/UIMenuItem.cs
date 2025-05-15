using System;
using Manager.Level;
using TMPro;
using UnityEngine;
namespace UI.Menu
{
    public class UIMenuItem : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI m_textLevelName = null;

        Action<Level> m_OnClick;
        Level m_level;

        public void LoadData(Level a_level, Action<Level> a_onClick)
        {
            m_level = a_level;
            m_textLevelName.text = m_level.m_strLevelID;
            m_OnClick = a_onClick;

        }

        public void OnClick()
        {
            m_OnClick?.Invoke(m_level);
        }
    }
}