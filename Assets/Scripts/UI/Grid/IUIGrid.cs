using System;
using System.Collections.Generic;
using Manager.Level;
using Manager.UI;
namespace UI.Grid
{
    public interface IUIGrid : IGlobalUI
    {
        /// <summary>
        /// Generate grid
        /// </summary>
        /// <param name="a_iRows">number of rows</param>
        /// <param name="a_iColumns">number of columns</param>
        /// <param name="a_icons">list of icons</param>
        /// <param name="a_onCardClicked">callback on particular card clicked</param>
        void LoadGrid(int a_iRows, int a_iColumns, List<IconType> a_icons, Action<IconType> a_onCardClicked);

        /// <summary>
        /// Clear the grid
        /// </summary>
        void ClearGrid();

        /// <summary>
        /// Hide the current opened cards, if combo does not happen
        /// </summary>
        void HideCurrentFlippedCards();

        /// <summary>
        /// Clear the current opened cards, if combo happens
        /// </summary> 
        void ClearCurrentFlippedCards();
    }
}