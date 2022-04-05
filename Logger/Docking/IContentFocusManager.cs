namespace ZappyLogger.Docking
{
    internal interface IContentFocusManager
    {
        #region Public methods

        void Activate(IDockContent content);
        void GiveUpFocus(IDockContent content);
        void AddToList(IDockContent content);
        void RemoveFromList(IDockContent content);

        #endregion
    }
}