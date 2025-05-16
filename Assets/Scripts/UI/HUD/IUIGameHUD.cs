
using Manager.UI;

namespace UI.GameHUD
{
    public interface IUIGameHUD : IGlobalUI
    {
        /// <summary>
        /// Show best flip count panel
        /// </summary>
        void TryShowBestFlipCountPanel();

        /// <summary>
        /// Set match needed to clear the cards
        /// </summary>
        /// <param name="a_iMatchNeeded">value</param>
        void SetMatchNeededValue(int a_iMatchNeeded);
    }
}