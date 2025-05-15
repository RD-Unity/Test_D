using System;

namespace Manager.UI
{
    public interface IGlobalUI
    {
        /// <summary>
        /// Show UI
        /// </summary>
        /// <param name="a_callback">callback after show</param>
        void Show(Action a_callback = null);

        /// <summary>
        /// Hide UI
        /// </summary>
        /// <param name="a_callback">callback after hide</param>
        void Hide(Action a_callback = null);

        /// <summary>
        /// To set whether UI is interactable 
        /// </summary>
        /// <param name="a_bIsInteractable">intractable value</param>
        void SetInteractable(bool a_bIsInteractable);
    }
}