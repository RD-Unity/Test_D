using System;
using Manager.Level;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace UI.Grid
{
    /// <summary>
    /// Monobehavior class which is attached to the every cards of the grid 
    /// </summary>
    public class UICard : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI m_textCardType = null;
        [SerializeField]
        Image m_image = null;

        IconType m_currentIconType = IconType.None;

        /// <summary>
        /// Current attached IconType
        /// </summary>
        public IconType IconType => m_currentIconType;

        Action<UICard> m_onClick;
        bool m_bIsIconVisible = false;

        /// <summary>
        /// Load Data to the card
        /// </summary>
        /// <param name="a_iconType">icon type</param>
        /// <param name="a_onClick">callback on click</param>
        public void LoadData(IconType a_iconType, Action<UICard> a_onClick)
        {
            m_currentIconType = a_iconType;
            if (m_currentIconType == IconType.None)
            {
                Clear();
                return;
            }
            m_onClick = a_onClick;
            m_textCardType.text = m_currentIconType.ToString();
            m_textCardType.gameObject.SetActive(false);
            m_bIsIconVisible = false;
            m_image.enabled = true;
        }

        /// <summary>
        /// enable the card
        /// </summary>
        public void Enable()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Disable the card
        /// </summary>
        public void Disable()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// show the icon of card
        /// </summary>
        public void FlipToShowIcon()
        {
            if (m_bIsIconVisible)
            {
                return;
            }
            m_textCardType.gameObject.SetActive(true);
            m_bIsIconVisible = true;
        }

        /// <summary>
        /// Hide the icon of card
        /// </summary>
        public void FlipToHideIcon()
        {
            if (!m_bIsIconVisible)
            {
                return;
            }
            m_textCardType.gameObject.SetActive(false);
            m_bIsIconVisible = false;

        }

        /// <summary>
        /// Clear the card
        /// </summary>
        public void Clear()
        {
            Reset();
            m_image.enabled = false;
        }

        /// <summary>
        /// Reset the card
        /// </summary>
        public void Reset()
        {
            m_onClick = null;
            m_textCardType.text = string.Empty;
            m_textCardType.gameObject.SetActive(false);
            m_bIsIconVisible = false;
            m_image.enabled = true;
        }

        public void OnClick()
        {
            if (m_bIsIconVisible || (m_currentIconType == IconType.None))
            {
                return;
            }
            FlipToShowIcon();
            m_onClick?.Invoke(this);
        }
    }
}