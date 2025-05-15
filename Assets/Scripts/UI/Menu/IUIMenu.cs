using System.Collections.Generic;
using Manager.Level;
using Manager.UI;

namespace UI.Menu
{
    public interface IUIMenu : IGlobalUI
    {
        /// <summary>
        /// Load Menu
        /// </summary>
        /// <param name="a_Levels">list of levels</param>
        void LoadData(List<Level> a_Levels);
    }
}