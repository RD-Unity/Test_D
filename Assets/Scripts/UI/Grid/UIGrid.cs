using System;
using System.Collections;
using System.Collections.Generic;
using Manager.Level;
using Manager.Sound;
using Manager.UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Grid
{
    public class UIGrid : MonoBehaviour, IUIGrid
    {
        public const string UI_ID = "UIGrid";
        const string CARD_FLIP_SFX_KEY = "flip_card";
        [SerializeField]
        Canvas m_canvas = null;
        [SerializeField]
        GraphicRaycaster m_graphicRaycaster = null;

        [SerializeField]
        GridLayoutGroup m_gridLayoutGroup = null;

        [SerializeField]
        UICard m_uiCardPrefab = null;

        void Awake()
        {
            UIManager.instance.RegisterUI(UI_ID, this);
            LoadPool();
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
            StopAllCoroutines();
            m_canvas.enabled = false;
            m_graphicRaycaster.enabled = false;
            a_callback?.Invoke();
        }
        void IGlobalUI.SetInteractable(bool a_bIsInteractable)
        {
            m_graphicRaycaster.enabled = a_bIsInteractable;
        }
        void IUIGrid.LoadGrid(int a_iRows, int a_iColumns, List<IconType> a_icons, Action<IconType> a_onCardClicked)
        {
            m_currentGridCardStatus.Clear();
            TuneGridLayoutProperties(a_iColumns);
            foreach (IconType i_iconType in a_icons)
            {
                UICard l_uiCard = GetUICardFromPool();
                l_uiCard.transform.SetAsLastSibling();
                l_uiCard.LoadData(i_iconType, OnClickCard);
                l_uiCard.Enable();
                if (i_iconType != IconType.None)
                    l_uiCard.FlipToShowIcon_Snap();
                m_currentGridCardStatus.Add(CardIconStatus.Cleared);
            }
            m_onClickCard = a_onCardClicked;
            StartCoroutine(IE_HideAllCards());
        }
        void IUIGrid.LoadGrid(int a_iRows, int a_iColumns, List<IconType> a_icons, Action<IconType> a_onCardClicked, List<CardIconStatus> a_savedData)
        {
            m_currentGridCardStatus.Clear();
            TuneGridLayoutProperties(a_iColumns);
            for (int i = 0; i < a_icons.Count; i++)
            {
                UICard l_uiCard = GetUICardFromPool();
                l_uiCard.transform.SetAsLastSibling();
                if (a_savedData[i] == CardIconStatus.Cleared)
                {
                    l_uiCard.Clear();
                }
                else
                {
                    l_uiCard.LoadData(a_icons[i], OnClickCard);
                    if (a_savedData[i] == CardIconStatus.Visible)
                    {
                        l_uiCard.FlipToShowIcon_Snap();
                        m_currentOpenedCards.Add(l_uiCard);
                    }
                }
                l_uiCard.Enable();
                m_currentGridCardStatus.Add(a_savedData[i]);
            }
            m_onClickCard = a_onCardClicked;
        }
        void IUIGrid.ClearGrid()
        {
            List<UICard> l_cards = new List<UICard>(m_pooledOutCards);
            foreach (UICard i_card in l_cards)
            {
                i_card.Reset();
                i_card.Disable();
                ReturnToPool(i_card);
            }
        }
        void IUIGrid.HideCurrentFlippedCards()
        {
            foreach (UICard i_card in m_currentOpenedCards)
            {
                i_card.ChangeCardStatus(CardIconStatus.Hidden);
            }
            StartCoroutine(IE_HideCurrentFlippedCards(new List<UICard>(m_currentOpenedCards)));
            m_currentOpenedCards.Clear();
        }
        void IUIGrid.ClearCurrentFlippedCards()
        {
            foreach (UICard i_card in m_currentOpenedCards)
            {
                i_card.ChangeCardStatus(CardIconStatus.Cleared);
            }
            StartCoroutine(IE_ClearCurrentFlippedCards(new List<UICard>(m_currentOpenedCards)));
            m_currentOpenedCards.Clear();
        }
        List<CardIconStatus> IUIGrid.GetAllCardStatus()
        {
            for (int i = 0; i < m_pooledOutCards.Count; i++)
            {
                m_currentGridCardStatus[i] = m_pooledOutCards[i].CardIconStatus;
            }
            return m_currentGridCardStatus;
        }
        #endregion


        #region Card Interaction
        List<UICard> m_currentOpenedCards = new List<UICard>();
        Action<IconType> m_onClickCard = null;
        WaitForSeconds m_iWaitForHideClearTime = new WaitForSeconds(0.33f);
        WaitForSeconds m_iWaitForInitialShowTime = new WaitForSeconds(2f);
        List<CardIconStatus> m_currentGridCardStatus = new List<CardIconStatus>();

        void OnClickCard(UICard a_uiCard)
        {
            m_currentOpenedCards.Add(a_uiCard);
            m_onClickCard?.Invoke(a_uiCard.IconType);
            SoundManager.instance.PlaySFX(CARD_FLIP_SFX_KEY);
        }
        IEnumerator IE_HideCurrentFlippedCards(List<UICard> a_cards)
        {
            yield return m_iWaitForHideClearTime;
            SoundManager.instance.PlaySFX(CARD_FLIP_SFX_KEY);
            foreach (UICard i_card in a_cards)
            {
                i_card.FlipToHideIcon();
            }
        }
        IEnumerator IE_ClearCurrentFlippedCards(List<UICard> a_cards)
        {
            yield return m_iWaitForHideClearTime;
            foreach (UICard i_card in a_cards)
            {
                i_card.Clear();
            }
        }
        IEnumerator IE_HideAllCards()
        {
            yield return m_iWaitForInitialShowTime;
            foreach (UICard i_card in m_pooledOutCards)
            {
                i_card.FlipToHideIcon();
            }
            SoundManager.instance.PlaySFX(CARD_FLIP_SFX_KEY);
        }
        #endregion


        #region Grid Tuning
        int m_iRefWidth = 1080, m_iCellSpacing = 20, m_iLeftRightExtra = 150;
        Vector2 m_v2Size200 = new Vector2(200, 200);

        /// <summary>
        /// Set grid sizes as per level data
        /// </summary>
        /// <param name="a_iColumns">number of columns</param>
        void TuneGridLayoutProperties(int a_iColumns)
        {
            m_gridLayoutGroup.constraintCount = a_iColumns;
            if (a_iColumns > 4)
            {
                int l_cellSize = (m_iRefWidth - m_iLeftRightExtra - (m_iCellSpacing * a_iColumns)) / a_iColumns;
                m_gridLayoutGroup.cellSize = Vector2.one * l_cellSize;
            }
            else
            {
                m_gridLayoutGroup.cellSize = m_v2Size200;
            }
        }
        #endregion


        #region Cards Pool
        [SerializeField]
        List<UICard> m_initialCards = null;
        Queue<UICard> m_cardPool = new Queue<UICard>();
        List<UICard> m_pooledOutCards = new List<UICard>();

        /// <summary>
        /// initialize pool quese
        /// </summary>
        void LoadPool()
        {
            m_cardPool.Clear();
            foreach (UICard i_card in m_initialCards)
            {
                m_cardPool.Enqueue(i_card);
            }
        }

        /// <summary>
        /// Get object from pool
        /// </summary>
        /// <returns>UICard</returns>
        UICard GetUICardFromPool()
        {
            UICard a_uiCard = (m_cardPool.Count > 0) ? m_cardPool.Dequeue() : GenerateCard();
            m_pooledOutCards.Add(a_uiCard);
            return a_uiCard;
        }

        /// <summary>
        /// Put object back into pool
        /// </summary>
        /// <param name="a_uiCard">UICard</param>
        void ReturnToPool(UICard a_uiCard)
        {
            m_cardPool.Enqueue(a_uiCard);
            m_pooledOutCards.Remove(a_uiCard);
        }

        /// <summary>
        /// Generate card, if pool is empty
        /// </summary>
        /// <returns>UICard</returns>
        UICard GenerateCard()
        {
            UICard l_card = Instantiate(m_uiCardPrefab, m_gridLayoutGroup.transform);
            l_card.transform.localScale = Vector3.one;
            return l_card;
        }
        #endregion
    }

}