using System;
using Manager.Level;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Manager.Image;
namespace UI.Grid
{
    /// <summary>
    /// Monobehavior class which is attached to the every cards of the grid 
    /// </summary>
    public class UICard : MonoBehaviour
    {
        [SerializeField]
        Image m_imageCardIcon = null;
        [SerializeField]
        Image m_image = null;
        [SerializeField]
        Animation m_animation = null;

        const string SHOW_ICON_ANIM_NAME = "FlipToShowIcon";
        const string HIDE_ICON_ANIM_NAME = "FlipToHideIcon";
        const float SHOW_ICON_ANIM_LENGTH = 0.33f;
        const float HIDE_ICON_ANIM_LENGTH = 0.33f;
        Vector3 m_v3ShowIconCardRotation = Vector3.up * 180;

        WaitForSeconds m_waitForShowAnimTime = new WaitForSeconds(SHOW_ICON_ANIM_LENGTH), m_waitForHideAnimTime = new WaitForSeconds(HIDE_ICON_ANIM_LENGTH);

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
            m_imageCardIcon.sprite = ImageManager.instance.GetSprite(m_currentIconType);
            m_imageCardIcon.gameObject.SetActive(false);
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
            m_animation.Play(SHOW_ICON_ANIM_NAME);
            m_bIsIconVisible = true;
        }
        public void FlipToShowIcon_Snap()
        {
            if (m_bIsIconVisible)
            {
                return;
            }
            m_imageCardIcon.gameObject.SetActive(true);
            transform.eulerAngles = m_v3ShowIconCardRotation;
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
            m_animation.Play(HIDE_ICON_ANIM_NAME);
            StartCoroutine(IE_FlipToHideIcon());
        }
        IEnumerator IE_FlipToHideIcon()
        {
            yield return m_waitForHideAnimTime;
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
            m_imageCardIcon.sprite = null;
            m_imageCardIcon.gameObject.SetActive(false);
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
        public void OnReachRotation90_ShowAnim()
        {
            m_imageCardIcon.gameObject.SetActive(true);
        }
        public void OnReachRotation90_HideAnim()
        {
            m_imageCardIcon.gameObject.SetActive(false);
        }
    }
}