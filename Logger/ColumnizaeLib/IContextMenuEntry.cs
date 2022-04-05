using System.Collections.Generic;

namespace ZappyLogger.ColumnizaeLib
{
    /// <summary>
    /// Implement this interface to add a menu entry to the context menu of ZappyLogger. 
    /// </summary>
    /// <remarks>
    /// <para>
    /// The methods in this interface will be called in the GUI thread. So make sure that there's no
    /// heavyweight work to do in your implementations.
    /// </para>
    /// </remarks>
    public interface IContextMenuEntry
    {
        #region Public methods

        /// <summary>
        /// This function is called from ZappyLogger if the context menu is about to be displayed. 
        /// Your implementation can control whether ZappyLogger will show a menu entry by returning
        /// an appropriate value.<br></br>
        /// </summary>
        /// <param name="lines">A list containing all selected line numbers.</param>
        /// <param name="columnizer">The currently selected Columnizer. You can use it to split log lines, 
        ///     if necessary.</param>
        /// <param name="callback">The callback interface implemented by ZappyLogger. You can use the functions
        ///     for retrieving log lines or pass it along to functions of the Columnizer if needed.</param>
        /// <returns>
        /// Return the string which should be displayed in the context menu.<br></br>
        /// You can control the menu behaviour by returning the the following values:<br></br>
        ///   <ul>
        ///   <li>Normal string:  The string is displayed as a menu entry</li>
        ///   <li>String starting with underscore: The string is displayed as a disabled menu entry</li>
        ///   <li>null: No menu entry is displayed.</li>
        ///   </ul>
        /// </returns>
        string GetMenuText(IList<int> lines, ILogLineColumnizer columnizer, IZappyLoggerCallback callback);


        /// <summary>
        /// This function is called from ZappyLogger if the menu entry is choosen by the user. <br></br>
        /// Note that this function is called from the GUI thread. So try to avoid time consuming operations.
        /// </summary>
        /// <param name="lines">A list containing all selected line numbers.</param>
        /// <param name="columnizer">The currently selected Columnizer. You can use it to split log lines, 
        ///     if necessary.</param>
        /// <param name="callback">The callback interface implemented by ZappyLogger. You can use the functions
        ///     for retrieving log lines or pass it along to functions of the Columnizer if needed.</param>
        void MenuSelected(IList<int> lines, ILogLineColumnizer columnizer, IZappyLoggerCallback callback);

        #endregion
    }
}